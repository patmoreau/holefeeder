@AGENTS.md

## Claude Code

The instructions above apply in full. A few Claude-specific notes:

- The sub-project guides are authoritative for their area — read `backend/CLAUDE.md`
  before backend work, `frontend/CLAUDE.md` (plus
  `frontend/apps/holefeeder-mobile/CLAUDE.md` for the mobile app) before frontend work.
- Verify against the source, not from memory of these files. When a guide disagrees with
  the code, the code wins and the guide is a bug to fix.
- Business rules in `docs/business-rules/` are shared by both sub-projects; a change to
  domain logic usually belongs there too.
- Commits: conventional-commit format from `docs/contributing/git-instructions.md`,
  structural and behavioral changes never mixed, the mobile E2E tags run when the change
  touches the mobile app, and never push.
