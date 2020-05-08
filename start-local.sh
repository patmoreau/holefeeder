#!/bin/bash

ACTION="all"
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

if [ $ACTION == "api" ] || [ $ACTION == "all" ]; then
    # build api
    dotnet restore src
    dotnet publish --no-restore --output publish/DrifterApps.Holefeeder.ServicesHosts.BudgetApi src/DrifterApps.Holefeeder.ServicesHosts.BudgetApi/DrifterApps.Holefeeder.ServicesHosts.BudgetApi.csproj

    docker build -t holefeeder-api:latest -f docker/api.dockerfile .
fi
if [ $ACTION == "ui" ] || [ $ACTION == "all" ]; then
    # build ui
    cd src/DrifterApps.Holefeeder.Presentations.UI
    npm install
    npm run-script build -- --outputPath=../../publish/DrifterApps.Holefeeder.Presentations.UI
    cd ../../

    docker build -t holefeeder-web:latest -f docker/web.dockerfile .
fi
if [ $ACTION == "docker" ] || [ $ACTION == "all" ]; then
    docker-compose -f docker/dev/docker-compose.yml up --force-recreate --build -d
    docker image prune -f
fi
