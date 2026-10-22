// Compile-time stubs ONLY. These verify source structure and expected API shape.
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NeoModLoader.api
{
    public abstract class BasicMod<T> : UnityEngine.MonoBehaviour where T : BasicMod<T>
    {
        protected abstract void OnModLoad();
        protected void LogInfo(string message) { }
    }

    // Test-only shape of the NML native-window base class. This does not emulate game runtime.
    public abstract class AbstractWideWindow<T> : UnityEngine.MonoBehaviour where T : AbstractWideWindow<T>
    {
        public static T Instance { get; protected set; }
        protected UnityEngine.Transform ContentTransform { get; set; }
        protected UnityEngine.Transform BackgroundTransform { get; set; }
        protected abstract void Init();
        public static T CreateAndInit(string windowId, UnityEngine.Vector2 size = default(UnityEngine.Vector2)) { return default(T); }
        public void SetSize(UnityEngine.Vector2 size) { }
        public virtual void OnFirstEnable() { }
        public virtual void OnNormalEnable() { }
        public virtual void OnNormalDisable() { }
    }
}

namespace UnityEngine
{
    public class Object
    {
        public static void DontDestroyOnLoad(Object target) { }
        public static void Destroy(Object target) { }
        public static T FindObjectOfType<T>() where T : Object => default(T);
        public static Object[] FindObjectsOfTypeAll(Type type) => Array.Empty<Object>();
    }
    public class Component : Object
    {
        public GameObject gameObject { get; set; }
        public Transform transform { get; set; }
        public T GetComponent<T>() where T : class => default(T);
        public T GetComponentInChildren<T>(bool includeInactive = false) where T : class => default(T);
        public T[] GetComponentsInChildren<T>(bool includeInactive = false) => Array.Empty<T>();
    }
    public class Behaviour : Component { }
    public class MonoBehaviour : Behaviour { }
    public class GameObject : Object
    {
        public GameObject(string name, params Type[] components) { this.name = name; }
        public string name { get; set; }
        public Transform transform { get; } = new RectTransform();
        public bool activeInHierarchy { get; set; }
        public SceneManagement.Scene scene { get; } = new SceneManagement.Scene();
        public void SetActive(bool value) { }
        public T AddComponent<T>() where T : Component, new() => new T();
        public Component AddComponent(Type type) => null;
        public T GetComponent<T>() where T : class => default(T);
        public Component GetComponent(Type type) => null;
        public T GetComponentInChildren<T>(bool includeInactive = false) where T : class => default(T);
        public T[] GetComponentsInChildren<T>(bool includeInactive = false) => Array.Empty<T>();
    }
    public class Transform : Component
    {
        public Transform parent { get; set; }
        public Vector3 position { get; set; }
        public Vector3 localScale { get; set; }
        public Quaternion localRotation { get; set; }
        public void SetParent(Transform parent, bool worldPositionStays = true) { }
        public Transform Find(string name) => null;
        public int GetSiblingIndex() => 0;
        public void SetSiblingIndex(int index) { }
        public void SetAsLastSibling() { }
    }
    public class RectTransform : Transform
    {
        public Vector2 anchorMin { get; set; }
        public Vector2 anchorMax { get; set; }
        public Vector2 offsetMin { get; set; }
        public Vector2 offsetMax { get; set; }
        public Vector2 pivot { get; set; }
        public Vector2 sizeDelta { get; set; }
        public Vector2 anchoredPosition { get; set; }
        public Rect rect { get; set; }
    }
    public class Canvas : Behaviour { public RenderMode renderMode { get; set; } public int sortingOrder { get; set; } public bool overrideSorting { get; set; } }
    public enum RenderMode { ScreenSpaceOverlay, ScreenSpaceCamera, WorldSpace }
    public class Font : Object { }
    public class Sprite : Object { public Vector4 border { get; set; } }
    public class Material : Object { }
    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x=x; this.y=y; }
        public static Vector2 zero => new Vector2(0,0);
        public static Vector2 one => new Vector2(1,1);
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x+b.x,a.y+b.y);
        public static bool operator !=(Vector2 a, Vector2 b) => a.x!=b.x || a.y!=b.y;
        public static bool operator ==(Vector2 a, Vector2 b) => !(a!=b);
        public override bool Equals(object o) => o is Vector2 v && this==v;
        public override int GetHashCode() => HashCode.Combine(x,y);
    }
    public struct Vector3
    {
        public float x,y,z;
        public Vector3(float x,float y,float z) { this.x=x;this.y=y;this.z=z; }
        public static Vector3 one => new Vector3(1,1,1);
        public static Vector3 operator +(Vector3 a,Vector3 b) => new Vector3(a.x+b.x,a.y+b.y,a.z+b.z);
    }
    public struct Vector4 { public float x,y,z,w; public float sqrMagnitude => x*x+y*y+z*z+w*w; }
    public struct Quaternion { public static Quaternion identity => new Quaternion(); }
    public struct Rect
    {
        public float x,y,width,height;
        public Vector2 position { get => new Vector2(x,y); set { x=value.x;y=value.y; } }
        public Vector2 size { get => new Vector2(width,height); set { width=value.x;height=value.y; } }
        public float sqrMagnitude => x*x+y*y+width*width+height*height;
        public static bool operator !=(Rect a,Rect b) => a.x!=b.x||a.y!=b.y||a.width!=b.width||a.height!=b.height;
        public static bool operator ==(Rect a,Rect b) => !(a!=b);
        public override bool Equals(object o) => o is Rect r && this==r;
        public override int GetHashCode() => HashCode.Combine(x,y,width,height);
    }
    public struct Color
    {
        public float r,g,b,a;
        public Color(float r,float g,float b,float a=1f){this.r=r;this.g=g;this.b=b;this.a=a;}
        public static Color white => new Color(1,1,1,1);
    }
    public class RectOffset { public RectOffset(int left,int right,int top,int bottom) { } }
    public enum FontStyle { Normal, Bold, Italic, BoldAndItalic }
    public enum TextAnchor { UpperLeft, MiddleLeft, MiddleCenter }
    public enum HorizontalWrapMode { Wrap, Overflow }
    public enum VerticalWrapMode { Truncate, Overflow }
    public static class Screen { public static int width { get; set; } public static int height { get; set; } public static Rect safeArea { get; set; } }
    public static class Mathf
    {
        public static float Clamp(float value,float min,float max)=>Math.Max(min,Math.Min(max,value));
        public static int Clamp(int value,int min,int max)=>Math.Max(min,Math.Min(max,value));
        public static float Min(float a,float b)=>Math.Min(a,b);
        public static float Max(float a,float b)=>Math.Max(a,b);
        public static int RoundToInt(float value)=>(int)Math.Round(value);
    }
    public static class Resources
    {
        public static T GetBuiltinResource<T>(string path) where T : Object => default(T);
        public static Object[] FindObjectsOfTypeAll(Type type) => Array.Empty<Object>();
    }
    public static class Debug
    {
        public static void Log(object message) { }
        public static void LogWarning(object message) { }
        public static void LogError(object message) { }
    }
    namespace SceneManagement { public struct Scene { public bool IsValid() => true; } }
}

