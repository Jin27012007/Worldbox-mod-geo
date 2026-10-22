using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NeoModLoader.api;
using UnityEngine;

/// <summary>
/// Minimal runtime proof for WorldBox Android + NeoModLoader.
/// This build intentionally does not create UI or change gameplay.
/// It records whether the source mod loads, enters Unity's lifecycle,
/// and what the running game actually exposes for the NPC-control panel.
/// </summary>
public sealed class Main : BasicMod<Main>
{
    private const string Build = "0.1.0-proof";
    private static readonly string[] ControlTypeNames =
    {
        "SelectedPowerTabButtons",
        "SelectedPowerTabTopButtons"
    };
    private static readonly string[] AssemblyNames =
    {
        "Assembly-CSharp",
        "NeoModLoader",
        "UnityEngine.CoreModule",
        "UnityEngine.UI"
    };
    private static readonly Dictionary<string, string> LastUiSignatures =
        new Dictionary<string, string>();

    private int frameCount;
    private bool enableReported;
    private bool startReported;
    private bool updateReported;
    private bool firstUiProbeDone;

    protected override void OnModLoad()
    {
        LogInfo("[OR-BASELINE] BUILD=" + Build + "; STAGE=ON_MOD_LOAD");
        LogInfo("[OR-BASELINE] ENV gameVersion=" + Safe(Application.version)
            + "; unityVersion=" + Safe(Application.unityVersion)
            + "; platform=" + Application.platform
            + "; screen=" + Screen.width + "x" + Screen.height);

        LogOwnAssembly();
        for (int i = 0; i < AssemblyNames.Length; i++)
            LogAssembly(AssemblyNames[i]);
        LogAssemblyByNameFragment("Harmony");

        LogTypeProbe("SelectedPowerTabButtons");
        LogTypeProbe("SelectedPowerTabTopButtons");
        LogTypeProbe("PowerTabController");
        LogTypeProbe("SelectedUnit");
        LogTypeProbe("SelectedObjects");

        LogInfo("[OR-BASELINE] NEXT=waiting for Unity lifecycle and live UI scan; no button/window is created in this proof build.");
    }

    private void OnEnable()
    {
        if (enableReported) return;
        enableReported = true;
        LogInfo("[OR-BASELINE] STAGE=ON_ENABLE");
    }

    private void Start()
    {
        if (startReported) return;
        startReported = true;
        LogInfo("[OR-BASELINE] STAGE=START_OK");
    }

    private void Update()
    {
        frameCount++;

        if (!updateReported)
        {
            updateReported = true;
            LogInfo("[OR-BASELINE] STAGE=UPDATE_OK; frame=" + frameCount);
        }

        if (!firstUiProbeDone || frameCount % 90 == 0)
        {
            firstUiProbeDone = true;
            ProbeControlPanels();
        }
    }

    private static string Safe(string value)
    {
        return string.IsNullOrEmpty(value) ? "(empty)" : value;
    }

    private void LogOwnAssembly()
    {
        try
        {
            Assembly assembly = typeof(Main).Assembly;
            LogInfo("[OR-BASELINE] OWN_ASSEMBLY name=" + assembly.GetName().Name
                + "; fullName=" + assembly.FullName
                + "; mvid=" + assembly.ManifestModule.ModuleVersionId);
        }
        catch (Exception ex)
        {
            LogInfo("[OR-BASELINE] OWN_ASSEMBLY_ERROR=" + ex.GetType().Name + ": " + ex.Message);
        }
    }

    private void LogAssembly(string simpleName)
    {
        bool found = false;
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            try
            {
                Assembly assembly = assemblies[i];
                AssemblyName identity = assembly.GetName();
                if (!string.Equals(identity.Name, simpleName, StringComparison.OrdinalIgnoreCase))
                    continue;

                found = true;
                LogInfo("[OR-BASELINE] ASSEMBLY name=" + identity.Name
                    + "; version=" + (identity.Version == null ? "(null)" : identity.Version.ToString())
                    + "; mvid=" + assembly.ManifestModule.ModuleVersionId);
            }
            catch (Exception ex)
            {
                LogInfo("[OR-BASELINE] ASSEMBLY_READ_ERROR target=" + simpleName
                    + "; error=" + ex.GetType().Name);
            }
        }

