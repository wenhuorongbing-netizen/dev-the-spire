(async function () {
  const baseData = window.SPIRE_PLUS_DATA;
  let lang = initialLang();
  let data = selectData(baseData, lang);
  let labels = data.labels;
  const app = document.getElementById("app");
  const header = document.getElementById("siteHeader");
  const routes = new Set(["updates", "install", "forum", "issues", "about"]);
  const fallbackIcon = "assets/relics/relic.png";
  const appVersion = new URL(document.currentScript?.src || location.href, location.href).searchParams.get("v") || "local";
  let loc = await loadLocalization();
  document.documentElement.lang = lang === "en" ? "en" : "zh-CN";

  // Flat list of all updates across groups for easy inspector index lookups
  let allUpdateItems = [];
  function flattenUpdateItems() {
    allUpdateItems = [];
    data.updateGroups.forEach(group => {
      group.items.forEach(item => {
        allUpdateItems.push({
          ...item,
          icon: item.icon || group.icon,
          groupDefaultVanilla: group.defaultVanilla,
          groupDefaultVanillaEn: group.defaultVanillaEn
        });
      });
    });
  }
  flattenUpdateItems();

  let activeInspectItem = allUpdateItems[0] || null;
  let pinnedInspectIdentity = null;

  function initialLang() {
    const requested = new URLSearchParams(location.search).get("lang") ||
      localStorage.getItem("spireplus-lang") ||
      "zh";
    return normalizeLang(requested);
  }

  function normalizeLang(value) {
    return String(value || "").toLowerCase().startsWith("en") ? "en" : "zh";
  }

  function selectData(base, selectedLang) {
    const patch = base.i18n?.[selectedLang] || {};
    const groupPatches = patch.updateGroups || [];
    const itemPatches = patch.items || {};
    const tagTranslations = patch.tagTranslations || {};
    return {
      ...base,
      labels: { ...base.labels, ...(patch.labels || {}) },
      summary: patch.summary || base.summary,
      package: {
        ...base.package,
        ...(patch.package || {}),
        meta: patch.package?.meta || base.package.meta
      },
      installSteps: patch.installSteps || base.installSteps,
      requirements: patch.requirements || base.requirements,
      assetPolicy: patch.assetPolicy || base.assetPolicy,
      forum: { ...base.forum, ...(patch.forum || {}) },
      knownIssues: patch.knownIssues || base.knownIssues,
      changeLog: patch.changeLog || base.changeLog,
      locFiles: patch.locFiles || base.locFiles,
      mechanics: patch.mechanics || base.mechanics || [],
      updateGroups: base.updateGroups.map((group, index) => {
        const groupPatch = groupPatches[index] || {};
        const groupItems = groupPatch.items || {};
        return {
          ...group,
          ...groupPatch,
          items: group.items.map((item) => {
            const patchKey = itemKey(item);
            const itemPatch = itemPatches[patchKey] || groupItems[patchKey] || {};
            return {
              ...item,
              ...itemPatch,
              tags: itemPatch.tags || translateTags(item.tags, tagTranslations)
            };
          })
        };
      })
    };
  }

  function itemKey(item) {
    return item.i18nKey || item.descKey || item.titleKey || item.title || item.ancientId || "";
  }

  function inspectIdentity(item) {
    if (!item) return "";
    if (item.kind === "mechanic") return `mechanic:${item.mechanicId}`;
    return `item:${itemKey(item)}`;
  }

  function isPinned(item = activeInspectItem) {
    return Boolean(pinnedInspectIdentity && pinnedInspectIdentity === inspectIdentity(item));
  }

  function getMechanic(id) {
    return (data.mechanics || []).find((mechanic) => mechanic.id === id);
  }

  function getMechanicTitle(mechanic) {
    return text(lang === "en" ? mechanic.titleEn || mechanic.title : mechanic.title);
  }

  function getMechanicBody(mechanic) {
    return text(lang === "en" ? mechanic.descEn || mechanic.desc : mechanic.desc);
  }

  function getMechanicBullets(mechanic) {
    return lang === "en" ? mechanic.bulletsEn || mechanic.bullets || [] : mechanic.bullets || [];
  }

  function mechanicInspectItem(id) {
    const mechanic = getMechanic(id);
    if (!mechanic) return null;
    return {
      kind: "mechanic",
      mechanicId: mechanic.id,
      title: getMechanicTitle(mechanic),
      desc: getMechanicBody(mechanic),
      icon: mechanic.icon || "assets/relics/relic.png",
      tags: mechanic.tags || [labels.mechanicTag || "机制"]
    };
  }

  function translateTags(tags, translations) {
    return (tags || []).map((tag) => translations[tag] || tag);
  }

  async function setLanguage(nextLang) {
    const normalized = normalizeLang(nextLang);
    if (normalized === lang) return;
    lang = normalized;
    localStorage.setItem("spireplus-lang", lang);
    data = selectData(baseData, lang);
    labels = data.labels;
    loc = await loadLocalization();
    document.documentElement.lang = lang === "en" ? "en" : "zh-CN";
    flattenUpdateItems();
    activeInspectItem = allUpdateItems[0] || null;
    pinnedInspectIdentity = null;
    render();
  }

  function text(value, replacements = {}) {
    let output = String(value || "");
    for (const [placeholder, replacement] of Object.entries(replacements)) {
      output = output.replaceAll(placeholder, replacement);
    }
    return output
      .replace(/\[\/?[a-z]+\]/gi, "")
      .replace(/\{[^}]+\}/g, "?")
      .replace(/\s+/g, " ")
      .trim();
  }

  function escapeHtml(value) {
    return String(value || "")
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");
  }

  function escapeAttr(value) {
    return escapeHtml(value).replace(/"/g, "&quot;");
  }

  function cssString(value) {
    if (window.CSS?.escape) return CSS.escape(value);
    return String(value).replace(/\\/g, "\\\\").replace(/"/g, '\\"');
  }

  function localize(item, field) {
    const direct = lang === "en" ? item[field + "En"] || item[field] : item[field];
    if (direct) return text(direct, item.replacements);
    const key = item[field + "Key"];
    const namespace = item.namespace;
    return text((loc[namespace] || {})[key] || key || "", item.replacements);
  }

  function detailLabel(entry) {
    if (Array.isArray(entry)) return text(entry[0] || "");
    const direct = lang === "en" ? entry.labelEn || entry.label : entry.label;
    return text(direct || "");
  }

  function detailBody(item, entry) {
    if (Array.isArray(entry)) return text(entry[1] || "", item.replacements);
    const direct = lang === "en" ? entry.textEn || entry.text : entry.text;
    if (direct) return text(direct, item.replacements);
    if (entry.key) {
      const namespace = entry.namespace || item.namespace;
      return text((loc[namespace] || {})[entry.key] || entry.key, item.replacements);
    }
    return "";
  }

  function detailSearchText(item) {
    return ([...(item.details || []), ...extraSourceDetails(item)])
      .map((entry) => [detailLabel(entry), detailBody(item, entry)].join(" "))
      .join(" ");
  }

  function isPlaceholderVanilla(value) {
    const normalized = text(value).replace(/\s+/g, " ").trim().toLowerCase();
    if (!normalized) return true;
    return [
      "\u539f\u7248\u65e0\u6b64",
      "\u539f\u7248\u9996\u9886\u6ca1\u6709 a19 \u4e13\u5c5e\u80fd\u529b\u6216 a20 \u70d9\u5370\u5f62\u6001",
      "no equivalent vanilla content",
      "vanilla bosses do not have a19 dedicated abilities or a20 branded form"
    ].some((snippet) => normalized.includes(snippet));
  }

  function renderItemDetails(item) {
    const details = [...(item.details || []), ...extraSourceDetails(item)];
    if (!details.length) return "";
    let rowsHtml = "";
    for (const entry of details) {
      const label = detailLabel(entry);
      const body = detailBody(item, entry);
      if (!label && !body) continue;
      rowsHtml += `
        <div class="detail-row">
          <strong class="detail-title">${label}</strong>
          <span class="detail-copy">${formatStsText(body)}</span>
        </div>
      `;
    }
    return `
      <details class="item-details" open>
        <summary>${labels.effectDetails || labels.expandDetails || "具体效果"}</summary>
        <div class="detail-grid">${rowsHtml}</div>
      </details>
    `;
  }

  function extraSourceDetails(item) {
    if (item?.descKey !== "BOSS_SEAL_CHOSEN_DECREE.summary") return [];
    return [
      {
        label: "威仪",
        labelEn: "Majesty",
        text: "下一次防御或屏障动作每层额外获得[blue]8[/blue]点格挡。A19最多[blue]2[/blue]层；A20烙印形态最多[blue]3[/blue]层，且一次防御或屏障最多消耗[blue]2[/blue]层。",
        textEn: "The next defense or barrier action gains [blue]8[/blue] extra Block per stack. A19 caps at [blue]2[/blue]; A20 Branded Form caps at [blue]3[/blue] and can spend at most [blue]2[/blue] stacks on one defense or barrier action."
      },
      {
        label: "御令牌",
        labelEn: "Royal Decree card",
        text: "本回合打出被标记的束缚牌，可以避免御令惩罚。打出非御令束缚牌时，女王获得[blue]1[/blue]层威仪。",
        textEn: "Playing the marked Bound card this turn avoids the decree penalty. Playing a non-Decree Bound card gives Queen [blue]1[/blue] Majesty."
      }
    ];
  }

  function findUpdateItemByKey(key) {
    return allUpdateItems.find((item) => itemKey(item) === key);
  }

  function itemTitle(item) {
    if (!item) return "";
    return localize(item, "title") || text(item.title || item.titleEn || item.i18nKey || itemKey(item));
  }

  function itemSearchText(item) {
    if (!item) return "";
    const vanilla = text(lang === "en"
      ? item.vanillaEn || item.vanilla || item.groupDefaultVanillaEn || item.groupDefaultVanilla
      : item.vanilla || item.groupDefaultVanilla);
    return [
      itemTitle(item),
      localize(item, "desc") || text(item.current),
      isPlaceholderVanilla(vanilla) ? "" : vanilla,
      detailSearchText(item),
      (item.tags || []).join(" ")
    ].join(" ");
  }

  function relationRules() {
    return [
      {
        terms: ["档案页", "临时页", "Archive Page", "Archive Pages"],
        mechanics: ["archive_pages", "temporary"],
        items: [
          "EZMB_MORVI.pages.INITIAL.options.morvi_overdue_library.description",
          "EZMB_MORVI_ARCHIVE_DRAW_PAGE.description",
          "EZMB_MORVI_ARCHIVE_VEIL_PAGE.description",
          "EZMB_MORVI_ARCHIVE_BURN_PAGE.description",
          "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.description",
          "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.description",
          "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.description"
        ]
      },
      {
        terms: ["苗床", "枯壳", "Seedbed", "Withered Husk"],
        mechanics: ["seedbed", "temporary", "blight_sprout"],
        items: [
          "EZMB_URDA.pages.INITIAL.options.urda_seedbed.description",
          "EZMB_URDA_SEEDBED.description",
          "EZMB_WITHERED_HUSK.description",
          "EZMB_ROOT_BUD.description"
        ]
      },
      {
        terms: ["根蚀", "根芽", "Rootblight", "Blight Sprout"],
        mechanics: ["rootblight", "blight_sprout"],
        items: [
          "LEVEL_14.description",
          "LEVEL_15.description",
          "LEVEL_18.description",
          "EZMB_ROOT.description",
          "EZMB_DEEP_ROOT.description",
          "EZMB_ROOTBLIGHT_III.description",
          "EZMB_ROOT_BUD.description"
        ]
      },
      {
        terms: ["血债", "赃物锁", "契约", "Blood Debt", "Stolen Lock", "Contract"],
        mechanics: ["blood_debt", "stolen_lock", "contract"],
        items: [
          "VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description",
          "EZMB_VAKUU_KNIFE_CONTRACT.description",
          "EZMB_VAKUU_TEMPTATION.description",
          "EZMB_VAKUU_SHELTER_CONTRACT.description",
          "EZMB_VAKUU_TRICK_CONTRACT.description",
          "EZMB_VAKUU_CASH_OUT_CONTRACT.description"
        ]
      },
      {
        terms: ["裁决", "延期判决", "Verdict", "Deferred Verdict"],
        mechanics: ["verdict", "replay"],
        items: [
          "EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description"
        ]
      },
      {
        terms: ["御令", "威仪", "束缚", "Royal Decree", "Majesty", "Bound"],
        mechanics: ["royal_decree", "majesty", "bound"],
        items: [
          "BOSS_SEAL_CHOSEN_DECREE.summary"
        ]
      },
      {
        terms: ["债务", "Debt", "红墨", "Red Ink"],
        mechanics: ["debt", "red_ink_debt"],
        items: [
          "SEAL_OF_GOLD.description",
          "EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.description",
          "EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.description",
          "EZMB_MORVI_RED_INK_OVERDRAFT.description"
        ]
      },
      {
        terms: ["裂变", "火印", "溢火", "铸令", "战旗", "Fission", "Firemark", "Overflow", "Forge Token", "Banner"],
        mechanics: ["fission", "firemark", "overflow", "forge_token", "banner"],
        items: [
          "LEVEL_12.description",
          "LEVEL_13.description",
          "LEVEL_16.description",
          "FIREMARK_MIGHT.description",
          "FIREMARK_GIANT.description",
          "FIREMARK_FORGE_ARMOR.description",
          "FIREMARK_CONSTANT_HEAL.description",
          "BANNER_VANGUARD.description",
          "BANNER_SHIELDWALL.description",
          "BANNER_BLOOD_PRIZE.description",
          "BANNER_PRESSING_LINE.description",
          "BANNER_LAST_STAND.description"
        ]
      },
      {
        terms: ["执迷", "Enthralled"],
        items: ["BLOOD_SOAKED_ROSE.description", "ENTHRALLED.description"]
      },
      {
        terms: ["愚行", "Folly"],
        items: ["PRESERVED_FOG.description", "FOLLY.description"]
      }
    ];
  }

  function relatedForItem(item) {
    const currentKey = itemKey(item);
    const content = itemSearchText(item).toLowerCase();
    const relatedItems = new Map();
    const relatedMechanics = new Map();

    function addItem(key) {
      if (!key || key === currentKey || relatedItems.has(key)) return;
      const target = findUpdateItemByKey(key);
      if (target) relatedItems.set(key, target);
    }

    function addMechanic(id) {
      if (!id || relatedMechanics.has(id)) return;
      const mechanic = getMechanic(id);
      if (mechanic) relatedMechanics.set(id, mechanic);
    }

    for (const mechanic of data.mechanics || []) {
      const terms = lang === "en" ? mechanic.termsEn || mechanic.terms || [] : mechanic.terms || [];
      if (terms.some((term) => content.includes(String(term).toLowerCase()))) addMechanic(mechanic.id);
    }

    for (const rule of relationRules()) {
      if (rule.terms.some((term) => content.includes(String(term).toLowerCase()))) {
        (rule.items || []).forEach(addItem);
        (rule.mechanics || []).forEach(addMechanic);
      }
    }

    (item.relatedItems || []).forEach(addItem);
    (item.relatedMechanics || []).forEach(addMechanic);

    return {
      items: [...relatedItems.values()].slice(0, 12),
      mechanics: [...relatedMechanics.values()].slice(0, 12)
    };
  }

  function relatedForMechanic(mechanic) {
    const relatedItems = (mechanic.relatedItemKeys || [])
      .map(findUpdateItemByKey)
      .filter(Boolean)
      .slice(0, 12);
    const relatedMechanics = (mechanic.relatedMechanicIds || [])
      .map(getMechanic)
      .filter(Boolean)
      .slice(0, 12);
    return { items: relatedItems, mechanics: relatedMechanics };
  }

  function renderRelatedLinks(item) {
    if (!item || item.mechanicHub) return "";
    const related = item.kind === "mechanic"
      ? relatedForMechanic(getMechanic(item.mechanicId) || {})
      : relatedForItem(item);
    const itemLinks = related.items.map((target) =>
      `<button type="button" class="link-chip" data-item-key="${escapeAttr(itemKey(target))}">${itemTitle(target)}</button>`
    ).join("");
    const mechanicLinks = related.mechanics.map((mechanic) =>
      `<button type="button" class="link-chip mechanic-chip" data-mechanic-id="${escapeAttr(mechanic.id)}">${getMechanicTitle(mechanic)}</button>`
    ).join("");
    if (!itemLinks && !mechanicLinks) return "";
    return `
      <section class="related-panel">
        ${mechanicLinks ? `
          <div class="related-block">
            <h4>${labels.relatedMechanics || "机制解释"}</h4>
            <div class="link-chip-row">${mechanicLinks}</div>
          </div>
        ` : ""}
        ${itemLinks ? `
          <div class="related-block">
            <h4>${labels.relatedChanges || "相关改动"}</h4>
            <div class="link-chip-row">${itemLinks}</div>
          </div>
        ` : ""}
      </section>
    `;
  }

  function MechanicCard(mechanic, compact = false) {
    const bullets = getMechanicBullets(mechanic);
    const bulletHtml = compact ? "" : `
      <ul>
        ${bullets.map((line) => `<li>${formatStsText(text(line))}</li>`).join("")}
      </ul>
    `;
    return `
      <article class="mechanic-card" id="mechanic-${escapeAttr(mechanic.id)}">
        <button type="button" class="mechanic-title" data-mechanic-id="${escapeAttr(mechanic.id)}">${getMechanicTitle(mechanic)}</button>
        <p>${formatStsText(getMechanicBody(mechanic))}</p>
        ${bulletHtml}
      </article>
    `;
  }

  function MechanicCodexComponent() {
    return `
      <section class="mechanic-codex">
        <h3>${labels.mechanicCodex || "机制资料库"}</h3>
        <div class="mechanic-codex-grid">
          ${(data.mechanics || []).map((mechanic) => MechanicCard(mechanic)).join("")}
        </div>
      </section>
    `;
  }

  function MechanicInspectorComponent(item) {
    const mechanic = getMechanic(item.mechanicId);
    if (!mechanic) return "";
    const tagsHtml = (item.tags || []).map(tag => `<span class="tag">${tag}</span>`).join("");
    return `
      <div class="inspector-card-preview mechanic-inspector">
        <div class="inspector-art-frame">
          ${renderImage(item.icon, item.title)}
        </div>
        <div class="inspector-header">
          <h2>${item.title}</h2>
          <div class="inspector-tags">${tagsHtml}</div>
        </div>
        ${InspectorActions(item)}
        <div class="inspector-comp">
          <div>
            <h4 class="inspector-sect-title">${labels.mechanicTag || "机制"}</h4>
            <div class="inspector-desc-block current-box">
              <p>${formatStsText(getMechanicBody(mechanic))}</p>
            </div>
          </div>
        </div>
        ${MechanicCard(mechanic)}
        ${renderRelatedLinks(item)}
      </div>
    `;
  }

  function formatStsText(text) {
    if (!text) return "";
    let escaped = escapeHtml(text);

    const mappings = [
      { key: "sts-keyword-red", words: ["伤害", "攻击", "造成", "生命", "HP", "Damage", "Attack", "Attacks", "deal", "deals"] },
      { key: "sts-keyword-blue", words: ["格挡", "防御", "Block", "Gain Block"] },
      { key: "sts-keyword-green", words: ["能量", "能量制限", "Energy", "energy"] },
      { key: "sts-keyword-purple", words: ["消耗", "虚无", "诅诅", "诅咒", "脆弱", "虚弱", "易伤", "Exhaust", "Ethereal", "Curse", "Curses", "Vulnerable", "Weak"] },
      { key: "sts-keyword-gold", words: ["遗物", "先古之民", "先古", "金币", "Relic", "Ancient", "Gold"] }
    ];

    let index = 0;
    const replacements = {};
    function register(word, className, link = null) {
      const marker = `__STS_MARKER_${index}__`;
      const safeWord = escapeHtml(word);
      if (link?.mechanicId) {
        replacements[marker] = `<button type="button" class="text-keyword-link ${className}" data-mechanic-id="${escapeAttr(link.mechanicId)}">${safeWord}</button>`;
      } else if (link?.itemKey) {
        replacements[marker] = `<button type="button" class="text-keyword-link ${className}" data-item-key="${escapeAttr(link.itemKey)}">${safeWord}</button>`;
      } else {
        replacements[marker] = `<span class="${className}">${safeWord}</span>`;
      }
      index++;
      return marker;
    }

    const flatKeywords = [];
    for (const mechanic of data.mechanics || []) {
      const terms = lang === "en" ? mechanic.termsEn || mechanic.terms || [] : mechanic.terms || [];
      for (const word of terms) {
        flatKeywords.push({
          word,
          className: mechanic.keywordClass || "sts-keyword-gold",
          link: { mechanicId: mechanic.id }
        });
      }
    }
    for (const group of mappings) {
      for (const word of group.words) {
        flatKeywords.push({ word, className: group.key });
      }
    }
    flatKeywords.sort((a, b) => b.word.length - a.word.length);

    function escapeRegExp(string) {
      return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    }

    for (const item of flatKeywords) {
      const regex = new RegExp(
        /^[a-zA-Z]/.test(item.word)
          ? '\\b' + escapeRegExp(item.word) + '\\b'
          : escapeRegExp(item.word),
        'gi'
      );
      escaped = escaped.replace(regex, (match) => register(match, item.className, item.link));
    }

    for (const marker in replacements) {
      escaped = escaped.replace(new RegExp(marker, 'g'), replacements[marker]);
    }

    return escaped;
  }

  function renderImage(src, alt) {
    if (!src) return `<img src="${fallbackIcon}" alt="${alt || ''}" />`;
    const pointsToLocalSource = src.includes("source%20code") || src.includes("source code");
    if (pointsToLocalSource && location.hostname.endsWith("github.io")) {
      return `<span class="source-art-placeholder" role="img" aria-label="${alt || labels.sourceArtPlaceholder} ${labels.sourceArtPlaceholder}">
        <strong>${titleInitials(alt)}</strong>
        <small>${labels.sourceArtPlaceholder || "原版"}</small>
      </span>`;
    }
    return `<img src="${src}" alt="${alt || ''}" onerror="this.onerror=null; this.src='${fallbackIcon}'" loading="lazy" decoding="async" />`;
  }

  function titleInitials(value) {
    const compact = Array.from(String(value || labels.sourceArtPlaceholder || "原版")
      .replace(/[^\p{Letter}\p{Number}]/gu, ""));
    return compact.slice(0, 2).join("") || (labels.sourceArtPlaceholder || "原版").slice(0, 2);
  }

  async function loadLocalization() {
    const result = {};
    await Promise.all(
      Object.entries(data.locFiles).map(async ([name, url]) => {
        const requestUrl = new URL(url, location.href);
        requestUrl.searchParams.set("v", appVersion);
        const response = await fetch(requestUrl, { cache: "no-cache" });
        result[name] = await response.json();
      })
    );
    return result;
  }

  function route() {
    const hash = location.hash.slice(1) || "updates";
    return routes.has(hash) ? hash : "updates";
  }

  function forumHref() {
    return isLocal() ? data.forum.localUrl : data.forum.url;
  }

  function forumEmbedHref() {
    const url = new URL(forumHref(), location.href);
    const forumRoute = new URLSearchParams(location.search).get("forumRoute");
    url.searchParams.set("embedded", "1");
    if (/^\/(?:new|posts\/[0-9a-f-]{36})$/i.test(forumRoute || "")) {
      url.hash = forumRoute;
    }
    return url.href;
  }

  function isLocal() {
    return ["", "localhost", "127.0.0.1", "::1"].includes(location.hostname);
  }

  function normalize(value) {
    return String(value || "").replace(/\s+/g, " ").trim().toLowerCase();
  }

  // --- Component Definitions ---

  function HeaderComponent(activeRoute) {
    const navLinks = [
      { id: "updates", label: labels.navUpdates },
      { id: "install", label: labels.navInstall },
      { id: "forum", label: labels.navForum },
      { id: "issues", label: labels.navIssues },
      { id: "about", label: labels.navAbout }
    ].map(item => `
      <a href="#${item.id}" class="${activeRoute === item.id ? 'active' : ''}" data-route="${item.id}">${item.label}</a>
    `).join("");

    return `
      <a href="#updates" class="brand" data-route="updates">
        ${renderImage("assets/ancients/urda/ezmb_urda_map_icon.png", "")}
        <span>
          <div class="brand-title-row">
            <strong>Spire Plus</strong>
            <div class="beating-heart-container">
              <div class="beating-heart"></div>
            </div>
          </div>
          <small>${labels.brandSub}</small>
        </span>
      </a>
      <nav class="site-nav" aria-label="main">
        ${navLinks}
        <div class="lang-switch">
          <button type="button" class="${lang === 'zh' ? 'active' : ''}" data-lang="zh">中</button>
          <button type="button" class="${lang === 'en' ? 'active' : ''}" data-lang="en">EN</button>
        </div>
      </nav>
    `;
  }

  function HeroComponent() {
    const imagesHtml = [
      "assets/events/ezmb_urda.png",
      "assets/events/ezmb_morvi.png",
      "assets/events/ezmb_lotha.png"
    ].map(src => renderImage(src, "")).join("");

    return `
      <section class="hero">
        <div class="hero-images">${imagesHtml}</div>
        <div class="hero-copy">
          ${labels.releaseLine ? `<p class="release-line">${labels.releaseLine}</p>` : ""}
          <h1>${labels.heroTitle}</h1>
          <p>${labels.heroCopy}</p>
          <div class="hero-choices-list">
            <button type="button" class="hero-choice-btn" data-choice="install">
              <span class="choice-prefix">[${lang === 'en' ? 'Option A' : '选项一'}]</span>
              <span class="choice-text">${labels.download || '下载模组'} (Receive Spire Plus. Lose 0 HP)</span>
            </button>
            <button type="button" class="hero-choice-btn" data-choice="updates">
              <span class="choice-prefix">[${lang === 'en' ? 'Option B' : '选项二'}]</span>
              <span class="choice-text">${lang === 'en' ? 'Browse balance improvements' : '浏览平衡性优化项目'}</span>
            </button>
          </div>
        </div>
      </section>
    `;
  }

  function IntroFeaturesComponent() {
    return `
      <section class="mod-intro-section">
        <h2 class="intro-heading">${labels.modIntroTitle}</h2>
        <div class="intro-grid">
          <div class="feat-card">
            <h3>${labels.featAscensionTitle}</h3>
            <p>${labels.featAscensionDesc}</p>
          </div>
          <div class="feat-card">
            <h3>${labels.featPhilosophyTitle}</h3>
            <p>${labels.featPhilosophyDesc}</p>
          </div>
          <div class="feat-card">
            <h3>${labels.featRewardTitle}</h3>
            <p>${labels.featRewardDesc}</p>
          </div>
        </div>
      </section>
    `;
  }

  function CodexControls() {
    const chipButtons = data.updateGroups.map(group => `
      <button type="button" class="chip" data-filter="${group.short}">${group.short}</button>
    `).join("");

    return `
      <section class="tool-row">
        <label class="search">
          <span>${labels.search}</span>
          <input id="updateSearch" type="search" placeholder="${labels.searchPlaceholder}" />
        </label>
        <div class="chips" id="updateFilters">
          <button type="button" class="chip active" data-filter="${labels.all}">${labels.all}</button>
          ${chipButtons}
        </div>
      </section>
    `;
  }

  function InspectorActions(item) {
    const locked = isPinned(item);
    return `
      <div class="inspector-actions">
        <button type="button" class="focus-btn ${locked ? "locked" : ""}" data-pin-current="true">
          ${locked ? labels.pinnedFocus || "已锁定" : labels.lockFocus || "锁定关注"}
        </button>
        ${locked ? `<button type="button" class="focus-btn ghost" data-unpin-current="true">${labels.unlockFocus || "取消锁定"}</button>` : ""}
      </div>
    `;
  }

  function CardInspectorComponent(item) {
    if (!item) {
      return `
        <div class="inspector-placeholder">
          <div class="inspector-placeholder-icon">🎴</div>
          <p>${lang === 'en' ? 'Hover over a card to inspect details' : '将鼠标悬停在卡牌上以查看详情'}</p>
        </div>
      `;
    }

    if (item.kind === "mechanic") return MechanicInspectorComponent(item);

    const title = localize(item, "title");
    const current = localize(item, "desc") || text(item.current);
    const vanilla = text(lang === 'en'
      ? item.vanillaEn || item.vanilla || item.groupDefaultVanillaEn || item.groupDefaultVanilla
      : item.vanilla || item.groupDefaultVanilla);
    const showVanilla = !isPlaceholderVanilla(vanilla);
    const tagsHtml = (item.tags || []).map(tag => `<span class="tag">${tag}</span>`).join("");

    return `
      <div class="inspector-card-preview">
        <div class="inspector-art-frame">
          ${renderImage(item.icon, title)}
        </div>
        <div class="inspector-header">
          <h2>${title}</h2>
          <div class="inspector-tags">${tagsHtml}</div>
        </div>
        ${InspectorActions(item)}
        <div class="inspector-comp">
          ${showVanilla ? `
            <div>
              <h4 class="inspector-sect-title">${labels.vanilla} (Vanilla)</h4>
              <div class="inspector-desc-block vanilla-box">
                <p>${formatStsText(vanilla)}</p>
              </div>
            </div>
          ` : ""}
          <div>
            <h4 class="inspector-sect-title">${labels.current} (Spire Plus)</h4>
            <div class="inspector-desc-block current-box">
              <p>${formatStsText(current)}</p>
            </div>
          </div>
        </div>
        ${renderItemDetails(item)}
        ${item.mechanicHub ? MechanicCodexComponent() : renderRelatedLinks(item)}
      </div>
    `;
  }

  function RelicCardComponent(item, index, isActive) {
    const title = localize(item, "title");
    const current = localize(item, "desc") || text(item.current);
    const vanilla = text(lang === 'en'
      ? item.vanillaEn || item.vanilla || item.groupDefaultVanillaEn || item.groupDefaultVanilla
      : item.vanilla || item.groupDefaultVanilla);
    const searchableVanilla = isPlaceholderVanilla(vanilla) ? "" : vanilla;
    const detailsText = detailSearchText(item);
    const tagsHtml = (item.tags || []).map(tag => `<span class="tag">${tag}</span>`).join("");
    const searchString = normalize([title, current, searchableVanilla, detailsText, (item.tags || []).join(" ")].join(" "));
    const identity = inspectIdentity(item);
    const pinnedClass = pinnedInspectIdentity === identity ? "pinned-inspect" : "";

    return `
      <article class="compare-card ${isActive ? 'active-inspect' : ''} ${pinnedClass}" style="--index: ${index}" data-search="${searchString}" data-index="${index}" data-item-key="${escapeAttr(itemKey(item))}">
        <div class="card-header-row">
          <div class="card-art-frame">
            ${renderImage(item.icon, title)}
          </div>
          <div class="card-title-block">
            <h3>${title}</h3>
            <div class="tags">${tagsHtml}</div>
          </div>
        </div>
        <dl>
          <dt>${labels.current}</dt>
          <dd class="sts-card-current">${formatStsText(current)}</dd>
        </dl>
      </article>
    `;
  }

  function UpdatesBoardComponent() {
    let cardIndex = 0;
    const groupsHtml = data.updateGroups.map(group => {
      const itemsHtml = group.items.map(item => {
        const itemWithDefaults = {
          ...item,
          icon: item.icon || group.icon,
          groupDefaultVanilla: group.defaultVanilla,
          groupDefaultVanillaEn: group.defaultVanillaEn
        };
        const isActive = activeInspectItem && itemKey(activeInspectItem) === itemKey(itemWithDefaults);
        return RelicCardComponent(itemWithDefaults, cardIndex++, isActive);
      }).join("");

      return `
        <section class="compare-group" data-group="${group.short}">
          <div class="group-head">
            ${renderImage(group.icon, group.title)}
            <div>
              <h2>${group.title}</h2>
              <p>${group.note}</p>
            </div>
          </div>
          <div class="compare-list">
            ${itemsHtml}
          </div>
        </section>
      `;
    }).join("");

    return `
      <section class="update-board">
        <aside class="inspector-pane" id="inspectorPane">
          ${CardInspectorComponent(activeInspectItem)}
        </aside>
        <div class="cards-pane">
          ${groupsHtml}
          <div id="searchEmptyState" class="empty-state hidden">
            <p>${labels.noSearchMatched || "没有找到匹配的改动。"}</p>
            <a href="#" class="clear-search-link">${labels.clearSearch || "清除搜索"}</a>
          </div>
        </div>
      </section>
    `;
  }

  function paintInspector() {
    const inspectorPane = app.querySelector("#inspectorPane");
    if (inspectorPane) {
      inspectorPane.innerHTML = CardInspectorComponent(activeInspectItem);
    }
    const activeIdentity = inspectIdentity(activeInspectItem);
    const pinnedIdentity = pinnedInspectIdentity;
    app.querySelectorAll(".compare-card").forEach((card) => {
      const identity = `item:${card.dataset.itemKey || ""}`;
      card.classList.toggle("active-inspect", identity === activeIdentity);
      card.classList.toggle("pinned-inspect", Boolean(pinnedIdentity && identity === pinnedIdentity));
    });
  }

  function selectInspectItem(item, { pin = false, reveal = false } = {}) {
    if (!item) return;
    activeInspectItem = item;
    if (pin) pinnedInspectIdentity = inspectIdentity(item);
    paintInspector();

    if (reveal && item.kind !== "mechanic") {
      const card = app.querySelector(`.compare-card[data-item-key="${cssString(itemKey(item))}"]`);
      if (card) {
        card.scrollIntoView({ block: "center", behavior: "smooth" });
      }
    }
  }

  function selectMechanic(id, { pin = true } = {}) {
    const item = mechanicInspectItem(id);
    if (item) selectInspectItem(item, { pin });
  }

  function unpinInspector() {
    pinnedInspectIdentity = null;
    paintInspector();
  }

  function SpireMapStepsComponent(title, steps, cssClass) {
    const listItems = steps.map((step, idx) => {
      // Map node indicators
      let stepIndicator = idx + 1;
      if (idx === 0) stepIndicator = "?";
      if (idx === 1) stepIndicator = "⚔";
      if (idx === 3) stepIndicator = "⛺";
      if (idx === steps.length - 1) stepIndicator = "☠";

      return `
        <li class="step-node">
          <span class="step-icon">${stepIndicator}</span>
          <div class="step-text">${step}</div>
        </li>
      `;
    }).join("");

    return `
      <section class="panel ${cssClass} spire-steps-container">
        <h2>${title}</h2>
        <ol class="spire-steps-list">
          ${listItems}
        </ol>
      </section>
    `;
  }

  function InstallComponent() {
    const local = isLocal();
    const primaryDownloadUrl = local ? data.package.localDownload : data.package.releaseDownload;

    let metaRows = "";
    for (const [key, value] of data.package.meta) {
      const isHash = key === "哈希" || key === "Hash" || key === "\u54c8\u5e0c";
      metaRows += `
        <dt>${key}</dt>
        <dd data-meta-field="${packageMetaField(key)}" class="${isHash ? 'hash-container' : ''}">
          ${isHash ? `
            <span class="hash">${value}</span>
            <button type="button" class="copy-hash-btn" data-action="copy-hash">${labels.copy || "复制"}</button>
          ` : value}
        </dd>
      `;
    }

    return `
      <div class="page-head">
        <h1>${labels.installTitle}</h1>
        <p class="lead">${labels.installLead}</p>
      </div>
      <section class="install-grid">
        <section class="merchant-shop">
          <h2>${labels.currentDownload}</h2>
          <div class="merchant-rug">
            <div class="merchant-speech" id="merchantSpeech">
              ${lang === 'en' ? 'Welcome, traveler! Looking for power?' : '欢迎，旅行者！在寻找登顶尖塔的力量吗？'}
            </div>
            <div class="download-group required-group">
              <h3 class="download-group-title">${labels.requiredFilesTitle || "第一步：下载必要文件 (Required Files)"}</h3>
              <div class="download-actions">
                <a id="primaryDownloadLink" href="${primaryDownloadUrl}" class="button primary" download data-hover="spireplus">${labels.download}</a>
                <a href="${data.package.baseLibRelease}" class="button" target="_blank" rel="noopener" data-hover="baselib">${labels.openBaseLib}</a>
              </div>
            </div>
            <div class="download-group">
              <h3 class="download-group-title">${labels.optionalFilesTitle || "辅助与源代码 (Optional Links)"}</h3>
              <div class="download-actions">
                <a id="releasesPageLink" href="${data.package.releasesPage}" class="button" target="_blank" rel="noopener" data-hover="releases">${labels.openRelease}</a>
                <a href="${data.package.repository}" class="button" target="_blank" rel="noopener" data-hover="repo">${labels.openRepo}</a>
              </div>
            </div>
            <dl class="meta" id="packageMetaTable">
              ${metaRows}
            </dl>
          </div>
        </section>
        <div class="install-guides-pane">
          ${SpireMapStepsComponent(labels.steps, data.installSteps, "install-steps")}
          <div style="margin-top: 24px;"></div>
          ${SpireMapStepsComponent(labels.requirements, data.requirements, "requirements-steps")}
        </div>
      </section>
    `;
  }

  function AboutComponent() {
    return `
      <div class="page-head">
        <h1>${labels.aboutTitle}</h1>
        <p class="lead">${labels.aboutLead}</p>
      </div>
      <section class="about-grid">
        <section class="panel">
          <h2>${labels.introTitle || "项目说明"}</h2>
          <p>${labels.introCopy || "这是一个专为《杀戮尖塔 2》私测开发包设计的平衡性优化模组伴侣站点。"}</p>
        </section>
        ${SpireMapStepsComponent(labels.assetPolicy, data.assetPolicy, "asset-policy")}
      </section>
    `;
  }

  function ForumComponent() {
    return `
      <section class="forum-integrated">
        <iframe id="forumFrame" class="forum-frame" title="${labels.forumPublicTitle || labels.navForum}" src="${forumEmbedHref()}" loading="eager" style="display: block; width: 100%; min-height: calc(100vh - 82px); border: 0;"></iframe>
      </section>
    `;
  }

  function IssuesComponent() {
    const issuesHtml = data.knownIssues.map(([level, title, body]) => `
      <article class="issue" data-level="${level}">
        <strong>${level}${labels.issueSeparator || " · "}${title}</strong>
        <p>${body}</p>
      </article>
    `).join("");

    const changesHtml = data.changeLog.map(([title, body]) => `
      <article class="change">
        <strong>${title}</strong>
        <p>${body}</p>
      </article>
    `).join("");

    return `
      <div class="page-head">
        <h1>${labels.issuesTitle}</h1>
        <p class="lead">${labels.issuesLead}</p>
      </div>
      <section class="issue-grid">
        <div>
          <h2 class="issue-column-title">${labels.knownIssues}</h2>
          <div class="issue-list">${issuesHtml}</div>
        </div>
        <div>
          <h2 class="issue-column-title">${labels.changeLog}</h2>
          <div class="change-list">${changesHtml}</div>
        </div>
      </section>
    `;
  }

  // --- Network & Release Hydration ---

  async function hydrateLatestRelease(downloadLink, releaseLink, meta) {
    if (!data.package.latestReleaseApi) return;
    try {
      const response = await fetch(data.package.latestReleaseApi, {
        headers: { Accept: "application/vnd.github+json" },
        cache: "no-cache"
      });
      if (!response.ok) return;

      const release = await response.json();
      const asset = chooseSpirePlusAsset(release.assets || []);
      if (!asset?.browser_download_url) return;

      downloadLink.href = asset.browser_download_url;
      if (release.html_url) releaseLink.href = release.html_url;
      setPackageMeta(meta, "file", asset.name);
      setPackageMeta(meta, "version", release.tag_name || release.name || "");
      setPackageMeta(meta, "size", formatAssetSize(asset.size));

      const hash = extractSha256(release.body || "");
      if (hash) setPackageMeta(meta, "hash", hash.toUpperCase());
    } catch {
      // Keep fallbacks active
    }
  }

  function chooseSpirePlusAsset(assets) {
    return assets.find(asset => /^SpirePlus.*\.zip$/i.test(asset.name || "")) ||
      assets.find(asset => /spire.*plus.*\.zip$/i.test(asset.name || ""));
  }

  function packageMetaField(key) {
    const value = String(key || "").toLowerCase();
    if (key.includes("文件") || value === "file") return "file";
    if (key.includes("版本") || value === "version") return "version";
    if (key.includes("体积") || value === "size") return "size";
    if (key.includes("哈希") || value === "hash") return "hash";
    return "";
  }

  function setPackageMeta(meta, field, value) {
    if (!value) return;
    const node = meta.querySelector(`[data-meta-field="${field}"]`);
    const hash = node?.querySelector(".hash");
    if (hash) hash.textContent = value;
    else if (node) node.textContent = value;
  }

  function formatAssetSize(size) {
    if (!Number.isFinite(size)) return "";
    return lang === "en" ? `${size.toLocaleString("en-US")} bytes` : `${size.toLocaleString("zh-CN")} 字节`;
  }

  function extractSha256(textValue) {
    return String(textValue || "").match(/\b[A-Fa-f0-9]{64}\b/)?.[0] || "";
  }

  function resizeForumFrame(frame) {
    try {
      const doc = frame.contentDocument;
      if (!doc) return;
      const minimum = Math.max(640, window.innerHeight - header.getBoundingClientRect().height);
      const height = Math.max(
        minimum,
        doc.documentElement.scrollHeight,
        doc.body?.scrollHeight || 0
      );
      frame.style.height = `${height}px`;
    } catch {
      frame.style.height = "calc(100vh - 82px)";
    }
  }

  window.addEventListener("message", (event) => {
    if (event.origin !== location.origin) return;
    if (event.data?.type === "spire-plus-forum-height") {
      const frame = document.getElementById("forumFrame");
      if (!frame) return;
      const minimum = Math.max(640, window.innerHeight - header.getBoundingClientRect().height);
      frame.style.height = `${Math.max(minimum, Number(event.data.height) || 0)}px`;
    } else if (event.data?.type === "spire-plus-forum-route") {
      const nextRoute = event.data.route;
      const url = new URL(location.href);
      if (nextRoute && nextRoute !== "/") {
        url.searchParams.set("forumRoute", nextRoute);
      } else {
        url.searchParams.delete("forumRoute");
      }
      history.replaceState(null, "", url.toString());

      const frame = document.getElementById("forumFrame");
      if (frame) {
        const headerOffset = header.getBoundingClientRect().height || 58;
        const elementPosition = frame.getBoundingClientRect().top + window.scrollY;
        const offsetPosition = elementPosition - headerOffset - 16;
        window.scrollTo({
          top: offsetPosition,
          behavior: "smooth"
        });
      }
    }
  });

  // --- Filtering & Search ---

  function applyFilters() {
    const queryInput = document.getElementById("updateSearch");
    const query = queryInput ? normalize(queryInput.value) : "";
    const filter = document.querySelector("#updateFilters .active")?.dataset.filter || labels.all;
    let totalVisible = 0;

    // Scan through groups
    for (const group of document.querySelectorAll(".compare-group")) {
      const filterMatch = filter === labels.all || group.dataset.group === filter;
      let visibleInGroup = 0;

      for (const card of group.querySelectorAll(".compare-card")) {
        const searchMatch = !query || card.dataset.search.includes(query);
        card.classList.toggle("hidden", !searchMatch);
        if (searchMatch) visibleInGroup += 1;
      }

      const shouldShowGroup = filterMatch && visibleInGroup > 0;
      group.classList.toggle("hidden", !shouldShowGroup);
      if (shouldShowGroup) totalVisible += visibleInGroup;
    }

    const emptyState = document.getElementById("searchEmptyState");
    if (emptyState) {
      emptyState.classList.toggle("hidden", totalVisible > 0);
    }
  }

  // --- Redraw Loop ---

  function render() {
    const current = route();
    document.title = "Spire Plus | " + (
      current === "install" ? labels.navInstall :
      current === "forum" ? labels.navForum :
      current === "issues" ? labels.navIssues :
      current === "about" ? labels.navAbout :
      labels.navUpdates
    );

    app.replaceChildren();

    if (current === "install") {
      app.innerHTML = InstallComponent();

      // Bind copy hash button
      const copyBtns = app.querySelectorAll(".copy-hash-btn");
      copyBtns.forEach(btn => {
        btn.addEventListener("click", () => {
          const hashSpan = btn.previousElementSibling;
          if (hashSpan) {
            navigator.clipboard.writeText(hashSpan.textContent).then(() => {
              btn.textContent = labels.copied || "已复制";
              btn.classList.add("copied");
              setTimeout(() => {
                btn.textContent = labels.copy || "复制";
                btn.classList.remove("copied");
              }, 1500);
            });
          }
        });
      });

      // Bind merchant shop hover dialog lines
      const speechBalloon = app.querySelector("#merchantSpeech");
      const hoverTargets = app.querySelectorAll("[data-hover]");
      if (speechBalloon) {
        hoverTargets.forEach(target => {
          target.addEventListener("mouseenter", () => {
            const hoverType = target.dataset.hover;
            let phrase = "";
            if (hoverType === "spireplus") {
              phrase = lang === 'en'
                ? "Ah, the Spire Plus mod file! Extends the map, rebalances ancient relics, and boosts scientific difficulty. Essential!"
                : "啊，Spire Plus 模组压缩包！包含全套先古重构与 A11-A20 难度选择，绝对是最佳优化！";
            } else if (hoverType === "baselib") {
              phrase = lang === 'en'
                ? "BaseLib 3.1.4! Don't forget this. The template framework won't load anything without it!"
                : "BaseLib 3.1.4！千万别漏掉这个，它是所有 StS2 模组的基础底层支持库！";
            } else if (hoverType === "releases") {
              phrase = lang === 'en'
                ? "Checking previous archives? Help yourself, we keep all releases structured."
                : "想翻阅我们历史归档的私测版本？都在 GitHub Release 界面，请自便！";
            } else if (hoverType === "repo") {
              phrase = lang === 'en'
                ? "Looking at the source logic? Be careful, the code contains dangerous ancient relics."
                : "想研究底层 C# 源码架构？没问题，完整开源仓库就在这里！";
            }
            speechBalloon.textContent = phrase;
          });

          target.addEventListener("mouseleave", () => {
            speechBalloon.textContent = lang === 'en'
              ? "Welcome, traveler! Looking for power?"
              : "欢迎，旅行者！在寻找登顶尖塔的力量吗？";
          });
        });
      }

      const dlLink = app.querySelector("#primaryDownloadLink");
      const relLink = app.querySelector("#releasesPageLink");
      const meta = app.querySelector("#packageMetaTable");
      if (dlLink && relLink && meta) {
        hydrateLatestRelease(dlLink, relLink, meta);
      }
    } else if (current === "forum") {
      app.innerHTML = ForumComponent();
      const frame = app.querySelector("#forumFrame");
      if (frame) {
        frame.addEventListener("load", () => resizeForumFrame(frame));
      }
    } else if (current === "issues") {
      app.innerHTML = IssuesComponent();
    } else if (current === "about") {
      app.innerHTML = AboutComponent();
    } else {
      // Updates page
      app.innerHTML = `
        ${HeroComponent()}
        ${IntroFeaturesComponent()}
        ${CodexControls()}
        ${UpdatesBoardComponent()}
      `;

      const input = app.querySelector("#updateSearch");
      if (input) {
        input.addEventListener("input", applyFilters);
      }

      // Bind filter runic chips
      const chips = app.querySelector("#updateFilters");
      if (chips) {
        chips.addEventListener("click", (event) => {
          const chip = event.target.closest(".chip");
          if (!chip) return;
          for (const item of chips.querySelectorAll(".chip")) item.classList.remove("active");
          chip.classList.add("active");
          applyFilters();
        });
      }

      const clearLink = app.querySelector(".clear-search-link");
      if (clearLink) {
        clearLink.addEventListener("click", (e) => {
          e.preventDefault();
          if (input) {
            input.value = "";
            applyFilters();
          }
        });
      }

      // Bind card inspection hover and click listener
      const compareCards = app.querySelectorAll(".compare-card");
      compareCards.forEach(card => {
        const index = parseInt(card.dataset.index);
        const item = allUpdateItems[index];

        const selectInspect = (pin = false) => {
          if (!item) return;
          if (!pin && pinnedInspectIdentity) return;
          selectInspectItem(item, { pin });
        };

        card.addEventListener("mouseenter", () => selectInspect(false));
        card.addEventListener("click", (event) => {
          if (event.target.closest(".text-keyword-link, .link-chip")) return;
          selectInspect(true);
        });
      });

      app.querySelectorAll(".text-keyword-link[data-mechanic-id], .link-chip[data-mechanic-id], .mechanic-title[data-mechanic-id]").forEach(link => {
        link.addEventListener("click", (event) => {
          event.preventDefault();
          event.stopPropagation();
          selectMechanic(link.dataset.mechanicId, { pin: true });
        });
      });

      app.querySelectorAll(".text-keyword-link[data-item-key], .link-chip[data-item-key]").forEach(link => {
        link.addEventListener("click", (event) => {
          event.preventDefault();
          event.stopPropagation();
          const item = findUpdateItemByKey(link.dataset.itemKey);
          selectInspectItem(item, { pin: true, reveal: true });
        });
      });

      app.querySelectorAll("[data-pin-current]").forEach(button => {
        button.addEventListener("click", (event) => {
          event.preventDefault();
          event.stopPropagation();
          if (activeInspectItem) {
            pinnedInspectIdentity = inspectIdentity(activeInspectItem);
            paintInspector();
          }
        });
      });

      app.querySelectorAll("[data-unpin-current]").forEach(button => {
        button.addEventListener("click", (event) => {
          event.preventDefault();
          event.stopPropagation();
          unpinInspector();
        });
      });

      // Bind Hero choice event choices actions
      const heroChoiceBtns = app.querySelectorAll(".hero-choice-btn");
      heroChoiceBtns.forEach(btn => {
        btn.addEventListener("click", () => {
          const choiceType = btn.dataset.choice;
          location.hash = choiceType;
        });
      });
    }

    header.innerHTML = HeaderComponent(current);
    window.scrollTo({ top: 0, behavior: "instant" });
  }

  window.addEventListener("hashchange", render);
  document.addEventListener("click", (event) => {
    const mechanicLink = event.target.closest(".text-keyword-link[data-mechanic-id], .link-chip[data-mechanic-id], .mechanic-title[data-mechanic-id]");
    if (mechanicLink) {
      event.preventDefault();
      event.stopPropagation();
      selectMechanic(mechanicLink.dataset.mechanicId, { pin: true });
      return;
    }

    const itemLink = event.target.closest(".text-keyword-link[data-item-key], .link-chip[data-item-key]");
    if (itemLink) {
      event.preventDefault();
      event.stopPropagation();
      const item = findUpdateItemByKey(itemLink.dataset.itemKey);
      selectInspectItem(item, { pin: true, reveal: true });
      return;
    }

    const pinButton = event.target.closest("[data-pin-current]");
    if (pinButton) {
      event.preventDefault();
      if (activeInspectItem) {
        pinnedInspectIdentity = inspectIdentity(activeInspectItem);
        paintInspector();
      }
      return;
    }

    const unpinButton = event.target.closest("[data-unpin-current]");
    if (unpinButton) {
      event.preventDefault();
      unpinInspector();
      return;
    }

    const langButton = event.target.closest("[data-lang]");
    if (langButton) {
      event.preventDefault();
      setLanguage(langButton.dataset.lang);
      return;
    }
    const link = event.target.closest("[data-route]");
    if (!link) return;
    event.preventDefault();
    location.hash = link.dataset.route;
  });

  render();
})();
