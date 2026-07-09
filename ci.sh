#!/bin/bash
set -e

echo "========================================="
echo "  Running ci"
echo "========================================="

# Step 1: Restore
echo ""
echo "[1/4] Restoring packages..."
dotnet restore Template.slnx
echo "Restore successful"

# Step 2: Build
echo ""
echo "[2/4] Building..."
dotnet build Template.slnx
echo "Build successful"

# Step 3: Build efbundle
echo ""
echo "[3/4] Building efbundle..."
dotnet tool install --global dotnet-ef 2>/dev/null || true
dotnet ef bundle --project src/Persistence/Persistence.csproj --context AppDbContext --output efbundle --runtime linux-x64 --framework net10.0
echo "efbundle built: efbundle/efbundle"

# Step 4: Tests
echo ""
echo "[4/4] Running tests..."
mkdir -p test-results
dotnet test Template.slnx --no-build --verbosity normal --logger "trx;LogFileName=test-results.trx" --results-directory "test-results"
echo "Tests passed"

echo ""
echo "========================================="
echo "  All ci checks passed!"
echo "========================================="
