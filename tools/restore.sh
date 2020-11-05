uri=$1 # mongodb://user:pass@localhost:27017/holefeeder?authSource=holefeeder
container=$2

docker cp accounts.json $container:/tmp/accounts.json
docker cp cashflows.json $container:/tmp/cashflows.json
docker cp categories.json $container:/tmp/categories.json
docker cp objectsData.json $container:/tmp/objectsData.json
docker cp transactions.json $container:/tmp/transactions.json
docker cp users.json $container:/tmp/users.json

docker exec $container mongoimport --uri $uri -c accounts --file=/tmp/accounts.json --mode=upsert
docker exec $container mongoimport --uri $uri -c cashflows --file=/tmp/cashflows.json --mode=upsert
docker exec $container mongoimport --uri $uri -c categories --file=/tmp/categories.json --mode=upsert
docker exec $container mongoimport --uri $uri -c objectsData --file=/tmp/objectsData.json --mode=upsert
docker exec $container mongoimport --uri $uri -c transactions --file=/tmp/transactions.json --mode=upsert
docker exec $container mongoimport --uri $uri -c users --file=/tmp/users.json --mode=upsert
