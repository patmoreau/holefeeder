---
mode: agent
description: Deploy Holefeeder to the Staging environment
---

<!-- Generated from docs/contributing/tasks/backend/deploy-staging.md by scripts/sync-agent-files.sh. Do not edit. -->

# Skill: Deploy Holefeeder to Staging

> **Audience:** contributors *working on* this repository. Not a published document — see `AGENTS.md` at the repository root.

Use this skill to deploy Holefeeder to Staging.

Run from `backend/`. `GIT_COMMIT` stamps the web app's version label (bottom of the
navigation drawer, `dev · <commit>` outside CI):

```bash
GIT_COMMIT=$(git rev-parse HEAD) docker compose up -d --build
```
---
