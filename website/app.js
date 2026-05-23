(async function () {
  const baseData = window.SPIRE_PLUS_DATA;
  let lang = initialLang();
  let data = selectData(baseData, lang);
  let labels = data.labels;
  const app = document.getElementById("app");
  const header = document.getElementById("siteHeader");
  const routes = new Set(["updates", "install", "forum", "issues"]);
  const fallbackIcon = "assets/relics/relic.png";
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
        const response = await fetch(url);
        result[name] = await response.json();
      })
    );
    return result;
  }

  function route() {
    const hash = location.hash.slice(1) || "updates";
    return routes.has(hash) ? hash : "updates";
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
      ["issues", labels.navIssues]
    ]) {
      const link = el("a", id === activeRoute ? "active" : "", label);
      link.href = "#" + id;
      link.dataset.route = id;
      nav.appendChild(link);
    }
    header.appendChild(nav);

    const langSwitch = el("div", "lang-switch");
    for (const [id, label] of [["zh", "中文"], ["en", "EN"]]) {
      const langButton = el("button", id === lang ? "active" : "", label);
      langButton.type = "button";
      langButton.dataset.lang = id;
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
    copy.appendChild(el("p", "release-line", labels.releaseLine));
    copy.appendChild(el("h1", "", labels.heroTitle));
    copy.appendChild(el("p", "", labels.heroCopy));
    const actions = el("div", "hero-actions");
    actions.appendChild(button(labels.download, "#install", true));
    actions.appendChild(button(labels.viewIssues, "#issues", false));
    copy.appendChild(actions);
    hero.appendChild(copy);
    app.appendChild(hero);

    const intro = el("section", "intro-panel");
    intro.appendChild(el("strong", "", labels.introTitle));
    intro.appendChild(el("p", "", labels.introCopy));
    app.appendChild(intro);

    const summary = el("section", "summary-strip quick-nav");
    for (const [value, label, routeId] of data.summary) {
      const card = el("a", "stat-card quick-card");
      card.href = "#" + routeId;
      card.dataset.route = routeId;
      card.appendChild(el("strong", "", value));
      card.appendChild(el("span", "", label));
      summary.appendChild(card);
    }
    app.appendChild(summary);

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
    for (const group of document.querySelectorAll(".compare-group")) {
      const filterMatch = filter === labels.all || group.dataset.group === filter;
      let visible = 0;
      for (const card of group.querySelectorAll(".compare-card")) {
        const searchMatch = !query || card.dataset.search.includes(query);
        card.classList.toggle("hidden", !searchMatch);
        if (searchMatch) visible += 1;
      }
      group.classList.toggle("hidden", !filterMatch || visible === 0);
    }
  }

  function renderInstall() {
    app.appendChild(renderPageHead(labels.installTitle, labels.installLead));
    const grid = el("section", "install-grid");
    const local = isLocal();
    const primaryDownloadUrl = local ? data.package.localDownload : data.package.releaseDownload;

    const pkg = panel(labels.currentDownload);
    const actions = el("div", "download-actions");
    actions.appendChild(button(labels.download, primaryDownloadUrl, true));
    if (!local) actions.appendChild(button(labels.openRelease, data.package.releasesPage, false));
    actions.appendChild(button(labels.openBaseLib, data.package.baseLibRelease, false));
    if (local) actions.appendChild(button(labels.openRelease, data.package.releasesPage, false));
    actions.appendChild(button(labels.openRepo, data.package.repository, false));
    pkg.appendChild(actions);
    const meta = el("dl", "meta");
    for (const [key, value] of data.package.meta) {
      meta.appendChild(el("dt", "", key));
      meta.appendChild(el("dd", key === "\u54c8\u5e0c" ? "hash" : "", value));
    }
    pkg.appendChild(meta);

    grid.appendChild(pkg);
    grid.appendChild(listPanel(labels.steps, data.installSteps));
    grid.appendChild(listPanel(labels.requirements, data.requirements));
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
    app.appendChild(renderPageHead(labels.forumTitle, labels.forumLead));
    const layout = el("section", "forum-board");
    const entry = el("div", "forum-entry");
    const copy = el("div", "");
    copy.appendChild(el("h2", "", labels.forumPublicTitle));
    copy.appendChild(el("p", "", data.forum.notice));
    const bullets = el("ul", "forum-points");
    for (const point of data.forum.points || []) bullets.appendChild(el("li", "", point));
    copy.appendChild(bullets);
    entry.appendChild(copy);

    const actions = el("div", "forum-actions");
    const forumUrl = isLocal() ? data.forum.localUrl : data.forum.url;
    const primary = button(labels.openForum, forumUrl, true);
    primary.target = "_blank";
    primary.rel = "noopener";
    actions.appendChild(primary);
    const status = button(labels.forumHealth, forumUrl.replace(/\/$/, "") + "/healthz", false);
    status.target = "_blank";
    status.rel = "noopener";
    actions.appendChild(status);
    for (const [label, href] of data.forum.links || []) {
      const link = button(label, href, false);
      link.target = "_blank";
      link.rel = "noopener";
      actions.appendChild(link);
    }
    entry.appendChild(actions);
    layout.appendChild(entry);

    const note = el("div", "forum-deploy-note");
    note.appendChild(el("strong", "", labels.forumDeployTitle));
    note.appendChild(el("p", "", labels.forumDeployCopy));
    layout.appendChild(note);
    app.appendChild(layout);
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

  function render() {
    const current = route();
    document.title = "Spire Plus | " + (
      current === "install" ? labels.navInstall :
      current === "forum" ? labels.navForum :
      current === "issues" ? labels.navIssues :
      labels.navUpdates
    );
    app.replaceChildren();
    renderHeader(current);
    if (current === "install") renderInstall();
    else if (current === "forum") renderForum();
    else if (current === "issues") renderIssues();
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
