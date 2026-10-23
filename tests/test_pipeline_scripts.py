from __future__ import annotations

import contextlib
import importlib.util
import io
import json
import os
import sys
import tempfile
import unittest
import zipfile
from pathlib import Path
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[1]


def load_script(name: str, relative: str):
    path = ROOT / relative
    spec = importlib.util.spec_from_file_location(name, path)
    module = importlib.util.module_from_spec(spec)
    assert spec is not None and spec.loader is not None
    spec.loader.exec_module(module)
    return module


class PipelineScriptTests(unittest.TestCase):
    def test_reference_preparation_copies_required_and_extra_unity_dlls(self):
        script = load_script("prepare_references_tested", "scripts/prepare_references.py")
        self.assertIn(
            "com.mkarpenko.worldbox/files/mods/NML/Assembly-CSharp-Publicized.dll",
            script.REQUIRED["Assembly-CSharp-Publicized.dll"],
        )
        self.assertEqual(
            script.REQUIRED["0Harmony.dll"],
            ("com.mkarpenko.worldbox/files/mods/NML/Assemblies/0Harmony.dll",),
        )
        with tempfile.TemporaryDirectory() as temporary:
            temp = Path(temporary)
            archive_path = temp / "target.zip"
            destination = temp / "references"
            destination.mkdir()
            (destination / "UnityEngine.StaleModule.dll").write_bytes(b"stale version")
            with zipfile.ZipFile(archive_path, "w") as archive:
                for target_name, candidates in script.REQUIRED.items():
                    source_name = candidates[0]
                    archive.writestr(source_name, ("fake-assembly:" + target_name).encode("utf-8"))
                archive.writestr(
                    script.GAME_PREFIX + "UnityEngine.ExtraModule.dll",
                    b"fake-extra-module",
                )
            with patch.object(script, "DEST", destination), patch.object(sys, "argv", ["prepare_references.py", str(archive_path)]):
                output = io.StringIO()
                with contextlib.redirect_stdout(output):
                    result = script.main()
            self.assertEqual(result, 0)
            for target_name in script.REQUIRED:
                self.assertTrue((destination / target_name).is_file(), target_name)
            self.assertTrue((destination / "UnityEngine.ExtraModule.dll").is_file())
            self.assertFalse((destination / "UnityEngine.StaleModule.dll").exists())
            self.assertIn("never be committed", output.getvalue())

    def test_reference_checker_distinguishes_missing_complete_and_partial_sets(self):
        script = load_script("check_references_tested", "scripts/check_references.py")
        with tempfile.TemporaryDirectory() as temporary:
            temp = Path(temporary)
            refs = temp / "references"
            refs.mkdir()
            output_path = temp / "github-output.txt"
            with patch.object(script, "REFS", refs), patch.dict(os.environ, {"GITHUB_OUTPUT": str(output_path)}):
                output = io.StringIO()
                with contextlib.redirect_stdout(output):
                    self.assertEqual(script.main(), 0)
                self.assertIn("available=false", output_path.read_text(encoding="utf-8"))

                for name in script.REQUIRED:
                    (refs / name).write_bytes(b"test-only-placeholder")
                output_path.write_text("", encoding="utf-8")
                self.assertEqual(script.main(), 0)
                self.assertIn("available=true", output_path.read_text(encoding="utf-8"))

                for name in script.REQUIRED[1:]:
                    (refs / name).unlink()
                errors = io.StringIO()
                with contextlib.redirect_stderr(errors):
                    self.assertEqual(script.main(), 1)
                self.assertIn("partial reference set", errors.getvalue())

    def test_packager_refuses_missing_dll(self):
        script = load_script("package_mod_missing_dll_tested", "scripts/package_mod.py")
        with tempfile.TemporaryDirectory() as temporary:
            temp = Path(temporary)
            missing_dll = temp / "missing.dll"
            with patch.object(script, "DIST", temp / "dist"), patch.object(sys, "argv", ["package_mod.py", str(missing_dll)]):
                errors = io.StringIO()
                with contextlib.redirect_stderr(errors):
                    self.assertEqual(script.main(), 1)
                self.assertIn("No ZIP was created", errors.getvalue())
                self.assertFalse((temp / "dist").exists())

    def test_packager_outputs_only_mod_files_and_manifest(self):
        script = load_script("package_mod_tested", "scripts/package_mod.py")
        with tempfile.TemporaryDirectory() as temporary:
            temp = Path(temporary)
            (temp / "mod.json").write_text(json.dumps({
                "name": "Test mod",
                "author": "test",
                "version": "0.0.1-test",
                "description": "test fixture",
                "GUID": "test.mod.fixture",
            }), encoding="utf-8")
            locales = temp / "Locales"
            locales.mkdir()
            (locales / "en.json").write_text("{}", encoding="utf-8")
            dll = temp / "ObservatorioDosReinos.dll"
            dll.write_bytes(b"not-a-real-binary; package fixture only")
            dist = temp / "dist"
            with patch.object(script, "ROOT", temp), patch.object(script, "DIST", dist), patch.object(
                sys, "argv", ["package_mod.py", str(dll)]
            ):
                output = io.StringIO()
                with contextlib.redirect_stdout(output):
                    self.assertEqual(script.main(), 0)
            package = dist / "Observatorio-dos-Reinos-0.0.1-test.zip"
            self.assertTrue(package.is_file())
            with zipfile.ZipFile(package) as archive:
                names = set(archive.namelist())
            self.assertIn("Observatorio dos Reinos/mod.json", names)
            self.assertIn("Observatorio dos Reinos/ObservatorioDosReinos.dll", names)
            self.assertIn("Observatorio dos Reinos/Locales/en.json", names)
            self.assertIn("Observatorio dos Reinos/INSTALL.txt", names)
            self.assertFalse(any("Assembly-CSharp" in name for name in names))
            self.assertFalse(any("NeoModLoader.dll" in name for name in names))
            self.assertFalse(any(name.endswith((".cs", ".csproj", ".pdb")) for name in names))


if __name__ == "__main__":
    unittest.main(verbosity=2)
