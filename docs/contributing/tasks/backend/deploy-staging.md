---
name: deploy-staging
description: Deploy Holefeeder to the Staging environment
---

# Skill: Deploy Holefeeder to Staging

> **Audience:** contributors *working on* this repository. Not a published document — see `AGENTS.md` at the repository root.

Use this skill to deploy Holefeeder to Staging.

Run from `backend/`. `GIT_COMMIT` stamps the web app's version label (bottom of the
navigation drawer, `dev · <commit>` outside CI):

```bash
GIT_COMMIT=$(git rev-parse HEAD) docker compose up -d --build
```
---
