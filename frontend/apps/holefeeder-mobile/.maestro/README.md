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

The two tags need **different builds**, because the E2E build replaces Auth0 with a
link-driven stand-in whose login button does nothing:

```bash
pnpm ios:e2e && pnpm test:e2e:ios        # tag: regression — injected session, fast
pnpm ios:e2e:auth && pnpm test:e2e:auth  # tag: auth — real Auth0 login, slow
```

`run.sh` reads the flag out of the installed bundle and refuses to run against the wrong
build, so a mismatch fails immediately with an explanation instead of mid-flow.

Both builds are Release, which embeds the JS bundle — cold launches need no Metro, and
that is what makes `clearState` safe. They point at the development endpoints
(`localtest.me`), so the local Docker stack must be up.

## Credentials

`run.sh` reads `../.env.e2e.local`, which is gitignored. Create it once:

```bash
cp .env.e2e.template .env.e2e.local
```

Fill in the Auth0 dev-tenant user plus the dedicated E2E application (a Native app with
the password grant enabled, authorized for the API with `read:user write:user`).

For any tag other than `auth`, `run.sh` mints an access token from that application via
the password grant and passes it to Maestro. Flows reach the values as
`${MAESTRO_E2E_TOKEN}`, `${MAESTRO_E2E_EMAIL}` and `${MAESTRO_E2E_PASSWORD}`. A refused
token request aborts the run with Auth0's reason; the password is never echoed.

Quote any value containing spaces (`E2E_AUTH0_SCOPE`) — the file is sourced by `sh`.

## Tags

| Tag          | Build           | Meaning                                                  |
| ------------ | --------------- | -------------------------------------------------------- |
| `regression` | `ios:e2e`       | Session injected by link. Fast. Run on every change.      |
| `auth`       | `ios:e2e:auth`  | Drives the hosted Auth0 pages. Slow, network-bound.       |

An untagged flow runs under no tag and so is never executed — always tag.

## Selectors

Prefer `id:` over visible text: text is translated, ids are not. Ids come from
the `testID` prop on the `App*` wrappers, which `@expo/ui` maps to
`accessibilityIdentifier`. Naming is `<screen>-<element>`, e.g.
`welcome-signup-button`. See the mobile `CLAUDE.md` for the full convention.

Text selectors are unavoidable inside the Auth0 pages, which are not our UI.
