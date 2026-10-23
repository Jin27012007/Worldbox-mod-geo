# Reproducible build and package

This project has three distinct validation levels:

1. Source checks validate JSON/XML, compile source against isolated test doubles and test the local data model. They do not make an installable mod.
2. A real build compiles ObservatorioDosReinos.csproj against the target WorldBox 0.51.4 and NeoModLoader assemblies. Only this step produces the mod DLL.
3. Packaging runs only when the compiled DLL exists. It includes the mod's own DLL, manifest, locales and install notes; it excludes all game/loader references.

## Prepare references

Use the ZIP from the WorldBox Android installation that matches the target version. The reference-preparation script supports the paths found in the supplied Android ZIP, including the publicized assembly under files/mods/NML/ and Harmony under files/mods/NML/Assemblies/.

From the repository root:

    python3 scripts/prepare_references.py /path/to/com.mkarpenko.worldbox.zip
    python3 scripts/validate_repository.py

The script copies the required assemblies and Unity modules into references/. It removes stale DLLs first to prevent accidental mixes of game versions. The directory is Git-ignored. Never commit game, Unity, NeoModLoader or Harmony DLLs to this public repository. The script fails if the archive layout lacks required files.

## One-command build

With the .NET 8 SDK installed, run:

    bash scripts/build_local.sh /path/to/com.mkarpenko.worldbox.zip

The script prepares the references, runs static and pipeline tests, compiles/tests against stubs, builds the production project against the private assemblies, packages the mod and prints a SHA-256 hash. It stops at the first failing step. The expected assembly is bin/Release/netstandard2.1/ObservatorioDosReinos.dll, and the ZIP is dist/Observatorio-dos-Reinos-<version>.zip.

For separate steps, these commands are also available:

    dotnet build ObservatorioDosReinos.csproj --configuration Release
    python3 scripts/package_mod.py

The packager refuses a missing or empty DLL, validates the archive and excludes the source, build references and game/loader DLLs.

## Android-only workflow with GitHub Codespaces

The repo contains a .devcontainer configuration with the .NET 8 SDK and Python. To build from the Android browser:

1. Open the repository in GitHub and create a Codespace on the foundation/modding-pipeline branch (or on the branch after this PR is merged).
2. In the Codespace explorer, create a folder named build-input and upload the WorldBox ZIP there. That folder is Git-ignored; do not commit the archive.
3. Open the terminal and run:

       bash scripts/build_local.sh build-input/com.mkarpenko.worldbox.zip

4. Download the generated ZIP from dist/ using the Codespace file explorer. Back up the installed mod before replacing it.

GitHub-hosted Actions do not automatically see uncommitted files in a Codespace. Therefore, the hosted workflow builds a real DLL only if the full private reference set exists on its own runner. Without references it reports SKIP and creates no installable package; partial references or real-build errors fail the job.

## Release gate

A successful real build is not a runtime pass. Test the package in WorldBox Android 0.51.4 with the matching NeoModLoader and verify:
- the intended name/version appears in the mod list;
- logs contain [OR-BASELINE] STAGE=ON_MOD_LOAD and STAGE=UPDATE_OK;
- button hook/window logs appear;
- the button is visible and opens the window;
- opening the native information window does not introduce a regression.

A stub CI pass must never be described as proof of these runtime properties.
