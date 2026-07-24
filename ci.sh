#!/usr/bin/env bash
set -euo pipefail

SOLUTION="Template.slnx"

echo "========================================="
echo "  Running CI"
echo "========================================="

echo
echo "Restore, build and test. It doesn't work separately."
dotnet test "$SOLUTION" --configuration Release
echo "Tests passed"

echo
echo "========================================="
echo "  All CI checks passed!"
echo "========================================="