#!/bin/bash

# Automatically get the host machine's IP address
HOST_IP=$(ip route show | grep default | awk '{print $3}')

BASE_DOMAIN="localtest.me"
SUBDOMAINS=("whoami")

for SUBDOMAIN in "${SUBDOMAINS[@]}"; do
  FULL_DOMAIN="$SUBDOMAIN.$BASE_DOMAIN"

  # Check if the entry already exists in the hosts file
  if ! grep -q "$HOST_IP $FULL_DOMAIN" /etc/hosts; then
    # Add the entry to the hosts file
    echo "$HOST_IP $FULL_DOMAIN" | tee -a /etc/hosts > /dev/null
    echo "Hosts file updated for $FULL_DOMAIN"
  else
    echo "Hosts file entry for $FULL_DOMAIN already exists."
  fi
done