        if (!found)
            LogInfo("[OR-BASELINE] ASSEMBLY_NOT_FOUND target=" + simpleName);
    }

    private void LogAssemblyByNameFragment(string fragment)
    {
        int found = 0;
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            try
            {
                Assembly assembly = assemblies[i];
                AssemblyName identity = assembly.GetName();
                if (identity.Name == null || identity.Name.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                found++;
                LogInfo("[OR-BASELINE] ASSEMBLY_MATCH fragment=" + fragment
                    + "; name=" + identity.Name
                    + "; version=" + (identity.Version == null ? "(null)" : identity.Version.ToString())
                    + "; mvid=" + assembly.ManifestModule.ModuleVersionId);
            }
            catch { }
        }

        if (found == 0)
            LogInfo("[OR-BASELINE] ASSEMBLY_MATCH_NONE fragment=" + fragment);
    }

    private void LogTypeProbe(string simpleName)
    {
        Type type = FindLoadedType(simpleName);
        if (type == null)
        {
            LogInfo("[OR-BASELINE] TYPE_PROBE name=" + simpleName + "; found=false");
            return;
        }

        string assemblyName = "(unknown)";
        string mvid = "(unknown)";
        try
        {
            assemblyName = type.Assembly.GetName().Name;
            mvid = type.Assembly.ManifestModule.ModuleVersionId.ToString();
        }
        catch { }

        LogInfo("[OR-BASELINE] TYPE_PROBE name=" + simpleName
            + "; found=true; fullName=" + type.FullName
            + "; assembly=" + assemblyName + "; mvid=" + mvid);

        MethodInfo update = null;
        try
        {
            update = type.GetMethod(
                "Update",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null);
        }
        catch (Exception ex)
        {
            LogInfo("[OR-BASELINE] TYPE_METHOD_ERROR type=" + simpleName + "; error=" + ex.GetType().Name);
        }

        LogInfo("[OR-BASELINE] TYPE_METHOD type=" + simpleName
            + "; method=Update(); found=" + (update != null)
            + (update == null ? "" : "; private=" + update.IsPrivate
                + "; public=" + update.IsPublic
                + "; assembly=" + update.IsAssembly));

        FieldInfo[] fields;
        try
        {
            fields = type.GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        catch (Exception ex)
        {
            LogInfo("[OR-BASELINE] TYPE_FIELDS_ERROR type=" + simpleName + "; error=" + ex.GetType().Name);
            return;
        }

        int reported = 0;
        for (int i = 0; i < fields.Length && reported < 30; i++)
        {
            string name = fields[i].Name;
            if (name.IndexOf("button", StringComparison.OrdinalIgnoreCase) < 0
                && name.IndexOf("possess", StringComparison.OrdinalIgnoreCase) < 0
                && name.IndexOf("spectate", StringComparison.OrdinalIgnoreCase) < 0
                && name.IndexOf("select", StringComparison.OrdinalIgnoreCase) < 0
                && name.IndexOf("window", StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            LogInfo("[OR-BASELINE] TYPE_FIELD type=" + simpleName
                + "; field=" + name + "; fieldType=" + fields[i].FieldType.FullName);
            reported++;
        }

        if (reported == 0)
            LogInfo("[OR-BASELINE] TYPE_FIELDS_NONE type=" + simpleName);
    }

    private static readonly Dictionary<string, Type> LoadedTypeIndex =
        new Dictionary<string, Type>(StringComparer.Ordinal);
    private static bool loadedTypeIndexBuilt;

    private static Type FindLoadedType(string requestedName)
    {
        if (string.IsNullOrEmpty(requestedName)) return null;
        BuildLoadedTypeIndex();

        Type result;
        return LoadedTypeIndex.TryGetValue(requestedName, out result) ? result : null;
    }

    private static void BuildLoadedTypeIndex()
    {
        if (loadedTypeIndexBuilt) return;
        loadedTypeIndexBuilt = true;

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            Type[] types;
            try
            {
                types = assemblies[i].GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }
            catch
            {
                continue;
            }

            if (types == null) continue;
            for (int t = 0; t < types.Length; t++)
            {
                Type candidate = types[t];
                if (candidate == null) continue;

                if (!string.IsNullOrEmpty(candidate.FullName)
                    && !LoadedTypeIndex.ContainsKey(candidate.FullName))
                    LoadedTypeIndex.Add(candidate.FullName, candidate);

                // Full names are stored first, so a namespaced type can always be requested
                // unambiguously. The simple name is only a convenience for game types whose
                // full namespace differs between WorldBox builds.
                if (!string.IsNullOrEmpty(candidate.Name)
                    && !LoadedTypeIndex.ContainsKey(candidate.Name))
                    LoadedTypeIndex.Add(candidate.Name, candidate);
            }
        }
    }

    private void ProbeControlPanels()
    {
        string selectedUnit = ReadStaticBoolean("SelectedUnit", "isSet");
        string selectedObjects = ReadStaticBoolean("SelectedObjects", "isNanoObjectSet");
        LogInfo("[OR-BASELINE] SELECTION_PROBE frame=" + frameCount
            + "; SelectedUnit.isSet=" + selectedUnit
            + "; SelectedObjects.isNanoObjectSet=" + selectedObjects);

        for (int n = 0; n < ControlTypeNames.Length; n++)
        {
            string typeName = ControlTypeNames[n];
            Type type = FindLoadedType(typeName);
            if (type == null)
            {
                LogSignatureIfChanged(typeName, "type-not-loaded", "[OR-BASELINE] UI_SCAN type=" + typeName
                    + "; result=TYPE_NOT_LOADED");
                continue;
            }

            UnityEngine.Object[] objects;
            try
            {
                objects = Resources.FindObjectsOfTypeAll(type);
            }
            catch (Exception ex)
            {
                LogSignatureIfChanged(typeName, "scan-error:" + ex.GetType().Name,
                    "[OR-BASELINE] UI_SCAN type=" + typeName
                    + "; result=SCAN_ERROR; error=" + ex.GetType().Name + ": " + ex.Message);
                continue;
            }

            List<Component> sceneComponents = new List<Component>();
            if (objects != null)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    Component component = objects[i] as Component;
                    if (component == null || component.gameObject == null) continue;
                    try
                    {
                        if (!component.gameObject.scene.IsValid()) continue;
                    }
                    catch { continue; }

                    sceneComponents.Add(component);
                }
            }

            StringBuilder signature = new StringBuilder();
            signature.Append("count=").Append(sceneComponents.Count);
            signature.Append(";unit=").Append(selectedUnit);
            signature.Append(";objects=").Append(selectedObjects);

            for (int i = 0; i < sceneComponents.Count; i++)
            {
                Component component = sceneComponents[i];
                signature.Append("|").Append(component.gameObject.name)
                    .Append(":active=").Append(component.gameObject.activeInHierarchy)
                    .Append(":parent=").Append(component.transform.parent == null
                        ? "(none)"
                        : component.transform.parent.gameObject.name)
                    .Append(":buttons=").Append(CountChildButtons(component));
            }

            string signatureText = signature.ToString();
            if (!HasSignatureChanged(typeName, signatureText)) continue;

            LogInfo("[OR-BASELINE] UI_SCAN type=" + typeName
                + "; totalObjects=" + (objects == null ? 0 : objects.Length)
                + "; sceneComponents=" + sceneComponents.Count
                + "; selectedUnit=" + selectedUnit
                + "; selectedObjects=" + selectedObjects);

            if (sceneComponents.Count == 0)
            {
                LogInfo("[OR-BASELINE] UI_SCAN_DETAIL type=" + typeName
                    + "; result=NO_LIVE_SCENE_COMPONENT");
                continue;
            }

            int limit = Math.Min(sceneComponents.Count, 10);
            for (int i = 0; i < limit; i++)
            {
                LogComponentHierarchy(sceneComponents[i]);
            }

            if (sceneComponents.Count > limit)
                LogInfo("[OR-BASELINE] UI_SCAN_TRUNCATED type=" + typeName
                    + "; omitted=" + (sceneComponents.Count - limit));
        }
    }

    private static bool HasSignatureChanged(string key, string signature)
    {
        string previous;
        if (LastUiSignatures.TryGetValue(key, out previous)
            && string.Equals(previous, signature, StringComparison.Ordinal))
            return false;

        LastUiSignatures[key] = signature;
        return true;
    }

    private void LogSignatureIfChanged(string key, string signature, string message)
    {
        if (HasSignatureChanged(key, signature)) LogInfo(message);
    }

    private static string ReadStaticBoolean(string typeName, string methodName)
    {
        Type type = FindLoadedType(typeName);
        if (type == null) return "UNKNOWN_TYPE_MISSING";

        try
        {
            MethodInfo method = type.GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null);
            if (method == null) return "UNKNOWN_METHOD_MISSING";

            object value = method.Invoke(null, null);
            if (value is bool) return ((bool)value).ToString();
            return "UNKNOWN_RETURN_" + (value == null ? "NULL" : value.GetType().Name);
        }
        catch (Exception ex)
        {
            return "ERROR_" + ex.GetType().Name;
        }
    }

    private static int CountChildButtons(Component component)
    {
        Type buttonType = FindLoadedType("UnityEngine.UI.Button");
        if (buttonType == null) buttonType = FindLoadedType("Button");
        if (buttonType == null) return -1;

        try
        {
            Component[] buttons = component.GetComponentsInChildren(buttonType, true);
            return buttons == null ? 0 : buttons.Length;
        }
        catch
        {
            return -2;
        }
    }

    private void LogComponentHierarchy(Component component)
    {
        if (component == null || component.gameObject == null) return;

        LogInfo("[OR-BASELINE] UI_COMPONENT type=" + component.GetType().FullName
            + "; name=" + component.gameObject.name
            + "; activeSelf=" + component.gameObject.activeSelf
            + "; activeInHierarchy=" + component.gameObject.activeInHierarchy
            + "; hierarchy=" + DescribeHierarchy(component.transform));

        LogInstanceButtonFields(component);
        LogChildButtons(component);
    }

    private void LogInstanceButtonFields(Component component)
    {
        FieldInfo[] fields;
        try
        {
            fields = component.GetType().GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        catch { return; }

        int reported = 0;
        for (int i = 0; i < fields.Length && reported < 24; i++)
        {
            string name = fields[i].Name;
            if (name.IndexOf("button", StringComparison.OrdinalIgnoreCase) < 0
                && name.IndexOf("possess", StringComparison.OrdinalIgnoreCase) < 0
                && name.IndexOf("spectate", StringComparison.OrdinalIgnoreCase) < 0
                && name.IndexOf("select", StringComparison.OrdinalIgnoreCase) < 0
                && name.IndexOf("window", StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            try
            {
                object value = fields[i].GetValue(component);
                GameObject gameObject = value as GameObject;
                Component referencedComponent = value as Component;
                if (referencedComponent != null) gameObject = referencedComponent.gameObject;

                if (gameObject == null)
                {
                    LogInfo("[OR-BASELINE] UI_FIELD type=" + component.GetType().Name
                        + "; field=" + name + "; null=" + (value == null)
                        + "; valueType=" + (value == null ? "(null)" : value.GetType().FullName));
                }
                else
                {
                    LogInfo("[OR-BASELINE] UI_FIELD type=" + component.GetType().Name
                        + "; field=" + name
                        + "; object=" + gameObject.name
                        + "; active=" + gameObject.activeInHierarchy
                        + "; hierarchy=" + DescribeHierarchy(gameObject.transform));
                }
                reported++;
            }
            catch (Exception ex)
            {
                LogInfo("[OR-BASELINE] UI_FIELD_ERROR field=" + name + "; error=" + ex.GetType().Name);
            }
        }
    }

    private void LogChildButtons(Component component)
    {
        Type buttonType = FindLoadedType("UnityEngine.UI.Button");
        if (buttonType == null) buttonType = FindLoadedType("Button");
        if (buttonType == null)
        {
            LogInfo("[OR-BASELINE] UI_BUTTONS type=" + component.GetType().Name
                + "; result=BUTTON_TYPE_NOT_LOADED");
            return;
        }

        Component[] buttons;
        try
        {
            buttons = component.GetComponentsInChildren(buttonType, true);
        }
        catch (Exception ex)
        {
            LogInfo("[OR-BASELINE] UI_BUTTONS type=" + component.GetType().Name
                + "; result=ERROR; error=" + ex.GetType().Name + ": " + ex.Message);
            return;
        }

        int count = buttons == null ? 0 : buttons.Length;
        LogInfo("[OR-BASELINE] UI_BUTTONS type=" + component.GetType().Name + "; count=" + count);
        int limit = Math.Min(count, 20);
        for (int i = 0; i < limit; i++)
        {
            Component button = buttons[i];
            if (button == null || button.gameObject == null) continue;
            LogInfo("[OR-BASELINE] UI_BUTTON name=" + button.gameObject.name
                + "; active=" + button.gameObject.activeInHierarchy
                + "; hierarchy=" + DescribeHierarchy(button.transform)
                + "; rect=" + DescribeRect(button.transform as RectTransform));
        }

        if (count > limit)
            LogInfo("[OR-BASELINE] UI_BUTTONS_TRUNCATED omitted=" + (count - limit));
    }

    private static string DescribeHierarchy(Transform transform)
    {
        if (transform == null) return "(no-transform)";
        List<string> levels = new List<string>();
        Transform current = transform;
        int depth = 0;
        while (current != null && current.gameObject != null && depth < 8)
        {
            levels.Add(current.gameObject.name + "[active=" + current.gameObject.activeInHierarchy
                + ";rect=" + DescribeRect(current as RectTransform) + "]");
            current = current.parent;
            depth++;
        }

        return string.Join(" <- ", levels.ToArray());
    }

    private static string DescribeRect(RectTransform rect)
    {
        if (rect == null) return "(no-RectTransform)";
        return "anchorMin=" + rect.anchorMin
            + ";anchorMax=" + rect.anchorMax
            + ";pivot=" + rect.pivot
            + ";anchoredPosition=" + rect.anchoredPosition
            + ";sizeDelta=" + rect.sizeDelta;
    }
}
