---
description: >
  Install the local mkcert root CA into an iOS Simulator's keychain so it trusts
  the *.localtest.me TLS certs served by the local Traefik reverse-proxy (needed
  for PowerSync wss:// and the API to connect from the simulator). Load when the
  user asks to trust the local/mkcert cert on the simulator, fix simulator TLS /
  WSS handshake failures, or add a root cert to a simulator.
---

Goal: run `xcrun simctl keychain <UDID> add-root-cert "$(mkcert -CAROOT)/rootCA.pem"`
against the simulator the user chooses. Never hardcode a UDID — always discover
the current simulators and ask which one.

## Steps

1. **Sanity-check prerequisites.** Confirm the mkcert root CA exists:
   ```bash
   test -f "$(mkcert -CAROOT)/rootCA.pem" && echo "rootCA: $(mkcert -CAROOT)/rootCA.pem" || echo "MISSING"
   ```
   If `mkcert` is not installed or `rootCA.pem` is missing, stop and tell the user
   to install mkcert / run `mkcert -install` first — do not proceed.

2. **List simulators and identify candidates.** Prefer a Booted device (the one the
   user is actively running); fall back to all available devices if none is booted:
   ```bash
   xcrun simctl list devices available
   ```
   Note the `Booted` device(s) and each device's name + UDID (the UUID in parentheses).

3. **Ask which simulator** using the AskUserQuestion tool. Present the discovered
   devices as options, labeled with the device name and state, e.g.
   `iPhone 16 Pro — Booted`. Put any Booted device first and mark it
   `(Recommended)`. Do not guess — the user picks. If exactly one device is Booted
   and the user has clearly implied "the one I'm using", you may still confirm it is
   the intended target before running.

4. **Run the command** with the chosen UDID:
   ```bash
   xcrun simctl keychain <UDID> add-root-cert "$(mkcert -CAROOT)/rootCA.pem"
   ```
   A silent exit 0 means success (the command prints nothing on success).

5. **Report and advise reboot.** Tell the user it succeeded and that the simulator
   usually needs a reboot to pick up the new trusted root:
   ```bash
   xcrun simctl shutdown <UDID> && xcrun simctl boot <UDID>
   ```
   Offer to run the reboot; don't do it unprompted (it disrupts their running app).

## Notes

- The target simulator must exist; if the user's device isn't Booted, adding the
  cert still works (it is written to that device's keychain), but it must be booted
  to use it.
- This only makes the simulator trust the cert. It does NOT start the Traefik
  reverse-proxy or the backend — if `https://*.localtest.me` returns connection
  refused (`000`), that is a separate infra problem (proxy down), not a trust issue.
