#!/usr/bin/env python3
"""Make CI skip absent private references clearly, but reject partial sets."""
from __future__ import annotations

import os
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
REFS = ROOT / "references"
REQUIRED = (
    "NeoModLoader.dll",
    "Assembly-CSharp-Publicized.dll",
    "0Harmony.dll",
    "UnityEngine.dll",
    "UnityEngine.CoreModule.dll",
    "UnityEngine.UI.dll",
    "UnityEngine.TextRenderingModule.dll",
    "UnityEngine.TextCoreTextEngineModule.dll",
    "UnityEngine.TextCoreFontEngineModule.dll",
    "UnityEngine.UIModule.dll",
)


def main() -> int:
    all_dlls = list(REFS.glob("*.dll")) if REFS.exists() else []
    output = os.environ.get("GITHUB_OUTPUT")
    if not all_dlls:
        print("SKIP: no private reference DLLs supplied; no real mod DLL/package will be produced.")
        if output:
            with open(output, "a", encoding="utf-8") as stream:
                stream.write("available=false\n")
        return 0

    missing = [name for name in REQUIRED if not (REFS / name).is_file()]
    if missing:
        print("ERROR: partial reference set; refusing to report a successful real build.", file=sys.stderr)
        for name in missing:
            print(f"  missing references/{name}", file=sys.stderr)
        return 1

    if output:
        with open(output, "a", encoding="utf-8") as stream:
            stream.write("available=true\n")
    print("PASS: all required WorldBox/NML reference assemblies are present.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
