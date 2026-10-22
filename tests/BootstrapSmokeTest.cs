using System;
using System.Reflection;
using NeoModLoader.api;
using UnityEngine;

internal static class BootstrapSmokeTest
{
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("Test failed: " + message);
    }

    private static void InvokePrivate(Main instance, string methodName)
    {
        MethodInfo method = typeof(Main).GetMethod(methodName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        Require(method != null, "Unity lifecycle method exists: " + methodName);
        method.Invoke(instance, null);
    }

    public static int Main()
    {
        GameObject root = new GameObject("SelectedPowerTabTopButtons");
        Canvas canvas = root.AddComponent<Canvas>() as Canvas;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 7;
        canvas.overrideSorting = true;
        SelectedPowerTabTopButtons panel = root.AddComponent<SelectedPowerTabTopButtons>() as SelectedPowerTabTopButtons;
        panel.AttachForTest(root);

        Main mod = new Main();
        mod.InvokeOnModLoadForTest();
        InvokePrivate(mod, "OnEnable");
        InvokePrivate(mod, "Start");
        InvokePrivate(mod, "Update");

        string logs = string.Join("\n", BasicMod<Main>.CapturedLogs.ToArray());
        Require(logs.Contains("BUILD=0.1.0-proof; STAGE=ON_MOD_LOAD"), "OnModLoad marker");
        Require(logs.Contains("DECLARATION name=Observatorio dos Reinos — Runtime Proof; uid=com.jin27012007.observatoriodosreinos; folder=/Mods/ObservatorioRuntimeProof"), "actual loaded mod declaration and folder marker");
        Require(logs.Contains("STAGE=ON_ENABLE"), "OnEnable marker");
        Require(logs.Contains("STAGE=START_OK"), "Start marker");
        Require(logs.Contains("STAGE=UPDATE_OK"), "Update marker");
        Require(logs.Contains("TYPE_PROBE name=SelectedPowerTabTopButtons; found=true"), "target type discovery");
        Require(logs.Contains("TYPE_FIELD type=SelectedPowerTabTopButtons; field=_button_possession"), "possession field discovery");
        Require(logs.Contains("UI_COMPONENT type=SelectedPowerTabTopButtons"), "scene component inspection");
        Require(logs.Contains("canvas=name=SelectedPowerTabTopButtons;renderMode=ScreenSpaceOverlay;sortingOrder=7;overrideSorting=True"), "Canvas render mode and sorting order inspection");
        Require(logs.Contains("UI_BUTTON name=Button_Possession"), "child button hierarchy inspection");
        Require(logs.Contains("mvid="), "assembly MVID logging");

        Console.WriteLine("PASS: runtime-probe logic and lifecycle markers exercised against test doubles.");
        Console.WriteLine("LIMIT: this does not prove Android/Unity runtime compatibility or UI rendering.");
        return 0;
    }
}
