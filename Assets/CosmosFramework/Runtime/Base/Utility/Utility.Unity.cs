using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;

namespace Cosmos
{
    public static partial class Utility
    {
        /// <summary>
        /// 这个类封装了所有跟Unity相关的工具函数，是所有Utiltiy中需要引入UnityEngine的类
        /// </summary>
        public static class Unity
        {
            static ICoroutineHelper coroutineHelper;
            static ICoroutineHelper CoroutineHelper
            {
                get
                {
                    if (coroutineHelper == null)
                    {
                        var go = new GameObject("CoroutineHelper");
                        go.hideFlags = HideFlags.HideInHierarchy;
                        GameObject.DontDestroyOnLoad(go);
                        coroutineHelper = go.AddComponent<CoroutineHelper>();
                    }
                    return coroutineHelper;
                }
            }
            /// <summary>
            /// PlayerPrefs持久化前缀
            /// </summary>
            public const string Perfix = "Cosmos";
            /// <summary>
            /// 持久化数据层路径，可写入
            /// </summary>
            public static readonly string PathURL =
#if UNITY_ANDROID
        "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
        Application.dataPath + "/Raw/";  
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
        "file://" + Application.dataPath + "/StreamingAssets/";
#else
        string.Empty;  
#endif
            public static int Random(int min, int max)
            {
                return UnityEngine.Random.Range(min, max);
            }
            public static float Random(float min, float max)
            {
                return UnityEngine.Random.Range(min, max);
            }

