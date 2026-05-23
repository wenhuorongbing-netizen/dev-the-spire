import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { forumConfigured, forumReadOnly, supabase } from "./supabase";
import type { ForumPost, ForumReply, Route } from "./types";

const PAGE_SIZE = 20;
const CLIENT_ID_KEY = "spire-plus-forum-client-id";
const URL_PATTERN = /\b(?:https?:\/\/|www\.)\S+/gi;

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
  return compact.length > 150 ? `${compact.slice(0, 150)}...` : compact;
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

function Header() {
  return (
    <header className="forum-header">
      <button type="button" className="brand-button" onClick={() => navigate("/")}>
        Spire Plus 论坛
      </button>
      <button type="button" className="primary-button" onClick={() => navigate("/new")} disabled={forumReadOnly || !forumConfigured}>
        发帖
      </button>
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

function HomePage() {
  const [posts, setPosts] = useState<ForumPost[]>([]);
  const [page, setPage] = useState(0);
  const [hasMore, setHasMore] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadPosts = useCallback(async (nextPage = 0) => {
    if (!supabase) return;
    setLoading(true);
    setError("");
    const from = nextPage * PAGE_SIZE;
    const to = from + PAGE_SIZE;
    const { data, error: queryError } = await supabase
      .from("forum_posts")
      .select("id,author_name,title,body,reply_count,last_activity_at,created_at")
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
    setPosts(nextPage === 0 ? rows.slice(0, PAGE_SIZE) : [...posts, ...rows.slice(0, PAGE_SIZE)]);
  }, [posts]);

  useEffect(() => {
    void loadPosts(0);
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <main className="page">
      <section className="page-title">
        <h1>Spire Plus 论坛</h1>
        <p>无需注册。这里用纯文本讨论测试体验、平衡反馈、构筑记录和安装问题。</p>
      </section>

      {forumReadOnly ? <section className="notice">论坛当前为只读模式。</section> : null}
      {error ? <ErrorPanel message={error} onRetry={() => void loadPosts(0)} /> : null}
      {!error && loading && posts.length === 0 ? <p className="muted">正在加载帖子...</p> : null}

      {!error && !loading && posts.length === 0 ? (
        <section className="notice">
          <p>还没有帖子，发第一帖。</p>
          <button type="button" className="primary-button" onClick={() => navigate("/new")} disabled={forumReadOnly}>
            发帖
          </button>
        </section>
      ) : null}

      <section className="post-list" aria-label="帖子列表">
        {posts.map((post) => (
          <article key={post.id} className="post-row">
            <button type="button" className="post-link" onClick={() => navigate(`/posts/${post.id}`)}>
              <span className="post-title">{post.title}</span>
              <span className="post-summary">{bodyPreview(post.body)}</span>
            </button>
            <div className="post-meta">
              <span>{post.author_name}</span>
              <span>{formatTime(post.last_activity_at)}</span>
              <span>{post.reply_count} 条回复</span>
            </div>
          </article>
        ))}
      </section>

      {hasMore ? (
        <div className="pagination">
          <button type="button" className="secondary-button" disabled={loading} onClick={() => void loadPosts(page + 1)}>
            {loading ? "正在加载..." : "加载更多"}
          </button>
        </div>
      ) : null}
    </main>
  );
}

function NewPostPage() {
  const [authorName, setAuthorName] = useState("");
  const [title, setTitle] = useState("");
  const [body, setBody] = useState("");
  const [website, setWebsite] = useState("");
  const [error, setError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault();
    if (!supabase || forumReadOnly) return;
    if (website.trim()) return;
    const bodyError = validateBody(body, 10000);
    const titleText = title.trim();
    if (!titleText) {
      setError("标题不能为空。");
      return;
    }
    if (bodyError) {
      setError(bodyError);
      return;
    }

    setSubmitting(true);
    setError("");
    const { data, error: insertError } = await supabase
      .from("forum_posts")
      .insert({
        author_name: normalizeAuthor(authorName),
        title: titleText,
        body: body.trim(),
        client_id: getClientId()
      })
      .select("id")
      .single();
    setSubmitting(false);

    if (insertError || !data) {
      setError("发布失败。可能是内容过长、链接过多，或发帖太频繁。");
      return;
    }

    navigate(`/posts/${data.id}`);
  }

  return (
    <main className="page narrow">
      <button type="button" className="text-button" onClick={() => navigate("/")}>
        返回帖子列表
      </button>
      <h1>发帖</h1>
      <form className="form" onSubmit={(event) => void submit(event)}>
        <label>
          名字
          <input value={authorName} maxLength={32} placeholder="留空显示为匿名玩家" onChange={(event) => setAuthorName(event.target.value)} />
        </label>
        <label>
          标题
          <input value={title} required maxLength={120} placeholder="写一个清楚的标题" onChange={(event) => setTitle(event.target.value)} />
        </label>
        <label>
          正文
          <textarea value={body} required maxLength={10000} rows={12} placeholder="只支持纯文本，换行会保留。" onChange={(event) => setBody(event.target.value)} />
        </label>
        <label className="honeypot" aria-hidden="true">
          网站
          <input tabIndex={-1} autoComplete="off" value={website} onChange={(event) => setWebsite(event.target.value)} />
        </label>
        {error ? <p className="form-error">{error}</p> : null}
        <button type="submit" className="primary-button wide-button" disabled={submitting || forumReadOnly}>
          {submitting ? "正在发布..." : "发布帖子"}
        </button>
      </form>
    </main>
  );
}

function ReplyForm({ postId, onReplied }: { postId: string; onReplied: () => Promise<void> }) {
  const [authorName, setAuthorName] = useState("");
  const [body, setBody] = useState("");
  const [website, setWebsite] = useState("");
  const [error, setError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault();
    if (!supabase || forumReadOnly) return;
    if (website.trim()) return;
    const bodyError = validateBody(body, 5000);
    if (bodyError) {
      setError(bodyError);
      return;
    }

    setSubmitting(true);
    setError("");
    const { error: insertError } = await supabase.from("forum_replies").insert({
      post_id: postId,
      author_name: normalizeAuthor(authorName),
      body: body.trim(),
      client_id: getClientId()
    });
    setSubmitting(false);

    if (insertError) {
      setError("回复失败。可能是内容过长、链接过多，或回复太频繁。");
      return;
    }

    setAuthorName("");
    setBody("");
    await onReplied();
  }

  return (
    <form className="form reply-form" onSubmit={(event) => void submit(event)}>
      <h2>回复</h2>
      <label>
        名字
        <input value={authorName} maxLength={32} placeholder="留空显示为匿名玩家" onChange={(event) => setAuthorName(event.target.value)} />
      </label>
      <label>
        回复内容
        <textarea value={body} required maxLength={5000} rows={7} placeholder="写下你的回复。" onChange={(event) => setBody(event.target.value)} />
      </label>
      <label className="honeypot" aria-hidden="true">
        网站
        <input tabIndex={-1} autoComplete="off" value={website} onChange={(event) => setWebsite(event.target.value)} />
      </label>
      {error ? <p className="form-error">{error}</p> : null}
      <button type="submit" className="primary-button wide-button" disabled={submitting || forumReadOnly}>
        {submitting ? "正在回复..." : "发布回复"}
      </button>
    </form>
  );
}

function PostPage({ id }: { id: string }) {
  const [post, setPost] = useState<ForumPost | null>(null);
  const [replies, setReplies] = useState<ForumReply[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadPost = useCallback(async () => {
    if (!supabase) return;
    setError("");
    const [{ data: postData, error: postError }, { data: replyData, error: replyError }] = await Promise.all([
      supabase
        .from("forum_posts")
        .select("id,author_name,title,body,reply_count,last_activity_at,created_at")
        .eq("id", id)
        .single(),
      supabase
        .from("forum_replies")
        .select("id,post_id,author_name,body,created_at")
        .eq("post_id", id)
        .order("created_at", { ascending: true })
        .order("id", { ascending: true })
    ]);
    setLoading(false);
    if (postError || replyError || !postData) {
      setError("帖子不存在或论坛暂时无法连接。");
      return;
    }
    setPost(postData);
    setReplies(replyData ?? []);
  }, [id]);

  useEffect(() => {
    setLoading(true);
    void loadPost();
  }, [loadPost]);

  if (loading) {
    return <main className="page narrow"><p className="muted">正在加载帖子...</p></main>;
  }

  if (error || !post) {
    return (
      <main className="page narrow">
        <button type="button" className="text-button" onClick={() => navigate("/")}>返回帖子列表</button>
        <ErrorPanel message={error || "论坛暂时无法连接。"} onRetry={() => void loadPost()} />
      </main>
    );
  }

  return (
    <main className="page narrow">
      <button type="button" className="text-button" onClick={() => navigate("/")}>返回帖子列表</button>
      <article className="post-detail">
        <h1>{post.title}</h1>
        <div className="post-meta">
          <span>{post.author_name}</span>
          <span>发布于 {formatTime(post.created_at)}</span>
          <span>{post.reply_count} 条回复</span>
        </div>
        <p className="plain-text">{post.body}</p>
      </article>

      <section className="replies" aria-label="回复列表">
        <h2>回复</h2>
        {replies.length === 0 ? <p className="muted">还没有回复。</p> : null}
        {replies.map((reply) => (
          <article key={reply.id} className="reply">
            <div className="post-meta">
              <span>{reply.author_name}</span>
              <span>{formatTime(reply.created_at)}</span>
            </div>
            <p className="plain-text">{reply.body}</p>
          </article>
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

  const page = useMemo(() => {
    if (!forumConfigured) return <NotConfigured />;
    if (route.name === "new") return <NewPostPage />;
    if (route.name === "post") return <PostPage id={route.id} />;
    return <HomePage />;
  }, [route]);

  return (
    <>
      <Header />
      {page}
    </>
  );
}
