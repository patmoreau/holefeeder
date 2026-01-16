#!/bin/bash
set -e

echo "Starting DB Migrator..."

# Wait for Postgres to be ready
until pg_isready -h postgres -U "$POSTGRES_USER"; do
  echo "Waiting for postgres..."
  sleep 2
done

echo "Postgres is ready. Applying manual migrations..."

# Run the idempotent SQL
# Note: We connect to POSTGRES_APP_DB but as SuperUser (POSTGRES_USER) to have permissions
psql -v ON_ERROR_STOP=1 -h postgres -U "$POSTGRES_USER" -d "$POSTGRES_APP_DB" <<-EOSQL
    -- 1. Grant Replication to App User (Idempotent: ALTER is safe)
    ALTER USER $POSTGRES_APP_USER WITH REPLICATION;

    -- 2. Create Publication if it doesn't exist
    DO \$\$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_publication WHERE pubname = 'powersync') THEN
            CREATE PUBLICATION powersync FOR ALL TABLES;
        END IF;
    END
    \$\$;
EOSQL

echo "Migration script completed successfully."
