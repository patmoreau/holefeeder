#!/bin/bash

ENV="local.env"
IP="localhost"

while getopts ":e:i:" opt; do
  case $opt in
    (e) ENV=$OPTARG ;;
    (i) IP=$OPTARG ;;
    (?)
      echo "Invalid option: -$OPTARG" >&2
      exit 1
      ;;
    (:)
      echo "Option -$OPTARG requires an argument." >&2
      exit 1
      ;;
  esac
done

#
# This file should be used to prepare and run your WebProxy after set up your .env file
# Source: https://github.com/evertramos/docker-compose-letsencrypt-nginx-proxy-companion
#

# 1. Check if .env file exists
if [ -e $ENV ]; then
    source $ENV
else 
    echo "Please set up your $ENV file before starting your environment."
    exit 1
fi

# 2. Create docker network
docker network create $NETWORK $NETWORK_OPTIONS

# 3. Verify if second network is configured
if [ ! -z ${STAGING_NETWORK+X} ]; then
    docker network create $STAGING_NETWORK $STAGING_NETWORK_OPTIONS
fi
if [ ! -z ${SHARED_NETWORK+X} ]; then
    docker network create $SHARED_NETWORK $SHARED_NETWORK_OPTIONS
fi

# 4. Download the latest version of nginx.tmpl
curl https://raw.githubusercontent.com/jwilder/nginx-proxy/master/nginx.tmpl > nginx.tmpl

# 5. Update local images
docker-compose pull

# 6. Add any special configuration if it's set in .env file

# Check if user set to use Special Conf Files
if [ ! -z ${USE_NGINX_CONF_FILES+X} ] && [ "$USE_NGINX_CONF_FILES" = true ]; then

    # Create the conf folder if it does not exists
    mkdir -p $NGINX_FILES_PATH/conf.d

    # Copy the special configurations to the nginx conf folder
    cp -R ./conf.d/* $NGINX_FILES_PATH/conf.d

    # Check if there was an error and try with sudo
    if [ $? -ne 0 ]; then
        sudo cp -R ./conf.d/* $NGINX_FILES_PATH/conf.d
    fi

    # If there was any errors inform the user
    if [ $? -ne 0 ]; then
        echo
        echo "#######################################################"
        echo
        echo "There was an error trying to copy the nginx conf files."
        echo "The webproxy will still work, your custom configuration"
        echo "will not be loaded."
        echo 
        echo "#######################################################"
    fi
fi 

# 7. Start proxy

# Check if you have multiple network
if [ -z ${STAGING_NETWORK+X} ] && [ -z ${SHARED_NETWORK+X} ]; then
    docker-compose up --force-recreate --build -d
else
    docker-compose -f docker-compose-multiple-networks.yml up --force-recreate --build -d
fi

exit 0