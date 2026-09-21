# Git Instructions

> **Audience:** contributors *working on* this repository. See [AGENTS.md](../../AGENTS.md).

## Commit Branching Strategy

1. Commit directly to main unless branching is specified.

## Commit Discipline

Only commit when:

1. ALL tests are passing
2. ALL compiler/linter warnings resolved
3. Single logical unit of work

"ALL tests" includes the Maestro E2E tags when the change touches the mobile app —
`regression`, `onboarding` and `auth`, each against the build its tag requires. They live
outside `pnpm test` and CI, so nothing else will catch a flow that has gone stale. See
`frontend/apps/holefeeder-mobile/CLAUDE.md`.

Never mix structural and behavioral changes in the same commit.
Always make structural changes first when both are needed.

## Commit Message Rules

- Use Conventional Commits specification
- Subject line max 50 characters, capitalized, no trailing period
- Separate subject from body with blank line
- Wrap body at 72 characters
- Imperative mood ("Add unit tests" not "Added unit tests")
- State whether commit is structural or behavioral
- Add co-authored tag with agent name and model used similar to this
  `Co-Authored-By: Claude Opus 5 <noreply@anthropic.com>`

## Never

- Commit secrets or confidential information
- Push to GitHub by yourself
