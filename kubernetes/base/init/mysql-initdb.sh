#!/bin/bash
echo "** Creating users and privileges"

mysql -u root -p"$(cat /etc/mysql/credentials/MYSQL_ROOT_PASSWORD)" -e \
  "CREATE USER '$(cat /etc/mysql/credentials/MYSQL_USER)'@'%' IDENTIFIED BY '$(cat /etc/mysql/credentials/MYSQL_PASSWORD)';"

mysql -u root -p"$(cat /etc/mysql/credentials/MYSQL_ROOT_PASSWORD)" -e \
  "GRANT CREATE ON *.* TO '$(cat /etc/mysql/credentials/MYSQL_USER)'@'%';"

mysql -u root -p"$(cat /etc/mysql/credentials/MYSQL_ROOT_PASSWORD)" -e \
  "GRANT ALL ON \`budgeting\_%\`.* TO '$(cat /etc/mysql/credentials/MYSQL_USER)'@'%';"

mysql -u root -p"$(cat /etc/mysql/credentials/MYSQL_ROOT_PASSWORD)" -e \
  "GRANT ALL ON \`object\_store\_%\`.* TO '$(cat /etc/mysql/credentials/MYSQL_USER)'@'%';"

echo "** Finished creating users and privileges"
