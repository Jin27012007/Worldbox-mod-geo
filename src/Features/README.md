# Feature modules

Place new feature coordinators in this directory as C# files. The production project and the stub compile project both include every src/**/*.cs file automatically.

Before a module is considered complete, it needs a clear statement of what it changes, a logic test, a successful build against the target WorldBox/NML assemblies, and runtime verification inside the Android game. Keep native API access isolated and do not claim game-world changes for values that exist only in memory.
