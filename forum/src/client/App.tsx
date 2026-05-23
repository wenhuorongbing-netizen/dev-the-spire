import { FormEvent, type ReactNode, useCallback, useEffect, useMemo, useRef, useState } from "react";
import { forumConfigured, forumReadOnly, supabase } from "./supabase";
import type { ForumCategory, ForumPost, ForumReply, Route } from "./types";

const PAGE_SIZE = 20;
const CLIENT_ID_KEY = "spire-plus-forum-client-id";
const URL_PATTERN = /\b(?:https?:\/\/|www\.)\S+/gi;
const queryParams = new URLSearchParams(window.location.search);
const EMBEDDED_MODE = queryParams.get("embedded") === "1";

function redirectStandaloneForumToSite() {
  const cleanPath = window.location.pathname.replace(/\/+$/, "");
  if (EMBEDDED_MODE || !cleanPath.endsWith("/forum")) return;

  const current = new URL(window.location.href);
  const target = new URL("../", current);
  current.searchParams.forEach((value, key) => {
    if (key !== "embedded") target.searchParams.set(key, value);
  });

  const forumRoute = current.hash.replace(/^#/, "");
  if (forumRoute && forumRoute !== "/") target.searchParams.set("forumRoute", forumRoute);
  target.hash = "forum";
  window.location.replace(target.toString());
}

redirectStandaloneForumToSite();

const CATEGORIES: Array<{ id: ForumCategory; label: string; hint: string }> = [
  { id: "discussion", label: "讨论", hint: "体验、想法、一般问题" },
  { id: "bug", label: "Bug 反馈", hint: "崩溃、异常、显示错误" },
  { id: "balance", label: "平衡反馈", hint: "数值、强度、路线压力" },
  { id: "build", label: "构筑", hint: "卡组、遗物、战斗记录" },
  { id: "install", label: "安装", hint: "下载、依赖、加载问题" }
];

type CategoryFilter = ForumCategory | "all";

type ContentBlock = {
  type: "paragraph" | "quote" | "list" | "code";
  lines: string[];
};

function getClientId(): string {
  const existing = localStorage.getItem(CLIENT_ID_KEY);
  if (existing) return existing;
  const next = crypto.randomUUID();
  localStorage.setItem(CLIENT_ID_KEY, next);
  return next;
}

function parseRoute(): Route {
  const path = (window.location.hash.replace(/^#/, "") || "/").replace(/\/+$/, "") || "/";
  if (path.endsWith("/new")) return { name: "new" };
  const match = path.match(/^\/posts\/([0-9a-f-]{36})$/i);
  if (match) return { name: "post", id: match[1] };
  return { name: "home" };
}

function navigate(path: string) {
  const next = path.startsWith("/") ? path : `/${path}`;
  if (window.location.hash === `#${next}`) {
    window.dispatchEvent(new HashChangeEvent("hashchange"));
    return;
  }
  window.location.hash = next;
}

function scrollToReplyForm() {
  document.getElementById("reply-form")?.scrollIntoView({ behavior: "smooth", block: "start" });
}

function postEmbeddedHeight() {
  if (!EMBEDDED_MODE || window.parent === window) return;
  const height = Math.max(
    document.documentElement.scrollHeight,
    document.body.scrollHeight
  );
  window.parent.postMessage({ type: "spire-plus-forum-height", height }, window.location.origin);
}

function formatTime(value: string): string {
  return new Intl.DateTimeFormat("zh-CN", {
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit"
  }).format(new Date(value));
}

function bodyPreview(value: string): string {
  const compact = value.replace(/\s+/g, " ").trim();
  return compact.length > 120 ? `${compact.slice(0, 120)}...` : compact;
}

function normalizeAuthor(value: string): string {
  const trimmed = value.trim();
  return trimmed || "匿名玩家";
}

function validateBody(value: string, max: number): string | null {
  const trimmed = value.trim();
  if (!trimmed) return "内容不能为空。";
  if (trimmed.length > max) return `内容不能超过 ${max} 字。`;
  if ((trimmed.match(URL_PATTERN) || []).length > 5) return "链接数量过多。";
  return null;
}

function categoryLabel(value: string): string {
  return CATEGORIES.find((category) => category.id === value)?.label ?? "讨论";
}

function authorInitial(name: string): string {
  return Array.from(normalizeAuthor(name))[0] ?? "匿";
}

function postUrl(id: string): string {
  if (EMBEDDED_MODE) {
    const url = new URL("../", window.location.href);
    url.searchParams.set("forumRoute", `/posts/${id}`);
    url.hash = "forum";
    return url.toString();
  }

  const url = new URL(window.location.href);
  url.searchParams.delete("embedded");
  url.hash = `/posts/${id}`;
  return url.toString();
}

function renderInlineText(text: string): ReactNode[] {
  const nodes: ReactNode[] = [];
  const matcher = new RegExp(URL_PATTERN.source, "gi");
  let lastIndex = 0;
  let match: RegExpExecArray | null;

  while ((match = matcher.exec(text)) !== null) {
    const value = match[0];
    const index = match.index;
    if (index > lastIndex) nodes.push(text.slice(lastIndex, index));
    const href = value.startsWith("www.") ? `https://${value}` : value;
    nodes.push(
      <a key={`${index}-${value}`} href={href} target="_blank" rel="noreferrer">
        {value}
      </a>
    );
    lastIndex = index + value.length;
  }

  if (lastIndex < text.length) nodes.push(text.slice(lastIndex));
  return nodes;
}

function parseContent(value: string): ContentBlock[] {
  const blocks: ContentBlock[] = [];
  let active: ContentBlock | null = null;
  let inCode = false;

  function flush() {
    if (active && active.lines.length > 0) blocks.push(active);
    active = null;
  }

  function push(type: ContentBlock["type"], line: string) {
    if (!active || active.type !== type) {
      flush();
      active = { type, lines: [] };
    }
    active.lines.push(line);
  }

  for (const rawLine of value.replace(/\r\n/g, "\n").split("\n")) {
    const line = rawLine.replace(/\s+$/, "");
    const trimmed = line.trim();

    if (trimmed.startsWith("```")) {
      if (inCode) {
        inCode = false;
        flush();
      } else {
        flush();
        active = { type: "code", lines: [] };
        inCode = true;
      }
      continue;
    }

    if (inCode) {
      push("code", line);
      continue;
    }

    if (!trimmed) {
      flush();
      continue;
    }

    if (trimmed.startsWith(">")) {
      push("quote", trimmed.replace(/^>\s?/, ""));
      continue;
    }

    if (/^[-*]\s+/.test(trimmed)) {
      push("list", trimmed.replace(/^[-*]\s+/, ""));
      continue;
    }

    push("paragraph", trimmed);
  }

  flush();
  return blocks;
}

function FormattedText({ value }: { value: string }) {
  const blocks = useMemo(() => parseContent(value), [value]);

  return (
    <div className="formatted-text">
      {blocks.map((block, index) => {
        if (block.type === "code") {
          return <pre key={index}>{block.lines.join("\n")}</pre>;
        }
        if (block.type === "quote") {
          return <blockquote key={index}>{renderInlineText(block.lines.join("\n"))}</blockquote>;
        }
        if (block.type === "list") {
          return (
            <ul key={index}>
              {block.lines.map((line, itemIndex) => (
                <li key={`${index}-${itemIndex}`}>{renderInlineText(line)}</li>
              ))}
            </ul>
          );
        }
        return <p key={index}>{renderInlineText(block.lines.join(" "))}</p>;
      })}
    </div>
  );
}

function Header({ routeName }: { routeName: Route["name"] }) {
  const isComposing = routeName === "new";
  const isReadingPost = routeName === "post";

  return (
    <header className="forum-header">
      <button type="button" className="brand-button" onClick={() => navigate("/")}>
        Spire Plus 论坛
      </button>
      <nav className="header-nav" aria-label="论坛导航">
        <button type="button" className="text-button compact" onClick={() => navigate("/")}>
          主题列表
        </button>
        {isComposing ? (
          <button type="button" className="secondary-button" onClick={() => navigate("/")}>
            返回列表
          </button>
        ) : isReadingPost ? (
          <>
            <button type="button" className="text-button compact" onClick={() => navigate("/new")} disabled={forumReadOnly || !forumConfigured}>
              发新帖
            </button>
            <button type="button" className="primary-button" onClick={scrollToReplyForm} disabled={forumReadOnly || !forumConfigured}>
              回帖
            </button>
          </>
        ) : (
          <button type="button" className="primary-button" onClick={() => navigate("/new")} disabled={forumReadOnly || !forumConfigured}>
            发新帖
          </button>
        )}
      </nav>
    </header>
  );
}

function NotConfigured() {
  return (
    <main className="page narrow">
      <section className="notice">
        <h1>论坛还没有连接数据库</h1>
        <p>
          这个页面已经可以部署到 GitHub Pages。要开放发帖，需要先创建 Supabase 项目，运行
          <code> forum/supabase/schema.sql </code>
          ，再配置 <code>VITE_SUPABASE_URL</code> 和 <code>VITE_SUPABASE_ANON_KEY</code>。
        </p>
      </section>
    </main>
  );
}

function ErrorPanel({ message, onRetry }: { message: string; onRetry: () => void }) {
  return (
    <section className="notice error">
      <p>{message}</p>
      <button type="button" className="secondary-button" onClick={onRetry}>
        重试
      </button>
    </section>
  );
}

function CategoryPill({ category }: { category: ForumCategory }) {
  return <span className={`category-pill category-${category}`}>{categoryLabel(category)}</span>;
}

function CategoryTabs({ value, onChange }: { value: CategoryFilter; onChange: (next: CategoryFilter) => void }) {
  return (
    <div className="category-tabs" aria-label="主题筛选">
      <button type="button" className={value === "all" ? "active" : ""} aria-pressed={value === "all"} onClick={() => onChange("all")}>
        全部
      </button>
      {CATEGORIES.map((category) => (
        <button key={category.id} type="button" className={value === category.id ? "active" : ""} aria-pressed={value === category.id} onClick={() => onChange(category.id)}>
          {category.label}
        </button>
      ))}
    </div>
  );
}

function CategorySelect({ value, onChange }: { value: ForumCategory; onChange: (next: ForumCategory) => void }) {
  return (
    <label>
      分类
      <select value={value} onChange={(event) => onChange(event.target.value as ForumCategory)}>
        {CATEGORIES.map((category) => (
          <option key={category.id} value={category.id}>
            {category.label} - {category.hint}
          </option>
        ))}
      </select>
    </label>
  );
}

function HomePage() {
  const [posts, setPosts] = useState<ForumPost[]>([]);
  const [page, setPage] = useState(0);
  const [categoryFilter, setCategoryFilter] = useState<CategoryFilter>("all");
  const [hasMore, setHasMore] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadPosts = useCallback(async (nextPage = 0, selectedCategory: CategoryFilter = categoryFilter) => {
    if (!supabase) return;
    setLoading(true);
    setError("");
    const from = nextPage * PAGE_SIZE;
    const to = from + PAGE_SIZE;
    let query = supabase
      .from("forum_posts")
      .select("id,author_name,title,body,category,reply_count,last_activity_at,created_at")
      .eq("status", "visible");

    if (selectedCategory !== "all") {
      query = query.eq("category", selectedCategory);
    }

    const { data, error: queryError } = await query
      .order("last_activity_at", { ascending: false })
      .order("id", { ascending: false })
      .range(from, to);

    setLoading(false);
    if (queryError) {
      setError("论坛暂时无法连接。");
      return;
    }

    const rows = data ?? [];
    setPage(nextPage);
    setHasMore(rows.length > PAGE_SIZE);
    setPosts((current) => (nextPage === 0 ? rows.slice(0, PAGE_SIZE) : [...current, ...rows.slice(0, PAGE_SIZE)]));
  }, [categoryFilter]);

  useEffect(() => {
    void loadPosts(0, categoryFilter);
  }, [categoryFilter, loadPosts]);

  return (
    <main className="page forum-home">
      <section className="board-header">
        <div>
          <p className="eyebrow">玩家论坛</p>
          <h1>主题列表</h1>
        </div>
        <p>不需要注册。开新帖时写清版本、进阶、路线、构筑或复现步骤，其他玩家可以直接接着回帖。</p>
      </section>

      <div className="board-toolbar">
        <CategoryTabs value={categoryFilter} onChange={setCategoryFilter} />
        <button type="button" className="primary-button" onClick={() => navigate("/new")} disabled={forumReadOnly}>
          发新帖
        </button>
      </div>

      {forumReadOnly ? <section className="notice">论坛当前为只读模式。</section> : null}
      {error ? <ErrorPanel message={error} onRetry={() => void loadPosts(0, categoryFilter)} /> : null}
      {!error && loading && posts.length === 0 ? <p className="muted">正在加载主题...</p> : null}

      {!error && !loading && posts.length === 0 ? (
        <section className="notice">
          <p>这个分类还没有帖子。</p>
          <button type="button" className="primary-button" onClick={() => navigate("/new")} disabled={forumReadOnly}>
            发第一帖
          </button>
        </section>
      ) : null}

      <section className="thread-list" aria-label="主题列表">
        {posts.length > 0 ? (
          <div className="thread-list-head" aria-hidden="true">
            <span>主题</span>
            <span>回帖</span>
            <span>最后活动</span>
          </div>
        ) : null}
        {posts.map((post) => (
          <article key={post.id} className="thread-row">
            <button type="button" className="thread-main" onClick={() => navigate(`/posts/${post.id}`)}>
              <span className="thread-title-line">
                <CategoryPill category={post.category} />
                <span className="post-title">{post.title}</span>
              </span>
              <span className="post-summary">{bodyPreview(post.body)}</span>
              <span className="thread-byline">
                {post.author_name} · {formatTime(post.created_at)}
              </span>
            </button>
            <div className="thread-stat">
              <strong>{post.reply_count}</strong>
              <span>回帖</span>
            </div>
            <div className="thread-activity">
              <span>最后活动</span>
              <strong>{formatTime(post.last_activity_at)}</strong>
            </div>
          </article>
        ))}
      </section>

      {hasMore ? (
        <div className="pagination">
          <button type="button" className="secondary-button" disabled={loading} onClick={() => void loadPosts(page + 1, categoryFilter)}>
            {loading ? "正在加载..." : "加载更多"}
          </button>
        </div>
      ) : null}
    </main>
  );
}

function NewPostPage() {
  const [category, setCategory] = useState<ForumCategory>("discussion");
  const [authorName, setAuthorName] = useState("");
  const [title, setTitle] = useState("");
  const [body, setBody] = useState("");
  const [website, setWebsite] = useState("");
  const [titleError, setTitleError] = useState("");
  const [bodyErrorText, setBodyErrorText] = useState("");
  const [formError, setFormError] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [mobileMode, setMobileMode] = useState<"edit" | "preview">("edit");
  const titleRef = useRef<HTMLInputElement>(null);
  const bodyRef = useRef<HTMLTextAreaElement>(null);

  async function submit(event: FormEvent) {
    event.preventDefault();
    if (!supabase || forumReadOnly) return;
    if (website.trim()) return;
    const bodyError = validateBody(body, 10000);
    const titleText = title.trim();
    setTitleError("");
    setBodyErrorText("");
    setFormError("");
    if (!titleText) {
      setTitleError("标题不能为空。");
      window.requestAnimationFrame(() => titleRef.current?.focus());
      return;
    }
    if (bodyError) {
      setBodyErrorText(bodyError);
      window.requestAnimationFrame(() => bodyRef.current?.focus());
      return;
    }

    setSubmitting(true);
    const { data, error: insertError } = await supabase
      .from("forum_posts")
      .insert({
        author_name: normalizeAuthor(authorName),
        title: titleText,
        body: body.trim(),
        category,
        client_id: getClientId()
      })
      .select("id")
      .single();
    setSubmitting(false);

    if (insertError || !data) {
      setFormError("发布失败。可能是内容过长、链接过多，或发帖太频繁。");
      return;
    }

    navigate(`/posts/${data.id}`);
  }

  return (
    <main className="page composer-page">
      <button type="button" className="text-button" onClick={() => navigate("/")}>
        返回主题列表
      </button>
      <div className="mobile-composer-tabs" role="group" aria-label="发帖视图">
        <button type="button" className={mobileMode === "edit" ? "active" : ""} aria-pressed={mobileMode === "edit"} onClick={() => setMobileMode("edit")}>
          编辑
        </button>
        <button type="button" className={mobileMode === "preview" ? "active" : ""} aria-pressed={mobileMode === "preview"} onClick={() => setMobileMode("preview")}>
          预览
        </button>
      </div>
      <section className={`composer-layout mobile-mode-${mobileMode}`}>
        <form className="form composer-form" noValidate onSubmit={(event) => void submit(event)}>
          <div>
            <p className="eyebrow">新主题</p>
            <h1>发新帖</h1>
          </div>
          <CategorySelect value={category} onChange={setCategory} />
          <label>
            昵称
            <input value={authorName} maxLength={32} placeholder="留空显示为匿名玩家" onChange={(event) => setAuthorName(event.target.value)} />
          </label>
          <label>
            标题
            <input ref={titleRef} value={title} required maxLength={120} placeholder="一句话说明问题或观点" onChange={(event) => setTitle(event.target.value)} />
            {titleError ? <span className="field-error">{titleError}</span> : null}
          </label>
          <label>
            正文
            <textarea ref={bodyRef} value={body} required maxLength={10000} rows={14} placeholder="写清楚现象、版本、路线、卡组或复现步骤。" onChange={(event) => setBody(event.target.value)} />
            <span className="format-hint">支持引用 &gt;、列表 -、代码块 ``` 和链接。</span>
            {bodyErrorText ? <span className="field-error">{bodyErrorText}</span> : null}
          </label>
          <label className="honeypot" aria-hidden="true">
            网站
            <input tabIndex={-1} autoComplete="off" value={website} onChange={(event) => setWebsite(event.target.value)} />
          </label>
          <div className="composer-footer">
            <span>{body.trim().length}/10000</span>
            {formError ? <span className="form-error">{formError}</span> : null}
          </div>
          <button type="submit" className="primary-button wide-button" disabled={submitting || forumReadOnly}>
            {submitting ? "正在发布..." : "发布新帖"}
          </button>
        </form>

        <aside className="preview-panel">
          <div className="preview-title">
            <span>预览</span>
            <CategoryPill category={category} />
          </div>
          <h2>{title.trim() || "未命名主题"}</h2>
          <FormattedText value={body.trim() || "正文会在这里预览。"} />
        </aside>
      </section>
    </main>
  );
}

function ReplyForm({ postId, onReplied }: { postId: string; onReplied: () => Promise<void> }) {
  const [authorName, setAuthorName] = useState("");
  const [body, setBody] = useState("");
  const [website, setWebsite] = useState("");
  const [bodyErrorText, setBodyErrorText] = useState("");
  const [formError, setFormError] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const bodyRef = useRef<HTMLTextAreaElement>(null);

  async function submit(event: FormEvent) {
    event.preventDefault();
    if (!supabase || forumReadOnly) return;
    if (website.trim()) return;
    const bodyError = validateBody(body, 5000);
    setBodyErrorText("");
    setFormError("");
    if (bodyError) {
      setBodyErrorText(bodyError);
      window.requestAnimationFrame(() => bodyRef.current?.focus());
      return;
    }

    setSubmitting(true);
    const { error: insertError } = await supabase.from("forum_replies").insert({
      post_id: postId,
      author_name: normalizeAuthor(authorName),
      body: body.trim(),
      client_id: getClientId()
    });
    setSubmitting(false);

    if (insertError) {
      setFormError("回帖失败。可能是内容过长、链接过多，或回帖太频繁。");
      return;
    }

    setAuthorName("");
    setBody("");
    await onReplied();
  }

  return (
    <form id="reply-form" className="form reply-form" noValidate onSubmit={(event) => void submit(event)}>
      <div className="reply-composer-head">
        <h2>发表回帖</h2>
        <span>{body.trim().length}/5000</span>
      </div>
      <label>
        昵称
        <input value={authorName} maxLength={32} placeholder="留空显示为匿名玩家" onChange={(event) => setAuthorName(event.target.value)} />
      </label>
      <label>
        回帖内容
        <textarea ref={bodyRef} value={body} required maxLength={5000} rows={7} placeholder="写下你的回帖。" onChange={(event) => setBody(event.target.value)} />
        <span className="format-hint">支持引用 &gt;、列表 -、代码块 ``` 和链接。</span>
        {bodyErrorText ? <span className="field-error">{bodyErrorText}</span> : null}
      </label>
      <label className="honeypot" aria-hidden="true">
        网站
        <input tabIndex={-1} autoComplete="off" value={website} onChange={(event) => setWebsite(event.target.value)} />
      </label>
      {body.trim() ? (
        <div className="inline-preview">
          <FormattedText value={body} />
        </div>
      ) : null}
      {formError ? <p className="form-error">{formError}</p> : null}
      <button type="submit" className="primary-button wide-button" disabled={submitting || forumReadOnly}>
        {submitting ? "正在回帖..." : "发表回帖"}
      </button>
    </form>
  );
}

function ForumMessage({
  author,
  time,
  floor,
  role,
  children
}: {
  author: string;
  time: string;
  floor: number;
  role?: string;
  children: ReactNode;
}) {
  return (
    <article className="forum-message">
      <aside className="author-rail">
        <span className="avatar">{authorInitial(author)}</span>
        <strong>{normalizeAuthor(author)}</strong>
        {role ? <span>{role}</span> : null}
      </aside>
      <div className="message-content">
        <div className="message-meta">
          <span>#{floor}</span>
          <span>{formatTime(time)}</span>
        </div>
        {children}
      </div>
    </article>
  );
}

function PostPage({ id }: { id: string }) {
  const [post, setPost] = useState<ForumPost | null>(null);
  const [replies, setReplies] = useState<ForumReply[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [replyError, setReplyError] = useState("");
  const [copyState, setCopyState] = useState("复制链接");

  const loadPost = useCallback(async () => {
    if (!supabase) return;
    setError("");
    setReplyError("");
    const { data: postData, error: postError } = await supabase
      .from("forum_posts")
      .select("id,author_name,title,body,category,reply_count,last_activity_at,created_at")
      .eq("id", id)
      .eq("status", "visible")
      .maybeSingle();

    setLoading(false);
    if (postError) {
      setPost(null);
      setReplies([]);
      setError("论坛暂时无法连接，请稍后重试。");
      return;
    }
    if (!postData) {
      setPost(null);
      setReplies([]);
      setError("帖子不存在，可能已被隐藏、删除，或链接不是最新。");
      return;
    }

    setPost(postData);

    const { data: replyData, error: repliesError } = await supabase
      .from("forum_replies")
      .select("id,post_id,author_name,body,created_at")
      .eq("post_id", id)
      .eq("status", "visible")
      .order("created_at", { ascending: true })
      .order("id", { ascending: true });

    if (repliesError) {
      setReplies([]);
      setReplyError("回帖暂时无法加载，主帖仍可阅读。");
      return;
    }

    setReplies(replyData ?? []);
  }, [id]);

  useEffect(() => {
    setLoading(true);
    void loadPost();
  }, [loadPost]);

  async function copyLink() {
    await navigator.clipboard.writeText(postUrl(id));
    setCopyState("已复制");
    window.setTimeout(() => setCopyState("复制链接"), 1600);
  }

  if (loading) {
    return <main className="page narrow"><p className="muted">正在加载帖子...</p></main>;
  }

  if (error || !post) {
    return (
      <main className="page narrow">
        <button type="button" className="text-button" onClick={() => navigate("/")}>返回主题列表</button>
        <ErrorPanel message={error || "论坛暂时无法连接。"} onRetry={() => void loadPost()} />
      </main>
    );
  }

  return (
    <main className="page topic-page">
      <div className="topic-toolbar">
        <button type="button" className="text-button compact" onClick={() => navigate("/")}>返回主题列表</button>
        <button type="button" className="secondary-button" onClick={() => void copyLink()}>{copyState}</button>
      </div>

      <ForumMessage author={post.author_name} time={post.created_at} floor={1} role="楼主">
        <div className="topic-heading">
          <CategoryPill category={post.category} />
          <h1>{post.title}</h1>
          <span>{post.reply_count} 条回帖 · 最后活动 {formatTime(post.last_activity_at)}</span>
        </div>
        <FormattedText value={post.body} />
      </ForumMessage>

      <section className="replies" aria-label="回帖列表">
        <div className="section-heading">
          <h2>回帖</h2>
          <span>{replies.length} 条</span>
        </div>
        {replyError ? <ErrorPanel message={replyError} onRetry={() => void loadPost()} /> : null}
        {replies.length === 0 ? <p className="muted">还没有回帖。</p> : null}
        {replies.map((reply, index) => (
          <ForumMessage key={reply.id} author={reply.author_name} time={reply.created_at} floor={index + 2}>
            <FormattedText value={reply.body} />
          </ForumMessage>
        ))}
      </section>

      <ReplyForm postId={id} onReplied={loadPost} />
    </main>
  );
}

export function App() {
  const [route, setRoute] = useState<Route>(() => parseRoute());

  useEffect(() => {
    const onRouteChange = () => setRoute(parseRoute());
    window.addEventListener("hashchange", onRouteChange);
    window.addEventListener("popstate", onRouteChange);
    return () => {
      window.removeEventListener("hashchange", onRouteChange);
      window.removeEventListener("popstate", onRouteChange);
    };
  }, []);

  useEffect(() => {
    if (!EMBEDDED_MODE) return;
    postEmbeddedHeight();
    const observer = new ResizeObserver(() => postEmbeddedHeight());
    observer.observe(document.documentElement);
    observer.observe(document.body);
    window.addEventListener("load", postEmbeddedHeight);
    return () => {
      observer.disconnect();
      window.removeEventListener("load", postEmbeddedHeight);
    };
  }, [route]);

  const page = useMemo(() => {
    if (!forumConfigured) return <NotConfigured />;
    if (route.name === "new") return <NewPostPage />;
    if (route.name === "post") return <PostPage id={route.id} />;
    return <HomePage />;
  }, [route]);

  return (
    <div className={EMBEDDED_MODE ? "forum-embed-root" : ""}>
      {EMBEDDED_MODE ? null : <Header routeName={route.name} />}
      {page}
    </div>
  );
}
