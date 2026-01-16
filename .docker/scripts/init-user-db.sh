#!/bin/bash
echo "** Creating users and privileges"
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
	-- Create user with replication privilege
	CREATE USER $POSTGRES_APP_USER WITH PASSWORD '$POSTGRES_APP_PASSWORD' REPLICATION;

	CREATE DATABASE $POSTGRES_APP_DB;
	GRANT ALL PRIVILEGES ON DATABASE $POSTGRES_APP_DB TO $POSTGRES_APP_USER;

    -- Connect to the new database
    \connect $POSTGRES_APP_DB

    -- Grant schema privileges (required for Postgres 15+)
    GRANT ALL PRIVILEGES ON SCHEMA public TO $POSTGRES_APP_USER;

    -- Create publication for PowerSync
    -- This allows the powersync service to subscribe to changes
    CREATE PUBLICATION powersync FOR ALL TABLES;
EOSQL
echo "** Finished creating users and privileges"
