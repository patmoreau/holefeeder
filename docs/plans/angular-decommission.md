# Decommission the Angular web app

Status: **not started** — waits for an explicit go-ahead, recorded 2026-10-24.

## Context

`Holefeeder.Web` is an ASP.NET Core host serving two SPAs from one image: the React
app (`frontend/apps/holefeeder-web`) at `/`, and the legacy Angular app
(`backend/src/Holefeeder.Web/ClientApp`) under `/angular/`. The React drawer's Legacy
item links to it. .NET is only in the image to host Angular-era pieces; React itself is
plain static files.

## Once Angular is retired

- **Stop serving with .NET.** Replace `backend/src/Holefeeder.Web/Dockerfile` with a
  Node build stage for React plus a static file server (e.g. nginx) with an SPA
  fallback to `index.html`. Drop the .NET SDK, Node-in-SDK and Chrome stages.
- **Carry over what the .NET host still provides**, or confirm nothing uses it:
  - `/healthz` — container health check; the static server needs an equivalent.
  - `/config` (`Features/ConfigRoutes.cs`) — Angular only.
  - `holefeeder-admin.<domain>/hc-ui` — routed to this container by the Traefik label
    in `backend/docker-compose.override.yaml` (and the homelab gateway in production).
  - Serilog request logging to stdout (Loki in production).
- **Keep `/ngsw-worker.js`** (Angular's safety worker, in
  `frontend/apps/holefeeder-web/public/`) for a while after removal, so browsers still
  holding Angular's service worker unregister it.
- **Remove** `ClientApp/`, the Angular parts of `Holefeeder.Web.csproj` and
  `Program.cs`, the `/angular` fallback, the Legacy drawer item (and its
  `tabs.legacy` string), and the Angular lint command in `backend/CLAUDE.md`.
- **Build context.** The image builds from the repository root only to reach both
  `backend/` and `frontend/`; with no backend code left it can build from `frontend/`,
  and `/.dockerignore` plus `CICD_CODE_FILE_PATTERNS` shrink accordingly.
- **Auth0.** Remove the `/angular` callback and logout URLs from the SPA application.
- **Production.** The homelab Quadlet unit and Traefik routes (outside this repo) keep
  working if the image name and port stay the same; check them if either changes.
