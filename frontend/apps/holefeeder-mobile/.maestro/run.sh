#!/usr/bin/env sh
# Runs the Maestro suite for a given tag (default: regression).
# Credentials come from .env.e2e.local, which is gitignored — see .env.e2e.template.
set -e

TAG="${1:-regression}"
MAESTRO_DIR="$(cd "$(dirname "$0")" && pwd)"
ENV_FILE="$MAESTRO_DIR/../.env.e2e.local"

if [ ! -f "$ENV_FILE" ]; then
  echo "Missing $ENV_FILE — copy .env.e2e.template to .env.e2e.local and fill it in." >&2
  exit 1
fi

set -a
. "$ENV_FILE"
set +a

exec maestro test \
  --include-tags="$TAG" \
  -e MAESTRO_E2E_EMAIL="$E2E_EMAIL" \
  -e MAESTRO_E2E_PASSWORD="$E2E_PASSWORD" \
  "$MAESTRO_DIR"
