#!/usr/bin/env sh
# Runs the Maestro suite for a given tag (default: regression).
# Credentials come from .maestro/.env.e2e.local, gitignored — see .env.e2e.template.
set -e

TAG="${1:-regression}"
MAESTRO_DIR="$(cd "$(dirname "$0")" && pwd)"
ENV_FILE="$MAESTRO_DIR/.env.e2e.local"

if [ ! -f "$ENV_FILE" ]; then
  echo "Missing $ENV_FILE — copy .env.e2e.template to .env.e2e.local (both in .maestro/) and fill it in." >&2
  exit 1
fi

set -a
. "$ENV_FILE"
set +a

require() {
  eval "value=\$$1"
  if [ -z "$value" ]; then
    echo "$1 is not set in $ENV_FILE" >&2
    exit 1
  fi
}

require E2E_EMAIL
require E2E_PASSWORD

# The two tags need different builds: `auth` drives the real login pages and so
# needs the Auth0 build, while everything else injects a session and needs the
# E2E build. Reading the flag out of the installed bundle turns a baffling
# mid-flow failure into an immediate explanation.
assert_installed_build() {
  udid=$(xcrun simctl list devices booted 2>/dev/null | sed -n 's/.*(\([0-9A-F-]\{36\}\)) (Booted).*/\1/p' | head -1 | tr -d '[:space:]')
  if [ -z "$udid" ]; then
    return 0
  fi

  container=$(xcrun simctl get_app_container "$udid" com.drifterapps.holefeeder app 2>/dev/null) || container=''
  config="$container/EXConstants.bundle/app.config"
  if [ -z "$container" ] || [ ! -f "$config" ]; then
    return 0
  fi

  installed=$(python3 -c "import json,sys; print(json.load(open(sys.argv[1])).get('extra',{}).get('EXPO_PUBLIC_E2E') or '')" "$config")

  if [ "$TAG" = "auth" ] && [ "$installed" = "true" ]; then
    echo "The installed app is the E2E build, whose login button is a no-op." >&2
    echo "Flows tagged 'auth' drive the real Auth0 pages — run: pnpm ios:e2e:auth" >&2
    exit 1
  fi

  if [ "$TAG" != "auth" ] && [ "$installed" != "true" ]; then
    echo "The installed app is not the E2E build, so the session link is ignored." >&2
    echo "Flows tagged '$TAG' inject a session — run: pnpm ios:e2e" >&2
    exit 1
  fi
}

assert_installed_build

# Mints an access token for E2E_USERNAME / E2E_USERPASSWORD through the password
# grant, so flows never have to drive the login pages.
mint_token() {
  python3 - <<'TOKEN_PY'
import json, os, sys, urllib.error, urllib.request

payload = json.dumps({
    'grant_type': 'http://auth0.com/oauth/grant-type/password-realm',
    'realm': os.environ['E2E_AUTH0_REALM'],
    'client_id': os.environ['E2E_AUTH0_CLIENT_ID'],
    'audience': os.environ['E2E_AUTH0_AUDIENCE'],
    'scope': os.environ['E2E_AUTH0_SCOPE'],
    'username': os.environ['E2E_USERNAME'],
    'password': os.environ['E2E_USERPASSWORD'],
}).encode()

request = urllib.request.Request(
    f"https://{os.environ['E2E_AUTH0_DOMAIN']}/oauth/token",
    data=payload,
    headers={'Content-Type': 'application/json'},
)

try:
    with urllib.request.urlopen(request, timeout=30) as response:
        print(json.load(response)['access_token'])
except urllib.error.HTTPError as error:
    body = json.loads(error.read() or b'{}')
    # Never echo the request: it carries the password.
    print(f"Auth0 refused the token request: {body.get('error')} — {body.get('error_description')}", file=sys.stderr)
    sys.exit(1)
TOKEN_PY
}

MAESTRO_E2E_TOKEN=''

# Flows tagged `auth` drive the real login pages and need no token. Everything
# else takes its session from an injected one, so mint it up front and fail
# loudly rather than letting a flow die on an unexplained blank screen.
if [ "$TAG" != "auth" ]; then
  require E2E_AUTH0_DOMAIN
  require E2E_AUTH0_CLIENT_ID
  require E2E_AUTH0_AUDIENCE
  require E2E_AUTH0_REALM
  require E2E_AUTH0_SCOPE

  MAESTRO_E2E_TOKEN=$(E2E_USERNAME="$E2E_EMAIL" E2E_USERPASSWORD="$E2E_PASSWORD" mint_token)
fi

# The Auth0 session outlives the app: clearState and clearKeychain do not touch the
# cookie jar that ASWebAuthenticationSession shares with Safari. So these flows are
# not independent — one that signs in leaves the next one facing silent SSO instead
# of the hosted pages it waits for. Run them in an order that keeps the session in a
# known state: logout clears it, signup and the cancelled signup leave it clear, and
# login is last because it leaves one behind.
if [ "$TAG" = "auth" ]; then
  status=0
  for flow in logout.yaml signup.yaml signup-cancelled.yaml login.yaml; do
    echo ""
    echo "== $flow"
    maestro test \
      -e MAESTRO_E2E_EMAIL="$E2E_EMAIL" \
      -e MAESTRO_E2E_PASSWORD="$E2E_PASSWORD" \
      "$MAESTRO_DIR/flows/auth/$flow" || status=1
  done
  exit "$status"
fi

# Onboarding flows need a caller the backend has never seen — a different Auth0 user
# — and that user's rows wiped so the run repeats.
MAESTRO_E2E_UNREGISTERED_TOKEN=''
if [ "$TAG" = "onboarding" ]; then
  require E2E_NEW_EMAIL
  require E2E_NEW_PASSWORD

  MAESTRO_E2E_UNREGISTERED_TOKEN=$(E2E_USERNAME="$E2E_NEW_EMAIL" E2E_USERPASSWORD="$E2E_NEW_PASSWORD" mint_token)

  # One flow at a time, each behind its own reset: the first flow registers the
  # user, and every flow after it would then meet an account that already exists
  # and never see onboarding at all.
  status=0
  for flow in "$MAESTRO_DIR"/flows/onboarding/*.yaml; do
    echo ""
    echo "== $(basename "$flow")"
    "$MAESTRO_DIR/reset-new-user.sh" "$MAESTRO_E2E_UNREGISTERED_TOKEN"
    maestro test \
      -e MAESTRO_E2E_EMAIL="$E2E_EMAIL" \
      -e MAESTRO_E2E_PASSWORD="$E2E_PASSWORD" \
      -e MAESTRO_E2E_TOKEN="$MAESTRO_E2E_TOKEN" \
      -e MAESTRO_E2E_UNREGISTERED_TOKEN="$MAESTRO_E2E_UNREGISTERED_TOKEN" \
      "$flow" || status=1
  done
  exit "$status"
fi

exec maestro test \
  --include-tags="$TAG" \
  -e MAESTRO_E2E_EMAIL="$E2E_EMAIL" \
  -e MAESTRO_E2E_PASSWORD="$E2E_PASSWORD" \
  -e MAESTRO_E2E_TOKEN="$MAESTRO_E2E_TOKEN" \
  -e MAESTRO_E2E_UNREGISTERED_TOKEN="$MAESTRO_E2E_UNREGISTERED_TOKEN" \
  "$MAESTRO_DIR"
