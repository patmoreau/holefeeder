uri='mongodb://192.168.2.12:32768/holefeeder'

mongoimport --uri $uri --collection accounts --file accounts.json --drop
mongoimport --uri $uri --collection cashflows --file cashflows.json --drop
mongoimport --uri $uri --collection categories --file categories.json --drop
mongoimport --uri $uri --collection objectsData --file objectsData.json --drop
mongoimport --uri $uri --collection transactions --file transactions.json --drop
