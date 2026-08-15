// Minimal UnityEngine stubs for compile-checking Cosmos code outside the Unity Editor.
// Only the API surface used by the checked code is declared.
using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public class Object
    {
        public string name { get; set; }
        public HideFlags hideFlags { get; set; }
        public override string ToString() { return name; }
        public static void Destroy(Object obj) { }
        public static void Destroy(Object obj, float t) { }
        public static void DestroyImmediate(Object obj) { }
        public static void DestroyImmediate(Object obj, bool allowDestroyingAssets) { }
        public static void DontDestroyOnLoad(Object target) { }
        public static T FindObjectOfType<T>() where T : Object { return null; }
        public static Object FindObjectOfType(Type type) { return null; }
        public static T[] FindObjectsOfType<T>() where T : Object { return null; }
        public static Object[] FindObjectsOfType(Type type) { return null; }
        public static bool operator ==(Object a, Object b) { return ReferenceEquals(a, b); }
        public static bool operator !=(Object a, Object b) { return !ReferenceEquals(a, b); }
        public static implicit operator bool(Object exists) { return exists != null; }
        public override bool Equals(object other) { return ReferenceEquals(this, other); }
        public override int GetHashCode() { return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this); }
    }

    public class Component : Object
    {
        public GameObject gameObject { get; set; }
        public Transform transform { get { return null; } }
        public T GetComponent<T>() where T : Component { return null; }
        public Component GetComponent(Type type) { return null; }
        public Component GetComponent(string type) { return null; }
        public T GetComponentInChildren<T>() where T : Component { return null; }
        public T GetComponentInParent<T>() where T : Component { return null; }
        public T[] GetComponents<T>() where T : Component { return null; }
        public void GetComponents<T>(List<T> results) where T : Component { }
        public Component[] GetComponents(Type type) { return null; }
        public T[] GetComponentsInChildren<T>(bool includeInactive = false) where T : Component { return null; }
        public T[] GetComponentsInParent<T>(bool includeInactive = false) where T : Component { return null; }
    }

    public class Behaviour : Component
    {
        public bool enabled { get; set; }
        public bool isActiveAndEnabled { get { return false; } }
    }

    public class MonoBehaviour : Behaviour
    {
        public Coroutine StartCoroutine(System.Collections.IEnumerator routine) { return null; }
        public void StopCoroutine(Coroutine routine) { }
        public void StopCoroutine(System.Collections.IEnumerator routine) { }
        public void StopAllCoroutines() { }
        public void Invoke(string methodName, float time) { }
        public void InvokeRepeating(string methodName, float time, float repeatRate) { }
        public void CancelInvoke() { }
        public bool IsInvoking(string methodName) { return false; }
    }

    public class Coroutine { }

    public class GameObject : Object
    {
        public GameObject() { }
        public GameObject(string name) { this.name = name; }
        public GameObject(string name, params Type[] components) { this.name = name; }
        public Transform transform { get { return null; } }
        public bool activeSelf { get; set; }
        public bool activeInHierarchy { get { return false; } }
        public int layer { get; set; }
        public GameObject gameObject { get { return this; } }
        public void SetActive(bool value) { }
        public T GetComponent<T>() where T : Component { return null; }
        public Component GetComponent(Type type) { return null; }
        public Component GetComponent(string type) { return null; }
        public T GetComponentInChildren<T>() where T : Component { return null; }
        public T GetComponentInParent<T>() where T : Component { return null; }
        public T[] GetComponents<T>() where T : Component { return null; }
        public Component[] GetComponents(Type type) { return null; }
        public T[] GetComponentsInChildren<T>(bool includeInactive = false) where T : Component { return null; }
        public T[] GetComponentsInParent<T>(bool includeInactive = false) where T : Component { return null; }
        public T AddComponent<T>() where T : Component { return null; }
        public Component AddComponent(Type type) { return null; }
        public static GameObject Find(string name) { return null; }
        public static T Instantiate<T>(T original) where T : Object { return original; }
        public static Object Instantiate(Object original) { return original; }
        public static Object Instantiate(Object original, Vector3 position, Quaternion rotation) { return original; }
        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object { return original; }
    }

    public class Transform : Component
    {
        public Vector3 position { get; set; }
        public Vector3 localPosition { get; set; }
        public Quaternion rotation { get; set; }
        public Quaternion localRotation { get; set; }
        public Vector3 localScale { get; set; }
        public Transform parent { get; set; }
        public int childCount { get; set; }
        public Vector3 forward { get { return Vector3.zero; } }
        public Vector3 up { get { return Vector3.up; } }
        public Vector3 right { get { return Vector3.right; } }
        public void SetParent(Transform p) { }
        public void SetParent(Transform p, bool worldPositionStays) { }
        public Transform GetChild(int index) { return null; }
        public Transform Find(string name) { return null; }
        public void SetAsLastSibling() { }
        public void SetAsFirstSibling() { }
        public void SetSiblingIndex(int index) { }
        public int GetSiblingIndex() { return 0; }
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation) { }
        public Vector3 TransformDirection(Vector3 direction) { return direction; }
        public Vector3 InverseTransformDirection(Vector3 direction) { return direction; }
        public Vector3 TransformPoint(Vector3 position) { return position; }
        public void LookAt(Transform target) { }
        public void LookAt(Vector3 worldPosition) { }
        public System.Collections.IEnumerator GetEnumerator() { return null; }
    }

    public class RectTransform : Transform
    {
        public Vector2 anchoredPosition { get; set; }
        public Vector2 sizeDelta { get; set; }
        public Vector2 anchorMin { get; set; }
        public Vector2 anchorMax { get; set; }
        public Vector2 pivot { get; set; }
        public Vector2 offsetMin { get; set; }
        public Vector2 offsetMax { get; set; }
        public Rect rect { get { return default(Rect); } }
        public void SetSizeWithCurrentAnchors(Axis axis, float size) { }
        public enum Axis { Horizontal = 0, Vertical = 1 }
    }

    public class Canvas : Behaviour
    {
        public RenderMode renderMode { get; set; }
        public int sortingOrder { get; set; }
        public float planeDistance { get; set; }
        public Camera worldCamera { get; set; }
        public Rect pixelRect { get { return default(Rect); } }
    }

    public enum RenderMode { ScreenSpaceOverlay = 0, ScreenSpaceCamera = 1, WorldSpace = 2 }

    public class CanvasGroup : Behaviour
    {
        public float alpha { get; set; }
        public bool interactable { get; set; }
        public bool blocksRaycasts { get; set; }
        public bool ignoreParentGroups { get; set; }
    }

    public class Animator : Behaviour
    {
        public float GetFloat(string name) { return 0; }
        public void SetFloat(string name, float value) { }
        public bool GetBool(string name) { return false; }
        public void SetBool(string name, bool value) { }
        public int GetInteger(string name) { return 0; }
        public void SetInteger(string name, int value) { }
        public void SetTrigger(string name) { }
        public void ResetTrigger(string name) { }
        public void Play(string stateName) { }
        public void Play(string stateName, int layer, float normalizedTime) { }
        public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex) { return null; }
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex) { return default(AnimatorStateInfo); }
        public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex) { return default(AnimatorStateInfo); }
        public bool HasState(int layerIndex, int stateID) { return false; }
        public float speed { get; set; }
        public bool isInitialized { get { return false; } }
    }

    public struct AnimatorStateInfo
    {
        public bool IsName(string name) { return false; }
        public bool IsTag(string tag) { return false; }
        public float normalizedTime { get; set; }
        public float length { get; set; }
        public float speed { get; set; }
        public int nameHash { get; set; }
        public int shortNameHash { get; set; }
        public int tagHash { get; set; }
        public int fullPathHash { get; set; }
        public int loop { get; set; }
    }

    public struct AnimatorClipInfo
    {
        public AnimationClip clip { get; set; }
        public float weight { get; set; }
    }

    public class AnimationClip : Object { }

    public class AudioSource : Behaviour
    {
        public AudioClip clip { get; set; }
        public bool isPlaying { get { return false; } }
        public bool playOnAwake { get; set; }
        public bool loop { get; set; }
        public bool mute { get; set; }
        public float pitch { get; set; }
        public float volume { get; set; }
        public float time { get; set; }
        public int priority { get; set; }
        public float panStereo { get; set; }
        public float spatialBlend { get; set; }
        public float reverbZoneMix { get; set; }
        public float dopplerLevel { get; set; }
        public float spread { get; set; }
        public float maxDistance { get; set; }
        public float minDistance { get; set; }
        public void Play() { }
        public void Play(ulong delay) { }
        public void Stop() { }
        public void Pause() { }
        public void UnPause() { }
        public void PlayOneShot(AudioClip clip) { }
        public void PlayOneShot(AudioClip clip, float volumeScale) { }
        public void SetScheduledStartTime(double time) { }
    }

    [Flags]
    public enum HideFlags
    {
        None = 0,
        HideInHierarchy = 1,
        HideInInspector = 2,
        DontSaveInEditor = 4,
        NotEditable = 8,
        DontSaveInBuild = 16,
        DontUnloadUnloadedScene = 32,
        DontSave = 52,
        HideAndDontSave = 61,
    }

    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
        public float magnitude { get { return (float)Math.Sqrt(x * x + y * y); } }
        public float sqrMagnitude { get { return x * x + y * y; } }
        public Vector2 normalized { get { return this; } }
        public void Set(float newX, float newY) { x = newX; y = newY; }
        public static Vector2 zero { get { return new Vector2(0, 0); } }
        public static Vector2 one { get { return new Vector2(1, 1); } }
        public static Vector2 up { get { return new Vector2(0, 1); } }
        public static Vector2 down { get { return new Vector2(0, -1); } }
        public static Vector2 left { get { return new Vector2(-1, 0); } }
        public static Vector2 right { get { return new Vector2(1, 0); } }
        public static float Angle(Vector2 from, Vector2 to) { return 0; }
        public static float Distance(Vector2 a, Vector2 b) { return 0; }
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) { return a; }
        public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.x + b.x, a.y + b.y); }
        public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.x - b.x, a.y - b.y); }
        public static Vector2 operator *(Vector2 a, float d) { return new Vector2(a.x * d, a.y * d); }
        public static Vector2 operator *(Vector2 a, Vector2 b) { return new Vector2(a.x * b.x, a.y * b.y); }
        public static Vector2 operator /(Vector2 a, float d) { return new Vector2(a.x / d, a.y / d); }
        public static bool operator ==(Vector2 lhs, Vector2 rhs) { return lhs.x == rhs.x && lhs.y == rhs.y; }
        public static bool operator !=(Vector2 lhs, Vector2 rhs) { return !(lhs == rhs); }
        public static implicit operator Vector3(Vector2 v) { return new Vector3(v.x, v.y, 0); }
        public static implicit operator Vector2(Vector3 v) { return new Vector2(v.x, v.y); }
        public override bool Equals(object other) { return false; }
        public override int GetHashCode() { return 0; }
        public override string ToString() { return string.Empty; }
    }

    public struct Vector3
    {
        public float x, y, z;
        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public Vector3(float x, float y) { this.x = x; this.y = y; this.z = 0; }
        public float magnitude { get { return (float)Math.Sqrt(x * x + y * y + z * z); } }
        public float sqrMagnitude { get { return x * x + y * y + z * z; } }
        public Vector3 normalized { get { return this; } }
        public void Set(float newX, float newY, float newZ) { x = newX; y = newY; z = newZ; }
        public static Vector3 zero { get { return new Vector3(0, 0, 0); } }
        public static Vector3 one { get { return new Vector3(1, 1, 1); } }
        public static Vector3 up { get { return new Vector3(0, 1, 0); } }
        public static Vector3 down { get { return new Vector3(0, -1, 0); } }
        public static Vector3 left { get { return new Vector3(-1, 0, 0); } }
        public static Vector3 right { get { return new Vector3(1, 0, 0); } }
        public static Vector3 forward { get { return new Vector3(0, 0, 1); } }
        public static Vector3 back { get { return new Vector3(0, 0, -1); } }
        public static float Angle(Vector3 from, Vector3 to) { return 0; }
        public static float Distance(Vector3 a, Vector3 b) { return 0; }
        public static float Dot(Vector3 lhs, Vector3 rhs) { return 0; }
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs) { return Vector3.zero; }
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) { return a; }
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta) { return target; }
        public static Vector3 operator +(Vector3 a, Vector3 b) { return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
        public static Vector3 operator -(Vector3 a, Vector3 b) { return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }
        public static Vector3 operator -(Vector3 a) { return new Vector3(-a.x, -a.y, -a.z); }
        public static Vector3 operator *(Vector3 a, float d) { return new Vector3(a.x * d, a.y * d, a.z * d); }
        public static Vector3 operator /(Vector3 a, float d) { return new Vector3(a.x / d, a.y / d, a.z / d); }
        public static bool operator ==(Vector3 lhs, Vector3 rhs) { return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z; }
        public static bool operator !=(Vector3 lhs, Vector3 rhs) { return !(lhs == rhs); }
        public override bool Equals(object other) { return false; }
        public override int GetHashCode() { return 0; }
        public override string ToString() { return string.Empty; }
    }

    public struct Quaternion
    {
        public float x, y, z, w;
        public static Quaternion identity { get { return new Quaternion(); } }
        public static Quaternion Euler(float x, float y, float z) { return identity; }
        public static Quaternion Euler(Vector3 euler) { return identity; }
        public static Quaternion LookRotation(Vector3 forward) { return identity; }
        public static Quaternion LookRotation(Vector3 forward, Vector3 upwards) { return identity; }
        public static Quaternion operator *(Quaternion lhs, Quaternion rhs) { return lhs; }
        public static Vector3 operator *(Quaternion rotation, Vector3 point) { return point; }
        public static float Angle(Quaternion a, Quaternion b) { return 0; }
        public static Quaternion Slerp(Quaternion a, Quaternion b, float t) { return a; }
        public override string ToString() { return string.Empty; }
    }

    public struct Color
    {
        public float r, g, b, a;
        public Color(float r, float g, float b) { this.r = r; this.g = g; this.b = b; this.a = 1; }
        public Color(float r, float g, float b, float a) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static Color white { get { return new Color(1, 1, 1, 1); } }
        public static Color black { get { return new Color(0, 0, 0, 1); } }
        public static Color red { get { return new Color(1, 0, 0, 1); } }
        public static Color green { get { return new Color(0, 1, 0, 1); } }
        public static Color blue { get { return new Color(0, 0, 1, 1); } }
        public static Color yellow { get { return new Color(1, 1, 0, 1); } }
        public static Color cyan { get { return new Color(0, 1, 1, 1); } }
        public static Color magenta { get { return new Color(1, 0, 1, 1); } }
        public static Color gray { get { return new Color(0.5f, 0.5f, 0.5f, 1); } }
        public static Color grey { get { return new Color(0.5f, 0.5f, 0.5f, 1); } }
        public static Color clear { get { return new Color(0, 0, 0, 0); } }
        public static Color operator *(Color a, float b) { return a; }
        public static Color operator +(Color a, Color b) { return a; }
        public string ToHexStringRGB() { return string.Empty; }
        public override string ToString() { return string.Empty; }
    }

    public struct Color32
    {
        public byte r, g, b, a;
        public Color32(byte r, byte g, byte b, byte a) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static implicit operator Color32(Color c) { return new Color32(); }
        public static implicit operator Color(Color32 c) { return new Color(); }
        public override string ToString() { return string.Empty; }
    }

    public struct Rect
    {
        public float x, y, width, height;
        public Rect(float x, float y, float width, float height) { this.x = x; this.y = y; this.width = width; this.height = height; }
        public Rect(Rect source) { x = source.x; y = source.y; width = source.width; height = source.height; }
        public Rect(Vector2 position, Vector2 size) { x = position.x; y = position.y; width = size.x; height = size.y; }
        public float xMin { get { return x; } }
        public float yMin { get { return y; } }
        public float xMax { get { return x + width; } }
        public float yMax { get { return y + height; } }
        public Vector2 position { get { return new Vector2(x, y); } }
        public Vector2 size { get { return new Vector2(width, height); } }
        public Vector2 center { get { return new Vector2(x + width / 2, y + height / 2); } }
        public bool Contains(Vector2 point) { return false; }
        public static Rect zero { get { return new Rect(0, 0, 0, 0); } }
    }

    public static class Mathf
    {
        public static float Max(float a, float b) { return a > b ? a : b; }
        public static int Max(int a, int b) { return a > b ? a : b; }
        public static float Min(float a, float b) { return a < b ? a : b; }
        public static int Min(int a, int b) { return a < b ? a : b; }
        public static float Clamp01(float value) { return value < 0 ? 0 : (value > 1 ? 1 : value); }
        public static float Clamp(float value, float min, float max) { return value < min ? min : (value > max ? max : value); }
        public static int Clamp(int value, int min, int max) { return value < min ? min : (value > max ? max : value); }
        public static float Lerp(float a, float b, float t) { return a + (b - a) * t; }
        public static float LerpUnclamped(float a, float b, float t) { return a + (b - a) * t; }
        public static float Abs(float value) { return value < 0 ? -value : value; }
        public static int Abs(int value) { return value < 0 ? -value : value; }
        public static float Sqrt(float value) { return (float)Math.Sqrt(value); }
        public static float Pow(float f, float p) { return (float)Math.Pow(f, p); }
        public static float Sin(float f) { return (float)Math.Sin(f); }
        public static float Cos(float f) { return (float)Math.Cos(f); }
        public static float Tan(float f) { return (float)Math.Tan(f); }
        public static float Asin(float f) { return (float)Math.Asin(f); }
        public static float Acos(float f) { return (float)Math.Acos(f); }
        public static float Atan(float f) { return (float)Math.Atan(f); }
        public static float Atan2(float y, float x) { return (float)Math.Atan2(y, x); }
        public static float Exp(float power) { return (float)Math.Exp(power); }
        public static float Log(float f) { return (float)Math.Log(f); }
        public static float Log10(float f) { return (float)Math.Log10(f); }
        public static float Floor(float f) { return (float)Math.Floor(f); }
        public static float Ceil(float f) { return (float)Math.Ceiling(f); }
        public static float Round(float f) { return (float)Math.Round(f); }
        public static int FloorToInt(float f) { return (int)Math.Floor(f); }
        public static int CeilToInt(float f) { return (int)Math.Ceiling(f); }
        public static int RoundToInt(float f) { return (int)Math.Round(f); }
        public static float Repeat(float t, float length) { return t - Floor(t / length) * length; }
        public static float PingPong(float t, float length) { return length - Abs(Repeat(t, length * 2) - length); }
        public static float Sign(float f) { return f < 0 ? -1 : 1; }
        public static float MoveTowards(float current, float target, float maxDelta) { return target; }
        public static float SmoothStep(float from, float to, float t) { return Lerp(from, to, t); }
        public static bool Approximately(float a, float b) { return Abs(a - b) < 1e-6f; }
        public static float DeltaAngle(float current, float target) { return 0; }
        public static float MoveTowardsAngle(float current, float target, float maxDelta) { return target; }
        public static float LerpAngle(float a, float b, float t) { return Lerp(a, b, t); }
        public const float PI = (float)Math.PI;
        public const float Deg2Rad = PI / 180f;
        public const float Rad2Deg = 180f / PI;
        public const float Infinity = float.PositiveInfinity;
        public const float NegativeInfinity = float.NegativeInfinity;
        public const float Epsilon = 1.401298E-45f;
    }

    public static class Debug
    {
        public static void Log(object message) { }
        public static void Log(object message, Object context) { }
        public static void LogWarning(object message) { }
        public static void LogWarning(object message, Object context) { }
        public static void LogError(object message) { }
        public static void LogError(object message, Object context) { }
        public static void LogErrorFormat(string format, params object[] args) { }
        public static void LogErrorFormat(Object context, string format, params object[] args) { }
        public static void LogWarningFormat(string format, params object[] args) { }
        public static void LogWarningFormat(Object context, string format, params object[] args) { }
        public static void LogFormat(string format, params object[] args) { }
        public static void LogAssertion(object message) { }
        public static void LogAssertion(object message, Object context) { }
        public static void LogAssertionFormat(string format, params object[] args) { }
        public static void LogException(Exception exception) { }
        public static void Assert(bool condition) { }
        public static void Assert(bool condition, object message) { }
        public static void DrawLine(Vector3 start, Vector3 end) { }
        public static void DrawLine(Vector3 start, Vector3 end, Color color) { }
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration) { }
        public static void DrawRay(Vector3 start, Vector3 dir) { }
        public static void DrawRay(Vector3 start, Vector3 dir, Color color) { }
    }

    public enum LogType
    {
        Error = 0, Assert = 1, Warning = 2, Log = 3, Exception = 4,
    }

    public static class Application
    {
        public static string persistentDataPath { get { return string.Empty; } }
        public static string streamingAssetsPath { get { return string.Empty; } }
        public static string dataPath { get { return string.Empty; } }
        public static string temporaryCachePath { get { return string.Empty; } }
        public static bool isPlaying { get; set; }
        public static bool isEditor { get; set; }
        public static bool isBatchMode { get; set; }
        public static bool runInBackground { get; set; }
        public static RuntimePlatform platform { get; set; }
        public static string version { get { return string.Empty; } }
        public static string unityVersion { get { return string.Empty; } }
        public static string productName { get { return string.Empty; } }
        public static string companyName { get { return string.Empty; } }
        public static event Action<string, string, LogType> logMessageReceived;
        public static void Quit() { }
        public static void OpenURL(string url) { }
        public static void SetStackTraceLogType(LogType logType, StackTraceLogType stackTraceType) { }
    }

    public enum StackTraceLogType { None, ScriptOnly, Full }

    public enum RuntimePlatform
    {
        WindowsPlayer = 2, OSXPlayer = 4, IPhonePlayer = 8, Android = 11,
        WindowsEditor = 1, OSXEditor = 3, LinuxEditor = 40, WebGLPlayer = 13,
        LinuxPlayer = 12, PS4 = 31, XboxOne = 21, Switch = 38,
    }

    public static class Time
    {
        public static float deltaTime { get { return 0; } }
        public static float fixedDeltaTime { get { return 0; } }
        public static float time { get { return 0; } }
        public static float unscaledTime { get { return 0; } }
        public static float realtimeSinceStartup { get { return 0; } }
        public static float timeScale { get; set; }
        public static int frameCount { get { return 0; } }
    }

    public static class Screen
    {
        public static int width { get { return 0; } }
        public static int height { get { return 0; } }
        public static float dpi { get { return 0; } }
        public static bool fullScreen { get; set; }
        public static ScreenOrientation orientation { get; set; }
    }

    public enum ScreenOrientation { Unknown = 0, Portrait = 1, PortraitUpsideDown = 2, LandscapeLeft = 3, LandscapeRight = 4 }

    public static class Input
    {
        public static bool GetKey(KeyCode key) { return false; }
        public static bool GetKeyDown(KeyCode key) { return false; }
        public static bool GetKeyUp(KeyCode key) { return false; }
        public static bool GetKey(string name) { return false; }
        public static bool GetKeyDown(string name) { return false; }
        public static bool GetKeyUp(string name) { return false; }
        public static bool GetButton(string buttonName) { return false; }
        public static bool GetButtonDown(string buttonName) { return false; }
        public static bool GetButtonUp(string buttonName) { return false; }
        public static float GetAxis(string axisName) { return 0; }
        public static float GetAxisRaw(string axisName) { return 0; }
        public static bool GetMouseButton(int button) { return false; }
        public static bool GetMouseButtonDown(int button) { return false; }
        public static bool GetMouseButtonUp(int button) { return false; }
        public static Vector3 mousePosition { get { return Vector3.zero; } }
        public static Vector2 mouseScrollDelta { get { return Vector2.zero; } }
        public static int touchCount { get { return 0; } }
        public static Touch GetTouch(int index) { return default(Touch); }
        public static bool GetTouch(int index, out Touch touch) { touch = default(Touch); return false; }
        public static bool touchSupported { get { return false; } }
        public static bool anyKey { get { return false; } }
        public static bool anyKeyDown { get { return false; } }
        public static string inputString { get { return string.Empty; } }
        public static void ResetInputAxes() { }
    }

    public struct Touch
    {
        public int fingerId { get; set; }
        public Vector2 position { get; set; }
        public Vector2 deltaPosition { get; set; }
        public float deltaTime { get; set; }
        public int tapCount { get; set; }
        public TouchPhase phase { get; set; }
    }

    public enum TouchPhase { Began, Moved, Stationary, Ended, Canceled }

    public enum KeyCode
    {
        None = 0, Backspace = 8, Tab = 9, Return = 13, Escape = 27, Space = 32,
        LeftArrow = 276, RightArrow = 275, UpArrow = 273, DownArrow = 274,
        Alpha0 = 48, Alpha1 = 49, Alpha2 = 50, Alpha3 = 51, Alpha4 = 52,
        Alpha5 = 53, Alpha6 = 54, Alpha7 = 55, Alpha8 = 56, Alpha9 = 57,
        A = 97, B = 98, C = 99, D = 100, E = 101, F = 102, G = 103, H = 104, I = 105,
        J = 106, K = 107, L = 108, M = 109, N = 110, O = 111, P = 112, Q = 113, R = 114,
        S = 115, T = 116, U = 117, V = 118, W = 119, X = 120, Y = 121, Z = 122,
        F1 = 282, F2 = 283, F3 = 284, F4 = 285, F5 = 286, F6 = 287, F7 = 288, F8 = 289,
        F9 = 290, F10 = 291, F11 = 292, F12 = 293, LeftShift = 304, RightShift = 303,
        LeftControl = 306, RightControl = 305, LeftAlt = 308, RightAlt = 307,
        Mouse0 = 323, Mouse1 = 324, Mouse2 = 325, Mouse3 = 326, Mouse4 = 327,
        Delete = 127, Home = 278, End = 279, PageUp = 280, PageDown = 281,
        Period = 46, Comma = 44, Minus = 45, Plus = 61, Quote = 39, BackQuote = 96,
        LeftBracket = 91, RightBracket = 93, Semicolon = 59, Slash = 47, Backslash = 92,
        CapsLock = 301, Numlock = 300, ScrollLock = 302, Insert = 277, Print = 316,
        JoystickButton0 = 330, JoystickButton1 = 331, JoystickButton2 = 332, JoystickButton3 = 333,
    }

    public struct LayerMask
    {
        public int value;
        public static implicit operator int(LayerMask mask) { return mask.value; }
        public static implicit operator LayerMask(int intVal) { return new LayerMask() { value = intVal }; }
        public static string LayerToName(int layer) { return string.Empty; }
        public static int NameToLayer(string layerName) { return 0; }
        public static int GetMask(params string[] layerNames) { return 0; }
    }

    public static class Gizmos
    {
        public static Color color { get; set; }
        public static void DrawLine(Vector3 from, Vector3 to) { }
        public static void DrawRay(Vector3 from, Vector3 direction) { }
        public static void DrawWireCube(Vector3 center, Vector3 size) { }
        public static void DrawCube(Vector3 center, Vector3 size) { }
        public static void DrawSphere(Vector3 center, float radius) { }
        public static void DrawWireSphere(Vector3 center, float radius) { }
        public static void DrawIcon(Vector3 center, string name) { }
    }

    public class GUI
    {
        public class Scope : IDisposable
        {
            public void Dispose() { }
        }
        public static Color color { get; set; }
        public static Color backgroundColor { get; set; }
        public static Color contentColor { get; set; }
        public static GUISkin skin { get; set; }
        public static GUIStyle label { get { return null; } }
        public static void Label(Rect position, string text) { }
        public static void Label(Rect position, GUIContent content) { }
        public static void Label(Rect position, string text, GUIStyle style) { }
        public static bool Button(Rect position, string text) { return false; }
        public static bool Button(Rect position, GUIContent content) { return false; }
        public static void Box(Rect position, string text) { }
        public static void DrawTexture(Rect position, Texture image) { }
        public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode) { }
        public static void SetNextControlName(string name) { }
        public static string TextField(Rect position, string text) { return text; }
        public static string TextField(Rect position, string text, GUIStyle style) { return text; }
        public static void Toggle(Rect position, bool value) { }
        public static void Toggle(Rect position, bool value, GUIContent content) { }
        public static bool FocusControl(string name) { return false; }
        public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords) { }
    }

    public class GUISkin : ScriptableObject
    {
        public GUIStyle label { get; set; }
        public GUIStyle box { get; set; }
        public GUIStyle button { get; set; }
        public GUIStyle textField { get; set; }
        public GUIStyle toggle { get; set; }
        public GUIStyle window { get; set; }
        public GUIStyle horizontalScrollbar { get; set; }
        public GUIStyle verticalScrollbar { get; set; }
    }

    public static class GUIUtility
    {
        public static int GetControlID(FocusType focus) { return 0; }
        public static int GetControlID(int hint, FocusType focusType) { return 0; }
        public static int GetControlID(GUIContent content, FocusType focus) { return 0; }
        public static int GetControlID(GUIContent content, FocusType focus, Rect rect) { return 0; }
        public static void ExitGUI() { }
        public static bool IsExitGUIException(Exception exception) { return false; }
        public static string GetNameOfFocusedControl() { return string.Empty; }
        public static Vector2 GUIToScreenPoint(Vector2 guiPoint) { return guiPoint; }
        public static float pixelsPerPoint { get { return 1; } }
        public static int hotControl { get; set; }
        public static int keyboardControl { get; set; }
        public static string systemCopyBuffer { get; set; }
        public static bool changed { get; set; }
    }

    public enum FocusType { Keyboard, Passive }

    public static class GUILayoutUtility
    {
        public static Rect GetRect(float width, float height, params GUILayoutOption[] options) { return default(Rect); }
        public static Rect GetRect(float width, float height, GUIStyle style, params GUILayoutOption[] options) { return default(Rect); }
        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, params GUILayoutOption[] options) { return default(Rect); }
        public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, params GUILayoutOption[] options) { return default(Rect); }
        public static Rect GetRect(GUIContent content, GUIStyle style, params GUILayoutOption[] options) { return default(Rect); }
        public static Rect GetLastRect() { return default(Rect); }
        public static Rect GetAspectRect(float aspect, params GUILayoutOption[] options) { return default(Rect); }
    }

    public enum TextAlignment { Left, Center, Right }

    public class GUILayoutOption
    {
    }

    public class MonoScript : Object
    {
        public Type GetClass() { return null; }
        public static MonoScript FromMonoBehaviour(Behaviour behaviour) { return null; }
    }

    public enum ScaleMode { StretchToFill, ScaleAndCrop, ScaleToFit }

    public class GUIStyle
    {
        public GUIStyle() { }
        public GUIStyle(GUIStyle other) { }
        public GUIStyle(string name) { }
        public GUIStyleState normal { get; set; }
        public GUIStyleState hover { get; set; }
        public GUIStyleState active { get; set; }
        public GUIStyleState focused { get; set; }
        public GUIStyleState onNormal { get; set; }
        public int fontSize { get; set; }
        public bool richText { get; set; }
        public Font font { get; set; }
        public TextAnchor alignment { get; set; }
        public Vector2 CalcSize(GUIContent content) { return Vector2.zero; }
        public float CalcHeight(GUIContent content, float width) { return 0; }
        public void Draw(Rect position, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus) { }
        public void Draw(Rect position, bool isHover, bool isActive, bool on, bool hasKeyboardFocus) { }
        public void Draw(Rect position, GUIContent content, int controlID) { }
        public void Draw(Rect position, GUIContent content, int controlID, bool on) { }
        public void Draw(Rect position, string text, int controlID, bool on) { }
        public override string ToString() { return string.Empty; }
    }

    public class GUIStyleState
    {
        public Color textColor { get; set; }
        public Texture2D background { get; set; }
        public Texture2D scaledBackgrounds { get; set; }
    }

    public enum TextAnchor { UpperLeft, UpperCenter, UpperRight, MiddleLeft, MiddleCenter, MiddleRight, LowerLeft, LowerCenter, LowerRight }

    public class GUIContent
    {
        public GUIContent() { }
        public GUIContent(string text) { }
        public GUIContent(string text, string tooltip) { }
        public GUIContent(Texture image) { }
        public GUIContent(Texture image, string tooltip) { }
        public string text { get; set; }
        public string tooltip { get; set; }
        public Texture image { get; set; }
        public static GUIContent none { get { return new GUIContent(); } }
        public static GUIContent Temp(string text) { return new GUIContent(text); }
    }

    public class Font : Object
    {
        public int fontSize { get; set; }
        public static Font CreateDynamicFontFromOSFont(string fontname, int size) { return null; }
    }

    public static class JsonUtility
    {
        public static string ToJson(object obj) { return string.Empty; }
        public static string ToJson(object obj, bool prettyPrint) { return string.Empty; }
        public static T FromJson<T>(string json) { return default(T); }
        public static object FromJson(string json, Type type) { return null; }
        public static void FromJsonOverwrite(string json, object objectToOverwrite) { }
    }

    public class AsyncOperation
    {
        public bool isDone { get; set; }
        public float progress { get; set; }
        public bool allowSceneActivation { get; set; }
        public event Action completed;
    }

    public class AssetBundleCreateRequest : AsyncOperation
    {
        public AssetBundle assetBundle { get; set; }
    }

    public class AssetBundleRequest : AsyncOperation
    {
        public Object asset { get; set; }
        public Object[] allAssets { get; set; }
    }

    public class AssetBundle : Object
    {
        public static AssetBundleCreateRequest LoadFromFileAsync(string path) { return null; }
        public static AssetBundleCreateRequest LoadFromFileAsync(string path, uint crc) { return null; }
        public static AssetBundleCreateRequest LoadFromFileAsync(string path, uint crc, ulong offset) { return null; }
        public static AssetBundle LoadFromFile(string path) { return null; }
        public AssetBundleRequest LoadAssetAsync(string name, Type type) { return null; }
        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name, Type type) { return null; }
        public AssetBundleRequest LoadAllAssetsAsync(Type type) { return null; }
        public AssetBundleRequest LoadAllAssetsAsync() { return null; }
        public Object LoadAsset(string name, Type type) { return null; }
        public T LoadAsset<T>(string name) where T : Object { return null; }
        public Object[] LoadAllAssets(Type type) { return null; }
        public void Unload(bool unloadAllLoadedObjects) { }
        public string name { get; set; }
    }

    public class Texture : Object
    {
        public int width { get; set; }
        public int height { get; set; }
        public FilterMode filterMode { get; set; }
        public TextureWrapMode wrapMode { get; set; }
        public int anisoLevel { get; set; }
        public bool isReadable { get { return false; } }
        public IntPtr GetNativeTexturePtr() { return IntPtr.Zero; }
    }

    public enum FilterMode { Point, Bilinear, Trilinear }

    public enum TextureWrapMode { Repeat, Clamp, Mirror, MirrorOnce }

    public enum TextureFormat
    {
        Alpha8 = 1, ARGB4444 = 2, RGB24 = 3, RGBA32 = 4, ARGB32 = 5,
        RGB565 = 7, R16 = 9, DXT1 = 10, DXT5 = 12, RGBA4444 = 13,
        BGRA32 = 14, RHalf = 15, RGHalf = 16, RGBAHalf = 17, RFloat = 18,
        RGFloat = 19, RGBAFloat = 20, YUY2 = 21, RGB9e5Float = 22,
        BC4 = 26, BC5 = 27, BC6H = 24, BC7 = 25, DXT1Crunched = 28,
        DXT5Crunched = 29, PVRTC_RGB2 = 30, PVRTC_RGBA2 = 31, PVRTC_RGB4 = 32,
        PVRTC_RGBA4 = 33, ETC_RGB4 = 34, ATC_RGB4 = 35, ATC_RGBA8 = 36,
        EAC_R = 41, EAC_R_SIGNED = 42, EAC_RG = 43, EAC_RG_SIGNED = 44,
        ETC2_RGB = 45, ETC2_RGBA1 = 46, ETC2_RGBA8 = 47, ASTC_4x4 = 48,
        ASTC_6x6 = 50, ASTC_8x8 = 52, ASTC_10x10 = 53, ASTC_12x12 = 54,
        R8 = 62, RG16 = 63, RGFloat16 = 64, RGBAUShort = 65,
    }

    public class Texture2D : Texture
    {
        public Texture2D(int width, int height) { }
        public Texture2D(int width, int height, TextureFormat textureFormat, bool mipChain) { }
        public Texture2D(int width, int height, TextureFormat textureFormat, bool mipChain, bool linear) { }
        public TextureFormat format { get { return TextureFormat.RGBA32; } }
        public Color GetPixel(int x, int y) { return Color.white; }
        public Color GetPixelBilinear(float u, float v) { return Color.white; }
        public Color[] GetPixels() { return null; }
        public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight) { return null; }
        public Color32[] GetPixels32() { return null; }
        public Color32[] GetPixels32(int miplevel) { return null; }
        public void SetPixel(int x, int y, Color color) { }
        public void SetPixels(Color[] colors) { }
        public void SetPixels32(Color32[] colors) { }
        public void Apply() { }
        public void Apply(bool updateMipmaps) { }
        public void Apply(bool updateMipmaps, bool makeNoLongerReadable) { }
        public bool LoadImage(byte[] data) { return false; }
        public void ReadPixels(Rect source, int destX, int destY) { }
        public void ReadPixels(Rect source, int destX, int destY, bool recalculateMipMaps) { }
        public static Texture2D CreateExternalTexture(int width, int height, TextureFormat format, bool mipChain, bool linear, IntPtr nativeTex) { return null; }
        public byte[] EncodeToPNG() { return null; }
        public byte[] EncodeToJPG() { return null; }
        public static Texture2D whiteTexture { get { return null; } }
    }

    public class RenderTexture : Texture
    {
        public RenderTexture(int width, int height, int depth) { }
        public RenderTexture(int width, int height, int depth, RenderTextureFormat format) { }
        public RenderTexture(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite) { }
        public void Release() { }
        public bool IsCreated() { return false; }
        public static RenderTexture active { get; set; }
        public static RenderTexture GetTemporary(int width, int height, int depthBuffer) { return null; }
        public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format) { return null; }
        public static void ReleaseTemporary(RenderTexture temp) { }
    }

    public enum RenderTextureFormat { ARGB32, Depth, ARGBHalf, Shadowmap, RGB565, ARGB4444, ARGB1555, Default, ARGB2101010, DefaultHDR, RGBA64, RGB111110Float, RG32, RGB9e5Float, RG16, R8 }

    public enum RenderTextureReadWrite { Default, Linear, sRGB }

    public class Sprite : Object
    {
        public Rect rect { get { return default(Rect); } }
        public Vector2 pivot { get { return Vector2.zero; } }
        public float pixelsPerUnit { get { return 0; } }
        public Vector4 border { get { return Vector4.zero; } }
        public Texture2D texture { get { return null; } }
        public Rect textureRect { get { return default(Rect); } }
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot) { return null; }
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit) { return null; }
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude) { return null; }
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType) { return null; }
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border) { return null; }
    }

    public enum SpriteMeshType { FullRect, Tight }

    public struct Vector4
    {
        public float x, y, z, w;
        public Vector4(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
        public Vector4(float x, float y) { this.x = x; this.y = y; this.z = 0; this.w = 0; }
        public static Vector4 zero { get { return new Vector4(0, 0, 0, 0); } }
        public static Vector4 one { get { return new Vector4(1, 1, 1, 1); } }
        public static Vector4 operator +(Vector4 a, Vector4 b) { return a; }
        public static Vector4 operator -(Vector4 a, Vector4 b) { return a; }
        public static Vector4 operator *(Vector4 a, float d) { return a; }
    }

    public class TextAsset : Object
    {
        public string text { get { return string.Empty; } }
        public byte[] bytes { get { return null; } }
    }

    public static class Resources
    {
        public static T Load<T>(string path) where T : Object { return null; }
        public static Object Load(string path) { return null; }
        public static Object Load(string path, Type type) { return null; }
        public static Object[] LoadAll(string path) { return null; }
        public static Object[] LoadAll(string path, Type type) { return null; }
        public static ResourceRequest LoadAsync(string path, Type type) { return null; }
        public static ResourceRequest LoadAsync(string path) { return null; }
        public static AsyncOperation UnloadUnusedAssets() { return null; }
    }

    public class ResourceRequest : AsyncOperation
    {
        public Object asset { get; set; }
    }

    public enum EventType
    {
        MouseDown, MouseUp, MouseMove, MouseDrag, KeyDown, KeyUp, ScrollWheel,
        Repaint, Layout, DragUpdated, DragPerform, DragExited, ExecuteCommand,
        ValidateCommand, Ignore, Used, MouseEnterWindow, MouseExitWindow,
        ContextClick, MouseUpWindow, MouseDownWindow,
    }

    public class Event
    {
        public static Event current { get; set; }
        public EventType type { get; set; }
        public EventType rawType { get; set; }
        public int button { get; set; }
        public Vector2 mousePosition { get; set; }
        public bool shift { get; set; }
        public bool control { get; set; }
        public bool command { get; set; }
        public bool alt { get; set; }
        public KeyCode keyCode { get; set; }
        public char character { get; set; }
        public int clickCount { get; set; }
        public bool capsLock { get; set; }
        public bool numeric { get; set; }
        public bool functionKey { get; set; }
        public bool isKey { get { return false; } }
        public bool isMouse { get { return false; } }
        public bool isScrollWheel { get { return false; } }
        public void Use() { }
        public Event() { }
        public static Event KeyboardEvent(string key) { return null; }
        public static Event MouseEvent(string key) { return null; }
    }

    public static class Random
    {
        public static float value { get { return 0; } }
        public static float Range(float min, float max) { return min; }
        public static int Range(int min, int max) { return min; }
        public static Vector3 insideUnitSphere { get { return Vector3.zero; } }
        public static Vector2 insideUnitCircle { get { return Vector2.zero; } }
        public static Vector3 onUnitSphere { get { return Vector3.zero; } }
        public static Quaternion rotation { get { return Quaternion.identity; } }
        public static Quaternion rotationUniform { get { return Quaternion.identity; } }
        public static void InitState(int seed) { }
        public static int seed { get; set; }
        public static Color ColorHSV() { return Color.white; }
    }

    public class ScriptableObject : Object
    {
        public static T CreateInstance<T>() where T : ScriptableObject { return null; }
        public static ScriptableObject CreateInstance(Type type) { return null; }
    }

    public class GameObjectUtility { }

    public enum AudioType { UNKNOWN = 0, ACC = 1, AIFF = 2, MPEG = 3, OGGVORBIS = 4, WAV = 5 }

    public class AudioClip : Object
    {
        public float length { get { return 0; } }
        public int channels { get { return 0; } }
        public int frequency { get { return 0; } }
        public float[] data { get { return null; } }
        public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream) { return null; }
    }

    public struct Hash128
    {
        public override string ToString() { return string.Empty; }
        public static Hash128 Parse(string hashString) { return default(Hash128); }
    }

    public interface ISerializationCallbackReceiver
    {
        void OnBeforeSerialize();
        void OnAfterDeserialize();
    }

    public sealed class SerializeField : Attribute
    {
    }

    public class SerializeReference : Attribute
    {
    }

    public class DisallowMultipleComponent : Attribute
    {
    }

    public class RequireComponent : Attribute
    {
        public RequireComponent(Type requiredComponent) { }
    }

    public class ExecuteInEditMode : Attribute
    {
    }

    public class AddComponentMenu : Attribute
    {
        public AddComponentMenu(string menuName) { }
    }

    public class SelectionBaseAttribute : Attribute
    {
    }

    public class HideInInspector : Attribute
    {
    }

    public class TooltipAttribute : Attribute
    {
        public string tooltip;
        public TooltipAttribute(string tooltip) { this.tooltip = tooltip; }
    }

    public class SpaceAttribute : Attribute
    {
        public float height;
        public SpaceAttribute() { }
        public SpaceAttribute(float height) { this.height = height; }
    }

    public class HeaderAttribute : Attribute
    {
        public string header;
        public HeaderAttribute(string header) { this.header = header; }
    }

    public class RangeAttribute : Attribute
    {
        public float min, max;
        public RangeAttribute(float min, float max) { this.min = min; this.max = max; }
    }

    public class MinAttribute : Attribute
    {
        public float min;
        public MinAttribute(float min) { this.min = min; }
    }

    public class SerializeFieldHelper { }

    public class ComputeShader : Object { }
    public class Shader : Object
    {
        public static Shader Find(string name) { return null; }
        public int FindPropertyIndex(string name) { return 0; }
    }

    public class Material : Object
    {
        public Material(Shader shader) { }
        public Shader shader { get; set; }
        public Color color { get; set; }
        public Texture mainTexture { get; set; }
        public void SetColor(string name, Color value) { }
        public void SetFloat(string name, float value) { }
        public void SetInt(string name, int value) { }
        public void SetTexture(string name, Texture value) { }
        public void SetVector(string name, Vector4 value) { }
        public float GetFloat(string name) { return 0; }
        public Color GetColor(string name) { return Color.white; }
    }

    public class Renderer : Component
    {
        public Material material { get; set; }
        public Material sharedMaterial { get; set; }
        public Material[] materials { get; set; }
        public Material[] sharedMaterials { get; set; }
        public bool enabled { get; set; }
        public void SetPropertyBlock(MaterialPropertyBlock properties) { }
    }

    public class MeshRenderer : Renderer { }

    public class SpriteRenderer : Renderer
    {
        public Sprite sprite { get; set; }
        public Color color { get; set; }
    }

    public class MaterialPropertyBlock
    {
        public void SetColor(string name, Color value) { }
        public void SetFloat(string name, float value) { }
        public void SetTexture(string name, Texture value) { }
    }

    public class Camera : Behaviour
    {
        public RenderTexture targetTexture { get; set; }
        public int pixelWidth { get { return 0; } }
        public int pixelHeight { get { return 0; } }
        public float fieldOfView { get; set; }
        public float nearClipPlane { get; set; }
        public float farClipPlane { get; set; }
        public Color backgroundColor { get; set; }
        public CameraClearFlags clearFlags { get; set; }
        public float depth { get; set; }
        public bool orthographic { get; set; }
        public float orthographicSize { get; set; }
        public Rect rect { get { return default(Rect); } }
        public void Render() { }
        public void RenderWithShader(Shader shader, string replacementTag) { }
        public Vector3 ScreenToWorldPoint(Vector3 position) { return position; }
        public Vector3 WorldToScreenPoint(Vector3 position) { return position; }
        public Vector3 ScreenToViewportPoint(Vector3 position) { return position; }
        public Vector3 ViewportToWorldPoint(Vector3 position) { return position; }
        public Ray ScreenPointToRay(Vector3 position) { return default(Ray); }
        public static Camera main { get { return null; } }
        public static Camera current { get { return null; } }
    }

    public enum CameraClearFlags { Skybox = 1, Color = 2, SolidColor = 2, Depth = 3, Nothing = 4 }

    public class Light : Behaviour
    {
        public Color color { get; set; }
        public float intensity { get; set; }
        public LightType type { get; set; }
        public float range { get; set; }
        public float spotAngle { get; set; }
    }

    public enum LightType { Spot, Directional, Point, Area }

    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;
        public Ray(Vector3 origin, Vector3 direction) { this.origin = origin; this.direction = direction; }
        public Vector3 GetPoint(float distance) { return origin + direction * distance; }
    }

    public struct RaycastHit
    {
        public Vector3 point { get; set; }
        public Vector3 normal { get; set; }
        public float distance { get; set; }
        public Transform transform { get { return null; } }
        public Collider collider { get { return null; } }
        public Rigidbody rigidbody { get { return null; } }
    }

    public class Collider : Component
    {
        public bool enabled { get; set; }
        public bool isTrigger { get; set; }
        public Bounds bounds { get { return default(Bounds); } }
    }

    public class BoxCollider : Collider
    {
        public Vector3 center { get; set; }
        public Vector3 size { get; set; }
    }

    public class SphereCollider : Collider
    {
        public Vector3 center { get; set; }
        public float radius { get; set; }
    }

    public class CapsuleCollider : Collider
    {
        public Vector3 center { get; set; }
        public float radius { get; set; }
        public float height { get; set; }
        public int direction { get; set; }
    }

    public class Rigidbody : Component
    {
        public Vector3 velocity { get; set; }
        public Vector3 angularVelocity { get; set; }
        public float mass { get; set; }
        public bool isKinematic { get; set; }
        public bool useGravity { get; set; }
        public void AddForce(Vector3 force) { }
        public void AddForce(Vector3 force, ForceMode mode) { }
        public void AddTorque(Vector3 torque) { }
        public void MovePosition(Vector3 position) { }
    }

    public enum ForceMode { Force, Acceleration, Impulse, VelocityChange }

    public struct Bounds
    {
        public Vector3 center;
        public Vector3 size;
        public Bounds(Vector3 center, Vector3 size) { this.center = center; this.size = size; }
        public Vector3 extents { get { return size * 0.5f; } }
        public Vector3 min { get { return center - extents; } }
        public Vector3 max { get { return center + extents; } }
        public bool Contains(Vector3 point) { return false; }
        public bool Intersects(Bounds bounds) { return false; }
    }

    public static class Physics
    {
        public static bool Raycast(Vector3 origin, Vector3 direction) { return false; }
        public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance) { return false; }
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance) { hitInfo = default(RaycastHit); return false; }
        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance) { hitInfo = default(RaycastHit); return false; }
        public static RaycastHit[] RaycastAll(Ray ray, float maxDistance) { return null; }
        public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo) { hitInfo = default(RaycastHit); return false; }
    }

    public class CustomYieldInstruction : System.Collections.IEnumerator
    {
        public virtual bool keepWaiting { get { return false; } }
        object System.Collections.IEnumerator.Current { get { return null; } }
        bool System.Collections.IEnumerator.MoveNext() { return keepWaiting; }
        void System.Collections.IEnumerator.Reset() { }
    }

    public class WaitForSecondsRealtime : CustomYieldInstruction
    {
        public float waitTime { get; set; }
        public WaitForSecondsRealtime(float time) { waitTime = time; }
        public override bool keepWaiting { get { return false; } }
    }

    public class WWW : CustomYieldInstruction
    {
        public WWW(string url) { }
        public bool isDone { get { return false; } }
        public string error { get { return null; } }
        public string text { get { return string.Empty; } }
        public byte[] bytes { get { return null; } }
        public Texture2D texture { get { return null; } }
        public AudioClip GetAudioClip() { return null; }
        public override bool keepWaiting { get { return !isDone; } }
    }

    public class YieldInstruction
    {
    }

    public class WaitForSeconds : YieldInstruction
    {
        public WaitForSeconds(float seconds) { }
    }

    public class WaitForEndOfFrame : YieldInstruction
    {
    }

    public class WaitUntil : YieldInstruction
    {
        public WaitUntil(Func<bool> predicate) { }
    }

    public class WaitWhile : YieldInstruction
    {
        public WaitWhile(Func<bool> predicate) { }
    }

    public class WaitForFixedUpdate : YieldInstruction
    {
    }

    public class CoroutineHelper { }

    public class GameObjectHelper { }

    public class Physics2D
    {
        public static bool Raycast(Vector2 origin, Vector2 direction) { return false; }
    }

    public class Profiling2 { }

    namespace Profiling
    {
        public static class Profiler
        {
            public static long GetRuntimeMemorySizeLong(Object obj) { return 0; }
            public static void BeginSample(string name) { }
            public static void EndSample() { }
        }
    }

    namespace SceneManagement
    {
        public enum LoadSceneMode { Single, Additive }

        public struct LoadSceneParameters
        {
            public LoadSceneMode loadSceneMode;
            public LoadSceneParameters(LoadSceneMode mode) { loadSceneMode = mode; }
        }

        public struct Scene
        {
            public string name { get; set; }
            public string path { get { return string.Empty; } }
            public int buildIndex { get { return 0; } }
            public bool IsValid() { return false; }
            public bool isLoaded { get { return false; } }
            public bool isDirty { get { return false; } }
            public GameObject[] GetRootGameObjects() { return null; }
            public void GetRootGameObjects(List<GameObject> rootGameObjects) { }
        }

        public static class SceneManager
        {
            public static event Action<Scene, LoadSceneMode> sceneLoaded;
            public static event Action<Scene> sceneUnloaded;
            public static event Action<Scene, LoadSceneMode> activeSceneChanged;
            public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single) { return null; }
            public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single) { return null; }
            public static Scene GetSceneByName(string name) { return default(Scene); }
            public static Scene GetSceneByPath(string scenePath) { return default(Scene); }
            public static Scene GetActiveScene() { return default(Scene); }
            public static bool SetActiveScene(Scene scene) { return false; }
            public static int sceneCount { get { return 0; } }
            public static Scene GetSceneAt(int index) { return default(Scene); }
            public static AsyncOperation UnloadSceneAsync(string sceneName) { return null; }
            public static AsyncOperation UnloadSceneAsync(Scene scene) { return null; }
        }
    }

    namespace Events
    {
        public delegate void UnityAction();
        public delegate void UnityAction<T0>(T0 arg0);
        public delegate void UnityAction<T0, T1>(T0 arg0, T1 arg1);
        public delegate void UnityAction<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);

        public class UnityEvent
        {
            readonly List<UnityAction> listeners = new List<UnityAction>();
            public void AddListener(UnityAction call) { listeners.Add(call); }
            public void RemoveListener(UnityAction call) { listeners.Remove(call); }
            public void RemoveAllListeners() { listeners.Clear(); }
            public void Invoke() { }
        }

        public class UnityEvent<T0> : UnityEvent
        {
            public void AddListener(UnityAction<T0> call) { }
            public void RemoveListener(UnityAction<T0> call) { }
        }

        public class UnityEvent<T0, T1> : UnityEvent
        {
            public void AddListener(UnityAction<T0, T1> call) { }
            public void RemoveListener(UnityAction<T0, T1> call) { }
        }

        public class UnityEvent<T0, T1, T2> : UnityEvent
        {
            public void AddListener(UnityAction<T0, T1, T2> call) { }
            public void RemoveListener(UnityAction<T0, T1, T2> call) { }
        }
    }

    namespace EventSystems
    {
        public class AbstractEventData
        {
            public bool used { get; set; }
            public void Reset() { }
            public void Use() { }
        }

        public class BaseEventData : AbstractEventData
        {
            public BaseEventData(UnityEngine.EventSystems.EventSystem eventSystem) { }
            public UnityEngine.EventSystems.EventSystem currentInputModule { get { return null; } }
            public GameObject selectedObject { get; set; }
        }

        public class EventSystem : Behaviour
        {
            public static EventSystem current { get; set; }
            public GameObject firstSelectedGameObject { get; set; }
            public void SetSelectedGameObject(GameObject selected) { }
        }

        public enum EventTriggerType
        {
            PointerEnter, PointerExit, PointerDown, PointerUp, PointerClick,
            Drag, Drop, Scroll, UpdateSelected, Select, Submit, Cancel,
            Move, BeginDrag, EndDrag, InitializePotentialDrag,
        }

        public class EventTrigger : MonoBehaviour
        {
            public List<Entry> triggers { get; set; }
            public class Entry
            {
                public EventTriggerType eventID;
                public TriggerEvent callback = new TriggerEvent();
            }
            [Serializable]
            public class TriggerEvent : UnityEngine.Events.UnityEvent<BaseEventData>
            {
            }
        }
    }

    namespace UI
    {
        public class UIBehaviour : MonoBehaviour { }

        public class Graphic : UIBehaviour
        {
            public Color color { get; set; }
            public bool raycastTarget { get; set; }
            public RectTransform rectTransform { get { return null; } }
            public Canvas canvas { get { return null; } }
            public void SetVerticesDirty() { }
            public void SetMaterialDirty() { }
        }

        public class MaskableGraphic : Graphic
        {
        }

        public class Image : MaskableGraphic
        {
            public Sprite sprite { get; set; }
            public bool preserveAspect { get; set; }
            public float fillAmount { get; set; }
            public Type type { get; set; }
            public void SetNativeSize() { }
            public enum Type { Simple, Sliced, Tiled, Filled }
        }

        public class Text : MaskableGraphic
        {
            public string text { get; set; }
            public Font font { get; set; }
            public int fontSize { get; set; }
            public FontStyle fontStyle { get; set; }
            public TextAnchor alignment { get; set; }
            public bool supportRichText { get; set; }
            public bool resizeTextForBestFit { get; set; }
            public int resizeTextMinSize { get; set; }
            public int resizeTextMaxSize { get; set; }
            public float preferredWidth { get { return 0; } }
            public float preferredHeight { get { return 0; } }
            public string textContents { get { return text; } }
        }

        public enum FontStyle { Normal, Bold, Italic, BoldAndItalic }

        public class Selectable : UIBehaviour
        {
            public bool interactable { get; set; }
            public Graphic targetGraphic { get; set; }
        }

        public class Button : Selectable
        {
            public ButtonClickedEvent onClick { get; set; }
            public class ButtonClickedEvent : UnityEngine.Events.UnityEvent { }
        }

        public class ScrollRect : UIBehaviour
        {
            public RectTransform content { get; set; }
            public bool horizontal { get; set; }
            public bool vertical { get; set; }
            public float movementType { get; set; }
            public Vector2 normalizedPosition { get; set; }
            public float horizontalNormalizedPosition { get; set; }
            public float verticalNormalizedPosition { get; set; }
            public void StopMovement() { }
        }

        public class Toggle : Selectable
        {
            public bool isOn { get; set; }
            public ToggleEvent onValueChanged { get; set; }
            public class ToggleEvent : UnityEngine.Events.UnityEvent<bool> { }
        }

        public class Slider : Selectable
        {
            public float minValue { get; set; }
            public float maxValue { get; set; }
            public float value { get; set; }
            public SliderEvent onValueChanged { get; set; }
            public class SliderEvent : UnityEngine.Events.UnityEvent<float> { }
        }

        public class RawImage : MaskableGraphic
        {
            public Texture texture { get; set; }
            public Rect uvRect { get; set; }
        }

        public class Mask : UIBehaviour
        {
            public bool showMaskGraphic { get; set; }
        }

        public class RectMask2D : UIBehaviour { }

        public class LayoutGroup : UIBehaviour { }

        public class HorizontalLayoutGroup : LayoutGroup { }

        public class VerticalLayoutGroup : LayoutGroup { }

        public class GridLayoutGroup : LayoutGroup { }

        public class LayoutElement : UIBehaviour
        {
            public float minWidth { get; set; }
            public float minHeight { get; set; }
            public float preferredWidth { get; set; }
            public float preferredHeight { get; set; }
            public float flexibleWidth { get; set; }
            public float flexibleHeight { get; set; }
        }

        public class ContentSizeFitter : UIBehaviour { }

        public class AspectRatioFitter : UIBehaviour { }
    }
}

