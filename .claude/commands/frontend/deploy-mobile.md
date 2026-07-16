---
mode: agent
name: deploy-mobile
description: >
  Deploy Holefeeder to a connected physical iPhone.
---

# Skill: Deploy Holefeeder to mobile device

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

3. **Report the result.** A successful run ends with `Build Succeeded`
   (0 errors) followed by `Installing …Holefeeder.app` → `Complete 100%`.
   Report success or surface the build error.

## Notes

- The device UDID is stable per device; discover it each run rather than
  hardcoding, so this keeps working if the phone is replaced.
- `pnpm run ios:deploy` first runs `preios:deploy` (`expo prebuild --clean`),
  which regenerates the native `ios/` directory before building.
