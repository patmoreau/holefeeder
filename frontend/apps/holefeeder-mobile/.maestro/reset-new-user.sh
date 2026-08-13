#!/usr/bin/env sh
# Wipes the never-registered E2E user's rows so the onboarding flows can run again.
# Takes that user's access token and reads the subject out of it, so the identity
# always matches the token the flows will inject.
#
# Talks to the local database directly. There is no API for this on purpose: the
# backend has no "delete my account" endpoint, and inventing one for tests would put
# a destructive route in production.
set -e

TOKEN="$1"
CONTAINER="${E2E_POSTGRES_CONTAINER:-holefeeder_postgres}"
DATABASE="${E2E_POSTGRES_DB:-holefeeder}"

if [ -z "$TOKEN" ]; then
  echo "reset-new-user.sh needs the unregistered user's access token" >&2
  exit 1
fi

SUBJECT=$(TOKEN="$TOKEN" python3 - <<'SUBJECT_PY'
import base64, json, os, sys

try:
    payload = os.environ['TOKEN'].split('.')[1]
    payload += '=' * (-len(payload) % 4)
    print(json.loads(base64.urlsafe_b64decode(payload))['sub'])
except Exception as error:  # noqa: BLE001 - any malformed token is the same failure
    print(f'Could not read the subject out of the token: {error}', file=sys.stderr)
    sys.exit(1)
SUBJECT_PY
)

if ! docker exec "$CONTAINER" true 2>/dev/null; then
  echo "Cannot reach the '$CONTAINER' container — is the local stack up (cd backend && docker compose up -d)?" >&2
  exit 1
fi

# Children first: every table points at users, and user_identities is how the
# subject resolves to a user id.
docker exec -e SUBJECT="$SUBJECT" "$CONTAINER" sh -c '
  psql -v ON_ERROR_STOP=1 -q -U "$POSTGRES_USER" -d '"$DATABASE"' \
    -v subject="$SUBJECT" <<SQL
BEGIN;
CREATE TEMP TABLE doomed AS
  SELECT user_id FROM user_identities WHERE identity_object_id = :'"'"'subject'"'"';
DELETE FROM transactions WHERE user_id IN (SELECT user_id FROM doomed);
DELETE FROM cashflows WHERE user_id IN (SELECT user_id FROM doomed);
DELETE FROM store_items WHERE user_id IN (SELECT user_id FROM doomed);
DELETE FROM categories WHERE user_id IN (SELECT user_id FROM doomed);
DELETE FROM accounts WHERE user_id IN (SELECT user_id FROM doomed);
DELETE FROM user_identities WHERE identity_object_id = :'"'"'subject'"'"';
DELETE FROM users WHERE id IN (SELECT user_id FROM doomed);
COMMIT;
SQL
'

echo "Reset the never-registered E2E user ($SUBJECT)"
