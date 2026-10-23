# Reproducible build and package

This project has three distinct validation levels:

1. Source checks validate JSON/XML, compile source against isolated test doubles and test the local data model. They do not make an installable mod.
2. A real build compiles ObservatorioDosReinos.csproj against the target WorldBox 0.51.4 and NeoModLoader assemblies. Only this step produces the mod DLL.
3. Packaging runs only when the compiled DLL exists. It includes the mod's own DLL, manifest, locales and install notes; it excludes all game/loader references.

## Prepare local references

Use the ZIP from the WorldBox Android installation that matches the target version. From the repository root run:

    python3 scripts/prepare_references.py /path/to/com.mkarpenko.worldbox.zip
    python3 scripts/validate_repository.py

The script copies only the required assemblies into references/. That directory is Git-ignored. Never commit the game, Unity, NeoModLoader or Harmony DLLs to this public repository. If the archive layout or version differs, the script fails instead of pretending the references are correct.

## Build and package

With the .NET 8 SDK installed:

    dotnet build ObservatorioDosReinos.csproj --configuration Release
    python3 scripts/package_mod.py

The expected assembly is bin/Release/netstandard2.1/ObservatorioDosReinos.dll. The package is dist/Observatorio-dos-Reinos-<version>.zip. The packager refuses to create a ZIP if the compiled DLL is missing or empty, verifies the archive and excludes reference binaries and C# source.

## Android-only workflow with GitHub Codespaces

The repository can be edited and built from the Android browser using Codespaces:

1. Open the repository in GitHub and create a Codespace on the development branch.
2. Upload the WorldBox ZIP into the Codespace using its file explorer, not into a tracked repository folder.
3. Run the reference preparation, build and package commands above.
4. Download the ZIP from the Codespace file explorer. Back up the installed mod before replacing it.

GitHub-hosted Actions do not automatically see uncommitted files in a Codespace. Thus the hosted workflow runs a real build only when a full set of private references is present on its own runner. With no references it reports SKIP and creates no mod package. With a partial reference set or a failed real build, the job fails.

## Release gate

A successful real build is not a runtime pass. Test the package in WorldBox Android 0.51.4 with the matching NeoModLoader and verify:
- the intended name/version appears in the mod list;
- logs contain [OR-BASELINE] STAGE=ON_MOD_LOAD and STAGE=UPDATE_OK;
- button hook/window logs appear;
- the button is visible and opens the window;
- opening the native information window does not introduce a regression.

A stub CI pass must never be described as proof of those runtime properties.