namespace UnityEditor
{
    using UnityEngine;
    public static class AssetDatabase
    {
        public static string AssetPathToGUID(string path) { return string.Empty; }
        public static string GUIDToAssetPath(string guid) { return string.Empty; }
        public static Object LoadAssetAtPath(string path, Type type) { return null; }
        public static T LoadAssetAtPath<T>(string path) where T : UnityEngine.Object { return null; }
        public static Object LoadMainAssetAtPath(string assetPath) { return null; }
        public static Object[] LoadAllAssetsAtPath(string path) { return null; }
        public static Object[] LoadAllAssetRepresentationsAtPath(string assetPath) { return null; }
        public static Texture2D GetCachedIcon(string path) { return null; }
        public static string[] GetDependencies(string path, bool recursive) { return null; }
        public static void Refresh() { }
        public static void Refresh(ImportAssetOptions options) { }
        public static string[] FindAssets(string filter) { return null; }
        public static string[] FindAssets(string filter, string[] searchInFolders) { return null; }
        public static void RemoveUnusedAssetBundleNames() { }
        public static void RemoveAssetBundleName(string assetBundleName, bool forceRemove) { }
        public static string GetAssetPath(Object obj) { return string.Empty; }
        public static string GetAssetPath(int instanceID) { return string.Empty; }
        public static bool IsValidFolder(string path) { return false; }
        public static string CreateFolder(string parentFolder, string newFolderName) { return string.Empty; }
        public static bool CreateAsset(Object asset, string path) { return false; }
        public static void SaveAssets() { }
        public static bool DeleteAsset(string path) { return false; }
        public static string MoveAsset(string oldPath, string newPath) { return string.Empty; }
        public static string[] GetSubFolders(string path) { return null; }
        public static string GenerateUniqueAssetPath(string path) { return path; }
        public static void ImportAsset(string path) { }
        public static void SaveAssetIfDirty(Object target) { }
        public static bool IsForeignAsset(Object obj) { return false; }
        public static bool IsMainAsset(Object obj) { return false; }
        public static bool IsSubAsset(Object obj) { return false; }
        public static bool Contains(Object obj) { return false; }
        public static void AddObjectToAsset(Object objectToAdd, Object assetObject) { }
        public static void SetLabels(Object obj, string[] labels) { }
        public static string[] GetLabels(Object obj) { return null; }
        public static string[] GetAllAssetBundleNames() { return null; }
        public static string[] GetAssetBundleDependencies(string assetBundleName, bool recursive) { return null; }
        public static void StartAssetEditing() { }
        public static void StopAssetEditing() { }
        public static string GetAssetOrScenePath(Object assetObject) { return string.Empty; }
        public static int GetAssetPathAndName(Object obj) { return 0; }
    }

