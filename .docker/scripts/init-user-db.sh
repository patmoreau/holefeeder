#!/bin/bash
echo "** Creating users and privileges"
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
	CREATE USER $POSTGRES_APP_USER WITH PASSWORD '$POSTGRES_APP_PASSWORD';
	CREATE DATABASE $POSTGRES_APP_DB;
	GRANT ALL PRIVILEGES ON DATABASE $POSTGRES_APP_DB TO $POSTGRES_APP_USER;
    \connect $POSTGRES_APP_DB
    GRANT ALL PRIVILEGES ON SCHEMA public TO $POSTGRES_APP_USER;
EOSQL
echo "** Finished creating users and privileges"
