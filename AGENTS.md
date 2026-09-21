# Holefeeder — Agent Instructions

## Engineering principles
@docs/contributing/engineering-principles.md

## Git workflow
@docs/contributing/git-instructions.md

## Task playbooks

Recurring procedures live in `docs/contributing/tasks/<area>/`, one file per task.
Read the matching playbook before starting that work:

| Task | When |
|---|---|
| `backend/add-api-endpoint.md` | Adding a REST endpoint to the Holefeeder API |
| `backend/deploy-staging.md` | Deploying the backend to Staging |
| `frontend/deploy-mobile.md` | Installing the production mobile build on a physical iPhone |
| `frontend/trust-simulator-cert.md` | Making an iOS Simulator trust the local mkcert root CA |

`scripts/sync-agent-files.sh` renders each playbook into the per-assistant command
formats — `.claude/commands/<area>/` (invocable as `/<area>:<task>`) and
`.github/prompts/` (invocable as `/<task>`). Those copies are **generated**: edit the
file in `docs/contributing/tasks/`, rerun the script, and commit both.
`scripts/sync-agent-files.sh --check` runs in the `linter` workflow and fails when a
copy is stale or orphaned.

Each source needs front matter with `name` (matching its filename) and a one-line
`description`, and must sit in an area directory rather than directly in `tasks/`.

## Structure

Monorepo with two independent sub-projects, each with its own authoritative agent guide:

| Path | Contents | Guide |
|---|---|---|
| `backend/` | .NET 10 solution — `Holefeeder.Api`, `.Application`, `.Domain`, `.Infrastructure`, plus the Angular SPA in `src/Holefeeder.Web/ClientApp` | `backend/CLAUDE.md` |
| `frontend/` | pnpm + Turborepo workspace — Expo/React Native mobile app, React web app, shared `@holefeeder/shared` | `frontend/CLAUDE.md`, and `frontend/apps/holefeeder-mobile/CLAUDE.md` for the mobile app |
| `docs/business-rules/` | Language-agnostic business rules shared by both sub-projects — consult when implementing or validating domain logic in either | — |
| `docs/design/`, `docs/plans/` | Design notes and work plans | — |
| `docs/contributing/` | Process docs and task playbooks, for people working on this repo | — |

The sub-project guides own their own build, test and convention details. This file and
`docs/contributing/` hold only what applies to the whole repository.

### Local development infrastructure

Shared services (Traefik reverse proxy, PostgreSQL, PowerSync) start via Docker Compose
from `backend/`:

```bash
cd backend && docker compose --profile local up -d
```

`--profile local` is required: `reverse-proxy` (Traefik), `adminer`, `portainer` and
`whoami` are declared with `profiles: ["local"]`, so a plain `docker compose up -d`
starts the API, PostgreSQL, PowerSync and the web app with no reverse proxy in front of
them. Everything then fails to resolve on `*.localtest.me` and clients report "cannot
reach the server".

Both sub-projects depend on this stack for local development.

## CI

Workflows on `main` and pull requests: `ci-cd`, `linter`, `frontend`, `codeql-analysis`.
Watch them for success after a push, per the git workflow above.
