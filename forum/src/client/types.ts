export type ForumCategory = "discussion" | "bug" | "balance" | "build" | "install";

export type ForumPost = {
  id: string;
  author_name: string;
  title: string;
  body: string;
  category: ForumCategory;
  status?: "visible" | "hidden" | "deleted";
  reply_count: number;
  last_activity_at: string;
  created_at: string;
};

export type ForumReply = {
  id: string;
  post_id: string;
  author_name: string;
  body: string;
  status?: "visible" | "hidden" | "deleted";
  created_at: string;
};

export type Route =
  | { name: "home" }
  | { name: "new" }
  | { name: "post"; id: string };
