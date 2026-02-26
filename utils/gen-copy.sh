#!/bin/bash

# Check if a prefix was provided
if [ -z "$1" ]; then
    echo "Usage: ./gen-copy.sh [inputed_prefix]"
    echo "Example: ./gen-copy.sh services/order-service"
    exit 1
fi

PREFIX=$1

# Find all .csproj files in the current directory and subdirectories
# We use a loop to format each path found
find . -name "*.csproj" -not -path "*/obj/*" -not -path "*/bin/*" | while read -r file; do
    # Remove the './' from the start of the find result
    CLEAN_PATH=${file#./}
    
    # Get the directory part of the path to use as the destination
    DIR_PATH=$(dirname "$CLEAN_PATH")
    
    # Print the formatted Docker COPY command
    echo "COPY [\"$CLEAN_PATH\", \"$PREFIX/$DIR_PATH/\"]"
done