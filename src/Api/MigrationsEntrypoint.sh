#!/bin/sh
set -e

echo "Running database migrations..."
./efbundle --connection "$CONNECTION_STRING" --verbose
echo "Migrations complete."

echo "Starting application..."
exec dotnet Api.dll