            /// <summary>
            /// 是否约等于另一个浮点数
            /// </summary>
            public static bool Approximately(float sourceValue, float targetValue)
            {
                return Mathf.Approximately(sourceValue, targetValue);
            }
            /// <summary>
            /// 限制一个向量在最大值与最小值之间
            /// </summary>
            public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
            {
                value.x = Mathf.Clamp(value.x, min.x, max.x);
                value.y = Mathf.Clamp(value.y, min.y, max.y);
                value.z = Mathf.Clamp(value.z, min.z, max.z);
                return value;
            }
            public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
            {
                value.x = Mathf.Clamp(value.x, min.x, max.x);
                value.y = Mathf.Clamp(value.y, min.y, max.y);
                return value;
            }
            /// <summary>
            /// 获得固定位数小数的向量
            /// </summary>
            public static Vector3 Round(Vector3 value, int decimals)
            {
                value.x = (float)Math.Round(value.x, decimals);
                value.y = (float)Math.Round(value.y, decimals);
                value.z = (float)Math.Round(value.z, decimals);
                return value;
            }
            /// <summary>
            /// 限制一个向量在最大值与最小值之间
            /// </summary>
            public static Vector3 Clamp(Vector3 value, float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
            {
                value.x = Mathf.Clamp(value.x, minX, maxX);
                value.y = Mathf.Clamp(value.y, minY, maxY);
                value.z = Mathf.Clamp(value.z, minZ, maxZ);
                return value;
            }
            public static Vector2 Clamp(Vector2 value, float minX, float minY, float maxX, float maxY)
            {
                value.x = Mathf.Clamp(value.x, minX, maxX);
                value.y = Mathf.Clamp(value.y, minY, maxY);
                return value;
            }
            /// <summary>
            /// 获得固定位数小数的向量
            /// </summary>
            public static Vector2 Round(Vector2 value, int decimals)
            {
                value.x = (float)Math.Round(value.x, decimals);
                value.y = (float)Math.Round(value.y, decimals);
                return value;
            }
            public static T Get<T>(Transform go) where T : Component
            {
                return go.GetComponent<T>();
            }
            /// <summary>
            /// 添加目标组件；默认不移除组件;
            /// 若removeExistComp为true，则移除已经存在的，并重新赋予；
            /// </summary>
            /// <typeparam name="T">目标组件</typeparam>
            /// <param name="go">目标对象</param>
            /// <param name="removeExistComp">是否移除已经存在的组件</param>
            /// <returns>返回添加的目标组件</returns>
            public static T Add<T>(GameObject go) where T : Component
            {
                return go.GetOrAddComponent<T>();
            }
            public static Component Add(Type type, GameObject go)
            {
                if (!typeof(Component).IsAssignableFrom(type))
                {
                    throw new NotImplementedException($"Type :{type} is not iherit from Component !");
                }
                if(go==null)
                    throw new ArgumentNullException($"GameObject is invalid !");
                return go.GetOrAddComponent(type);
            }
            /// <summary>
            /// 添加目标组件；默认不移除组件;
            /// 若removeExistComp为true，则移除已经存在的，并重新赋予；
            /// </summary>
            /// <typeparam name="T">目标组件</typeparam>
            /// <param name="go">目标对象</param>
            /// <returns>返回添加的目标组件</returns>
            public static T Add<T>(Transform go) where T : Component
            {
                return Add<T>(go.gameObject);
            }
            public static T Add<T>(Transform go, string subNode) where T : Component
            {
                var childGo = Child(go, subNode);
                var comp = Add<T>(childGo);
                return comp;
            }
            /// <summary>
            /// 实例化对象；
            /// 默认不移除原本就存在的T组件对象，若默认参数未true，则会移除本就存在的组件，再重新添加；
            /// </summary>
            /// <typeparam name="T">mono脚本</typeparam>
            /// <param name="spawnItem">生成的对象</param>
            /// <returns>返回生成成功后的组件对象</returns>
            public static T Instantiate<T>(GameObject spawnItem) where T : Component
            {
                if (spawnItem == null)
                    throw new ArgumentNullException("ObjectSpawner : spawnItem not exist !");
                var go = GameObject.Instantiate(spawnItem);
                return Add<T>(go);
            }
            /// <summary>
            /// 删除父节点下的子对象；
            /// </summary>
            /// <param name="go">目标对象</param>
            public static void DeleteChilds(Transform go)
            {
                var childCount = go.childCount;
                if (childCount == 0)
                    return;
                for (int i = 0; i < childCount; i++)
                {
                    GameObject.Destroy(go.GetChild(i).gameObject);
                }
            }
            public static GameObject Child(Transform go, string subNode)
            {
                var trans = go.GetComponentsInChildren<Transform>();
                var length = trans.Length;
                for (int i = 1; i < length; i++)
                {
                    if (trans[i].name.Equals(subNode))
                        return trans[i].gameObject;
                }
                return null;
            }
            /// <summary>
            /// 查找所有符合名称的子节点
            /// </summary>
            /// <param name="go">目标对象</param>
            /// <param name="subNode">子级别目标对象名称</param>
            /// <returns>名字符合的对象数组</returns>
            public static GameObject[] Childs(Transform go, string subNode)
            {
                var trans = go.GetComponentsInChildren<Transform>();
                List<GameObject> subGos = new List<GameObject>();
                var length = trans.Length;
                for (int i = 0; i < length; i++)
                {
                    if (trans[i].name.Contains(subNode))
                    {
                        subGos.Add(trans[i].gameObject);
                    }
                }
                return subGos.ToArray();
            }
            public static GameObject Child(GameObject go, string subNode)
            {
                return Child(go.transform, subNode);
            }
            /// <summary>
            /// 查找目标场景中的目标对象
            /// </summary>
            /// <param name="sceneName">传入的场景名</param>
            /// <param name="predicate">查找条件</param>
            /// <returns>查找到的对象</returns>
            public static GameObject FindSceneGameObject(string sceneName, Func<GameObject, bool> predicate)
            {
                var scene = SceneManager.GetSceneByName(sceneName);
                return scene.GetRootGameObjects().FirstOrDefault(predicate);
            }
            /// <summary>
            /// 场景是否被加载；
            /// </summary>
            /// <param name="sceneName">场景名</param>
            /// <returns>是否被加载</returns>
            public static bool IsSceneLoaded(string sceneName)
            {
                var scene = SceneManager.GetSceneByName(sceneName);
                if (scene != null)
                {
                    return scene.isLoaded;
                }
                return false;
            }
            /// <summary>
            /// 查找同级别
            /// </summary>
            /// <param name="go">同级别当前对象</param>
            /// <param name="subNode">同级别目标对象名称</param>
            /// <returns>查找到的目标对象</returns>
            public static GameObject Peer(Transform go, string subNode)
            {
                Transform tran = go.parent.Find(subNode);
                if (tran != null)
                    return tran.gameObject;
                else
                    return null;
            }
            /// <summary>
            /// 查找同级别其他对象；
            /// 略耗性能；
            /// </summary>
            /// <param name="go">同级别当前对象</param>
            /// <returns>当前级别下除此对象的其他同级的对象</returns>
            public static GameObject[] Peers(Transform go)
            {
                Transform parentTrans = go.parent;
                var childTrans = parentTrans.GetComponentsInChildren<Transform>();
                var length = childTrans.Length;
                List<GameObject> peersGo = new List<GameObject>();
                if (length > 0)
                {
                    for (int i = 0; i < length; i++)
                    {
                        if (childTrans[i].parent == parentTrans && childTrans[i] != go)
                        {
                            peersGo.Add(childTrans[i].gameObject);
                        }
                    }
                }
                return peersGo.Count > 0 ? peersGo.ToArray() : null;
            }
            /// <summary>
            /// 查找同级别下所有目标组件；
            /// 略耗性能；
            /// </summary>
            /// <typeparam name="T">目标组件</typeparam>
            /// <param name="go">同级别当前对象</param>
            /// <returns>当前级别下除此对象的其他同级的对象组件</returns>
            public static T[] PeersComponet<T>(Transform go) where T : Component
            {
                Transform parentTrans = go.parent;
                var childTrans = parentTrans.GetComponentsInChildren<Transform>();
                var length = childTrans.Length;
                List<T> peerComps = new List<T>();
                if (length > 0)
                {
                    for (int i = 0; i < length; i++)
                    {
                        if (childTrans[i].parent == parentTrans && childTrans[i] != go)
                        {
                            var comps = childTrans[i].GetComponents<T>();
                            if (comps != null)
                                peerComps.AddRange(comps);
                        }
                    }
                }
                return peerComps.Count > 0 ? peerComps.ToArray() : null;
            }
            /// <summary>
            /// 对unity对象进行升序排序
            /// </summary>
            /// <typeparam name="T">组件类型</typeparam>
            /// <typeparam name="K">排序的值</typeparam>
            /// <param name="comps">传入的组件数组</param>
            /// <param name="handler">处理的方法</param>
            public static void SortCompsByAscending<T, K>(T[] comps, Func<T, K> handler)
                where K : IComparable<K>
                where T : Component
            {
                Utility.Algorithm.SortByAscend(comps, handler);
                var length = comps.Length;
                for (int i = 0; i < length; i++)
                {
                    comps[i].transform.SetSiblingIndex(i);
                }
            }
            /// <summary>
            /// 对unity对象进行降序排序
            /// </summary>
            /// <typeparam name="T">组件类型</typeparam>
            /// <typeparam name="K">排序的值</typeparam>
            /// <param name="comps">传入的组件数组</param>
            /// <param name="handler">处理的方法</param>
            public static void SortCompsByDescending<T, K>(T[] comps, Func<T, K> handler)
        where K : IComparable<K>
        where T : Component
            {
                Utility.Algorithm.SortByDescend(comps, handler);
                var length = comps.Length;
                for (int i = 0; i < length; i++)
                {
                    comps[i].transform.SetSiblingIndex(i);
                }
            }
            public static GameObject[] Peers(GameObject go)
            {
                return Peers(go.transform);
            }
            /// <summary>
            /// 查找同级别
            /// </summary>
            /// <param name="go">同级别当前对象</param>
            /// <param name="subNode">同级别目标对象名称</param>
            /// <returns></returns>
            public static GameObject Peer(GameObject go, string subNode)
            {
                return Peer(go.transform, subNode);
            }
            public static GameObject Parent(Transform go, string parentNode)
            {
                var par = go.GetComponentsInParent<Transform>();
                return Algorithm.Find(par, p => p.gameObject.name == parentNode).gameObject;
            }
            public static GameObject Parent(GameObject go, string parentNode)
            {
                return Parent(go.transform, parentNode);
            }
            /// <summary>
            /// PlayerPrefs 是否存在key，否则报错显示信息
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static bool HasPrefsKey(string key)
            {
                if (PlayerPrefs.HasKey(GetPrefsKey(key)))
                    return true;
                else
                {
                    Debug.LogError("PlayerPrefs key " + key + "  not exist!");
                    return false;
                }
            }
            /// <summary>
            /// PlayerPrefs key
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static string GetPrefsKey(string key)
            {
                Utility.Text.ClearStringBuilder();
                return Utility.Text.Format(Perfix + "_" + key);
            }
            public static int GetPrefsInt(string key)
            {
                return PlayerPrefs.GetInt(GetPrefsKey(key));
            }
            public static string GetPrefsString(string key)
            {
                return PlayerPrefs.GetString(GetPrefsKey(key));
            }
            public static float GetPrefsFloat(string key)
            {
                return PlayerPrefs.GetFloat(GetPrefsKey(key));
            }
            public static void SetPrefsString(string key, string value)
            {
                string fullKey = GetPrefsKey(key);
                PlayerPrefs.DeleteKey(fullKey);
                PlayerPrefs.SetString(fullKey, value);
            }
            public static void SetPrefsInt(string key, int value)
            {
                string fullKey = GetPrefsKey(key);
                PlayerPrefs.DeleteKey(fullKey);
                PlayerPrefs.SetInt(fullKey, value);
            }
            public static void SetPrefsFloat(string key, int value)
            {
                string fullKey = GetPrefsKey(key);
                PlayerPrefs.DeleteKey(fullKey);
                PlayerPrefs.SetFloat(fullKey, value);
            }
            public static void RemovePrefsData(string key)
            {
                string fullKey = GetPrefsKey(key);
                PlayerPrefs.DeleteKey(fullKey);
            }
            public static void RemoveAllPrefsData()
            {
                PlayerPrefs.DeleteAll();
            }
            public static bool IsWifi { get { return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork; } }
            public static bool NetAvailable { get { return Application.internetReachability != NetworkReachability.NotReachable; } }
            /// <summary>
            /// Unity方法；
            /// 合并地址,返回Unity的绝对路径；
            /// 当前环境为：UNITY_STANDALONE_WIN；
            /// 跨平台地址未编写；
            /// </summary>
            /// <param name="relativePath">相对路径</param>
            /// <returns>返回绝对路径</returns>
            public static string CombineAppAbsolutePath(params string[] relativePath)
            {
                Utility.Text.ClearStringBuilder();
                for (int i = 0; i < relativePath.Length; i++)
                {
                    Utility.Text.StringBuilderCache.Append(relativePath[i] + "/");
                }
                return Application.dataPath + "/" + Utility.Text.StringBuilderCache.ToString();
            }
            /// <summary>
            /// Unity方法；
            ///  合并地址,返回持久化数据的绝对路径；
            /// 跨平台地址未编写；
            /// </summary>
            /// <param name="relativePath">相对路径</param>
            /// <returns>返回绝对路径</returns>
            public static string CombineAppPersistentPath(params string[] relativePath)
            {
                Utility.Text.ClearStringBuilder();
                for (int i = 0; i < relativePath.Length; i++)
                {
                    Utility.Text.StringBuilderCache.Append(relativePath[i] + "/");
                }
                return Application.persistentDataPath + "/" + Utility.Text.StringBuilderCache.ToString();
            }
            /// <summary>
            /// Unity方法；
            /// 合并地址,返回绝对路径；
            /// 当前环境为：UNITY_STANDALONE_WIN；
            /// 跨平台地址未编写；
            /// </summary>
            /// <param name="fileFullName">文件的完整名称（包括文件扩展名）</param>
            /// <param name="relativePath">相对路径</param>
            /// <returns></returns>
            // TODO 跨平台地址未编写
            public static string CombineAppAbsoluteFilePath(string fileFullName, params string[] relativePath)
            {
                return CombineAppAbsolutePath(relativePath) + fileFullName;
            }
            /// <summary>
            ///  Unity方法；
            /// 分解App绝对路径，返回相对路径；
            /// </summary>
            /// <param name="absolutePath">绝对路径</param>
            /// <returns>相对路径</returns>
            public static string DecomposeAppAbsolutePath(string absolutePath)
            {
                return absolutePath.Remove(0, Application.dataPath.Length);
            }
            /// <summary>
            /// 保存截屏到持久化路径；
            /// </summary>
            /// <param name="fileName">文件名，需要包含后缀</param>
            public static void CaptureScreen2PersistentDataPath(string fileName)
            {
                var fullPath = Path.Combine(Application.persistentDataPath, fileName);
                ScreenCapture.CaptureScreenshot(fullPath);
            }
            /// <summary>
            /// 保存截屏；
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <param name="fileName">文件名，需要包含后缀</param>
            public static void CaptureScreen(string filePath, string fileName)
            {
                var fullPath = Path.Combine(filePath, fileName);
                ScreenCapture.CaptureScreenshot(fullPath);
            }
            /// <summary>
            /// 通过相机截取屏幕
            /// </summary>
            /// <param name="camera">目标相机</param>
            /// <param name="fileName">文件的完整名，路径/文件名.后缀名</param>
            public static void CaptureScreenshotByCamera(Camera camera, string fileName)
            {
                var oldRenderTexture = camera.targetTexture;
                RenderTexture renderTexture;
                renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                camera.targetTexture = renderTexture;
                camera.Render();
                Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
                RenderTexture.active = renderTexture;
                texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                texture2D.Apply();
                byte[] bytes = texture2D.EncodeToPNG();
                texture2D.Compress(false);
                texture2D.Apply();
                RenderTexture.active = null;
                camera.targetTexture = oldRenderTexture;
                var fullPath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
                System.IO.File.WriteAllBytes(fullPath, bytes);
            }
            /// <summary>
            /// 通过相机截取屏幕并转换为Texture2D
            /// </summary>
            /// <param name="camera">目标相机</param>
            /// <returns>相机抓取的屏幕Texture2D</returns>
            public static Texture2D CaptureScreenshotByCameraAsTexture(Camera camera)
            {
                var oldRenderTexture = camera.targetTexture;
                RenderTexture renderTexture;
                renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                camera.targetTexture = renderTexture;
                camera.Render();
                Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
                RenderTexture.active = renderTexture;
                texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                texture2D.Apply();
                RenderTexture.active = null;
                camera.targetTexture = oldRenderTexture;
                return texture2D;
            }
            /// <summary>
            /// 通过相机截取屏幕并转换为Sprite
            /// </summary>
            /// <param name="camera">目标相机</param>
            /// <returns>相机抓取的屏幕Texture2D</returns>
            public static Sprite CaptureScreenshotByCameraAsSprite(Camera camera)
            {
                var texture2D = CaptureScreenshotByCameraAsTexture(camera);
                var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                return sprite;
            }

            #region  Coroutine
            public static Coroutine StartCoroutine(Coroutine routine, Action callBack)
            {
                return CoroutineHelper.StartCoroutine(routine, callBack);
            }
            public static Coroutine StartCoroutine(IEnumerator routine)
            {
                return CoroutineHelper.StartCoroutine(routine);
            }
            public static Coroutine StartCoroutine(Action handler)
            {
                return CoroutineHelper.StartCoroutine(handler);
            }
            public static Coroutine StartCoroutine(Action handler,Action callback)
            {
                return CoroutineHelper.StartCoroutine(handler,callback);
            }
            /// <summary>
            /// 延时协程；
            /// </summary>
            /// <param name="delay">延时的时间</param>
            /// <param name="callBack">延时后的回调函数</param>
            /// <returns>协程对象</returns>
            public static Coroutine DelayCoroutine(float delay, Action callBack)
            {
                return CoroutineHelper.DelayCoroutine(delay, callBack);
            }
            /// <summary>
            /// 条件协程；
            /// </summary>
            /// <param name="handler">目标条件</param>
            /// <param name="callBack">条件达成后执行的回调</param>
            /// <returns>协程对象</returns>
            public static Coroutine PredicateCoroutine(Func<bool> handler, Action callBack)
            {
                return CoroutineHelper.PredicateCoroutine(handler, callBack);
            }
            /// <summary>
            /// 嵌套协程；
            /// </summary>
            /// <param name="predicateHandler">条件函数</param>
            /// <param name="nestHandler">条件成功后执行的嵌套协程</param>
            /// <returns>Coroutine></returns>
            public static Coroutine PredicateNestCoroutine(Func<bool> predicateHandler, Action nestHandler)
            {
                return CoroutineHelper.PredicateNestCoroutine(predicateHandler, nestHandler);
            }
            public static void StopAllCoroutines()
            {
                CoroutineHelper.StopAllCoroutines();
            }
            public static void StopCoroutine(IEnumerator routine)
            {
                CoroutineHelper.StopCoroutine(routine);
            }
            public static void StopCoroutine(Coroutine routine)
            {
                CoroutineHelper.StopCoroutine(routine);
            }
            #endregion
        }
    }
}