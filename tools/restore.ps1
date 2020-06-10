[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $Uri, # mongodb://user:pass@localhost:27017/holefeeder?authSource=holefeeder

    [Parameter()]
    [string]
    $Container
)

Start-Process -FilePath docker -ArgumentList "cp","accounts.json","$Container\:/tmp/accounts.json" -Wait
Start-Process -FilePath docker -ArgumentList "cp","cashflows.json","$Container\:/tmp/cashflows.json" -Wait
Start-Process -FilePath docker -ArgumentList "cp","categories.json","$Container\:/tmp/categories.json" -Wait
Start-Process -FilePath docker -ArgumentList "cp","objectsData.json","$Container\:/tmp/objectsData.json" -Wait
Start-Process -FilePath docker -ArgumentList "cp","transactions.json","$Container\:/tmp/transactions.json" -Wait
Start-Process -FilePath docker -ArgumentList "cp","users.json","$Container\:/tmp/users.json" -Wait

Start-Process -FilePath docker -ArgumentList "exec","$Container","mongoimport","--uri","$Uri","-c accounts","--file=/tmp/accounts.json","--mode=upsert" -Wait
Start-Process -FilePath docker -ArgumentList "exec","$Container","mongoimport","--uri","$Uri","-c cashflows","--file=/tmp/cashflows.json","--mode=upsert" -Wait
Start-Process -FilePath docker -ArgumentList "exec","$Container","mongoimport","--uri","$Uri","-c categories","--file=/tmp/categories.json","--mode=upsert" -Wait
Start-Process -FilePath docker -ArgumentList "exec","$Container","mongoimport","--uri","$Uri","-c objectsData","--file=/tmp/objectsData.json","--mode=upsert" -Wait
Start-Process -FilePath docker -ArgumentList "exec","$Container","mongoimport","--uri","$Uri","-c transactions","--file=/tmp/transactions.json","--mode=upsert" -Wait
Start-Process -FilePath docker -ArgumentList "exec","$Container","mongoimport","--uri","$Uri","-c users","--file=/tmp/users.json","--mode=upsert" -Wait
