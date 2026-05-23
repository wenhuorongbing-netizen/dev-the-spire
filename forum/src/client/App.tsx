import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";

type PostSummary = {
  id: string;
  authorName: string;
  title: string;
  bodySummary: string;
  replyCount: number;
  createdAt: string;
  lastActivityAt: string;
};

type PostDetail = PostSummary & {
  body: string;
};

type Reply = {
  id: string;
  authorName: string;
  body: string;
  createdAt: string;
};

type Route =
  | { name: "home" }
  | { name: "new" }
  | { name: "post"; id: string };

type ApiError = {
  message: string;
  status: number;
};

const apiBase = "/api/v1";

function parseRoute(): Route {
  const path = window.location.pathname;
  if (path === "/new") {
    return { name: "new" };
  }

  const postMatch = path.match(/^\/posts\/(\d+)$/);
  if (postMatch) {
    return { name: "post", id: postMatch[1] };
  }

  return { name: "home" };
}

function navigate(path: string) {
  window.history.pushState(null, "", path);
  window.dispatchEvent(new PopStateEvent("popstate"));
}

async function requestJson<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(url, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...options?.headers
    }
  });

  if (!response.ok) {
    let message = "论坛暂时无法连接";
    try {
      const data = (await response.json()) as { message?: string };
      message = data.message ?? message;
    } catch {
      // Keep the generic message for non-JSON errors.
    }
    throw { message, status: response.status } satisfies ApiError;
  }

  return (await response.json()) as T;
}

