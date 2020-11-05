[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $HostName,

    [Parameter()]
    [string]
    $DatabaseName,

    [Parameter()]
    [string]
    $User,

    [Parameter()]
    [SecureString]
    $Password
)

$MongoExportToolPath='C:\Program Files\MongoDB\Tools\100\bin\mongoexport.exe'

$args = @(
    "-h $HostName",
    "-d $DatabaseName",
    "-u $User",
    "-p $Password",
    "-c accounts",
    "-o accounts.json"
)

Start-Process -FilePath $MongoExportToolPath -ArgumentList $args -Wait


host=mongodb://holefeederApi:6zu0vr8xJ3RD8h1kgjpb@localhost:27017/holefeeder?authSource=holefeeder
container=holefeeder_mongodb_1

docker exec -i $container mongoexport --uri $host -c accounts > accounts.json
docker exec -i $container mongoexport --uri $host -c cashflows > cashflows.json
docker exec -i $container mongoexport --uri $host -c categories > categories.json
docker exec -i $container mongoexport --uri $host -c objectsData > objectsData.json
docker exec -i $container mongoexport --uri $host -c transactions > transactions.json



scp root@drifterapps.com:/holefeeder/backup/accounts.json .