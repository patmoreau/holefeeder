# Maestro E2E suite

End-to-end flows for the iOS app. Complements the Jest suite — Maestro covers
navigation and wiring, Jest covers use-case logic.

## Layout

```text
.maestro/
  config.yaml        workspace config (which files are flows)
  run.sh             loads credentials, runs a tag
  flows/             runnable flows, one user journey each
    auth/            flows that drive the real Auth0 pages
  subflows/          reusable fragments, never run on their own
```

## Running

```bash
pnpm test:e2e:ios      # tag: regression — the everyday suite
pnpm test:e2e:auth     # tag: auth — real Auth0 login, slow and network-bound
```

Both need a booted simulator with the app already installed (`pnpm ios`).

## Credentials

`run.sh` reads `../.env.e2e.local`, which is gitignored. Create it once:

```bash
cp .env.e2e.template .env.e2e.local
```

Then fill in the Auth0 dev-tenant user. Values reach flows as
`${MAESTRO_E2E_EMAIL}` and `${MAESTRO_E2E_PASSWORD}`.

## Tags

| Tag          | Meaning                                                             |
| ------------ | ------------------------------------------------------------------- |
| `regression` | Fast and deterministic. Safe to run on every change.                 |
| `auth`       | Drives the hosted Auth0 pages. Slow, flaky, real network. Run rarely.|

An untagged flow runs under no tag and so is never executed — always tag.

## Selectors

Prefer `id:` over visible text: text is translated, ids are not. Ids come from
the `testID` prop on the `App*` wrappers, which `@expo/ui` maps to
`accessibilityIdentifier`. Naming is `<screen>-<element>`, e.g.
`welcome-signup-button`. See the mobile `CLAUDE.md` for the full convention.

Text selectors are unavoidable inside the Auth0 pages, which are not our UI.