function formatTime(value: string): string {
  return new Intl.DateTimeFormat("zh-CN", {
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit"
  }).format(new Date(value));
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

function Header() {
  return (
    <header className="site-header">
      <button type="button" className="brand" onClick={() => navigate("/")}>
        Spire Plus 论坛
      </button>
      <button type="button" className="primary-button" onClick={() => navigate("/new")}>
        发帖
      </button>
    </header>
  );
}

function HomePage() {
  const [posts, setPosts] = useState<PostSummary[]>([]);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState("");

  const loadPosts = useCallback(async (cursor?: string) => {
    if (cursor) {
      setLoadingMore(true);
    } else {
      setLoading(true);
      setPosts([]);
    }
    setError("");

    try {
      const params = new URLSearchParams({ limit: "20" });
      if (cursor) {
        params.set("cursor", cursor);
      }
      const data = await requestJson<{ posts: PostSummary[]; nextCursor: string | null }>(
        `${apiBase}/posts?${params.toString()}`
      );
      setPosts((current) => (cursor ? [...current, ...data.posts] : data.posts));
      setNextCursor(data.nextCursor);
    } catch {
      setError("论坛暂时无法连接");
    } finally {
      setLoading(false);
      setLoadingMore(false);
    }
  }, []);

  useEffect(() => {
    void loadPosts();
  }, [loadPosts]);

  return (
    <main className="page">
      <section className="page-title">
        <h1>Spire Plus 论坛</h1>
        <p>无需注册，留下名字或匿名发帖。这里用纯文本交流测试体验、平衡反馈和游玩记录。</p>
      </section>

      {error ? <ErrorPanel message={error} onRetry={() => void loadPosts()} /> : null}

      {!error && loading ? <p className="muted">正在加载帖子...</p> : null}

      {!error && !loading && posts.length === 0 ? (
        <section className="notice">
          <p>还没有帖子，发第一帖。</p>
          <button type="button" className="primary-button" onClick={() => navigate("/new")}>
            发帖
          </button>
        </section>
      ) : null}

      <section className="post-list" aria-label="帖子列表">
        {posts.map((post) => (
          <article key={post.id} className="post-row">
            <button type="button" className="post-link" onClick={() => navigate(`/posts/${post.id}`)}>
              <span className="post-title">{post.title}</span>
              <span className="post-summary">{post.bodySummary}</span>
            </button>
            <div className="post-meta">
              <span>{post.authorName}</span>
              <span>{formatTime(post.lastActivityAt)}</span>
              <span>{post.replyCount} 条回复</span>
            </div>
          </article>
        ))}
      </section>

      {nextCursor ? (
        <div className="pagination">
          <button
            type="button"
            className="secondary-button"
            disabled={loadingMore}
            onClick={() => void loadPosts(nextCursor)}
          >
            {loadingMore ? "正在加载..." : "加载更多"}
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
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");

  async function submit(event: FormEvent) {
    event.preventDefault();
    setSubmitting(true);
    setError("");

    try {
      const data = await requestJson<{ id: string }>(`${apiBase}/posts`, {
        method: "POST",
        body: JSON.stringify({ authorName, title, body, website })
      });
      navigate(`/posts/${data.id}`);
    } catch (requestError) {
      const apiError = requestError as ApiError;
      setError(apiError.status === 429 ? "发帖太频繁，请稍后再试" : apiError.message);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <main className="page narrow-page">
      <button type="button" className="text-button" onClick={() => navigate("/")}>
        返回帖子列表
      </button>
      <h1>发帖</h1>
      <form className="form" onSubmit={(event) => void submit(event)}>
        <label>
          名字
          <input
            value={authorName}
            maxLength={32}
            placeholder="留空显示为匿名玩家"
            onChange={(event) => setAuthorName(event.target.value)}
          />
        </label>
        <label>
          标题
          <input
            value={title}
            maxLength={120}
            required
            placeholder="写一个清楚的标题"
            onChange={(event) => setTitle(event.target.value)}
          />
        </label>
        <label>
          正文
          <textarea
            value={body}
            maxLength={10000}
            required
            rows={12}
            placeholder="只支持纯文本，换行会保留"
            onChange={(event) => setBody(event.target.value)}
          />
        </label>
        <label className="honeypot" aria-hidden="true">
          网站
          <input tabIndex={-1} autoComplete="off" value={website} onChange={(event) => setWebsite(event.target.value)} />
        </label>
        {error ? <p className="form-error">{error}</p> : null}
        <button type="submit" className="primary-button wide-button" disabled={submitting}>
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
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");

  async function submit(event: FormEvent) {
    event.preventDefault();
    setSubmitting(true);
    setError("");

    try {
      await requestJson<{ id: string }>(`${apiBase}/posts/${postId}/replies`, {
        method: "POST",
        body: JSON.stringify({ authorName, body, website })
      });
      setAuthorName("");
      setBody("");
      setWebsite("");
      await onReplied();
    } catch (requestError) {
      const apiError = requestError as ApiError;
      setError(apiError.status === 429 ? "发帖太频繁，请稍后再试" : apiError.message);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <form className="form reply-form" onSubmit={(event) => void submit(event)}>
      <h2>回复</h2>
      <label>
        名字
        <input
          value={authorName}
          maxLength={32}
          placeholder="留空显示为匿名玩家"
          onChange={(event) => setAuthorName(event.target.value)}
        />
      </label>
      <label>
        回复内容
        <textarea
          value={body}
          maxLength={5000}
          required
          rows={7}
          placeholder="写下你的回复"
          onChange={(event) => setBody(event.target.value)}
        />
      </label>
      <label className="honeypot" aria-hidden="true">
        网站
        <input tabIndex={-1} autoComplete="off" value={website} onChange={(event) => setWebsite(event.target.value)} />
      </label>
      {error ? <p className="form-error">{error}</p> : null}
      <button type="submit" className="primary-button wide-button" disabled={submitting}>
        {submitting ? "正在回复..." : "发布回复"}
      </button>
    </form>
  );
}

function PostPage({ id }: { id: string }) {
  const [post, setPost] = useState<PostDetail | null>(null);
  const [replies, setReplies] = useState<Reply[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadPost = useCallback(async () => {
    setError("");
    try {
      const data = await requestJson<{ post: PostDetail; replies: Reply[] }>(`${apiBase}/posts/${id}`);
      setPost(data.post);
      setReplies(data.replies);
    } catch (requestError) {
      const apiError = requestError as ApiError;
      setError(apiError.status === 404 ? "帖子不存在" : "论坛暂时无法连接");
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    setLoading(true);
    void loadPost();
  }, [loadPost]);

  if (loading) {
    return (
      <main className="page narrow-page">
        <p className="muted">正在加载帖子...</p>
      </main>
    );
  }

  if (error || !post) {
    return (
      <main className="page narrow-page">
        <button type="button" className="text-button" onClick={() => navigate("/")}>
          返回帖子列表
        </button>
        <ErrorPanel message={error || "论坛暂时无法连接"} onRetry={() => void loadPost()} />
      </main>
    );
  }

  return (
    <main className="page narrow-page">
      <button type="button" className="text-button" onClick={() => navigate("/")}>
        返回帖子列表
      </button>
      <article className="post-detail">
        <h1>{post.title}</h1>
        <div className="post-meta">
          <span>{post.authorName}</span>
          <span>发布于 {formatTime(post.createdAt)}</span>
          <span>{post.replyCount} 条回复</span>
        </div>
        <p className="plain-text">{post.body}</p>
      </article>

      <section className="replies" aria-label="回复列表">
        <h2>回复</h2>
        {replies.length === 0 ? <p className="muted">还没有回复。</p> : null}
        {replies.map((reply) => (
          <article key={reply.id} className="reply">
            <div className="post-meta">
              <span>{reply.authorName}</span>
              <span>{formatTime(reply.createdAt)}</span>
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
    const onPopState = () => setRoute(parseRoute());
    window.addEventListener("popstate", onPopState);
    return () => window.removeEventListener("popstate", onPopState);
  }, []);

  const page = useMemo(() => {
    if (route.name === "new") {
      return <NewPostPage />;
    }
    if (route.name === "post") {
      return <PostPage id={route.id} />;
    }
    return <HomePage />;
  }, [route]);

  return (
    <>
      <Header />
      {page}
    </>
  );
}