    [Flags]
    public enum ImportAssetOptions
    {
        Default = 0, ForceUpdate = 1, ForceSynchronousImport = 8, ImportRecursive = 256,
        DontDownloadFromCacheServer = 8192, ForceUncompressedImport = 16384,
    }

    public class AssetImporter
    {
        public string assetBundleName { get; set; }
        public string assetBundleVariant { get; set; }
        public string assetPath { get { return string.Empty; } }
        public static AssetImporter GetAtPath(string path) { return null; }
        public void SaveAndReimport() { }
        public static void CreateAsset(string path) { }
    }

    public static class EditorUtility
    {
        public static void DisplayProgressBar(string title, string info, float progress) { }
        public static void ClearProgressBar() { }
        public static void SetDirty(Object target) { }
        public static void FocusProjectWindow() { }
        public static string OpenFolderPanel(string title, string folder, string defaultName) { return string.Empty; }
        public static string SaveFilePanel(string title, string directory, string defaultName, string extension) { return string.Empty; }
        public static string OpenFilePanel(string title, string directory, string extension) { return string.Empty; }
        public static void RevealInFinder(string path) { }
        public static bool DisplayDialog(string title, string message, string ok) { return false; }
        public static bool DisplayDialog(string title, string message, string ok, string cancel) { return false; }
    }

