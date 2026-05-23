-- Spire Plus anonymous forum schema for Supabase.
-- Run this in the Supabase SQL editor after creating the project.

create extension if not exists pgcrypto;

create table if not exists public.forum_posts (
  id uuid primary key default gen_random_uuid(),
  author_name text not null default '匿名玩家',
  title text not null,
  body text not null,
  category text not null default 'discussion',
  status text not null default 'visible' check (status in ('visible', 'hidden', 'deleted')),
  client_id uuid not null,
  reply_count integer not null default 0 check (reply_count >= 0),
  last_activity_at timestamptz not null default now(),
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  check (char_length(trim(author_name)) between 1 and 32),
  check (char_length(trim(title)) between 1 and 120),
  check (char_length(trim(body)) between 1 and 10000),
  constraint forum_posts_category_check check (category in ('discussion', 'bug', 'balance', 'build', 'install'))
);

create table if not exists public.forum_replies (
  id uuid primary key default gen_random_uuid(),
  post_id uuid not null references public.forum_posts(id) on delete cascade,
  author_name text not null default '匿名玩家',
  body text not null,
  status text not null default 'visible' check (status in ('visible', 'hidden', 'deleted')),
  client_id uuid not null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  check (char_length(trim(author_name)) between 1 and 32),
  check (char_length(trim(body)) between 1 and 5000)
);

alter table public.forum_posts
  add column if not exists category text not null default 'discussion';

alter table public.forum_posts
  drop constraint if exists forum_posts_category_check;

alter table public.forum_posts
  add constraint forum_posts_category_check
  check (category in ('discussion', 'bug', 'balance', 'build', 'install'));

create index if not exists forum_posts_visible_activity_idx
  on public.forum_posts (last_activity_at desc, id desc)
  where status = 'visible';

create index if not exists forum_posts_client_recent_idx
  on public.forum_posts (client_id, created_at desc);

create index if not exists forum_posts_visible_category_activity_idx
  on public.forum_posts (category, last_activity_at desc, id desc)
  where status = 'visible';

create index if not exists forum_replies_post_visible_idx
  on public.forum_replies (post_id, created_at asc, id asc)
  where status = 'visible';

create index if not exists forum_replies_client_recent_idx
  on public.forum_replies (client_id, created_at desc);

create or replace function public.forum_url_count(input text)
returns integer
language sql
immutable
as $$
  select regexp_count(coalesce(input, ''), '(https?://|www\.)', 1, 'i');
$$;

create or replace function public.forum_normalize_author(input text)
returns text
language sql
immutable
as $$
  select case
    when nullif(trim(coalesce(input, '')), '') is null then '匿名玩家'
    else left(trim(input), 32)
  end;
$$;

create or replace function public.forum_recent_post_count(input_client_id uuid, input_window interval)
returns integer
language sql
stable
security definer
set search_path = public
as $$
  select count(*)::integer
  from public.forum_posts existing
  where existing.client_id = input_client_id
    and existing.created_at >= now() - input_window;
$$;

create or replace function public.forum_recent_reply_count(input_client_id uuid, input_window interval)
returns integer
language sql
stable
security definer
set search_path = public
as $$
  select count(*)::integer
  from public.forum_replies existing
  where existing.client_id = input_client_id
    and existing.created_at >= now() - input_window;
$$;

create or replace function public.forum_posts_before_insert()
returns trigger
language plpgsql
as $$
begin
  new.author_name := public.forum_normalize_author(new.author_name);
  new.title := trim(new.title);
  new.body := trim(new.body);
  if new.category is null or new.category not in ('discussion', 'bug', 'balance', 'build', 'install') then
    new.category := 'discussion';
  end if;
  new.status := 'visible';
  new.reply_count := 0;
  new.created_at := now();
  new.updated_at := now();
  new.last_activity_at := now();
  return new;
end;
$$;

