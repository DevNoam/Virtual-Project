#!/bin/bash

# Set the domain
DOMAIN="virtualproject.noamsapir.me"
SSL_FOLDER=$(ls -td /etc/letsencrypt/live/"$DOMAIN"-* | head -n 1)
DEST_FOLDER="/root/VirtualProject"

# Renew the specific subdomain non-interactively
if [ -f "$SSL_FOLDER/privkey.pem" ] && [ -f "$SSL_FOLDER/cert.pem" ] && [ -f "$SSL_FOLDER/chain.pem" ]; then
  echo "Renewal requires updating the .pfx file."
  
  # Create the .pfx file with an empty password, or specify one if needed
  openssl pkcs12 -export -out "$SSL_FOLDER/cert.pfx" -inkey "$SSL_FOLDER/privkey.pem" -in "$SSL_FOLDER/cert.pem" -certfile "$SSL_FOLDER/chain.pem" -passout pass:
  
  # Check if the .pfx file creation was successful
  if [ -f "$SSL_FOLDER/cert.pfx" ]; then
    mv -f "$SSL_FOLDER/cert.pfx" "$DEST_FOLDER"
  else
    echo "Error: Failed to create .pfx file."
    exit 1
  fi

  # Quit all detached screen sessions
  DETACHED_SCREENS=$(screen -ls | grep "Detached" | cut -d. -f1 | awk '{print $1}')
  if [ -n "$DETACHED_SCREENS" ]; then
    echo "$DETACHED_SCREENS" | xargs -I {} screen -X -S {} quit
  else
    echo "No detached screens found."
  fi
else
  echo "Error: SSL files are missing."
  exit 1
fi

exit 0
