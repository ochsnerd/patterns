#!/usr/bin/env bash
set -e
cd "$(dirname "$0")"

# https://github.com/dotnet/sdk/issues/16535
dotnet tool restore -v q > /dev/null
dotnet csharpier check .
dotnet format --verify-no-changes --severity warn
dotnet test
