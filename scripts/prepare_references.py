#!/usr/bin/env python3
"""Extract local-only build references from a user-supplied WorldBox Android ZIP."""
from __future__ import annotations

import hashlib
import sys
import zipfile
from pathlib import Path, PurePosixPath

ROOT = Path(__file__).resolve().parents[1]
DEST = ROOT / "references"
GAME_PREFIX = "com.mkarpenko.worldbox/files/mono/"
LOADER_PREFIX = "com.mkarpenko.worldbox/files/mods/"
NML_PREFIX = "com.mkarpenko.worldbox/files/mods/NML/Assemblies/"

REQUIRED = {
    "NeoModLoader.dll": (LOADER_PREFIX + "NeoModLoader.dll",),
    # The uploaded Android package stores this DLL one directory above 0Harmony.
    # Accept the alternate nested location used by some repackaged distributions.
    "Assembly-CSharp-Publicized.dll": (
        LOADER_PREFIX + "NML/Assembly-CSharp-Publicized.dll",
        NML_PREFIX + "Assembly-CSharp-Publicized.dll",
    ),
    "0Harmony.dll": (NML_PREFIX + "0Harmony.dll",),
    "UnityEngine.dll": (GAME_PREFIX + "UnityEngine.dll",),
    "UnityEngine.CoreModule.dll": (GAME_PREFIX + "UnityEngine.CoreModule.dll",),
    "UnityEngine.UI.dll": (GAME_PREFIX + "UnityEngine.UI.dll",),
    "UnityEngine.TextRenderingModule.dll": (GAME_PREFIX + "UnityEngine.TextRenderingModule.dll",),
    "UnityEngine.TextCoreTextEngineModule.dll": (GAME_PREFIX + "UnityEngine.TextCoreTextEngineModule.dll",),
    "UnityEngine.TextCoreFontEngineModule.dll": (GAME_PREFIX + "UnityEngine.TextCoreFontEngineModule.dll",),
    "UnityEngine.UIModule.dll": (GAME_PREFIX + "UnityEngine.UIModule.dll",),
}


def main() -> int:
    if len(sys.argv) != 2:
        print("Usage: python3 scripts/prepare_references.py /path/to/com.mkarpenko.worldbox.zip", file=sys.stderr)
        return 2
    archive_path = Path(sys.argv[1]).expanduser().resolve()
    if not archive_path.is_file():
        print(f"Archive not found: {archive_path}", file=sys.stderr)
        return 2
    try:
        with zipfile.ZipFile(archive_path) as archive:
            names = set(archive.namelist())
            missing = [name for name, candidates in REQUIRED.items()
                       if not any(source in names for source in candidates)]
            if missing:
                print("Target archive is missing required WorldBox/NML assemblies:", file=sys.stderr)
                for name in missing:
                    print(f"  - {name}", file=sys.stderr)
                print("No references were extracted. Check the package/version and its internal paths.", file=sys.stderr)
                return 1

            DEST.mkdir(parents=True, exist_ok=True)
            copied = {}
            for target, candidates in REQUIRED.items():
                source = next(candidate for candidate in candidates if candidate in names)
                data = archive.read(source)
                if not data:
                    raise ValueError(f"Empty assembly in archive: {source}")
                (DEST / target).write_bytes(data)
                copied[target] = (source, data)

            extras = sorted(
                name for name in names
                if name.startswith(GAME_PREFIX)
                and PurePosixPath(name).name.startswith("UnityEngine")
                and name.endswith(".dll")
            )
            for source in extras:
                target_name = PurePosixPath(source).name
                if target_name in copied:
                    continue
                data = archive.read(source)
                if data:
                    (DEST / target_name).write_bytes(data)
                    copied[target_name] = (source, data)
    except (OSError, zipfile.BadZipFile, KeyError, ValueError) as exc:
        print(f"Reference extraction failed: {type(exc).__name__}: {exc}", file=sys.stderr)
        return 1

    print(f"Private/local references prepared in {DEST}")
    print("These files are Git-ignored and must never be committed to the public repository.")
    for name, (source, data) in sorted(copied.items()):
        digest = hashlib.sha256(data).hexdigest()
        print(f"{name}\t{len(data)} bytes\tSHA256={digest}\tsource={source}")
    print("\nNext: dotnet build ObservatorioDosReinos.csproj --configuration Release")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