namespace UnityEngine.Events
{
    public delegate void UnityAction();
    public delegate void UnityAction<T>(T argument);
}

namespace UnityEngine.UI
{
    using UnityEngine;
    using UnityEngine.Events;
    public class Graphic : Behaviour { public Color color { get; set; } public Material material { get; set; } }
    public class Image : Graphic
    {
        public enum Type { Simple, Sliced, Tiled, Filled }
        public Sprite sprite { get; set; } public Type type { get; set; } public bool preserveAspect { get; set; }
    }
    public class Selectable : Behaviour
    {
        public enum Transition { None, ColorTint, SpriteSwap, Animation }
        public Graphic targetGraphic { get; set; } public Transition transition { get; set; } public ColorBlock colors { get; set; }
    }
    public struct ColorBlock { public Color normalColor, highlightedColor, pressedColor; }
    public class Button : Selectable { public ButtonClickedEvent onClick { get; } = new ButtonClickedEvent(); }
    public class Toggle : Selectable { public bool isOn { get; set; } public ToggleEvent onValueChanged { get; } = new ToggleEvent(); }
    public class Text : Graphic
    {
        public Font font { get; set; } public int fontSize { get; set; } public FontStyle fontStyle { get; set; }
        public TextAnchor alignment { get; set; } public string text { get; set; } public bool supportRichText { get; set; }
        public HorizontalWrapMode horizontalOverflow { get; set; } public VerticalWrapMode verticalOverflow { get; set; }
    }
    public class InputField : Selectable
    {
        public enum LineType { SingleLine, MultiLineSubmit, MultiLineNewline }
        public LineType lineType { get; set; } public Text textComponent { get; set; } public Graphic placeholder { get; set; } public string text { get; set; }
    }
    public class ScrollRect : Behaviour { public bool horizontal { get; set; } public bool vertical { get; set; } public RectTransform viewport { get; set; } public RectTransform content { get; set; } }
    public class Mask : Behaviour { public bool showMaskGraphic { get; set; } }
    public class LayoutElement : Behaviour { public float preferredHeight { get; set; } public float minHeight { get; set; } public float preferredWidth { get; set; } public float minWidth { get; set; } }
    public class HorizontalLayoutGroup : Behaviour { public float spacing { get; set; } public bool childControlWidth { get; set; } public bool childForceExpandWidth { get; set; } public bool childControlHeight { get; set; } public bool childForceExpandHeight { get; set; } }
    public class VerticalLayoutGroup : HorizontalLayoutGroup { public RectOffset padding { get; set; } }
    public class ContentSizeFitter : Behaviour
    {
        public enum FitMode { Unconstrained, MinSize, PreferredSize }
        public FitMode verticalFit { get; set; } public FitMode horizontalFit { get; set; }
    }
    public class CanvasScaler : Behaviour
    {
        public enum ScaleMode { ConstantPixelSize, ScaleWithScreenSize, ConstantPhysicalSize }
        public ScaleMode uiScaleMode { get; set; } public Vector2 referenceResolution { get; set; } public float matchWidthOrHeight { get; set; }
    }
    public class GraphicRaycaster : Behaviour { }
    public class Outline : Behaviour { public Color effectColor { get; set; } public Vector2 effectDistance { get; set; } }
    public class ButtonClickedEvent { public void AddListener(UnityAction callback) { } }
    public class ToggleEvent { public void AddListener(UnityAction<bool> callback) { } }
}


// Test-only stand-ins for WorldBox native UI entry points.
public class CanvasMain : UnityEngine.MonoBehaviour
{
    public static CanvasMain instance;
    public UnityEngine.GameObject canvas_ui;
}

public class ScrollWindow : UnityEngine.MonoBehaviour
{
    public static void showWindow(string id) { }
}


// Test-only Harmony surface; it does not execute patches.
namespace HarmonyLib
{
    using System.Reflection;
    public class Harmony
    {
        public Harmony(string id) { }
        public MethodInfo Patch(MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null,
            HarmonyMethod transpiler = null, HarmonyMethod finalizer = null) { return null; }
    }
    public class HarmonyMethod
    {
        public HarmonyMethod(MethodInfo method) { }
    }
}
