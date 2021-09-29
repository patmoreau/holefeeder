database="$MYSQL_BUDGETING_DATABASE"

echo "** Creating $database database and users"

sql=$(cat << EOF
CREATE DATABASE IF NOT EXISTS $database;
GRANT CREATE ON *.* TO '$MYSQL_USER'@'%';
GRANT ALL PRIVILEGES ON $database.* TO '$MYSQL_USER'@'%';
EOF
)

mysql -u root -p$MYSQL_ROOT_PASSWORD -e "$sql"

echo "** Finished creating $database database and users"