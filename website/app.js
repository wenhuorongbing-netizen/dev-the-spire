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
  let currentSearchQuery = "";
  let currentFilter = ""; // Empty string represents "All"
  let lastActiveRoute = "";
  let isUpdatesPageLoaded = false;

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
    isUpdatesPageLoaded = false;
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
      { key: "sts-keyword-attack", words: ["攻击牌", "攻击", "Attack", "Attacks"] },
      { key: "sts-keyword-skill", words: ["技能牌", "技能", "Skill", "Skills"] },
      { key: "sts-keyword-power", words: ["能力牌", "能力", "Power", "Powers"] },
      { key: "sts-keyword-status", words: ["临时状态牌", "状态牌", "状态", "Status", "Temporary Status"] },
      { key: "sts-keyword-curse", words: ["临时诅咒牌", "临时诅咒", "诅咒牌", "诅咒", "Curse", "Curses", "Temporary Curse"] },
      { key: "sts-keyword-stat-strength", words: ["力量", "Strength"] },
      { key: "sts-keyword-stat-dexterity", words: ["敏捷", "Dexterity"] },
      { key: "sts-keyword-stat-focus", words: ["集中", "Focus"] },
      { key: "sts-keyword-stat-vigor", words: ["活力", "Vigor"] },
      { key: "sts-keyword-stat-block", words: ["格挡", "Block", "Gain Block", "Defence"] },
      { key: "sts-keyword-stat-energy", words: ["能量限制", "能量", "费用", "Energy", "energy"] },
      { key: "sts-keyword-mech-blood-debt", words: ["血债", "Blood Debt"] },
      { key: "sts-keyword-mech-verdict", words: ["延期判决", "裁决", "封庭", "Verdict", "Deferred Verdict", "Closed Court"] },
      { key: "sts-keyword-mech-relic", words: ["遗物", "Relic", "Relics"] },
      { key: "sts-keyword-mech-ancient", words: ["先古之民", "先古", "Ancient", "Ancients"] },
      { key: "sts-keyword-mech-gold", words: ["金币", "Gold", "gold"] },
      { key: "sts-keyword-mech-fission", words: ["裂变附魔", "裂变", "Fission"] },
      { key: "sts-keyword-mech-seedbed", words: ["苗床", "种下", "Seedbed", "Planting", "plant", "planted"] },
      { key: "sts-keyword-mech-sprout", words: ["根芽", "Sprout", "Blight Sprout", "Sprouts"] },
      { key: "sts-keyword-mech-rootblight", words: ["根蚀", "Rootblight", "Rootblights"] },
      { key: "sts-keyword-mech-contract", words: ["契约", "Contract", "Contracts"] },
      { key: "sts-keyword-mech-temp-page", words: ["临时页", "页", "Temporary Page", "Page", "Pages"] },
      { key: "sts-keyword-mech-prestige", words: ["威仪", "Prestige", "Majesty"] },
      { key: "sts-keyword-stat-plated-armor", words: ["多重护甲", "护甲", "Plated Armor", "Armor"] },
      { key: "sts-keyword-mech-ethereal", words: ["虚无", "Ethereal"] },
      { key: "sts-keyword-mech-exhaust", words: ["消耗牌", "消耗", "Exhaust"] },
      { key: "sts-keyword-mech-retain", words: ["保留牌", "保留", "Retain"] },
      { key: "sts-keyword-mech-unplayable", words: ["无法打出", "Unplayable"] },
      { key: "sts-keyword-mech-inherent", words: ["固有", "Inherent", "Innate"] },
      { key: "sts-keyword-status-vulnerable", words: ["易伤", "Vulnerable"] },
      { key: "sts-keyword-status-weak", words: ["虚弱", "Weak"] },
      { key: "sts-keyword-status-frail", words: ["脆弱", "Frail"] },
      { key: "sts-keyword-status-poison", words: ["中毒", "Poison"] },
      { key: "sts-keyword-stat-draw", words: ["抽牌", "抽", "Draw", "Draws"] },
      { key: "sts-keyword-stat-temp-hp", words: ["临时生命", "Temp HP"] },
      { key: "sts-keyword-mech-loot-lock", words: ["赃物锁", "Loot Lock", "Loot-lock"] },
      { key: "sts-keyword-mech-loot", words: ["赃物", "Loot"] },
      { key: "sts-keyword-mech-copy", words: ["复制", "Copy"] }
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
    const cardTerms = ["放松", "愚行", "执迷", "至亮之焰", "临时页", "契约", "雨息", "枯壳", "苗床", "根蚀", "根芽", "血债", "威仪", "裁决", "裂变", "火印精英", "战旗房", "深层支线"];
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
        try {
          const requestUrl = new URL(url, location.href);
          if (location.protocol !== "file:") {
            requestUrl.searchParams.set("v", appVersion);
          }
          const response = await fetch(requestUrl, { cache: "no-cache" });
          if (!response.ok) throw new Error(`Status ${response.status}`);
          result[name] = await response.json();
        } catch (err) {
          console.error(`Failed to load localization file ${name}:`, err);
          result[name] = {};
        }
        if (!result[name] || Object.keys(result[name]).length === 0) {
          const embeddedLang = lang === "en" ? "en" : "zh";
          result[name] = window.SPIRE_PLUS_EMBEDDED_LOC?.[embeddedLang]?.[name] || {};
        }
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
    const mainImg = renderImage("assets/events/ezmb_urda.png", lang === 'en' ? 'Urda Event' : '先古之民 乌尔达');
    const thumb1 = renderImage("assets/events/ezmb_morvi.png", lang === 'en' ? 'Morvi Event' : '先古之民 默维');
    const thumb2 = renderImage("assets/events/ezmb_lotha.png", lang === 'en' ? 'Lotha Event' : '先古之民 罗萨');

    return `
      <section class="hero">
        <div class="hero-copy">
          ${labels.releaseLine ? `<p class="release-line">${labels.releaseLine}</p>` : ""}
          <h1>${labels.heroTitle}</h1>
          <p>${labels.heroCopy}</p>
          <div class="hero-choices-list">
            <button type="button" class="hero-choice-btn" data-choice="install">
              <span class="choice-prefix">[I]</span>
              <span class="choice-text"><strong>${labels.download || (lang === 'en' ? 'Download Mod Package' : '下载当前测试包')}</strong> (Lose 0 HP)</span>
            </button>
            <button type="button" class="hero-choice-btn" data-choice="updates">
              <span class="choice-prefix">[II]</span>
              <span class="choice-text"><strong>${lang === 'en' ? 'View Balance Archive' : '查看平衡性档案'}</strong></span>
            </button>
          </div>
        </div>
        <div class="hero-images">
          ${mainImg}
          ${thumb1}
          ${thumb2}
        </div>
      </section>
    `;
  }

  function IntroFeaturesComponent() {
    return `
      <section class="mod-intro-section">
        <h2 class="intro-heading">${labels.modIntroTitle}</h2>
        
        <div class="qol-feature-banner">
          <div class="qol-banner-content">
            <span class="qol-badge">${lang === 'en' ? 'Quality of Life (QoL)' : '游戏体验便利性优化 (QoL)'}</span>
            <h3>${lang === 'en' ? 'Crystal Sphere Foresight & Card Transform Preview' : '水晶球透视 与 卡牌变化预览'}</h3>
            <p class="qol-banner-desc">
              ${lang === 'en' 
                ? 'Spire Plus integrates practical quality-of-life tools directly into the game: Crystal Sphere Foresight allows you to click a button to peek through cells, revealing their contents, and Card Transform Preview displays the exact card you will receive before confirming.'
                : 'Spire Plus 直接在游戏内集成了实用的便利性（QoL）功能：水晶球「预知」允许玩家点击按钮提前透视格子内容，而「卡牌变化预览」则能在确认卡牌变化前展示锁定的结果。'}
            </p>
            <div class="qol-banner-features">
              <div class="qol-banner-feat-item">
                <strong>${lang === 'en' ? '🔮 Crystal Sphere Foresight' : '🔮 水晶球预知'}</strong>
                <span>${lang === 'en' ? 'Click a button to reveal all grid contents under semi-transparent fog, planning your flips perfectly without save-scumming.' : '点击预知按钮直接透视所有格子内容，合理规划路线，告别繁琐的存档读档。'}</span>
              </div>
              <div class="qol-banner-feat-item">
                <strong>${lang === 'en' ? '🃏 Card Transform Preview' : '🃏 卡牌变化预览'}</strong>
                <span>${lang === 'en' ? 'Displays the exact resulting card in the preview panel before you commit, eliminating blind gambling.' : '在确认变化前，直接在预览面板中显示该次变化锁定的最终卡牌，告别盲盒赌博。'}</span>
              </div>
            </div>
          </div>
          <div class="qol-banner-image-container inspector-preview-img-wrapper" title="${lang === 'en' ? 'Click to enlarge' : '点击查看大图'}">
            <img class="qol-banner-img inspector-preview-img" src="assets/previews/crystal_sphere_peek.png" alt="${lang === 'en' ? 'Crystal Sphere Peek Preview' : '水晶球预知透视效果'}" />
          </div>
        </div>

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
    const activeFilter = currentFilter || labels.all;
    const chipButtons = data.updateGroups.map(group => {
      const isActive = currentFilter === group.short;
      return `<button type="button" class="chip ${isActive ? 'active' : ''}" data-filter="${group.short}">${group.short}</button>`;
    }).join("");

    const isAllActive = !currentFilter || currentFilter === labels.all;

    return `
      <section class="tool-row">
        <label class="search">
          <span>${labels.search}</span>
          <input id="updateSearch" type="search" placeholder="${labels.searchPlaceholder}" value="${currentSearchQuery || ''}" />
        </label>
        <div class="chips" id="updateFilters">
          <button type="button" class="chip ${isAllActive ? 'active' : ''}" data-filter="${labels.all}">${labels.all}</button>
          ${chipButtons}
        </div>
      </section>
    `;
  }

  function localizeMechanic(mech) {
    if (!mech) return null;
    const isEn = (lang === "en");
    return {
      id: mech.id,
      title: isEn ? (mech.titleEn || mech.title) : mech.title,
      desc: isEn ? (mech.descEn || mech.desc) : mech.desc,
      bullets: isEn ? (mech.bulletsEn || mech.bullets || []) : (mech.bullets || []),
      icon: mech.icon,
      keywordClass: mech.keywordClass || "sts-keyword-gold"
    };
  }

  function findRelatedMechanics(item) {
    if (!data.mechanics) return [];
    const title = normalize(localize(item, "title"));
    const desc = normalize(localize(item, "desc") || item.current || "");
    const details = normalize(detailSearchText(item));
    const combinedText = [title, desc, details].join(" ");
    const itemKeyStr = itemKey(item);
    const itemMechanic = data.mechanics.find(mech => mech.id === itemKeyStr);

    const related = [];
    for (const mech of data.mechanics) {
      if (item.namespace === "mechanics" && mech.id === itemKeyStr) continue;

      const terms = ((lang === "en" ? (mech.termsEn || mech.terms) : mech.terms) || []);
      const hasTerm = terms.some(term => combinedText.includes(normalize(term)));
      const hasMechanicRelation = itemMechanic?.relatedMechanicIds?.includes(mech.id);
      const hasKey = mech.relatedItemKeys?.some(k => {
        return itemKeyStr.includes(k) || k.includes(itemKeyStr);
      });

      if (hasTerm || hasKey || hasMechanicRelation) {
        related.push(localizeMechanic(mech));
      }
    }
    return related;
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



    const title = localize(item, "title");
    const current = localize(item, "desc") || text(item.current);
    const vanilla = text(lang === 'en'
      ? item.vanillaEn || item.vanilla || item.groupDefaultVanillaEn || item.groupDefaultVanilla
      : item.vanilla || item.groupDefaultVanilla);
    const tagsHtml = (item.tags || []).map(tag => `<span class="tag">${tag}</span>`).join("");
    const isPinned = pinnedInspectItem && itemKey(pinnedInspectItem) === itemKey(item);

    const relatedMechanics = findRelatedMechanics(item);
    let relatedHtml = "";
    if (relatedMechanics.length > 0) {
      const sectTitle = lang === "en" ? "Related Mechanics" : "机制解释";
      const itemsHtml = relatedMechanics.map(mech => {
        const bulletsHtml = mech.bullets.map(b => `<li>${formatStsText(b, false)}</li>`).join("");
        return `
          <div class="mechanic-info-box">
            <div class="mechanic-info-header">
              ${renderImage(mech.icon, mech.title)}
              <h4 class="${mech.keywordClass}">${mech.title}</h4>
            </div>
            <p class="mechanic-info-desc">${formatStsText(mech.desc, false)}</p>
            ${bulletsHtml ? `<ul class="mechanic-info-bullets">${bulletsHtml}</ul>` : ""}
          </div>
        `;
      }).join("");
      relatedHtml = `
        <div class="inspector-related-mechanics">
          <h4 class="inspector-sect-title">${sectTitle}</h4>
          <div class="mechanics-grid">${itemsHtml}</div>
        </div>
      `;
    }

    return `
      <div class="inspector-card-preview">
        <button type="button" class="inspector-mobile-close" id="inspectorMobileClose" aria-label="Close">&times;</button>
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
        ${item.previewImage ? `
          <div class="inspector-preview-image-container">
            <h4 class="inspector-sect-title">${lang === 'en' ? 'In-Game Preview' : '实机预览效果'}</h4>
            <div class="inspector-preview-img-wrapper" title="${lang === 'en' ? 'Click to enlarge' : '点击查看大图'}">
              <img src="${item.previewImage}" alt="${title}" class="inspector-preview-img" />
            </div>
          </div>
        ` : ""}
        ${renderItemDetails(item)}
        ${relatedHtml}
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
    const namespaceClass = (item.isMechanicsCodex || item.namespace === "mechanics" || item.namespace === "mechanic") ? "type-mechanic" : (item.namespace ? `type-${item.namespace}` : "");
    const hasPreviewTag = (item.tags || []).some(t => t === "Preview tool" || t === "预览工具") ? "type-preview" : "";
    const cardClasses = ["compare-card", isActive ? "active-inspect" : "", namespaceClass, hasPreviewTag, isUpdatesPageLoaded ? "loaded" : ""].filter(Boolean).join(" ");
    const displayIndex = String(index + 1).padStart(2, '0');

    return `
      <article class="${cardClasses}" style="--index: ${index}" data-search="${searchString}" data-index="${index}">
        ${isPinned ? `<span class="pin-badge">${lang === 'en' ? 'Locked' : '已锁定'}</span>` : ""}
        <div class="card-header-row">
          <div class="card-art-frame">
            ${renderImage(item.icon, title)}
          </div>
          <div class="card-title-block">
            <div class="card-title-row-ex">
              <span class="card-index-num">${displayIndex}</span>
              <h3>${title}</h3>
            </div>
            <div class="tags">${tagsHtml}</div>
          </div>
        </div>
        <p class="sts-card-current">${formatStsText(current, true)}</p>
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
                <a href="${data.package.dependencyRelease}" class="button" target="_blank" rel="noopener" data-hover="dependency">${labels.openDependency}</a>
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
    const targetMap = {
      "血债": "mechanic_blood_debt",
      "威仪": "mechanic_forge_token",
      "裁决": "mechanic_verdict",
      "裂变": "mechanic_fission",
      "苗床": "mechanic_seedbed",
      "根芽": "mechanic_seedbed",
      "根蚀": "mechanic_seedbed",
      "火印精英": "mechanic_firemarked",
      "战旗房": "mechanic_banner",
      "深层支线": "mechanic_deepbranch",
      "放松": "帕尔之角",
      "愚行": "愚行",
      "执迷": "执迷",
      "至亮之焰": "至亮之焰",
      "雨息": "EZMB_URDA_RAIN_BREATH",
      "枯壳": "EZMB_WITHERED_HUSK",
      "临时页": "EZMB_MORVI_ARCHIVE_DRAW_PAGE",
      "契约": "EZMB_VAKUU_KNIFE_CONTRACT"
    };

    // First clear search query to ensure all cards are visible
    currentSearchQuery = "";
    currentFilter = ""; // Go back to "All"

    let searchValue = target;
    if (target in targetMap) {
      searchValue = targetMap[target];
    }

    currentSearchQuery = searchValue; // Search for the card title or key

    // Find the card to pin
    const cardItem = allUpdateItems.find(item => {
      const title = localize(item, "title");
      const key = itemKey(item);
      return title.includes(searchValue) || key === searchValue || item.title === searchValue || item.titleEn === searchValue;
    });

    if (cardItem) {
      pinnedInspectItem = cardItem;
      activeInspectItem = pinnedInspectItem;
    }

    render(); // Render with the new search query and pin

    // Find matched card in newly rendered DOM and scroll
    const matchedCard = Array.from(document.querySelectorAll(".compare-card:not(.hidden)"))[0];
    if (matchedCard) {
      matchedCard.scrollIntoView({ behavior: "smooth", block: "center" });
    }
  }

  // --- Redraw Loop ---

  function showLightbox(src, alt) {
    let overlay = document.getElementById("lightboxOverlay");
    if (!overlay) {
      overlay = document.createElement("div");
      overlay.id = "lightboxOverlay";
      overlay.className = "lightbox-overlay";
      overlay.innerHTML = `
        <div class="lightbox-content">
          <img class="lightbox-img" src="" alt="" />
          <div class="lightbox-caption"></div>
          <button type="button" class="lightbox-close">&times;</button>
        </div>
      `;
      document.body.appendChild(overlay);
      overlay.addEventListener("click", (e) => {
        if (!e.target.closest(".lightbox-img")) {
          overlay.classList.remove("active");
        }
      });
    }
    const lightboxImg = overlay.querySelector(".lightbox-img");
    const lightboxCaption = overlay.querySelector(".lightbox-caption");
    lightboxImg.src = src;
    lightboxImg.alt = alt;
    lightboxCaption.textContent = alt || "";
    overlay.classList.add("active");
  }

  function render() {
    const current = route();
    document.title = "Spire Plus | " + (
      current === "install" ? labels.navInstall :
      current === "forum" ? labels.navForum :
      current === "issues" ? labels.navIssues :
      current === "about" ? labels.navAbout :
      labels.navUpdates
    );

    const shouldLockHeight = (current === lastActiveRoute);
    const originalMinHeight = document.body.style.minHeight;
    const savedScrollY = window.scrollY;
    if (shouldLockHeight) {
      document.body.style.minHeight = `${document.body.scrollHeight}px`;
    }

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
                ? "Ah, the Spire Plus mod file. It bundles Ancient rewards, higher Ascension tests, and preview tools in one mod."
                : "这是 Spire Plus 私测包：先古奖励、高进阶测试和预览工具都在同一个 Mod 里。";
            } else if (hoverType === "previous-package") {
              phrase = lang === 'en'
                ? "STS2-RitsuLib 0.4.32 is required. Install it first, then enable Spire Plus."
                : "需要先安装 STS2-RitsuLib 0.4.32，再启用 Spire Plus。";
            } else if (hoverType === "releases") {
              phrase = lang === 'en'
                ? "Previous private-test archives live on the release page."
                : "历史私测包都在发布页里。";
            } else if (hoverType === "repo") {
              phrase = lang === 'en'
                ? "Source code and documentation are in the repository."
                : "源码、文档和测试记录都在仓库里。";
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
        input.addEventListener("input", () => {
          currentSearchQuery = input.value;
          applyFilters();
        });
      }

      const chips = app.querySelector("#updateFilters");
      if (chips) {
        chips.addEventListener("click", (event) => {
          const chip = event.target.closest(".chip");
          if (!chip) return;
          for (const item of chips.querySelectorAll(".chip")) item.classList.remove("active");
          chip.classList.add("active");
          currentFilter = chip.dataset.filter === labels.all ? "" : chip.dataset.filter;
          applyFilters();
        });
      }

      const clearLink = app.querySelector(".clear-search-link");
      if (clearLink) {
        clearLink.addEventListener("click", (e) => {
          e.preventDefault();
          if (input) {
            input.value = "";
            currentSearchQuery = "";
            applyFilters();
          }
        });
      }

      applyFilters();

      // Card inspector mouseenter / click logic
      const compareCards = app.querySelectorAll(".compare-card");
      const inspectorPane = app.querySelector("#inspectorPane");
      const cardsPane = app.querySelector(".cards-pane");

      const updatePinStateInDOM = () => {
        compareCards.forEach(c => {
          const cIndex = parseInt(c.dataset.index);
          const cItem = allUpdateItems[cIndex];
          const isPinned = pinnedInspectItem && itemKey(pinnedInspectItem) === itemKey(cItem);
          const isActive = pinnedInspectItem ? isPinned : (activeInspectItem && itemKey(activeInspectItem) === itemKey(cItem));
          c.classList.toggle("active-inspect", isActive);

          let pinBadge = c.querySelector(".pin-badge");
          if (isPinned && !pinBadge) {
            pinBadge = document.createElement("span");
            pinBadge.className = "pin-badge";
            pinBadge.textContent = lang === 'en' ? 'Locked' : '已锁定';
            c.insertBefore(pinBadge, c.firstChild);
          } else if (!isPinned && pinBadge) {
            pinBadge.remove();
          }
        });

        if (inspectorPane) {
          inspectorPane.innerHTML = CardInspectorComponent(pinnedInspectItem || activeInspectItem || allUpdateItems[0]);
        }
      };

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
          if (pinnedInspectItem) return; // Keep inspector locked to pinned card
          selectInspect();
        });

        card.addEventListener("click", (e) => {
          e.stopPropagation();
          if (pinnedInspectItem && itemKey(pinnedInspectItem) === itemKey(item)) {
            pinnedInspectItem = null;
          } else {
            pinnedInspectItem = item;
            activeInspectItem = item;
          }
          updatePinStateInDOM();
          const pane = document.getElementById("inspectorPane");
          if (pane) {
            pane.classList.add("mobile-open");
          }
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

      isUpdatesPageLoaded = true;
    }

    header.innerHTML = HeaderComponent(current);
    if (shouldLockHeight) {
      document.body.style.minHeight = originalMinHeight;
      window.scrollTo({ top: savedScrollY, behavior: "instant" });
    }

    if (current !== lastActiveRoute) {
      window.scrollTo({ top: 0, behavior: "instant" });
      lastActiveRoute = current;
      isUpdatesPageLoaded = false;
    }
  }

  window.addEventListener("hashchange", render);

  // Dynamic navigation click bindings (including cross-card hyperlinks and hero choices)
  document.addEventListener("click", (event) => {
    // Mobile close drawer trigger
    const mobileCloseBtn = event.target.closest("#inspectorMobileClose");
    if (mobileCloseBtn) {
      const pane = document.getElementById("inspectorPane");
      if (pane) {
        pane.classList.remove("mobile-open");
      }
      return;
    }

    // Lightbox image preview trigger
    const wrapper = event.target.closest(".inspector-preview-img-wrapper");
    if (wrapper) {
      const img = wrapper.querySelector(".inspector-preview-img");
      if (img) {
        showLightbox(img.src, img.alt);
      }
      return;
    }

    // Clear Pin click handler inside inspector
    const clearPinBtn = event.target.closest("#clearPinBtn");
    if (clearPinBtn) {
      pinnedInspectItem = null;

      const compareCards = document.querySelectorAll(".compare-card");
      const inspectorPane = document.getElementById("inspectorPane");

      compareCards.forEach(c => {
        const cIndex = parseInt(c.dataset.index);
        const cItem = allUpdateItems[cIndex];
        const pinBadge = c.querySelector(".pin-badge");
        if (pinBadge) pinBadge.remove();

        const isActive = activeInspectItem && itemKey(activeInspectItem) === itemKey(cItem);
        c.classList.toggle("active-inspect", isActive);
      });

      if (inspectorPane) {
        inspectorPane.innerHTML = CardInspectorComponent(activeInspectItem || allUpdateItems[0]);
      }
      return;
    }

    // Hero choice button routing (with smooth scroll support)
    const heroChoiceBtn = event.target.closest(".hero-choice-btn");
    if (heroChoiceBtn) {
      event.preventDefault();
      const choiceType = heroChoiceBtn.dataset.choice;
      const currentRoute = route();
      if (currentRoute === choiceType) {
        const targetEl = choiceType === "updates"
          ? document.querySelector(".update-board") || document.getElementById("updateFilters")
          : document.querySelector(".merchant-shop");
        if (targetEl) {
          targetEl.scrollIntoView({ behavior: "smooth", block: "start" });
        }
      } else {
        location.hash = choiceType;
        setTimeout(() => {
          const targetEl = choiceType === "updates"
            ? document.querySelector(".update-board") || document.getElementById("updateFilters")
            : document.querySelector(".merchant-shop");
          if (targetEl) {
            targetEl.scrollIntoView({ behavior: "smooth", block: "start" });
          }
        }, 150);
      }
      return;
    }

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

  render();
})();
