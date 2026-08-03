#!/usr/bin/env bash
set -euo pipefail

SOLUTION="Template.Api.slnx"

echo "========================================="
echo "  Running CI"
echo "========================================="

echo
echo "Restore, build and test. It doesn't work separately."
dotnet test "$SOLUTION" --configuration Release --filter "FullyQualifiedName!~FunctionalTests"
echo "Tests passed"

echo
echo "========================================="
echo "  All CI checks passed!"
echo "========================================="