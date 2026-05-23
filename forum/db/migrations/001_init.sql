CREATE TABLE IF NOT EXISTS schema_migrations (
  version text PRIMARY KEY,
  applied_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS forum_posts (
  id bigserial PRIMARY KEY,
  author_name varchar(32) NOT NULL DEFAULT '匿名玩家',
  title varchar(120) NOT NULL,
  body text NOT NULL,
  status varchar(16) NOT NULL DEFAULT 'visible'
    CHECK (status IN ('visible', 'hidden', 'deleted')),
  ip_hash char(64),
  user_agent_hash char(64),
  reply_count integer NOT NULL DEFAULT 0 CHECK (reply_count >= 0),
  last_activity_at timestamptz NOT NULL DEFAULT now(),
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now(),
  CHECK (char_length(trim(title)) BETWEEN 1 AND 120),
  CHECK (char_length(trim(body)) BETWEEN 1 AND 10000)
);

CREATE TABLE IF NOT EXISTS forum_replies (
  id bigserial PRIMARY KEY,
  post_id bigint NOT NULL REFERENCES forum_posts(id) ON DELETE CASCADE,
  author_name varchar(32) NOT NULL DEFAULT '匿名玩家',
  body text NOT NULL,
  status varchar(16) NOT NULL DEFAULT 'visible'
    CHECK (status IN ('visible', 'hidden', 'deleted')),
  ip_hash char(64),
  user_agent_hash char(64),
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now(),
  CHECK (char_length(trim(body)) BETWEEN 1 AND 5000)
);

CREATE INDEX IF NOT EXISTS idx_forum_posts_visible_activity
  ON forum_posts (status, last_activity_at DESC, id DESC);

CREATE INDEX IF NOT EXISTS idx_forum_replies_visible_post_created
  ON forum_replies (post_id, created_at ASC, id ASC)
  WHERE status = 'visible';

CREATE INDEX IF NOT EXISTS idx_forum_posts_ip_hash_created
  ON forum_posts (ip_hash, created_at DESC);

CREATE INDEX IF NOT EXISTS idx_forum_replies_ip_hash_created
  ON forum_replies (ip_hash, created_at DESC);