    public static class EditorGUIUtility
    {
        public static void PingObject(Object target) { }
        public static void PingObject(int targetInstanceID) { }
        public static Texture2D FindTexture(string name) { return null; }
        public static GUIContent TextContent(string name) { return null; }
        public static GUIContent IconContent(string name) { return null; }
        public static Texture2D whiteTexture { get { return null; } }
        public static int GetControlID(int hint, FocusType focusType) { return 0; }
        public static void AddCursorRect(Rect position, MouseCursor cursor) { }
        public static void AddCursorRect(Rect position, MouseCursor cursor, int controlID) { }
        public static float currentViewWidth { get { return 0; } }
        public static float singleLineHeight { get { return 0; } }
        public static float standardVerticalSpacing { get { return 0; } }
        public static bool isProSkin { get { return false; } }
    }

    public enum MouseCursor
    {
        Arrow, Text, ResizeVertical, ResizeHorizontal, Link, SlideArrow,
        ResizeUpRight, ResizeUpLeft, MoveArrow, VerticalSplit, HorizontalSplit,
        ArrowMinus, ArrowPlus, Pan, Orbit, Zoom, FPS, CustomCursor,
    }

    public static class Selection
    {
        public static Object activeObject { get; set; }
        public static Object[] objects { get; set; }
        public static string[] assetGUIDs { get; set; }
        public static int activeInstanceID { get; set; }
        public static int[] instanceIDs { get; set; }
        public static int count { get { return 0; } }
    }

