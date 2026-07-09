#!/bin/bash
set -e

# Step 1: Build CI image (contains app + efbundle)
docker build -f Dockerfile.ci -t deployer-ci .

# Step 2: Extract efbundle from the built image for migration
CONTAINER_ID=$(docker create deployer-ci)
docker cp "$CONTAINER_ID:/app/efbundle:./" efbundle
docker rm "$CONTAINER_ID"

# Step 3: Run migrations (requires CONNECTION_STRING env var)
if [ -z "$CONNECTION_STRING" ]; then
    echo "CONNECTION_STRING is not set. Skipping migration."
else
    echo "Running migrations..."
    chmod +x ./efbundle
    ./efbundle --verbose --connection "$CONNECTION_STRING"
    echo "Migrations applied."
fi

# Step 4: Run CI image (builds + tests)
docker run --rm -v /var/run/docker.sock:/var/run/docker.sock deployer-ci
