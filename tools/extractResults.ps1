Remove-Item tools\testresults -Recurse

$unittestslayerid=@(docker images --filter "label=testrunnerlayer=true" -q)
Write-Host "Choosing: {$($unittestslayerid[0])}"

docker create --name unittestcontainer $unittestslayerid[0]
docker cp unittestcontainer:/out/ tools/testresults
docker stop unittestcontainer
docker rm unittestcontainer