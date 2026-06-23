# Spire Plus Website

This is the static public-info site for `Spire Plus`. It records current player-facing effects, download links, known issues, and forum setup notes. It is not a release-ready claim.

## Local Preview

From the repository root:

```powershell
python -m http.server 4177 --bind 127.0.0.1
```

Open:

```text
http://127.0.0.1:4177/website/
```

In local preview, the current download button points to:

```text
../publish/SpirePlus-v0.1.0-private-beta.126.zip
```

## Public Deploy

`.github/workflows/spire-plus-site.yml` publishes `website/` to GitHub Pages.

In the public site, the current download button points to the versioned GitHub Release asset:

```text
https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/download/v0.1.0-private-beta.126/SpirePlus-v0.1.0-private-beta.126.zip
```

The release-page button points to:

```text
https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/tag/v0.1.0-private-beta.126
```

If the repository is private, GitHub Pages availability depends on the account plan. If GitHub reports that private Pages are unsupported, use a public Pages repository, make this repository public, or upgrade the plan before publishing the site.

Upload the zip asset to the matching GitHub Release before publishing the page.

## Forum

`website/#forum` links to the forum entry page. The forum UI is built into:

```text
website/forum/
```

The forum is still a static GitHub Pages page. Posts and replies are stored in Supabase PostgreSQL. Before enabling writes, create a Supabase project and run:

```text
forum/supabase/schema.sql
```

The GitHub Actions build expects these repository variables:

- `SPIRE_PLUS_SUPABASE_URL`
- `SPIRE_PLUS_SUPABASE_ANON_KEY`
- `SPIRE_PLUS_FORUM_READ_ONLY`, optional; set to `1` for read-only mode.

Without Supabase configuration, `/forum/` shows setup instructions instead of a fake form.

Full go-live steps are in `../docs/features/forum/go-live-checklist.md`.

## Editing

- Main content: `content-data.js`
- Page shell: `index.html`
- Styles: `styles.css`
- Rendering logic: `app.js`
- Images: `assets/`

Historical website QA journals belong under `../docs/archive/implementation-records/`, not in this public-site source folder.

`assets/` may contain Spire Plus-owned or generated resources, plus original game art only when permission and scope are documented. Current permission was confirmed by the project owner on 2026-05-23 for the vanilla relic icons and card portraits used on this site under `assets/source-art/`. Do not copy original non-art source materials from `source code/`.
