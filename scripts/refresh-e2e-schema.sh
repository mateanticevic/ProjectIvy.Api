#!/usr/bin/env bash
set -euo pipefail

: "${E2E_SCHEMA_SOURCE_CONNECTION_STRING:?Set E2E_SCHEMA_SOURCE_CONNECTION_STRING using metadata-read credentials. CONNECTION_STRING_MAIN is never used.}"
task_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$task_root"

dotnet tool restore
dotnet run --project tools/ProjectIvy.EndToEnd.Schema/ProjectIvy.EndToEnd.Schema.csproj -- \
    "$task_root/test/ProjectIvy.Api.EndToEnd.Test/Schema/Main.dacpac"
