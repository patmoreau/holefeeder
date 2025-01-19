#!/bin/bash
echo "** Creating users and privileges"

# Create the database
psql -U postgres -c "CREATE DATABASE $(cat /etc/postgresql/credentials/POSTGRES_DATABASE);"

# Create the user
psql -U postgres -c "CREATE USER $(cat /etc/postgresql/credentials/POSTGRES_USER) WITH PASSWORD '$(cat /etc/postgresql/credentials/POSTGRES_PASSWORD)';"

# Grant all privileges on the specific database to the user
psql -U postgres -c "GRANT ALL PRIVILEGES ON DATABASE $(cat /etc/postgresql/credentials/POSTGRES_DATABASE) TO $(cat /etc/postgresql/credentials/POSTGRES_USER);"

echo "** Finished creating users and privileges"
