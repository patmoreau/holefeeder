uri=$1 # mongodb://user:pass@localhost:27017/holefeeder?authSource=holefeeder
container=$2

docker exec -i $container mongoexport --uri $uri -c accounts > accounts.json
docker exec -i $container mongoexport --uri $uri -c cashflows > cashflows.json
docker exec -i $container mongoexport --uri $uri -c categories > categories.json
docker exec -i $container mongoexport --uri $uri -c objectsData > objectsData.json
docker exec -i $container mongoexport --uri $uri -c transactions > transactions.json
