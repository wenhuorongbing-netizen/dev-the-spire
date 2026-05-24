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
    if (!item.details?.length) return null;
    const details = el("details", "item-details");
    details.appendChild(el("summary", "", labels.expandDetails));
    const list = el("div", "detail-grid");
    for (const entry of item.details) {
      const label = detailLabel(entry);
      const body = detailBody(item, entry);
      if (!label && !body) continue;
      const row = el("div", "detail-row");
      row.appendChild(el("strong", "detail-title", label));
      row.appendChild(el("span", "detail-copy", body));
      list.appendChild(row);
    }
    details.appendChild(list);
    return details;
  }

  function el(tag, className, value) {
    const node = document.createElement(tag);
    if (className) node.className = className;
    if (value !== undefined) node.textContent = value;
    return node;
  }

  function image(src, alt) {
    const source = safeImageSource(src);
    if (source.placeholder) return sourceArtPlaceholder(alt);
    const img = document.createElement("img");
    img.src = source.src;
    img.alt = alt || "";
    img.loading = "lazy";
    img.decoding = "async";
    img.addEventListener("error", () => {
      img.src = fallbackIcon;
    });
    return img;
  }

  function safeImageSource(src) {
    if (!src) return { src: fallbackIcon };
    const pointsToLocalSource = src.includes("source%20code") || src.includes("source code");
    if (pointsToLocalSource && location.hostname.endsWith("github.io")) return { placeholder: true };
    return { src };
  }

  function sourceArtPlaceholder(alt) {
    const node = el("span", "source-art-placeholder");
    node.setAttribute("role", "img");
    node.setAttribute("aria-label", `${alt || labels.sourceArtPlaceholder} ${labels.sourceArtPlaceholder}`);
    node.appendChild(el("strong", "", titleInitials(alt)));
    node.appendChild(el("small", "", labels.sourceArtPlaceholder));
    return node;
  }

  function titleInitials(value) {
    const compact = Array.from(String(value || labels.sourceArtPlaceholder)
      .replace(/[^\p{Letter}\p{Number}]/gu, ""));
    return compact.slice(0, 2).join("") || labels.sourceArtPlaceholder.slice(0, 2);
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

  function renderHeader(activeRoute) {
    header.replaceChildren();
    const brand = el("a", "brand");
    brand.href = "#updates";
    brand.dataset.route = "updates";
    brand.appendChild(image("assets/ancients/urda/ezmb_urda_map_icon.png", ""));
    const brandText = el("span", "");
    brandText.appendChild(el("strong", "", "Spire Plus"));
    brandText.appendChild(el("small", "", labels.brandSub));
    brand.appendChild(brandText);
    header.appendChild(brand);

    const nav = el("nav", "site-nav");
    nav.setAttribute("aria-label", "main");
    for (const [id, label] of [
      ["updates", labels.navUpdates],
      ["install", labels.navInstall],
      ["forum", labels.navForum],
      ["issues", labels.navIssues],
      ["about", labels.navAbout]
    ]) {
      const link = el("a", id === activeRoute ? "active" : "", label);
      link.href = "#" + id;
      link.dataset.route = id;
      nav.appendChild(link);
    }
    header.appendChild(nav);

    const langSwitch = el("div", "lang-switch");
    for (const [id, label] of [["zh", "中文"], ["en", "EN"]]) {
      const isActive = id === lang;
      const langButton = el("button", isActive ? "active" : "", label);
      langButton.type = "button";
      langButton.dataset.lang = id;
      langButton.setAttribute("aria-pressed", isActive ? "true" : "false");
      langSwitch.appendChild(langButton);
    }
    header.appendChild(langSwitch);
  }

  function renderPageHead(title, lead) {
    const section = el("section", "page-head");
    section.appendChild(el("h1", "", title));
    section.appendChild(el("p", "", lead));
    return section;
  }

  function button(textValue, href, primary) {
    const link = el("a", primary ? "button primary" : "button", textValue);
    link.href = href;
    return link;
  }

  function renderUpdates() {
    const hero = el("section", "hero");
    const media = el("div", "hero-images");
    for (const src of [
      "assets/events/ezmb_urda.png",
      "assets/events/ezmb_morvi.png",
      "assets/events/ezmb_lotha.png"
    ]) {
      media.appendChild(image(src, ""));
    }
    hero.appendChild(media);
    const copy = el("div", "hero-copy");
    if (labels.releaseLine) {
      copy.appendChild(el("p", "release-line", labels.releaseLine));
    }
    copy.appendChild(el("h1", "", labels.heroTitle));
    copy.appendChild(el("p", "", labels.heroCopy));
    const actions = el("div", "hero-actions");
    actions.appendChild(button(labels.download, "#install", true));
    actions.appendChild(button(labels.viewIssues, "#issues", false));
    copy.appendChild(actions);
    hero.appendChild(copy);
    app.appendChild(hero);

    // Create Introduction Section to replace quick-nav cards
    const introSection = el("section", "mod-intro-section panel");
    const introTitle = el("h2", "intro-heading", labels.modIntroTitle);
    introSection.appendChild(introTitle);

    const introGrid = el("div", "intro-grid");

    // Feature 1: Re-imagined Ascension
    const feat1 = el("div", "feat-card");
    feat1.appendChild(el("h3", "", labels.featAscensionTitle));
    feat1.appendChild(el("p", "", labels.featAscensionDesc));
    introGrid.appendChild(feat1);

    // Feature 2: Design Philosophy
    const feat2 = el("div", "feat-card");
    feat2.appendChild(el("h3", "", labels.featPhilosophyTitle));
    feat2.appendChild(el("p", "", labels.featPhilosophyDesc));
    introGrid.appendChild(feat2);

    // Feature 3: High Risk High Reward
    const feat3 = el("div", "feat-card");
    feat3.appendChild(el("h3", "", labels.featRewardTitle));
    feat3.appendChild(el("p", "", labels.featRewardDesc));
    introGrid.appendChild(feat3);

    introSection.appendChild(introGrid);
    app.appendChild(introSection);

    const tools = el("section", "tool-row");
    const search = el("label", "search");
    search.appendChild(el("span", "", labels.search));
    const input = el("input", "");
    input.id = "updateSearch";
    input.type = "search";
    input.placeholder = labels.searchPlaceholder;
    search.appendChild(input);
    tools.appendChild(search);
    const chips = el("div", "chips");
    chips.id = "updateFilters";
    const all = el("button", "chip active", labels.all);
    all.type = "button";
    all.dataset.filter = labels.all;
    chips.appendChild(all);
    for (const group of data.updateGroups) {
      const chip = el("button", "chip", group.short);
      chip.type = "button";
      chip.dataset.filter = group.short;
      chips.appendChild(chip);
    }
    tools.appendChild(chips);
    app.appendChild(tools);

    const board = el("section", "update-board");
    for (const group of data.updateGroups) {
      const section = el("section", "compare-group");
      section.dataset.group = group.short;
      const head = el("div", "group-head");
      head.appendChild(image(group.icon, group.title));
      const groupText = el("div", "");
      groupText.appendChild(el("h2", "", group.title));
      groupText.appendChild(el("p", "", group.note));
      head.appendChild(groupText);
      section.appendChild(head);

      const list = el("div", "compare-list");
      for (const item of group.items) {
        const title = localize(item, "title");
        const current = localize(item, "desc") || text(item.current);
        const vanilla = text(lang === "en"
          ? item.vanillaEn || item.vanilla || group.defaultVanillaEn || group.defaultVanilla
          : item.vanilla || group.defaultVanilla);
        const detailsText = detailSearchText(item);
        const card = el("article", "compare-card");
        card.dataset.search = normalize([title, current, vanilla, detailsText, (item.tags || []).join(" ")].join(" "));
        card.appendChild(image(item.icon || group.icon, title));
        const body = el("div", "");
        body.appendChild(el("h3", "", title));
        const dl = el("dl", "");
        dl.appendChild(el("dt", "", labels.vanilla));
        dl.appendChild(el("dd", "", vanilla));
        dl.appendChild(el("dt", "", labels.current));
        dl.appendChild(el("dd", "", current));
        body.appendChild(dl);
        const tags = el("div", "tags");
        for (const tag of item.tags || []) tags.appendChild(el("span", "tag", tag));
        body.appendChild(tags);
        const itemDetails = renderItemDetails(item);
        if (itemDetails) body.appendChild(itemDetails);
        card.appendChild(body);
        list.appendChild(card);
      }
      section.appendChild(list);
      board.appendChild(section);
    }
    app.appendChild(board);

    const emptyState = el("div", "empty-state hidden");
    emptyState.id = "searchEmptyState";
    const emptyText = el("p", "", labels.noSearchMatched || "没有找到匹配的改动。");
    const clearLink = el("a", "clear-search-link", labels.clearSearch || "清除搜索");
    clearLink.href = "#";
    clearLink.addEventListener("click", (e) => {
      e.preventDefault();
      input.value = "";
      applyFilters();
    });
    emptyState.appendChild(emptyText);
    emptyState.appendChild(clearLink);
    app.appendChild(emptyState);

    input.addEventListener("input", applyFilters);
    chips.addEventListener("click", (event) => {
      const chip = event.target.closest(".chip");
      if (!chip) return;
      for (const item of chips.querySelectorAll(".chip")) item.classList.remove("active");
      chip.classList.add("active");
      applyFilters();
    });
  }

  function normalize(value) {
    return String(value || "").replace(/\s+/g, " ").trim().toLowerCase();
  }

  function applyFilters() {
    const query = normalize(document.getElementById("updateSearch").value);
    const filter = document.querySelector("#updateFilters .active")?.dataset.filter || labels.all;
    let totalVisible = 0;
    for (const group of document.querySelectorAll(".compare-group")) {
      const filterMatch = filter === labels.all || group.dataset.group === filter;
      let visible = 0;
      for (const card of group.querySelectorAll(".compare-card")) {
        const searchMatch = !query || card.dataset.search.includes(query);
        card.classList.toggle("hidden", !searchMatch);
        if (searchMatch) visible += 1;
      }
      const shouldShowGroup = filterMatch && visible > 0;
      group.classList.toggle("hidden", !shouldShowGroup);
      if (shouldShowGroup) totalVisible += visible;
    }
    const emptyState = document.getElementById("searchEmptyState");
    if (emptyState) {
      emptyState.classList.toggle("hidden", totalVisible > 0);
    }
  }

  function renderInstall() {
    app.appendChild(renderPageHead(labels.installTitle, labels.installLead));
    const grid = el("section", "install-grid");
    const local = isLocal();
    const primaryDownloadUrl = local ? data.package.localDownload : data.package.releaseDownload;

    const pkg = panel(labels.currentDownload);

    // Group 1: Prerequisite files downloads
    const requiredGroup = el("div", "download-group required-group");
    requiredGroup.appendChild(el("h3", "download-group-title", labels.requiredFilesTitle || "第一步：下载必要文件 (Required Files)"));
    const requiredActions = el("div", "download-actions");
    const downloadLink = button(labels.download, primaryDownloadUrl, true);
    requiredActions.appendChild(downloadLink);
    requiredActions.appendChild(button(labels.openBaseLib, data.package.baseLibRelease, true));
    requiredGroup.appendChild(requiredActions);
    pkg.appendChild(requiredGroup);

    // Group 2: Optional resources
    const optionalGroup = el("div", "download-group");
    optionalGroup.appendChild(el("h3", "download-group-title", labels.optionalFilesTitle || "辅助与源代码 (Optional Links)"));
    const optionalActions = el("div", "download-actions");
    const releaseLink = button(labels.openRelease, data.package.releasesPage, false);
    optionalActions.appendChild(releaseLink);
    optionalActions.appendChild(button(labels.openRepo, data.package.repository, false));
    optionalGroup.appendChild(optionalActions);
    pkg.appendChild(optionalGroup);

    const meta = el("dl", "meta");
    for (const [key, value] of data.package.meta) {
      meta.appendChild(el("dt", "", key));
      const isHash = key === "哈希" || key === "Hash" || key === "\u54c8\u5e0c";
      const dd = el("dd", isHash ? "hash-container" : "");
      dd.dataset.metaField = packageMetaField(key);
      if (isHash) {
        const hashSpan = el("span", "hash", value);
        const copyBtn = el("button", "copy-hash-btn", labels.copy || "复制");
        copyBtn.type = "button";
        copyBtn.addEventListener("click", () => {
          navigator.clipboard.writeText(hashSpan.textContent).then(() => {
            copyBtn.textContent = labels.copied || "已复制";
            copyBtn.classList.add("copied");
            setTimeout(() => {
              copyBtn.textContent = labels.copy || "复制";
              copyBtn.classList.remove("copied");
            }, 1500);
          });
        });
        dd.appendChild(hashSpan);
        dd.appendChild(copyBtn);
      } else {
        dd.textContent = value;
      }
      meta.appendChild(dd);
    }
    pkg.appendChild(meta);
    hydrateLatestRelease(downloadLink, releaseLink, meta);

    grid.appendChild(pkg);
    grid.appendChild(listPanel(labels.steps, data.installSteps));
    grid.appendChild(listPanel(labels.requirements, data.requirements));
    app.appendChild(grid);
  }

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
      // Static fallback links remain valid when GitHub API is unavailable or rate-limited.
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

  function renderAbout() {
    app.appendChild(renderPageHead(labels.aboutTitle, labels.aboutLead));

    const grid = el("section", "install-grid about-grid");
    const intro = panel(labels.introTitle);
    intro.appendChild(el("p", "", labels.introCopy));
    grid.appendChild(intro);
    grid.appendChild(listPanel(labels.assetPolicy, data.assetPolicy));
    app.appendChild(grid);
  }

  function isLocal() {
    return ["", "localhost", "127.0.0.1", "::1"].includes(location.hostname);
  }

  function panel(title) {
    const node = el("section", "panel");
    node.appendChild(el("h2", "", title));
    return node;
  }

  function listPanel(title, rows) {
    const node = panel(title);
    const list = el("ol", "steps");
    for (const row of rows) list.appendChild(el("li", "", row));
    node.appendChild(list);
    return node;
  }

  function renderForum() {
    const shell = el("section", "forum-integrated");
    Object.assign(shell.style, {
      width: "min(1280px, calc(100% - 32px))",
      margin: "0 auto",
      padding: "8px 0 0"
    });
    const frame = document.createElement("iframe");
    frame.id = "forumFrame";
    frame.className = "forum-frame";
    frame.title = labels.forumPublicTitle || labels.navForum;
    frame.src = forumEmbedHref();
    frame.loading = "eager";
    Object.assign(frame.style, {
      display: "block",
      width: "100%",
      minHeight: "calc(100vh - 82px)",
      border: "0",
      background: "#10110e"
    });
    frame.addEventListener("load", () => resizeForumFrame(frame));
    shell.appendChild(frame);
    app.appendChild(shell);
  }

  function renderIssues() {
    app.appendChild(renderPageHead(labels.issuesTitle, labels.issuesLead));
    const grid = el("section", "issue-grid");
    const issues = el("div", "");
    issues.appendChild(el("h2", "", labels.knownIssues));
    const issueList = el("div", "issue-list");
    for (const [level, title, body] of data.knownIssues) {
      const item = el("article", "issue");
      item.dataset.level = level;
      item.appendChild(el("strong", "", level + (labels.issueSeparator || " · ") + title));
      item.appendChild(el("p", "", body));
      issueList.appendChild(item);
    }
    issues.appendChild(issueList);
    grid.appendChild(issues);

    const changes = el("div", "");
    changes.appendChild(el("h2", "", labels.changeLog));
    const changeList = el("div", "change-list");
    for (const [title, body] of data.changeLog) {
      const item = el("article", "change");
      item.appendChild(el("strong", "", title));
      item.appendChild(el("p", "", body));
      changeList.appendChild(item);
    }
    changes.appendChild(changeList);
    grid.appendChild(changes);
    app.appendChild(grid);
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
    renderHeader(current);
    if (current === "install") renderInstall();
    else if (current === "forum") renderForum();
    else if (current === "issues") renderIssues();
    else if (current === "about") renderAbout();
    else renderUpdates();
    window.scrollTo({ top: 0, behavior: "instant" });
  }

  window.addEventListener("hashchange", render);
  document.addEventListener("click", (event) => {
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
