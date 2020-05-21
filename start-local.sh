#!/bin/bash

ACTION=""
while getopts ":a:" opt; do
    case $opt in
    a)
        ACTION=$OPTARG
        if [ $ACTION != "api" ] && [ $ACTION != "ui" ] && [ $ACTION != "docker" ] && [ $ACTION != "all" ]; then
            echo "Invalid -$opt [$ACTION] : available actions [api;ui;docker;all]"
        fi
        ;;
    \?)
        echo "Invalid option: -$OPTARG" >&2
        exit 1
        ;;
    :)
        echo "Option -$OPTARG requires an argument." >&2
        exit 1
        ;;
    esac
done

VERSION=`date +%y.%m.00-alpha`
echo building $VERSION
echo $VERSION > docker/VERSION

if [ $ACTION == "api" ] || [ $ACTION == "all" ]; then
    # build api
    dotnet restore src
    dotnet publish /property:Version=$VERSION --no-restore --output publish/DrifterApps.Holefeeder.ServicesHosts.BudgetApi src/DrifterApps.Holefeeder.ServicesHosts.BudgetApi/DrifterApps.Holefeeder.ServicesHosts.BudgetApi.csproj

    docker build -t holefeeder-api:$VERSION -f docker/api.dockerfile .
fi
if [ $ACTION == "ui" ] || [ $ACTION == "all" ]; then
    # build ui
    cd src/DrifterApps.Holefeeder.Presentations.UI
    npm install
    npm run-script build -- --outputPath=../../publish/DrifterApps.Holefeeder.Presentations.UI
    cd ../../

    docker build -t holefeeder-web:$VERSION -f docker/web.dockerfile .
fi
if [ $ACTION == "docker" ] || [ $ACTION == "all" ]; then
    docker-compose --env-file=docker/dev/.env -f docker/dev/docker-compose.yml up --force-recreate --build -d
    docker image prune -f
fi