    public class EditorBuildSettingsScene
    {
        public string path { get; set; }
        public bool enabled { get; set; }
        public EditorBuildSettingsScene() { }
        public EditorBuildSettingsScene(string path, bool enabled) { this.path = path; this.enabled = enabled; }
    }

    public static class EditorBuildSettings
    {
        public static EditorBuildSettingsScene[] scenes { get; set; }
        public static bool TryGetConfigObject<T>(string name, out T result) { result = default(T); return false; }
        public static void SetConfigObject(string name, UnityEngine.Object obj) { }
        public static void RemoveConfigObject(string name) { }
    }

    public static class EditorUserBuildSettings
    {
        public static BuildTarget activeBuildTarget { get { return BuildTarget.StandaloneWindows64; } }
        public static string activeBuildTargetName { get { return string.Empty; } }
        public static BuildTargetGroup selectedBuildTargetGroup { get { return BuildTargetGroup.Standalone; } }
        public static string GetBuildLocation(BuildTargetGroup targetGroup, BuildTarget target) { return string.Empty; }
        public static void SetBuildLocation(BuildTarget target, string location) { }
    }

    public static class PlayerSettings
    {
        public static string productName { get; set; }
        public static string bundleVersion { get; set; }
        public static string applicationIdentifier { get; set; }
        public static string companyName { get; set; }
        public static string GetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup) { return string.Empty; }
        public static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup, string defines) { }
        public static string[] GetScriptingDefineSymbolsForGroup2(BuildTargetGroup targetGroup) { return null; }
    }

    public enum BuildTargetGroup
    {
        Unknown = 0, Standalone = 1, WebGL = 13, iOS = 4, Android = 7,
    }

    public class AssetBundleManifest : UnityEngine.Object
    {
        public UnityEngine.Hash128 GetAssetBundleHash(string assetBundleName) { return default(UnityEngine.Hash128); }
        public string[] GetAllAssetBundles() { return null; }
        public string[] GetAllAssetBundlesWithVariant() { return null; }
        public string[] GetDirectDependencies(string assetBundleName) { return null; }
        public string[] GetAllDependencies(string assetBundleName) { return null; }
    }

    public enum BuildTarget
    {
        StandaloneWindows = 5, StandaloneWindows64 = 19, iOS = 9, Android = 13, WebGL = 20, NoTarget = -2,
        StandaloneOSX = 2, StandaloneLinux = 17, StandaloneLinux64 = 24,
        PS4 = 31, XboxOne = 21, Switch = 38,
    }

    [Flags]
    public enum BuildAssetBundleOptions
    {
        None = 0, UncompressedAssetBundle = 1, CollectDependencies = 2,
        DeterministicAssetBundle = 4, ForceRebuildAssetBundle = 8,
        IgnoreTypeTreeChanges = 16, AppendHashToAssetBundleName = 32,
        ChunkBasedCompression = 64, StrictMode = 128, DryRunBuild = 256,
        DisableWriteTypeTree = 512,
    }

    public static class BuildPipeline
    {
        public static AssetBundleManifest BuildAssetBundles(string outputPath, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform) { return null; }
        public static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform) { return null; }
    }

    public struct AssetBundleBuild
    {
        public string assetBundleName;
        public string assetBundleVariant;
        public string[] assetNames;
        public string[] addressableNames;
    }

    public class EditorWindow : ScriptableObject
    {
        public GUIContent titleContent;
        public Vector2 minSize;
        public Vector2 maxSize;
        public Rect position { get; set; }
        public bool autoRepaintOnSceneChange { get; set; }
        public static EditorWindow focusedWindow { get { return null; } }
        public static EditorWindow mouseOverWindow { get { return null; } }
        public void Close() { }
        public void Repaint() { }
        public void Show() { }
        public void ShowUtility() { }
        public void ShowPopup() { }
        public void Focus() { }
        public void ShowNotification(GUIContent content) { }
        public void RemoveNotification() { }
        public void OnInspectorUpdate() { }
        public static T GetWindow<T>(string title, bool utility) where T : EditorWindow { return null; }
        public static T GetWindow<T>() where T : EditorWindow { return null; }
        public static T GetWindow<T>(string title) where T : EditorWindow { return null; }
        public static T GetWindow<T>(bool utility, string title) where T : EditorWindow { return null; }
    }

    public class MenuItem : Attribute
    {
        public MenuItem(string itemName) { }
        public MenuItem(string itemName, bool isValidateFunction) { }
        public MenuItem(string itemName, bool isValidateFunction, int priority) { }
    }

    public class InitializeOnLoadAttribute : Attribute
    {
    }

    public class InitializeOnLoadMethodAttribute : Attribute
    {
    }

    public enum PlayModeStateChange
    {
        EnteredEditMode, ExitingEditMode, EnteredPlayMode, ExitingPlayMode,
    }

    public class GenericMenu
    {
        public delegate void MenuFunction();
        public delegate void MenuFunction2(object userData);
        public void AddItem(GUIContent content, bool on, MenuFunction func) { }
        public void AddItem(GUIContent content, bool on, MenuFunction2 func, object userData) { }
        public void AddDisabledItem(GUIContent content) { }
        public void AddDisabledItem(GUIContent content, bool on) { }
        public void AddSeparator(string path) { }
        public void DropDown(Rect position) { }
        public void ShowAsContext() { }
        public void ShowContext() { }
        public int GetItemCount() { return 0; }
        public void AllowDuplicateNativeMenuItems() { }
    }

    public static class DragAndDrop
    {
        public static UnityEngine.Object[] objectReferences { get; set; }
        public static string[] paths { get; set; }
        public static DragAndDropVisualMode visualMode { get; set; }
        public static int activeControlID { get; set; }
        public static void PrepareStartDrag() { }
        public static void StartDrag(string title) { }
        public static void AcceptDrag() { }
        public static void RejectDrag() { }
    }

    public enum DragAndDropVisualMode
    {
        None, Copy, Move, Link, Rejected, Generic,
    }

    public class CreateAssetMenuAttribute : Attribute
    {
        public string fileName;
        public string menuName;
        public int order;
    }

    public static class EditorGUILayout
    {
        public static Vector2 BeginScrollView(Vector2 scrollPosition) { return scrollPosition; }
        public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical) { return scrollPosition; }
        public static void EndScrollView() { }
        public static void BeginVertical(params GUILayoutOption[] options) { }
        public static void BeginVertical(string text, params GUILayoutOption[] options) { }
        public static void BeginVertical(GUIStyle style, params GUILayoutOption[] options) { }
        public static void EndVertical() { }
        public static void BeginHorizontal(params GUILayoutOption[] options) { }
        public static void BeginHorizontal(string text, params GUILayoutOption[] options) { }
        public static void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options) { }
        public static void EndHorizontal() { }
        public static void BeginFadeGroup(float value) { }
        public static void EndFadeGroup() { }
        public static void LabelField(string label) { }
        public static void LabelField(string label, params GUILayoutOption[] options) { }
        public static void LabelField(string label, string label2) { }
        public static void LabelField(string label, string label2, params GUILayoutOption[] options) { }
        public static void LabelField(string label, GUIStyle style, params GUILayoutOption[] options) { }
        public static void LabelField(GUIContent label, params GUILayoutOption[] options) { }
        public static void LabelField(GUIContent label, GUIStyle style, params GUILayoutOption[] options) { }
        public static string TextField(string label, string text) { return text; }
        public static string TextField(string text, params GUILayoutOption[] options) { return text; }
        public static string TextField(string text, GUIStyle style, params GUILayoutOption[] options) { return text; }
        public static string TextField(GUIContent label, string text, params GUILayoutOption[] options) { return text; }
        public static string TextField(GUIContent label, string text, GUIStyle style, params GUILayoutOption[] options) { return text; }
        public static string PasswordField(string label, string password) { return password; }
        public static string TextArea(string text, params GUILayoutOption[] options) { return text; }
        public static string TextArea(string text, GUIStyle style, params GUILayoutOption[] options) { return text; }
        public static bool Toggle(string label, bool value) { return value; }
        public static bool Toggle(string label, bool value, GUIStyle style) { return value; }
        public static bool Toggle(bool value, params GUILayoutOption[] options) { return value; }
        public static bool Toggle(GUIContent label, bool value) { return value; }
        public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick) { return foldout; }
        public static bool Foldout(bool foldout, string content) { return foldout; }
        public static bool Foldout(bool foldout, string content, GUIStyle style) { return foldout; }
        public static bool Foldout(bool foldout, GUIContent content) { return foldout; }
        public static bool Foldout(bool foldout, GUIContent content, GUIStyle style) { return foldout; }
        public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick) { return foldout; }
        public static int Popup(string label, int selectedIndex, string[] displayedOptions) { return selectedIndex; }
        public static int Popup(int selectedIndex, string[] displayedOptions, params GUILayoutOption[] options) { return selectedIndex; }
        public static int Popup(int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options) { return selectedIndex; }
        public static int Popup(string label, int selectedIndex, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options) { return selectedIndex; }
        public static T ObjectField<T>(string label, T obj, bool allowSceneObjects) where T : UnityEngine.Object { return obj; }
        public static T ObjectField<T>(T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : UnityEngine.Object { return obj; }
        public static UnityEngine.Object ObjectField(string label, UnityEngine.Object obj, System.Type objType, bool allowSceneObjects) { return obj; }
        public static UnityEngine.Object ObjectField(UnityEngine.Object obj, System.Type objType, bool allowSceneObjects, params GUILayoutOption[] options) { return obj; }
        public static bool Button(string text, params GUILayoutOption[] options) { return false; }
        public static bool Button(GUIContent content, params GUILayoutOption[] options) { return false; }
        public static bool Button(string text, GUIStyle style, params GUILayoutOption[] options) { return false; }
        public static void Space() { }
        public static void Space(float pixels) { }
        public static void HelpBox(string message, MessageType type) { }
        public static void HelpBox(string message, MessageType type, bool wide) { }
        public static void Separator() { }
        public static int IntField(string label, int value) { return value; }
        public static int IntField(string label, int value, params GUILayoutOption[] options) { return value; }
        public static int IntField(int value, params GUILayoutOption[] options) { return value; }
        public static float FloatField(string label, float value) { return value; }
        public static float FloatField(float value, params GUILayoutOption[] options) { return value; }
        public static long LongField(string label, long value) { return value; }
        public static System.Enum EnumPopup(string label, System.Enum selected) { return selected; }
        public static System.Enum EnumPopup(System.Enum selected, params GUILayoutOption[] options) { return selected; }
        public static System.Enum EnumFlagsField(string label, System.Enum selected) { return selected; }
        public static System.Enum EnumFlagsField(string label, System.Enum selected, params GUILayoutOption[] options) { return selected; }
        public static void BeginDisabledGroup(bool disabled) { }
        public static void EndDisabledGroup() { }
        public static void Indent() { }
        public static void Indent(int indent) { }
        public static void Unindent() { }
        public static void Unindent(int indent) { }
        public static bool ToggleLeft(string label, bool value, params GUILayoutOption[] options) { return value; }
        public static bool ToggleLeft(GUIContent label, bool value, params GUILayoutOption[] options) { return value; }
        public static void SelectableLabel(string label, params GUILayoutOption[] options) { }
        public static string DelayedTextField(string label, string text) { return text; }
        public static Vector2 Vector2Field(string label, Vector2 value) { return value; }
        public static Vector3 Vector3Field(string label, Vector3 value) { return value; }
        public static Color ColorField(string label, Color value) { return value; }
        public static void PropertyField(SerializedProperty property, params GUILayoutOption[] options) { }
        public static void PropertyField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options) { }
        public static Rect GetControlRect(bool hasLabel, float height) { return default(Rect); }
        public static void DrawRect(Rect rect, Color color) { }
        public class HorizontalScope : GUI.Scope
        {
            public Rect rect { get { return default(Rect); } }
            public HorizontalScope(params GUILayoutOption[] options) { }
            public HorizontalScope(GUIStyle style, params GUILayoutOption[] options) { }
        }
        public class VerticalScope : GUI.Scope
        {
            public Rect rect { get { return default(Rect); } }
            public VerticalScope(params GUILayoutOption[] options) { }
            public VerticalScope(GUIStyle style, params GUILayoutOption[] options) { }
        }
        public class ScrollViewScope : GUI.Scope
        {
            public Vector2 scrollPosition { get; set; }
            public bool handleScrollWheel { get; set; }
            public ScrollViewScope(Vector2 scrollPosition, params GUILayoutOption[] options) { }
            public ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options) { }
        }
    }

    public enum MessageType { None, Info, Warning, Error }

    public static class GUILayout
    {
        public static bool Button(string text, params GUILayoutOption[] options) { return false; }
        public static bool Button(string text, GUIStyle style, params GUILayoutOption[] options) { return false; }
        public static bool Button(GUIContent content, params GUILayoutOption[] options) { return false; }
        public static bool Button(GUIContent content, GUIStyle style, params GUILayoutOption[] options) { return false; }
        public static bool Button(Texture image, params GUILayoutOption[] options) { return false; }
        public static bool Button(Texture image, GUIStyle style, params GUILayoutOption[] options) { return false; }
        public static void Label(string text, params GUILayoutOption[] options) { }
        public static void Label(GUIContent content, params GUILayoutOption[] options) { }
        public static void Label(string text, GUIStyle style, params GUILayoutOption[] options) { }
        public static void Label(GUIContent content, GUIStyle style, params GUILayoutOption[] options) { }
        public static void Space(float pixels) { }
        public static void FlexibleSpace() { }
        public static void BeginHorizontal(params GUILayoutOption[] options) { }
        public static void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options) { }
        public static void EndHorizontal() { }
        public static void BeginVertical(params GUILayoutOption[] options) { }
        public static void BeginVertical(GUIStyle style, params GUILayoutOption[] options) { }
        public static void EndVertical() { }
        public static void Box(string text, params GUILayoutOption[] options) { }
        public static void Box(GUIContent content, params GUILayoutOption[] options) { }
        public static string TextField(string text, params GUILayoutOption[] options) { return text; }
        public static bool Toggle(bool value, params GUILayoutOption[] options) { return value; }
        public static int Toolbar(int selected, string[] texts, params GUILayoutOption[] options) { return selected; }
        public static int Toolbar(int selected, string[] texts, GUIStyle style, params GUILayoutOption[] options) { return selected; }
        public static int Toolbar(int selected, GUIContent[] contents, params GUILayoutOption[] options) { return selected; }
        public static int Toolbar(int selected, GUIContent[] contents, GUIStyle style, params GUILayoutOption[] options) { return selected; }
        public static int SelectionGrid(int selected, string[] texts, int xCount, params GUILayoutOption[] options) { return selected; }
        public static GUILayoutOption Width(float width) { return null; }
        public static GUILayoutOption Height(float height) { return null; }
        public static GUILayoutOption MinWidth(float minWidth) { return null; }
        public static GUILayoutOption MaxWidth(float maxWidth) { return null; }
        public static GUILayoutOption ExpandWidth(bool expand) { return null; }
        public static GUILayoutOption ExpandHeight(bool expand) { return null; }
        public static GUILayoutOption MinHeight(float minHeight) { return null; }
        public static GUILayoutOption MaxHeight(float maxHeight) { return null; }
    }

    public static class EditorApplication
    {
        public static double timeSinceStartup { get { return 0; } }
        public static bool isPlaying { get; set; }
        public static bool isPlayingOrWillChangePlaymode { get; set; }
        public static bool isCompiling { get { return false; } }
        public static bool isUpdating { get { return false; } }
        public static bool isPaused { get; set; }
        public static string applicationContentsPath { get { return string.Empty; } }
        public static string applicationPath { get { return string.Empty; } }
        public static event Action<PlayModeStateChange> playModeStateChanged;
        public static event Action update;
        public static void RepaintProjectWindow() { }
        public static void RepaintHierarchyWindow() { }
        public static void DirtyHierarchyWindow() { }
        public static void Step() { }
        public static void LockReloadAssemblies() { }
        public static void UnlockReloadAssemblies() { }
        public static void Exit(int returnValue) { }
    }

    public static class EditorGUI
    {
        public static bool Toggle(Rect position, bool value) { return value; }
        public static bool Toggle(Rect position, string label, bool value) { return value; }
        public static bool Foldout(Rect position, bool foldout, string content) { return foldout; }
        public static bool Foldout(Rect position, bool foldout, string content, bool toggleOnLabelClick) { return foldout; }
        public static bool Foldout(Rect position, bool foldout, GUIContent content, bool toggleOnLabelClick) { return foldout; }
        public static string TextField(Rect position, string text) { return text; }
        public static string TextField(Rect position, string label, string text) { return text; }
        public static string TextField(Rect position, GUIContent label, string text) { return text; }
        public static void LabelField(Rect position, string label) { }
        public static void LabelField(Rect position, string label, string label2) { }
        public static void LabelField(Rect position, GUIContent label, GUIContent label2) { }
        public static int Popup(Rect position, int selectedIndex, string[] displayedOptions) { return selectedIndex; }
        public static int Popup(Rect position, string label, int selectedIndex, string[] displayedOptions) { return selectedIndex; }
        public static int IntField(Rect position, int value) { return value; }
        public static int IntField(Rect position, string label, int value) { return value; }
        public static float FloatField(Rect position, float value) { return value; }
        public static float FloatField(Rect position, string label, float value) { return value; }
        public static bool Button(Rect position, string text) { return false; }
        public static bool Button(Rect position, GUIContent content) { return false; }
        public static void BeginDisabledGroup(bool disabled) { }
        public static void EndDisabledGroup() { }
        public static int indentLevel;
        public static void Separator(Rect position) { }
        public static void Space(Rect position) { }
        public static void DrawTextureAlpha(Rect position, Texture image) { }
        public static void DrawTextureTransparent(Rect position, Texture image) { }
        public static Color ColorField(Rect position, Color value) { return value; }
        public static Vector2 Vector2Field(Rect position, Vector2 value) { return value; }
        public static Vector3 Vector3Field(Rect position, Vector3 value) { return value; }
        public static void PropertyField(Rect position, SerializedProperty property) { }
        public static void PropertyField(Rect position, SerializedProperty property, GUIContent label) { }
        public static float GetPropertyHeight(SerializedProperty property) { return 0; }
        public static string TextArea(Rect position, string text) { return text; }
        public static bool HelpBox(Rect position, string message, MessageType type) { return false; }
    }

    public static class EditorStyles
    {
        public static GUIStyle boldLabel { get { return null; } }
        public static GUIStyle label { get { return null; } }
        public static GUIStyle box { get { return null; } }
        public static GUIStyle toolbar { get { return null; } }
        public static GUIStyle toolbarButton { get { return null; } }
        public static GUIStyle toolbarPopup { get { return null; } }
        public static GUIStyle whiteLabel { get { return null; } }
        public static GUIStyle miniButton { get { return null; } }
        public static GUIStyle textField { get { return null; } }
        public static GUIStyle helpBox { get { return null; } }
        public static GUIStyle foldout { get { return null; } }
        public static GUIStyle foldoutHeader { get { return null; } }
        public static GUIStyle toggle { get { return null; } }
        public static GUIStyle centeredLabel { get { return null; } }
    }

    public class SerializedObject
    {
        public SerializedObject(UnityEngine.Object obj) { }
        public SerializedProperty FindProperty(string propertyPath) { return null; }
        public void Update() { }
        public void ApplyModifiedProperties() { }
        public void ApplyModifiedPropertiesWithoutUndo() { }
        public void SetIsDifferentCacheDirty() { }
        public UnityEngine.Object targetObject { get { return null; } }
        public UnityEngine.Object[] targetObjects { get { return null; } }
        public bool isEditingMultipleObjects { get { return false; } }
    }

    public class SerializedProperty
    {
        public string name { get { return string.Empty; } }
        public string propertyPath { get { return string.Empty; } }
        public string type { get { return string.Empty; } }
        public string stringValue { get; set; }
        public int intValue { get; set; }
        public long longValue { get; set; }
        public float floatValue { get; set; }
        public double doubleValue { get; set; }
        public bool boolValue { get; set; }
        public Color colorValue { get; set; }
        public UnityEngine.Object objectReferenceValue { get; set; }
        public int objectReferenceInstanceIDValue { get; set; }
        public int enumValueIndex { get; set; }
        public string enumNames { get { return string.Empty; } }
        public SerializedPropertyType propertyType { get { return SerializedPropertyType.Generic; } }
        public int arraySize { get; set; }
        public bool isArray { get { return false; } }
        public bool hasChildren { get { return false; } }
        public bool hasVisibleChildren { get { return false; } }
        public int depth { get { return 0; } }
        public SerializedProperty GetArrayElementAtIndex(int index) { return null; }
        public void InsertArrayElementAtIndex(int index) { }
        public void DeleteArrayElementAtIndex(int index) { }
        public void ClearArray() { }
        public SerializedProperty Copy() { return null; }
        public bool Next(bool enterChildren) { return false; }
        public bool NextVisible(bool enterChildren) { return false; }
        public void Reset() { }
        public SerializedProperty FindPropertyRelative(string relativePropertyPath) { return null; }
        public float[] vector3Value { get; set; }
    }

    public enum SerializedPropertyType
    {
        Generic, Integer, Boolean, Float, String, Color, ObjectReference, LayerMask,
        Enum, Vector2, Vector3, Vector4, Rect, ArraySize, Character, AnimationCurve,
        Bounds, Gradient, Quaternion, ExposedReference, FixedBufferSize, Vector2Int,
        Vector3Int, RectInt, BoundsInt, ManagedReference,
    }

    public class PropertyDrawer : Attribute
    {
    }

    public class CustomPropertyDrawer : Attribute
    {
        public CustomPropertyDrawer(Type type) { }
    }

    public class CustomEditor : Attribute
    {
        public CustomEditor(Type type) { }
        public CustomEditor(Type type, bool editorForChildClasses) { }
    }

    public class Editor : ScriptableObject
    {
        public SerializedObject serializedObject { get { return null; } }
        public UnityEngine.Object target { get { return null; } }
        public UnityEngine.Object[] targets { get { return null; } }
        public virtual void OnInspectorGUI() { }
        public virtual void OnEnable() { }
        public static void CreateEditor(UnityEngine.Object targetObject) { }
    }

    public static class EditorGUILayoutUtility { }

    public class EditorGUILayoutEx { }

    namespace SceneManagement
    {
        public static class EditorSceneManager
        {
            public static UnityEngine.SceneManagement.Scene LoadSceneInPlayMode(string path, UnityEngine.SceneManagement.LoadSceneParameters parameters)
            {
                return default(UnityEngine.SceneManagement.Scene);
            }
            public static UnityEngine.AsyncOperation LoadSceneAsyncInPlayMode(string path, UnityEngine.SceneManagement.LoadSceneParameters parameters)
            {
                return null;
            }
            public static bool OpenScene(string scenePath) { return false; }
            public static bool SaveScene(UnityEngine.SceneManagement.Scene scene, string dstScenePath) { return false; }
        }
    }

    public class SceneView : EditorWindow
    {
        public Camera camera { get { return null; } }
        public static SceneView currentDrawingSceneView { get { return null; } }
        public static SceneView lastActiveSceneView { get { return null; } }
    }

    public static class HandleUtility
    {
        public static Vector2 GUIPointToScreenPixelCoordinate(Vector2 guiPoint) { return guiPoint; }
        public static float GetHandleSize(Vector3 position) { return 0; }
    }

    public static class Handles
    {
        public static Color color { get; set; }
        public static void BeginGUI() { }
        public static void EndGUI() { }
        public static void DrawLine(Vector3 p1, Vector3 p2) { }
        public static void DrawWireCube(Vector3 center, Vector3 size) { }
        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius) { }
    }

    public class SceneAsset : UnityEngine.Object { }

    public class DefaultAsset : UnityEngine.Object { }

    public class AssetModificationProcessor
    {
    }

    public class MonoBehaviourEditor { }

    public class PreviewRenderUtility { }

    namespace IMGUI
    {
        namespace Controls
        {
            public class TreeViewState
            {
                public Vector2 scrollPos;
                public List<int> expandedIDs = new List<int>();
                public List<int> selectedIDs = new List<int>();
                public string searchString;
                public float customColumnWidth;
                public int lastClickedID;
            }

            public class TreeViewItem
            {
                public TreeViewItem() { }
                public TreeViewItem(int id, int depth, string displayName) { this.id = id; this.depth = depth; this.displayName = displayName; }
                public int id { get; set; }
                public int depth { get; set; }
                public string displayName { get; set; }
                public Texture2D icon { get; set; }
                public Texture2D overlayIcon { get; set; }
                public TreeViewItem parent { get; set; }
                public List<TreeViewItem> children { get; set; }
                public bool hasChildren { get { return children != null && children.Count > 0; } }
                public void AddChild(TreeViewItem child)
                {
                    if (children == null)
                        children = new List<TreeViewItem>();
                    children.Add(child);
                    child.parent = this;
                }
            }

            public class TreeView
            {
                public TreeView(TreeViewState state) { this.state = state; }
                public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) { this.state = state; this.multiColumnHeader = multiColumnHeader; }
                public TreeViewState state { get; protected set; }
                protected TreeViewItem rootItem { get; set; }
                public Rect treeViewRect { get; protected set; }
                public bool showAlternatingRowBackgrounds { get; set; }
                public bool showBorder { get; set; }
                public bool showScrollbar { get; set; }
                public string searchString { get; set; }
                public MultiColumnHeader multiColumnHeader { get; set; }
                public float rowHeight { get; set; }
                public float topRowPadding { get; set; }
                public float bottomRowPadding { get; set; }
                public void Reload() { }
                public void Repaint() { }
                public void SetSelection(IList<int> selectedIDs) { }
                public void SetSelection(IList<int> selectedIDs, TreeViewSelectionOptions options) { }
                public IList<int> GetSelection() { return null; }
                public TreeViewItem FindItem(int id, TreeViewItem searchFromThisItem) { return null; }
                public IList<TreeViewItem> FindRows(IList<int> ids) { return null; }
                public void SetupParentsAndChildrenFromDepths(TreeViewItem root, List<TreeViewItem> rows) { }
                public void BeginRename(TreeViewItem item) { }
                public void BeginRename(TreeViewItem item, float delay) { }
                public Rect GetRowRect(int row) { return default(Rect); }
                public int GetRowCount() { return 0; }
                public System.Collections.Generic.IEnumerable<TreeViewItem> GetRows() { return null; }
                public void FrameItem(int id) { }
                public void SetFocus() { }
                public void SetFocusAndEnsureSelectedItem() { }
                public bool HasFocus() { return false; }
                public void ExpandAll() { }
                public void CollapseAll() { }
                public void ExpandItem(int id) { }
                public void SetExpanded(int id, bool expanded) { }
                public bool IsExpanded(int id) { return false; }
                public bool IsSelected(int id) { return false; }
                public void OnGUI(Rect rect) { }
                public bool IsRenaming() { return false; }
                public bool isDragging { get { return false; } }
                protected virtual TreeViewItem BuildRoot() { return null; }
                protected virtual IList<TreeViewItem> BuildRows(TreeViewItem root) { return null; }
                protected virtual void RowGUI(RowGUIArgs args) { }
                protected virtual bool CanRename(TreeViewItem item) { return false; }
                protected virtual void RenameEnded(RenameEndedArgs args) { }
                protected virtual Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item) { return rowRect; }
                protected virtual void ContextClickedItem(int id) { }
                protected virtual void ContextClicked() { }
                protected virtual void DoubleClickedItem(int id) { }
                protected virtual void SingleClickedItem(int id) { }
                protected virtual void SelectionChanged(IList<int> selectedIds) { }
                protected virtual void KeyEvent() { }
                protected virtual void ExpandedStateChanged() { }
                protected virtual bool CanStartDrag(CanStartDragArgs args) { return false; }
                protected virtual void SetupDragAndDrop(SetupDragAndDropArgs args) { }
                protected virtual DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args) { return DragAndDropVisualMode.None; }
                protected virtual void OnCreateRowGUI(RowGUIArgs rowGUIArgs) { }
                protected virtual bool CanMultiSelect(TreeViewItem item) { return true; }
                protected virtual bool IsRenamingItemAllowed(TreeViewItem item) { return false; }
                protected virtual void CommandEvent() { }
                protected virtual void SearchChanged(string newSearch) { }
                protected virtual void ItemBeginDrag() { }
                protected virtual void ItemEndDrag() { }

                public class RenameEndedArgs
                {
                    public int itemID { get; set; }
                    public string originalName { get; set; }
                    public string newName { get; set; }
                    public bool acceptedRename { get; set; }
                    public bool isRenaming { get; set; }
                }

                public class RowGUIArgs
                {
                    public TreeViewItem item { get; set; }
                    public Rect rowRect { get; set; }
                    public int row { get; set; }
                    public bool isSelected { get; set; }
                    public bool isFocused { get; set; }
                    public bool isRenaming { get; set; }
                    public bool isRepainting { get; set; }
                    public bool isDropTarget { get; set; }
                    public bool selected { get { return isSelected; } }
                    public bool focused { get { return isFocused; } }
                    public Rect GetCellRect(int columnIndex) { return rowRect; }
                    public int GetNumVisibleColumns() { return 1; }
                    public int GetColumn(int columnIndex) { return columnIndex; }
                    public IList<int> GetColumns() { return new List<int>(); }
                    public float GetContentIndent(TreeViewItem item) { return 0; }
                    public Rect GetRowRect(int row) { return rowRect; }
                }

                public class CanStartDragArgs
                {
                    public TreeViewItem item;
                    public List<TreeViewItem> draggedItems;
                }

                public class SetupDragAndDropArgs
                {
                    public List<TreeViewItem> draggedItems;
                    public int insertAtIndex;
                }

                public class DragAndDropArgs
                {
                    public DragAndDropVisualMode dragAndDropVisualMode;
                    public int insertAtIndex;
                    public TreeViewItem parentItem;
                }

                public class DefaultGUI
                {
                    public static void Label(Rect rect, string label, bool selected, bool focused) { }
                    public static void LabelRightAligned(Rect rect, string label, bool selected, bool focused) { }
                    public static void IconLabel(Rect rect, string label, Texture2D icon, bool selected, bool focused) { }
                    public static void Background(Rect rect, bool selected, bool focused) { }
                    public static void FoldoutLabel(Rect rect, string label, bool selected, bool focused) { }
                    public static void TreeViewBoldLabel(Rect rect, string label) { }
                }
            }

            public enum TreeViewSelectionOptions
            {
                None = 0,
                FireSelectionChanged = 1,
                RevealAndFrame = 2,
            }

            public class MultiColumnHeaderState
            {
                public MultiColumnHeaderState(Column[] columns) { this.columns = columns; }
                public Column[] columns { get; set; }
                public int[] sortedColumns { get { return null; } }
                public class Column
                {
                    public GUIContent headerContent { get; set; }
                    public float width { get; set; }
                    public float minWidth { get; set; }
                    public float maxWidth { get; set; }
                    public bool canSort { get; set; }
                    public bool sortedAscending { get; set; }
                    public int sortedColumnIndex { get; set; }
                    public TextAlignment headerTextAlignment { get; set; }
                    public TextAlignment sortingArrowAlignment { get; set; }
                    public bool allowToggleVisibility { get; set; }
                    public bool autoResize { get; set; }
                }
            }

            public class MultiColumnHeader
            {
                public MultiColumnHeader(MultiColumnHeaderState state) { this.state = state; }
                public MultiColumnHeaderState state { get; set; }
                public float height { get; set; }
                public float minColumnWidth { get; set; }
                public event Action<MultiColumnHeader> sortingChanged;
                public void ResizeToFit() { }
                public void OnGUI(Rect rect, float xScroll) { }
                public Rect GetCellRect(int visibleColumnIndex, Rect rowRect) { return rowRect; }
                public int GetColumn(int visibleColumnIndex) { return visibleColumnIndex; }
                public int GetVisibleColumnCount() { return 0; }
                public IList<int> GetVisibleColumns() { return null; }
                public bool IsColumnVisible(int columnIndex) { return true; }
                public void SetSorting(int columnIndex, bool ascending) { }
                public void SortColumns() { }
                public void SetSortDirection(int columnIndex, bool ascending) { }
                public void SetSortingColumns(IList<int> sortColumns, IList<bool> sortAscending) { }
                public void SetHeight(float height) { }
                public bool IsSortedAscending(int columnIndex) { return false; }
            }

            public class SearchField
            {
                public string OnGUI(Rect rect, string text, GUIStyle style, GUIStyle cancelButtonStyle, GUIStyle cancelButtonEmptyStyle) { return text; }
                public string OnGUI(Rect rect, string text) { return text; }
                public string OnToolbarGUI(string text, params GUILayoutOption[] options) { return text; }
                public string OnToolbarGUI(Rect rect, string text) { return text; }
                public void SetFocus() { }
                public bool HasFocus() { return false; }
                public void ClearFocus() { }
                public int searchFieldControlID { get; set; }
                public event Action downOrUpArrowKeyPressed;
            }
        }
    }
}

