#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

if [[ "$#" -ne 1 ]]; then
  echo "Usage: bash scripts/build_local.sh build-input/com.mkarpenko.worldbox.zip" >&2
  exit 2
fi

ARCHIVE="$1"
echo "== 1/6: Prepare local references from the target game ZIP =="
python3 scripts/prepare_references.py "$ARCHIVE"

echo
echo "== 2/6: Validate manifest, project and locales =="
python3 scripts/validate_repository.py

echo
echo "== 3/6: Test reference and package helpers =="
python3 -m unittest discover -s tests -p 'test_*.py' -v

echo
echo "== 4/6: Compile and test source against isolated stubs =="
dotnet build tests/CompileStubs.csproj --configuration Release --verbosity minimal
dotnet run --project tests/CompileStubs.csproj --configuration Release --no-build

echo
echo "== 5/6: Compile the production mod against the real local assemblies =="
dotnet build ObservatorioDosReinos.csproj --configuration Release --verbosity minimal

echo
echo "== 6/6: Package the compiled mod =="
python3 scripts/package_mod.py
sha256sum dist/Observatorio-dos-Reinos-*.zip

echo
echo "Build and package pipeline completed. This still does not prove game runtime behavior."
