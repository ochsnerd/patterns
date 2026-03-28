#!/usr/bin/env bash

set -e
dotnet new console -n "$1"
dotnet new gitignore -o "$1"
dotnet sln add "$1"
dotnet add Tests reference "$1"