create or replace function public.forum_replies_before_insert()
returns trigger
language plpgsql
as $$
begin
  new.author_name := public.forum_normalize_author(new.author_name);
  new.body := trim(new.body);
  new.status := 'visible';
  new.created_at := now();
  new.updated_at := now();
  return new;
end;
$$;

create or replace function public.forum_replies_after_insert()
returns trigger
language plpgsql
security definer
set search_path = public
as $$
begin
  update public.forum_posts
  set reply_count = reply_count + 1,
      last_activity_at = now(),
      updated_at = now()
  where id = new.post_id and status = 'visible';
  return new;
end;
$$;

drop trigger if exists forum_posts_before_insert_trigger on public.forum_posts;
create trigger forum_posts_before_insert_trigger
before insert on public.forum_posts
for each row execute function public.forum_posts_before_insert();

drop trigger if exists forum_replies_before_insert_trigger on public.forum_replies;
create trigger forum_replies_before_insert_trigger
before insert on public.forum_replies
for each row execute function public.forum_replies_before_insert();

drop trigger if exists forum_replies_after_insert_trigger on public.forum_replies;
create trigger forum_replies_after_insert_trigger
after insert on public.forum_replies
for each row execute function public.forum_replies_after_insert();

alter table public.forum_posts enable row level security;
alter table public.forum_replies enable row level security;

revoke all on table public.forum_posts from anon, authenticated;
revoke all on table public.forum_replies from anon, authenticated;

grant select (id, author_name, title, body, category, status, reply_count, last_activity_at, created_at)
  on public.forum_posts to anon, authenticated;
grant insert (author_name, title, body, category, client_id)
  on public.forum_posts to anon, authenticated;

grant select (id, post_id, author_name, body, status, created_at)
  on public.forum_replies to anon, authenticated;
grant insert (post_id, author_name, body, client_id)
  on public.forum_replies to anon, authenticated;

grant execute on function public.forum_url_count(text) to anon, authenticated;
grant execute on function public.forum_recent_post_count(uuid, interval) to anon, authenticated;
grant execute on function public.forum_recent_reply_count(uuid, interval) to anon, authenticated;

drop policy if exists "forum posts are publicly readable" on public.forum_posts;
create policy "forum posts are publicly readable"
on public.forum_posts
for select
to anon, authenticated
using (status = 'visible');

drop policy if exists "anonymous users can create forum posts" on public.forum_posts;
create policy "anonymous users can create forum posts"
on public.forum_posts
for insert
to anon, authenticated
with check (
  status = 'visible'
  and client_id is not null
  and category in ('discussion', 'bug', 'balance', 'build', 'install')
  and char_length(trim(title)) between 1 and 120
  and char_length(trim(body)) between 1 and 10000
  and public.forum_url_count(body) <= 5
  and public.forum_recent_post_count(client_id, interval '10 minutes') < 3
  and public.forum_recent_post_count(client_id, interval '1 day') < 20
);

drop policy if exists "forum replies are publicly readable" on public.forum_replies;
create policy "forum replies are publicly readable"
on public.forum_replies
for select
to anon, authenticated
using (
  status = 'visible'
  and exists (
    select 1
    from public.forum_posts post
    where post.id = forum_replies.post_id
      and post.status = 'visible'
  )
);

drop policy if exists "anonymous users can create forum replies" on public.forum_replies;
create policy "anonymous users can create forum replies"
on public.forum_replies
for insert
to anon, authenticated
with check (
  status = 'visible'
  and client_id is not null
  and char_length(trim(body)) between 1 and 5000
  and public.forum_url_count(body) <= 5
  and exists (
    select 1
    from public.forum_posts post
    where post.id = forum_replies.post_id
      and post.status = 'visible'
  )
  and public.forum_recent_reply_count(client_id, interval '10 minutes') < 10
  and public.forum_recent_reply_count(client_id, interval '1 day') < 80
);
