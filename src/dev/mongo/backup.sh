host=ds036967.mlab.com:36967
db=holefeeder
user=$1
password=$2

mongoexport -h $host -d $db -c accounts -u $user -p $password -o accounts.json
mongoexport -h $host -d $db -c cashflows -u $user -p $password -o cashflows.json
mongoexport -h $host -d $db -c categories -u $user -p $password -o categories.json
mongoexport -h $host -d $db -c objectsData -u $user -p $password -o objectsData.json
mongoexport -h $host -d $db -c transactions -u $user -p $password -o transactions.json
