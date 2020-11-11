uri=$1 # mongodb://user:pass@localhost:27017/holefeeder?authSource=holefeeder
container=$2

docker exec -i $container sh -c "mongoimport --uri $uri --collection accounts --mode upsert" < accounts.json
docker exec -i $container sh -c "mongoimport --uri $uri --collection cashflows --mode upsert" < cashflows.json
docker exec -i $container sh -c "mongoimport --uri $uri --collection categories --mode upsert" < categories.json
docker exec -i $container sh -c "mongoimport --uri $uri --collection objectsData --mode upsert" < objectsData.json
docker exec -i $container sh -c "mongoimport --uri $uri --collection transactions --mode upsert" < transactions.json
