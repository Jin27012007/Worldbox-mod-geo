# Adding features to the Observatory

New features should live in separate source files and remain part of the production project. ObservatorioDosReinos.csproj automatically includes every C# file under src/**, so adding a feature does not require manually editing the compile list.

## Recommended layout

- src/Features/: one class or coordinator per gameplay feature, such as kingdom policies or world events.
- src/Integrations/: narrow adapters to native WorldBox and NeoModLoader APIs.
- src/UI/: windows, widgets and UI integration.
- src/Diagnostics/: probes, logging and version compatibility.
- tests/: stub tests. These do not replace testing inside the actual game.

The current implementation is still concentrated in src/ObservatorioMod.cs. This is known technical debt, not a template for another giant file. Refactor one class at a time, preserve required namespaces/public names and run a real-reference build before changing behavior.

## Contract for each feature

1. Define exactly what it changes. A proposal held in memory must not be described as a change to a real kingdom.
2. Isolate calls into unstable native APIs in a narrow integration layer.
3. Handle null objects and scenes that are not yet initialized.
4. Emit logs for start, errors and results, including the build version and stage.
5. Write logic tests and state what those tests cannot verify.
6. Compile against the actual target assemblies and test on Android before calling the feature complete.

## Workflow

- Add C# files under src/Features, src/Integrations or src/UI.
- Update Main initialization only when necessary and avoid side effects during mod load.
- Add tests under tests/.
- Run python3 scripts/validate_repository.py.
- Run dotnet build tests/CompileStubs.csproj -c Release and execute its smoke test.
- With private references ready, run dotnet build ObservatorioDosReinos.csproj -c Release.
- Package the result and validate it in WorldBox Android. Preserve runtime evidence with the change.

Never include WorldBox, Unity, NeoModLoader or Harmony DLLs in the mod distribution ZIP. They are compile references, not files to copy into the mod folder. Never label a change fixed based only on stubs, compiler success or the general Modded: True indicator.
