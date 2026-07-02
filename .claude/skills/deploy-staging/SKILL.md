---
mode: agent
name: deploy-staging
description: >
  Deploy Holefeeder to Staging environment.
---

# Skill: Deploy Holefeeder to Staging

Use this skill to deploy Holefeeder to Staging.

```bash
docker compose -f docker-compose.yaml -f docker-compose.staging.yaml up -d --build
```
---
