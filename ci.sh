#!/usr/bin/env bash
set -euo pipefail

echo "========================================="
echo "  Running CI"
echo "========================================="

echo
echo "Restore, build and test. It doesn't work separately."
dotnet test --solution Template.Api.slnx --configuration Release -- --filter "FullyQualifiedName!~FunctionalTests" --ignore-exit-code 8
echo "Tests passed"

echo
echo "========================================="
echo "  All CI checks passed!"
echo "========================================="