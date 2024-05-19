#!/bin/bash
echo "** Creating users and privileges"

# Create the database
mysql -u root -p"$(cat /etc/mysql/credentials/MYSQL_ROOT_PASSWORD)" -e \
    "CREATE DATABASE IF NOT EXISTS $(cat /etc/mysql/credentials/MYSQL_DATABASE);"

# Create the user
mysql -u root -p"$(cat /etc/mysql/credentials/MYSQL_ROOT_PASSWORD)" -e \
    "CREATE USER '$(cat /etc/mysql/credentials/MYSQL_USER)'@'%' IDENTIFIED BY '$(cat /etc/mysql/credentials/MYSQL_PASSWORD)';"

# Grant all privileges on the specific database to the user
mysql -u root -p"$(cat /etc/mysql/credentials/MYSQL_ROOT_PASSWORD)" -e \
    "GRANT ALL ON $(cat /etc/mysql/credentials/MYSQL_DATABASE).* TO '$(cat /etc/mysql/credentials/MYSQL_USER)'@'%';"

# Flush privileges to ensure they take effect immediately
mysql -u root -p"$(cat /etc/mysql/credentials/MYSQL_ROOT_PASSWORD)" -e \
    "FLUSH PRIVILEGES;"

echo "** Finished creating users and privileges"
