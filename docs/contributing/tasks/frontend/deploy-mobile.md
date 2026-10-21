---
name: deploy-mobile
description: Build the production mobile app and install it on the connected iPhone
---

# Skill: Deploy Holefeeder to mobile device

> **Audience:** contributors *working on* this repository. Not a published document — see `AGENTS.md` at the repository root.

Build the Holefeeder production app and install it on the connected iPhone,
non-interactively (no device picker prompt) and without leaving a Metro
bundler process running afterward.

## Steps

1. **Discover the connected device UDID.** Never rely on the interactive
   device picker and never match by name (the name contains a typographic
   apostrophe that breaks shell quoting). Physical-device UDIDs have the form
   `XXXXXXXX-XXXXXXXXXXXXXXXX` (8 hex, dash, 16 hex):

   ```bash
   xcrun xctrace list devices 2>&1 | grep -iE "iphone|ipad" | grep -viE "simulator" \
     | grep -oE '[0-9A-Fa-f]{8}-[0-9A-Fa-f]{16}' | head -1
   ```

   If this prints nothing, the device is not connected/reachable — check
   `xcrun xctrace list devices` for a device under **Devices Offline**, ask the
   user to plug in and unlock the iPhone (tap **Trust** if prompted), then retry.

2. **Deploy to that UDID.** Run in the background (the native build takes
   several minutes) and pass the UDID explicitly:

   ```bash
   cd frontend/apps/holefeeder-mobile/ && pnpm run ios:deploy -- --device <UDID>
   ```

   The `ios:deploy` script uses `--no-bundler`, so once the app is built,
   installed, and launched, the CLI **exits on its own** — no lingering Metro
   process to stop. (Release builds embed the JS bundle, so Metro isn't needed
   at runtime.)

3. **Check the install, then record the deploy.** A successful run ends with
   `Build Succeeded` (0 errors) followed by `Installing …Holefeeder.app` →
   `Complete 100%`. `CommandError: … the device is locked` after that line
   only means the app could not be opened; the install itself succeeded.

   Only when the install completed, commit the version file the deploy wrote:

   ```bash
   git add frontend/apps/holefeeder-mobile/deploy-version.json
   git commit -m "chore(mobile): Deploy <version> (<build>)"
   ```

   with the version and build from the `Deploying …` line at the start of the
   run. If the build or the install failed, leave the file uncommitted: the
   next deploy recomputes the number from the last committed one.

4. **Report the result**: the version and build, the commit it was built from,
   and that Settings → About on the phone should show the same
   `<version> (<build>) · <commit>`.

## Notes

- The device UDID is stable per device; discover it each run rather than
  hardcoding, so this keeps working if the phone is replaced.
- `pnpm run ios:deploy` first runs `preios:deploy`, which writes the next
  version to `deploy-version.json` and then runs `expo prebuild --clean` to
  regenerate the native `ios/` directory before building.
- The version is the deploy date, `YYYY.M.D`, and the build counts that day's
  deploys from 1. iOS shows them as `2026.10.20 (2)`. The commit is the one the
  app was built from, with `-dirty` when the working tree had other changes.
  The next number is computed from the committed `deploy-version.json`, so it
  only moves once a deploy is recorded.
