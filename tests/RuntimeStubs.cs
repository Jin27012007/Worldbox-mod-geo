using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NeoModLoader.api;
using UnityEngine;

public class SelectedPowerTabTopButtons : MonoBehaviour
{
    private GameObject _button_possession;
    private GameObject _button_spectate;
    private GameObject _button_open_window;
    private GameObject _button_favorite;

    private void Update() { }

    public void AttachForTest(GameObject root)
    {
        gameObject = root;
        _button_possession = new GameObject("Button_Possession");
        _button_possession.transform.SetParent(root.transform, false);
        _button_possession.AddComponent(typeof(UnityEngine.UI.Button));

        _button_spectate = new GameObject("Button_Spectate");
        _button_spectate.transform.SetParent(root.transform, false);
        _button_spectate.AddComponent(typeof(UnityEngine.UI.Button));

        _button_open_window = new GameObject("Button_OpenWindow");
        _button_open_window.transform.SetParent(root.transform, false);
        _button_open_window.AddComponent(typeof(UnityEngine.UI.Button));

        _button_favorite = new GameObject("Button_Favorite");
        _button_favorite.transform.SetParent(root.transform, false);
        _button_favorite.AddComponent(typeof(UnityEngine.UI.Button));
    }
}

public static class SelectedUnit
{
    public static bool isSet() { return false; }
}

public static class SelectedObjects
{
    public static bool isNanoObjectSet() { return false; }
}

namespace NeoModLoader.api
{
    public abstract class BasicMod<T> : MonoBehaviour where T : BasicMod<T>
    {
        public static readonly List<string> CapturedLogs = new List<string>();
        protected abstract void OnModLoad();
        public static void LogInfo(string message) { CapturedLogs.Add(message); }
        public void InvokeOnModLoadForTest() { OnModLoad(); }
    }
}

namespace UnityEngine
{
    public class Object
    {
        private static readonly List<Object> AllObjects = new List<Object>();
        protected Object() { AllObjects.Add(this); }

        public static Object[] FindObjectsOfTypeAll(Type type)
        {
            List<Object> result = new List<Object>();
            for (int i = 0; i < AllObjects.Count; i++)
            {
                Object item = AllObjects[i];
                if (item != null && type.IsInstanceOfType(item)) result.Add(item);
            }
            return result.ToArray();
        }
    }

    public class Component : Object
    {
        public GameObject gameObject { get; set; }
        public Transform transform { get { return gameObject == null ? null : gameObject.transform; } }

        public Component GetComponent(Type type)
        {
            return gameObject == null ? null : gameObject.GetComponent(type);
        }

        public Component[] GetComponentsInChildren(Type type, bool includeInactive)
        {
            return gameObject == null ? new Component[0] : gameObject.GetComponentsInChildren(type, includeInactive);
        }
    }

    public class Behaviour : Component { }
    public class MonoBehaviour : Behaviour { }

    public class GameObject : Object
    {
        private readonly List<Component> _components = new List<Component>();
        public string name { get; set; }
        public Transform transform { get; private set; }
        public bool activeSelf { get; private set; } = true;
        public bool activeInHierarchy
        {
            get { return activeSelf && (transform == null || transform.parent == null || transform.parent.gameObject.activeInHierarchy); }
        }
        public SceneManagement.Scene scene { get; private set; } = new SceneManagement.Scene();

        public GameObject(string name, params Type[] components)
        {
            this.name = name;
            RectTransform rect = new RectTransform();
            transform = rect;
            rect.gameObject = this;
            _components.Add(rect);
            if (components != null)
                for (int i = 0; i < components.Length; i++) AddComponent(components[i]);
        }

        public void SetActive(bool active) { activeSelf = active; }

        public Component AddComponent(Type type)
        {
            Component component = (Component)Activator.CreateInstance(type, true);
            component.gameObject = this;
            _components.Add(component);
            return component;
        }

        public T AddComponent<T>() where T : Component, new()
        {
            T component = new T();
            component.gameObject = this;
            _components.Add(component);
            return component;
        }

        public Component GetComponent(Type type)
        {
            if (type.IsInstanceOfType(transform)) return transform;
            for (int i = 0; i < _components.Count; i++)
                if (type.IsInstanceOfType(_components[i])) return _components[i];
            return null;
        }

        public T GetComponent<T>() where T : class
        {
            return GetComponent(typeof(T)) as T;
        }

        public Component[] GetComponentsInChildren(Type type, bool includeInactive)
        {
            List<Component> result = new List<Component>();
            Object[] all = Object.FindObjectsOfTypeAll(typeof(Component));
            for (int i = 0; i < all.Length; i++)
            {
                Component c = all[i] as Component;
                if (c == null || c.gameObject == null || !type.IsInstanceOfType(c)) continue;
                if (!includeInactive && !c.gameObject.activeInHierarchy) continue;
                Transform current = c.transform;
                while (current != null)
                {
                    if (current == transform) { result.Add(c); break; }
                    current = current.parent;
                }
            }
            return result.ToArray();
        }
    }

    public class Transform : Component
    {
        public Transform parent { get; private set; }
        public Vector3 localScale { get; set; } = Vector3.one;

        public void SetParent(Transform newParent, bool worldPositionStays = true)
        {
            parent = newParent;
        }
    }

    public class RectTransform : Transform
    {
        public Vector2 anchorMin { get; set; }
        public Vector2 anchorMax { get; set; }
        public Vector2 pivot { get; set; }
        public Vector2 anchoredPosition { get; set; }
        public Vector2 sizeDelta { get; set; }
        public Rect rect { get { return new Rect(0, 0, sizeDelta.x, sizeDelta.y); } }
    }

    public class Canvas : Behaviour
    {
        public RenderMode renderMode { get; set; }
        public int sortingOrder { get; set; }
        public bool overrideSorting { get; set; }
    }

    public enum RenderMode { ScreenSpaceOverlay, ScreenSpaceCamera, WorldSpace }
    public enum RuntimePlatform { Android, WindowsPlayer, LinuxPlayer }

    public static class Application
    {
        public static string version = "0.51.4-test";
        public static string unityVersion = "2022.3-test";
        public static RuntimePlatform platform = RuntimePlatform.Android;
    }

    public static class Screen
    {
        public static int width = 480;
        public static int height = 1031;
    }

    public static class Resources
    {
        public static Object[] FindObjectsOfTypeAll(Type type) { return Object.FindObjectsOfTypeAll(type); }
    }

    public struct Vector2
    {
        public float x;
        public float y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
        public override string ToString() { return "(" + x + "," + y + ")"; }
    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public static Vector3 one { get { return new Vector3(1, 1, 1); } }
    }

    public struct Rect
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public Rect(float x, float y, float width, float height)
        { this.x = x; this.y = y; this.width = width; this.height = height; }
    }

    namespace SceneManagement
    {
        public struct Scene { public bool IsValid() { return true; } }
    }
}

namespace UnityEngine.UI
{
    public class Button : UnityEngine.Component { }
}
