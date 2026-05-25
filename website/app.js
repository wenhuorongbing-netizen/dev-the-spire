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

  // State Management
  let allUpdateItems = [];
  let pinnedInspectItem = null;
  let activeInspectItem = null;
  let activeMechanicId = "blood_debt";

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
    // Append the Mechanics Codex virtual card
    allUpdateItems.push({
      title: "Spire Plus Codex & Rules",
      titleEn: "Spire Plus Codex & Rules",
      isMechanicsCodex: true,
      tags: [lang === 'en' ? "Rules" : "机制库", lang === 'en' ? "Codex" : "知识库"],
      icon: "assets/ancients/urda/ezmb_urda_map_icon.png",
      namespace: "custom_codex"
    });
  }
  flattenUpdateItems();

  activeInspectItem = allUpdateItems[0] || null;

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
    pinnedInspectItem = null;
    activeInspectItem = allUpdateItems[0] || null;
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
    return (item.details || [])
      .map((entry) => [detailLabel(entry), detailBody(item, entry)].join(" "))
      .join(" ");
  }

  function renderItemDetails(item) {
    if (!item.details?.length) return "";
    let rowsHtml = "";
    for (const entry of item.details) {
      const label = detailLabel(entry);
      const body = detailBody(item, entry);
      if (!label && !body) continue;
      rowsHtml += `
        <div class="detail-row">
          <strong class="detail-title">${label}</strong>
          <span class="detail-copy">${formatStsText(body, false)}</span>
        </div>
      `;
    }
    return `
      <details class="item-details">
        <summary>${labels.expandDetails || "展开具体效果"}</summary>
        <div class="detail-grid">${rowsHtml}</div>
      </details>
    `;
  }

  function formatStsText(textVal, isCurrent) {
    if (!textVal) return "";
    let escaped = textVal
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");

    const mappings = [
      { key: "sts-keyword-attack", words: ["攻击", "Attack", "Attacks"] },
      { key: "sts-keyword-skill", words: ["技能", "Skill", "Skills"] },
      { key: "sts-keyword-power", words: ["能力", "Power", "Powers"] },
      { key: "sts-keyword-status", words: ["状态", "Status", "Temporary Status", "临时状态"] },
      { key: "sts-keyword-curse", words: ["诅咒", "Curse", "Curses", "Temporary Curse", "临时诅咒"] },

      { key: "sts-keyword-stat-strength", words: ["力量", "Strength"] },
      { key: "sts-keyword-stat-dexterity", words: ["敏捷", "Dexterity"] },
      { key: "sts-keyword-stat-focus", words: ["集中", "Focus"] },
      { key: "sts-keyword-stat-vigor", words: ["活力", "Vigor"] },
      { key: "sts-keyword-stat-block", words: ["格挡", "防御", "Block", "Gain Block", "Defence"] },
      { key: "sts-keyword-stat-energy", words: ["能量", "能量制限", "Energy", "energy"] },

      { key: "sts-keyword-mech-blood-debt", words: ["血债", "Blood Debt", "赃物锁", "破锁赃物", "Loot Lock", "Loot-lock", "Loot"] },
      { key: "sts-keyword-mech-verdict", words: ["裁决", "延期裁决", "封庭", "Verdict", "Deferred Verdict", "Closed Court"] },
      { key: "sts-keyword-mech-relic", words: ["遗物", "Relic", "Relics"] },
      { key: "sts-keyword-mech-ancient", words: ["先古", "先古之民", "Ancient", "Ancients"] },
      { key: "sts-keyword-mech-gold", words: ["金币", "金", "Gold", "gold"] },
      { key: "sts-keyword-mech-fission", words: ["裂变", "Fission", "裂变牌", "裂变率", "裂变附魔"] },
      { key: "sts-keyword-mech-seedbed", words: ["苗床", "Seedbed", "枯壳", "Withered Husk"] },
      { key: "sts-keyword-mech-sprout", words: ["根芽", "Sprout", "Blight Sprout", "Sprouts"] },
      { key: "sts-keyword-mech-rootblight", words: ["根蚀", "Rootblight", "Rootblights"] },
      { key: "sts-keyword-mech-contract", words: ["契约", "Contract", "Contracts"] },
      { key: "sts-keyword-mech-temp-page", words: ["临时页", "Temporary Page", "页", "Page", "Pages"] }
    ];

    let index = 0;
    const replacements = {};
    function register(word, className) {
      const marker = `__STS_MARKER_${index}__`;
      replacements[marker] = `<span class="${className}">${word}</span>`;
      index++;
      return marker;
    }

    const flatKeywords = [];
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
      escaped = escaped.replace(regex, (match) => register(match, item.className));
    }

    for (const marker in replacements) {
      escaped = escaped.replace(new RegExp(marker, 'g'), replacements[marker]);
    }

    // Wrap numbers/changed digits in Spire Plus current text block
    if (isCurrent) {
      escaped = escaped.replace(/(?<!<[^>]*)\b(\d+(?:%|层|个|张|格|费|HP|金币|格挡|力量|敏捷)?|\+\d+|-\d+)\b/g, '<span class="sts-upgrade-val">$1</span>');
    }

    // Parse cross-card hyperlinks
    escaped = addCodexHyperlinks(escaped);

    return escaped;
  }

  function addCodexHyperlinks(htmlString) {
    const cardTerms = ["放松", "愚行", "执迷", "至亮之焰", "临时页", "契约", "雨息", "枯壳", "苗床", "根蚀", "根芽"];
    let output = htmlString;
    cardTerms.forEach(term => {
      const regex = new RegExp(`(?<!<[^>]*)${term}`, 'g');
      output = output.replace(regex, `<a href="#" class="codex-link" data-target="${term}">${term}</a>`);
    });
    return output;
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

  function CardInspectorComponent(item) {
    if (!item) {
      return `
        <div class="inspector-placeholder">
          <div class="inspector-placeholder-icon">🎴</div>
          <p>${lang === 'en' ? 'Hover over a card to inspect details' : '将鼠标悬停在卡牌上以查看详情'}</p>
        </div>
      `;
    }

    if (item.isMechanicsCodex) {
      const mechanics = [
        { id: "blood_debt", name: lang === 'en' ? "Blood Debt" : "血债与赃物锁", desc: lang === 'en' ? "Blood Debt: Added by Vakuu. When you have Blood Debt, taking unblocked damage increases your Blood Debt. You must play Contract cards to break Loot Locks or use rest site options to reduce it." : "血债与赃物锁：瓦库试炼特有负面层数。拥有血债时，受到未格挡的伤害会累积等量血债；每层血债会增加受到的生命流失。需要打出契约卡牌打破赃物锁，或在休息处使用切肉选项来减少血债。" },
        { id: "forge_token", name: lang === 'en' ? "Forge Token" : "威仪与铸令", desc: lang === 'en' ? "Forge Token: Earned by defeating Firemarked Elites. Can be used at the next rest site to either Rest (upgrades 1 random card) or Smith (heals 7 HP)." : "铸令与威仪：击败火印精英获得。可在下一个休息处使用：休息随机升级1张卡牌，锻造额外回复7 HP。" },
        { id: "verdict", name: lang === 'en' ? "Verdict" : "洛莎之裁决", desc: lang === 'en' ? "Verdict & Closed Court: Lotha's core mechanic. Delays damage or curse triggers, storing them as pending actions or cards in hand to be resolved in later phases." : "裁决与延期裁决：洛莎的核心机制。推迟生命流失或诅咒卡牌的结算，转化为挂起契约或临时手牌状态，允许玩家在后续回合中通过特定卡牌手段进行化解。" },
        { id: "fission", name: lang === 'en' ? "Fission" : "裂变附魔", desc: lang === 'en' ? "Fission: Ascension 13 modifier. Fission cards in card rewards have their cost reduced by 1 but are Exhausted after being played." : "裂变：进阶13开启的特殊附魔。卡牌奖励中的攻击或技能牌有几率获得裂变附魔，卡牌耗能-1，但打出后获得消耗属性。" },
        { id: "seedbed", name: lang === 'en' ? "Seedbeds & Sprouts" : "苗床与根芽", desc: lang === 'en' ? "Seedbeds & Sprouts: Urda and Ascension 14/15/18 mechanic. Seedbeds allow planting temporary status cards. Blight Sprouts surface during combats and add Rootblight I if left unresolved." : "苗床与根芽：乌尔姖与进阶14/15/18的连带机制。苗床可以优先种下临时卡牌。根芽会在战斗中萌发，若在战后未能处理，会向牌组中加入永久的根蚀 I 诅咒。" },
        { id: "firemarked", name: lang === 'en' ? "Firemarked Elites" : "火印精英", desc: lang === 'en' ? "Firemarked Elites: Ascension 12. Enhanced elites with increased HP and Strength. Defeating them grants Forge Tokens and 4-card rewards." : "火印精英：进阶12引入的强化精英。精英会随机获得火印宿主能力（力量、生命、熔甲、回复），击败后获得铸令，卡牌奖励提升为4选1。" },
        { id: "banner", name: lang === 'en' ? "Banner Rooms" : "战旗房", desc: lang === 'en' ? "Banner Rooms: Ascension 16. Enhanced combat rooms containing a Bannerbearer (Vanguard, Shieldwall, etc.). Offers higher rewards and a 15% Fission card chance." : "战旗房：进阶16引入的强化普通战斗。会随机刷出战旗效果（先锋、盾阵、血赏等），战斗奖励卡牌具有15%的裂变几率。" },
        { id: "deepbranch", name: lang === 'en' ? "Deep Branch" : "深层支线", desc: lang === 'en' ? "Deep Branch: Ascension 17. High-risk optional branch routes in Act 2 and Act 3 that reconnect to the main route and award an extra Uncommon relic." : "深层支线：进阶17引入的分支路线。第二、三幕中会插入3-4节点长度的极度危险支线，通往支线终点会额外赠送一个罕见遗物。" }
      ];

      let menuItemsHtml = "";
      mechanics.forEach(m => {
        const isActive = activeMechanicId === m.id;
        menuItemsHtml += `
          <button type="button" class="codex-menu-item ${isActive ? 'active-menu-item' : ''}" data-mechanic="${m.id}">
            <strong>${m.name}</strong>
            <span>${m.desc.slice(0, 20)}...</span>
          </button>
        `;
      });

      const activeMech = mechanics.find(m => m.id === activeMechanicId) || mechanics[0];

      return `
        <div class="inspector-card-preview">
          <div class="inspector-header">
            <h2>${lang === 'en' ? 'Mechanics Codex' : '先古与进阶机制详解'}</h2>
            <span class="tag">${lang === 'en' ? 'Rules Reference' : '机制规则速查'}</span>
            ${pinnedInspectItem && pinnedInspectItem.isMechanicsCodex ? `
              <div style="margin-top: 6px;">
                <span class="pin-badge">${lang === 'en' ? 'Locked' : '已锁定'}</span>
                <button type="button" class="clear-pin-btn" id="clearPinBtn">${lang === 'en' ? 'Clear' : '解锁'}</button>
              </div>
            ` : ""}
          </div>
          <div class="codex-menu">
            ${menuItemsHtml}
          </div>
          <div class="inspector-comp" style="margin-top: 8px;">
            <h4 class="inspector-sect-title">${activeMech.name}</h4>
            <div class="inspector-desc-block current-box">
              <p>${formatStsText(activeMech.desc, false)}</p>
            </div>
          </div>
        </div>
      `;
    }

    const title = localize(item, "title");
    const current = localize(item, "desc") || text(item.current);
    const vanilla = text(lang === 'en'
      ? item.vanillaEn || item.vanilla || item.groupDefaultVanillaEn || item.groupDefaultVanilla
      : item.vanilla || item.groupDefaultVanilla);
    const tagsHtml = (item.tags || []).map(tag => `<span class="tag">${tag}</span>`).join("");
    const isPinned = pinnedInspectItem && itemKey(pinnedInspectItem) === itemKey(item);

    return `
      <div class="inspector-card-preview">
        <div class="inspector-art-frame">
          ${renderImage(item.icon, title)}
        </div>
        <div class="inspector-header">
          <h2>${title}</h2>
          <div class="inspector-tags">${tagsHtml}</div>
          ${isPinned ? `
            <div style="margin-top: 6px;">
              <span class="pin-badge">${lang === 'en' ? 'Locked' : '已锁定'}</span>
              <button type="button" class="clear-pin-btn" id="clearPinBtn">${lang === 'en' ? 'Clear' : '解锁'}</button>
            </div>
          ` : ""}
        </div>
        <div class="inspector-comp">
          <div>
            <h4 class="inspector-sect-title">${labels.vanilla} (Vanilla)</h4>
            <div class="inspector-desc-block vanilla-box">
              <p>${formatStsText(vanilla, false)}</p>
            </div>
          </div>
          <div>
            <h4 class="inspector-sect-title">${labels.current} (Spire Plus)</h4>
            <div class="inspector-desc-block current-box">
              <p>${formatStsText(current, true)}</p>
            </div>
          </div>
        </div>
        ${renderItemDetails(item)}
      </div>
    `;
  }

  function RelicCardComponent(item, index, isActive) {
    const title = localize(item, "title");
    const current = localize(item, "desc") || text(item.current);
    const vanilla = text(lang === 'en'
      ? item.vanillaEn || item.vanilla || item.groupDefaultVanillaEn || item.groupDefaultVanilla
      : item.vanilla || item.groupDefaultVanilla);
    const detailsText = detailSearchText(item);
    const tagsHtml = (item.tags || []).map(tag => `<span class="tag">${tag}</span>`).join("");

    // Create search payload containing tags, titles, and text
    const searchString = normalize([title, current, vanilla, detailsText, (item.tags || []).join(" ")].join(" "));
    const isPinned = pinnedInspectItem && itemKey(pinnedInspectItem) === itemKey(item);

    return `
      <article class="compare-card ${isActive ? 'active-inspect' : ''}" style="--index: ${index}" data-search="${searchString}" data-index="${index}">
        ${isPinned ? `<span class="pin-badge">${lang === 'en' ? 'Locked' : '已锁定'}</span>` : ""}
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
          <dd class="sts-card-current">${formatStsText(current, true)}</dd>
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

    const codexItem = allUpdateItems.find(item => item.isMechanicsCodex);
    const codexCardIndex = allUpdateItems.indexOf(codexItem);
    const isCodexActive = activeInspectItem && activeInspectItem.isMechanicsCodex;
    const codexCardHtml = RelicCardComponent(codexItem, codexCardIndex, isCodexActive);

    return `
      <section class="update-board">
        <aside class="inspector-pane" id="inspectorPane">
          ${CardInspectorComponent(activeInspectItem)}
        </aside>
        <div class="cards-pane">
          ${groupsHtml}
          <section class="compare-group" data-group="${lang === 'en' ? 'Rules' : '机制库'}">
            <div class="group-head">
              ${renderImage(codexItem.icon, codexItem.title)}
              <div>
                <h2>${lang === 'en' ? 'Rules & Codex' : '先古与进阶机制详解'}</h2>
                <p>${lang === 'en' ? 'Quick reference guide for new gameplay mechanisms' : 'Spire Plus 新增机制规则速查指南'}</p>
              </div>
            </div>
            <div class="compare-list">
              ${codexCardHtml}
            </div>
          </section>
          <div id="searchEmptyState" class="empty-state hidden">
            <p>${labels.noSearchMatched || "没有找到匹配的改动。"}</p>
            <a href="#" class="clear-search-link">${labels.clearSearch || "清除搜索"}</a>
          </div>
        </div>
      </section>
    `;
  }

  function SpireMapStepsComponent(title, steps, cssClass) {
    const listItems = steps.map((step, idx) => {
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

  // --- Router & Jump Actions ---

  function triggerJump(target) {
    const allChip = document.querySelector("#updateFilters [data-filter='" + labels.all + "']");
    if (allChip) {
      for (const chip of document.querySelectorAll("#updateFilters .chip")) chip.classList.remove("active");
      allChip.classList.add("active");
    }

    const input = document.getElementById("updateSearch");
    if (input) {
      input.value = target;
      applyFilters();
    }

    const matchedCard = Array.from(document.querySelectorAll(".compare-card:not(.hidden)"))[0];
    if (matchedCard) {
      matchedCard.click();
      matchedCard.scrollIntoView({ behavior: "smooth", block: "center" });
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
      // Updates view
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

      // Card inspector mouseenter / click logic
      const compareCards = app.querySelectorAll(".compare-card");
      const inspectorPane = app.querySelector("#inspectorPane");
      const cardsPane = app.querySelector(".cards-pane");

      compareCards.forEach(card => {
        const index = parseInt(card.dataset.index);
        const item = allUpdateItems[index];

        const selectInspect = () => {
          if (!item) return;
          activeInspectItem = item;
          if (inspectorPane) {
            inspectorPane.innerHTML = CardInspectorComponent(item);
          }
          // Highlight active look only
          compareCards.forEach(c => c.classList.remove("active-inspect"));
          card.classList.add("active-inspect");
        };

        card.addEventListener("mouseenter", () => {
          // If not pinned, or temporarily previewing, update active inspect
          selectInspect();
        });

        card.addEventListener("click", (e) => {
          e.stopPropagation();
          if (pinnedInspectItem && itemKey(pinnedInspectItem) === itemKey(item)) {
            // Unpin if clicked again
            pinnedInspectItem = null;
          } else {
            // Pin this card
            pinnedInspectItem = item;
          }
          render(); // Refresh list to update locking states
        });
      });

      // Restore pinned inspect card on leaving card pane
      if (cardsPane) {
        cardsPane.addEventListener("mouseleave", () => {
          activeInspectItem = pinnedInspectItem || allUpdateItems[0];
          if (inspectorPane) {
            inspectorPane.innerHTML = CardInspectorComponent(activeInspectItem);
          }
          compareCards.forEach(c => {
            const index = parseInt(c.dataset.index);
            const item = allUpdateItems[index];
            const isInspect = activeInspectItem && itemKey(activeInspectItem) === itemKey(item);
            c.classList.toggle("active-inspect", isInspect);
          });
        });
      }

      // Bind Clear Pin click handler inside inspector
      document.addEventListener("click", (e) => {
        const clearPinBtn = e.target.closest("#clearPinBtn");
        if (clearPinBtn) {
          pinnedInspectItem = null;
          render();
        }
      });

      // Bind Hero choice log actions
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

  // Dynamic navigation click bindings (including cross-card hyperlinks)
  document.addEventListener("click", (event) => {
    // Cross-card codex link routing
    const codexLink = event.target.closest(".codex-link");
    if (codexLink) {
      event.preventDefault();
      const target = codexLink.dataset.target;
      if (route() !== "updates") {
        location.hash = "updates";
        setTimeout(() => triggerJump(target), 80);
      } else {
        triggerJump(target);
      }
      return;
    }

    // Language button routing
    const langButton = event.target.closest("[data-lang]");
    if (langButton) {
      event.preventDefault();
      setLanguage(langButton.dataset.lang);
      return;
    }

    // Tab button routing
    const link = event.target.closest("[data-route]");
    if (!link) return;
    event.preventDefault();
    location.hash = link.dataset.route;
  });

  // Listener for Mechanics sub-menu clicks
  document.addEventListener("click", (event) => {
    const codexMenuItem = event.target.closest(".codex-menu-item");
    if (codexMenuItem) {
      event.stopPropagation();
      activeMechanicId = codexMenuItem.dataset.mechanic;
      const inspectorPane = document.getElementById("inspectorPane");
      if (inspectorPane) {
        inspectorPane.innerHTML = CardInspectorComponent(activeInspectItem);
      }
    }
  });

  render();
})();
