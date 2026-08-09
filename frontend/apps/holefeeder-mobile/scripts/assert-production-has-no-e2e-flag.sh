#!/usr/bin/env sh
# Fails if a production app config can carry EXPO_PUBLIC_E2E.
#
# That flag swaps Auth0 for a link-driven stand-in, so shipping it would ship an
# authentication bypass. app.config.ts drops it whenever APP_ENV=production; this
# forces the variable on and proves the drop still happens.
set -e

cd "$(dirname "$0")/.."

value=$(APP_ENV=production EXPO_PUBLIC_E2E=true npx expo config --type public --json |
  node -e "
    let raw = '';
    process.stdin.on('data', (chunk) => (raw += chunk));
    process.stdin.on('end', () => {
      const extra = (JSON.parse(raw).extra) || {};
      process.stdout.write(extra.EXPO_PUBLIC_E2E == null ? '' : String(extra.EXPO_PUBLIC_E2E));
    });
  ")

if [ -n "$value" ]; then
  echo "A production config exposed EXPO_PUBLIC_E2E=$value — the E2E auth bypass could ship." >&2
  echo "Restore the APP_ENV guard on EXPO_PUBLIC_E2E in app.config.ts." >&2
  exit 1
fi

echo "Production config carries no E2E flag."
