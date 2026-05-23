import { createClient, type SupabaseClient } from "@supabase/supabase-js";
import type { ForumPost, ForumReply } from "./types";

type Database = {
  public: {
    Tables: {
      forum_posts: {
        Row: ForumPost;
        Insert: {
          author_name?: string;
          title: string;
          body: string;
          client_id: string;
        };
        Update: never;
        Relationships: [];
      };
      forum_replies: {
        Row: ForumReply;
        Insert: {
          post_id: string;
          author_name?: string;
          body: string;
          client_id: string;
        };
        Update: never;
        Relationships: [];
      };
    };
    Views: {};
    Functions: {};
    Enums: {};
    CompositeTypes: {};
  };
};

const url = import.meta.env.VITE_SUPABASE_URL as string | undefined;
const anonKey = import.meta.env.VITE_SUPABASE_ANON_KEY as string | undefined;

export const forumReadOnly = import.meta.env.VITE_FORUM_READ_ONLY === "1";
export const forumConfigured = Boolean(url && anonKey && !url.includes("your-project-ref"));

export const supabase: SupabaseClient<Database> | null = forumConfigured
  ? createClient<Database>(url!, anonKey!, {
      auth: {
        persistSession: false,
        autoRefreshToken: false
      }
    })
  : null;
