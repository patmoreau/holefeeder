---
name: commit
description: >
  Commit staged or specified changes following project conventions.
  Load when the user asks to commit, stage changes, or create a git commit.
---

Before committing:
1. Run `pnpm lint` — fix all errors before proceeding.
2. Run `pnpm typecheck` if TypeScript files were modified.
3. Run `pnpm test` if source files were modified.

Commit message format: Conventional Commits (`type(scope): description`).
- Types: feat, fix, chore, refactor, test, docs, style
- Scope: the app or package affected (e.g. `mobile`, `web`, `core`)
- Keep the subject line under 72 characters

Stage specific files — never `git add -A` or `git add .`.

Do not commit `.env`, secrets, or lock files unless explicitly asked.

End every commit message with:
Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>