namespace UnityEditorInternal
{
    using UnityEngine;
    using UnityEditor;
    public class ReorderableList
    {
        public delegate void ElementCallbackDelegate(Rect rect, int index, bool isActive, bool isFocused);
        public delegate void HeaderCallbackDelegate(Rect rect);
        public delegate void AddCallbackDelegate(ReorderableList list);
        public delegate void RemoveCallbackDelegate(ReorderableList list);
        public delegate void ChangedCallbackDelegate(ReorderableList list);
        public delegate void CanChangeCallbackDelegate(ReorderableList list);

        public ReorderableList(System.Collections.IList elements, Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) { }
        public ReorderableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) { }
        public ElementCallbackDelegate drawElementCallback;
        public ElementCallbackDelegate drawElementBackgroundCallback;
        public HeaderCallbackDelegate drawHeaderCallback;
        public AddCallbackDelegate onAddCallback;
        public AddCallbackDelegate onAddDropdownCallback;
        public RemoveCallbackDelegate onRemoveCallback;
        public ChangedCallbackDelegate onChangedCallback;
        public ChangedCallbackDelegate onReorderCallback;
        public CanChangeCallbackDelegate onCanAddCallback;
        public CanChangeCallbackDelegate onCanRemoveCallback;
        public float elementHeight { get; set; }
        public float headerHeight { get; set; }
        public float footerHeight { get; set; }
        public float elementHeightCallback { get; set; }
        public int count { get { return 0; } }
        public System.Collections.IList list { get; set; }
        public SerializedObject serializedObject { get; set; }
        public SerializedProperty serializedProperty { get; set; }
        public bool showDefaultBackground { get; set; }
        public bool draggable { get; set; }
        public bool displayAdd { get; set; }
        public bool displayRemove { get; set; }
        public bool displayHeader { get; set; }
        public void DoLayout() { }
        public void DoLayoutList() { }
        public void DoList(Rect rect) { }
        public void DrawHeader(Rect rect) { }
        public void DrawElement(Rect rect, int index, bool selected, bool focused) { }
        public void DrawFooter(Rect rect) { }
        public void DrawElementBackground(Rect rect, int index, bool selected, bool focused) { }
        public void ShowDropdown(Rect rect, int index) { }
        public void Select(int index) { }
        public int index { get; set; }
        public void ClearSelection() { }
        public bool HasKeyboardControl() { return false; }
    }
}

