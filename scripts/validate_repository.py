#!/usr/bin/env python3
"""Fast repository checks independent of proprietary game DLLs."""
from __future__ import annotations

import json
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]


def main() -> int:
    try:
        manifest = json.loads((ROOT / "mod.json").read_text(encoding="utf-8"))
        required = ("name", "author", "version", "description", "GUID")
        for key in required:
            if not isinstance(manifest.get(key), str) or not manifest[key].strip():
                raise ValueError(f"mod.json has missing/empty string field: {key}")
        if not re.fullmatch(r"[A-Za-z0-9._-]+", manifest["version"]):
            raise ValueError("mod.json version contains unsupported characters")
        if not re.fullmatch(r"[A-Za-z0-9._-]+", manifest["GUID"]):
            raise ValueError("mod.json GUID contains unsupported characters")

        project = ET.parse(ROOT / "ObservatorioDosReinos.csproj").getroot()
        tfm = project.findtext("./PropertyGroup/TargetFramework")
        if tfm != "netstandard2.1":
            raise ValueError(f"Production project must target netstandard2.1, found {tfm!r}")
        includes = [node.get("Include", "") for node in project.findall(".//Compile")]
        if not any("src/**/*.cs" in value for value in includes):
            raise ValueError("Production project must automatically include src/**/*.cs")

        sources = sorted((ROOT / "src").rglob("*.cs"))
        if not sources:
            raise ValueError("No C# source files found in src/")
        source_text = "\n".join(path.read_text(encoding="utf-8") for path in sources)
        versions = re.findall(r'BuildVersion\s*=\s*"([^"]+)"', source_text)
        if not versions:
            raise ValueError("Could not find runtime BuildVersion constant in src/")
        if any(version != manifest["version"] for version in versions):
            raise ValueError(f"Version mismatch: mod.json={manifest['version']!r}; source={versions!r}")

        locales = sorted((ROOT / "Locales").glob("*.json"))
        for locale in locales:
            json.loads(locale.read_text(encoding="utf-8"))

        print("PASS: mod.json has valid JSON and the required fields.")
        print(f"PASS: production target is {tfm}; it includes future src/**/*.cs files.")
        print(f"PASS: {len(sources)} C# source file(s); runtime and manifest version = {manifest['version']}.")
        print(f"PASS: {len(locales)} locale JSON file(s) parse.")
        print("LIMIT: this is static validation, not a real WorldBox/NML build or runtime test.")
        return 0
    except (OSError, json.JSONDecodeError, ET.ParseError, ValueError) as exc:
        print(f"FAIL: {type(exc).__name__}: {exc}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
