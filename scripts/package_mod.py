#!/usr/bin/env python3
"""Package only the mod manifest, compiled DLL, locales, and install notes."""
from __future__ import annotations

import hashlib
import json
import shutil
import sys
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DEFAULT_DLL = ROOT / "bin" / "Release" / "netstandard2.1" / "ObservatorioDosReinos.dll"
DIST = ROOT / "dist"
FOLDER_NAME = "Observatorio dos Reinos"


def main() -> int:
    dll = Path(sys.argv[1]).expanduser().resolve() if len(sys.argv) > 1 else DEFAULT_DLL
    if len(sys.argv) > 2:
        print("Usage: python3 scripts/package_mod.py [path/to/ObservatorioDosReinos.dll]", file=sys.stderr)
        return 2
    manifest_path = ROOT / "mod.json"
    if not manifest_path.is_file():
        print("mod.json not found.", file=sys.stderr)
        return 1
    if not dll.is_file() or dll.stat().st_size == 0:
        print(f"Compiled DLL missing or empty: {dll}", file=sys.stderr)
        print("No ZIP was created. Build ObservatorioDosReinos.csproj against the real references first.", file=sys.stderr)
        return 1

    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    version = manifest.get("version")
    if not isinstance(version, str) or not version:
        print("Invalid version in mod.json.", file=sys.stderr)
        return 1

    if DIST.exists():
        shutil.rmtree(DIST)
    stage = DIST / FOLDER_NAME
    stage.mkdir(parents=True)
    shutil.copy2(manifest_path, stage / "mod.json")
    shutil.copy2(dll, stage / dll.name)
    if (ROOT / "icon.png").is_file():
        shutil.copy2(ROOT / "icon.png", stage / "icon.png")
    if (ROOT / "Locales").is_dir():
        shutil.copytree(ROOT / "Locales", stage / "Locales")

    notes = (
        f"Observatório dos Reinos — {version}\n\n"
        "Development package. A real-reference build confirms compilation only; "
        "WorldBox Android 0.51.4 and the matching NeoModLoader runtime still need testing. "
        "Back up the existing mod folder before replacing it.\n\n"
        "Extract this folder as one mod folder. Keep mod.json and ObservatorioDosReinos.dll "
        "at the root of that folder. Do not copy Unity, Assembly-CSharp, NeoModLoader or "
        "0Harmony reference assemblies into the mod directory.\n"
    )
    (stage / "INSTALL.txt").write_text(notes, encoding="utf-8")

    package_path = DIST / f"Observatorio-dos-Reinos-{version}.zip"
    with zipfile.ZipFile(package_path, "w", compression=zipfile.ZIP_DEFLATED, compresslevel=9) as archive:
        for path in sorted(stage.rglob("*")):
            if path.is_file():
                archive.write(path, Path(FOLDER_NAME) / path.relative_to(stage))

    with zipfile.ZipFile(package_path) as archive:
        names = archive.namelist()
        required = {
            f"{FOLDER_NAME}/mod.json",
            f"{FOLDER_NAME}/ObservatorioDosReinos.dll",
            f"{FOLDER_NAME}/INSTALL.txt",
        }
        missing = required.difference(names)
        if missing:
            print("Incomplete package, missing: " + ", ".join(sorted(missing)), file=sys.stderr)
            return 1
        forbidden = [
            name for name in names
            if name.lower().endswith((".pdb", ".cs", ".csproj"))
            or "references/" in name.lower()
            or Path(name).name in {
                "Assembly-CSharp.dll", "Assembly-CSharp-Publicized.dll",
                "NeoModLoader.dll", "0Harmony.dll"
            }
        ]
        if forbidden:
            print("Package contains files that must not ship: " + ", ".join(forbidden), file=sys.stderr)
            return 1

    data = package_path.read_bytes()
    print(f"PASS: package created: {package_path}")
    print(f"Version: {version}; bytes: {len(data)}; SHA256: {hashlib.sha256(data).hexdigest()}")
    print("Contents limited to the mod DLL, manifest, locales, optional icon and install notes.")
    print("A ZIP does not prove runtime loading or UI rendering inside WorldBox.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