namespace UnityEngine.Networking
{
    public class DownloadHandler : IDisposable
    {
        public string text { get { return string.Empty; } }
        public byte[] data { get { return null; } }
        public void Dispose() { }
        protected virtual byte[] GetData() { return null; }
        protected virtual string GetText() { return string.Empty; }
    }

    public class DownloadHandlerBuffer : DownloadHandler { }

    public class DownloadHandlerTexture : DownloadHandler
    {
        public UnityEngine.Texture2D texture { get { return null; } }
        public static UnityEngine.Texture2D GetContent(UnityWebRequest www) { return null; }
    }

    public class DownloadHandlerAudioClip : DownloadHandler
    {
        public UnityEngine.AudioClip audioClip { get { return null; } }
        public static UnityEngine.AudioClip GetContent(UnityWebRequest www) { return null; }
    }

    public class DownloadHandlerAssetBundle : DownloadHandler
    {
        public UnityEngine.AssetBundle assetBundle { get { return null; } }
        public static UnityEngine.AssetBundle GetContent(UnityWebRequest www) { return null; }
    }

    public class DownloadHandlerFile : DownloadHandler
    {
        public DownloadHandlerFile(string path) { }
        public DownloadHandlerFile(string path, bool append) { }
        public bool removeFileOnAbort { get; set; }
    }

