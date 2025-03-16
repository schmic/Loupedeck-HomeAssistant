#!/bin/bash

# Create config directory
mkdir -p ~/.loupedeck/homeassistant

# Check if token is provided as argument
if [ -z "$1" ]; then
    echo "Please provide your Home Assistant API token as an argument"
    echo "Usage: ./setup_config.sh <token> [url]"
    exit 1
fi

# Set default URL if not provided
URL=${2:-"http://homeassistant.local:8123"}

# Create config file
cat > ~/.loupedeck/homeassistant/homeassistant.json << EOF
{
    "token": "$1",
    "url": "$URL"
}
EOF

echo "Configuration file created at: ~/.loupedeck/homeassistant/homeassistant.json"
echo "Token: $1"
echo "URL: $URL"