    public class UploadHandler : IDisposable
    {
        public byte[] data { get; set; }
        public string contentType { get; set; }
        public void Dispose() { }
    }

    public class UploadHandlerRaw : UploadHandler
    {
        public UploadHandlerRaw(byte[] data) { }
    }

    public class UnityWebRequestAsyncOperation : UnityEngine.AsyncOperation
    {
        public UnityWebRequest webRequest { get; set; }
    }

    public class UnityWebRequest : IDisposable
    {
        public const string kHttpVerbGET = "GET";
        public const string kHttpVerbPOST = "POST";
        public const string kHttpVerbPUT = "PUT";
        public const string kHttpVerbHEAD = "HEAD";

        public enum Result
        {
            InProgress = 0,
            Success = 1,
            ConnectionError = 2,
            ProtocolError = 3,
            DataProcessingError = 4,
        }

        public string url { get; set; }
        public string method { get; set; }
        public DownloadHandler downloadHandler { get; set; }
        public UploadHandler uploadHandler { get; set; }
        public string error { get; set; }
        public bool isDone { get; set; }
        public float downloadProgress { get; set; }
        public float uploadProgress { get; set; }
        public bool isNetworkError { get; set; }
        public bool isHttpError { get; set; }
        public Result result { get; set; }
        public long responseCode { get; set; }
        public bool disposeDownloadHandlerOnDispose { get; set; }
        public bool disposeUploadHandlerOnDispose { get; set; }
        public int timeout { get; set; }
        public int redirectLimit { get; set; }
        public ulong downloadedBytes { get; set; }
        public ulong uploadProgressBytes { get; set; }

        public UnityWebRequest() { }
        public UnityWebRequest(string url) { this.url = url; }
        public UnityWebRequest(string url, string method) { this.url = url; this.method = method; }

        public UnityWebRequestAsyncOperation SendWebRequest() { return null; }
        public void Dispose() { }
        public void Abort() { }
        public string GetRequestHeader(string name) { return null; }
        public string GetResponseHeader(string name) { return null; }
        public void SetRequestHeader(string name, string value) { }
        public System.Collections.Generic.Dictionary<string, string> GetResponseHeaders() { return null; }

        public static UnityWebRequest Get(string url) { return new UnityWebRequest(url, kHttpVerbGET); }
        public static UnityWebRequest Post(string url, string postData) { return new UnityWebRequest(url, kHttpVerbPOST); }
        public static UnityWebRequest Put(string url, byte[] bodyData) { return new UnityWebRequest(url, kHttpVerbPUT); }
        public static UnityWebRequest Put(string url, string bodyData) { return new UnityWebRequest(url, kHttpVerbPUT); }
        public static UnityWebRequest Head(string url) { return new UnityWebRequest(url, kHttpVerbHEAD); }
    }

    public static class UnityWebRequestTexture
    {
        public static UnityWebRequest GetTexture(string url) { return new UnityWebRequest(url); }
    }

    public static class UnityWebRequestMultimedia
    {
        public static UnityWebRequest GetAudioClip(string url, UnityEngine.AudioType audioType) { return new UnityWebRequest(url); }
    }

    public static class UnityWebRequestAssetBundle
    {
        public static UnityWebRequest GetAssetBundle(string url) { return new UnityWebRequest(url); }
    }
}

namespace Cosmos.Unity.EditorCoroutines.Editor
{
    using System.Collections;
    public class EditorCoroutine
    {
        public static EditorCoroutine Start(IEnumerator routine) { return null; }
    }

    public static class EditorCoroutineUtility
    {
        public static EditorCoroutine StartCoroutineOwnerless(IEnumerator routine) { return null; }
        public static void StopCoroutine(EditorCoroutine coroutine) { }
    }
}
