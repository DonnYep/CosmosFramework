using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Video;

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
            public static readonly string StreamingAssetsPathURL =
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
                if (go == null)
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
            /// 判断是否是路径；
            /// 需要注意根目录下的文件可能不带/或\符号！
            /// </summary>
            /// <param name="path">路径str</param>
            /// <returns>是否是路径</returns>
            public static bool IsPath(string path)
            {
                return path.Contains("\\") || path.Contains("/");
            }

            #region CaptureScreenshot
            /// <summary>
            /// 通过相机截取屏幕并转换为Texture2D
            /// </summary>
            /// <param name="camera">目标相机</param>
            /// <returns>相机抓取的屏幕Texture2D</returns>
            public static Texture2D CameraScreenshotAsTextureRGB(Camera camera)
            {
                return CameraScreenshotAsTexture(camera, TextureFormat.RGB565);
            }
            public static Texture2D CameraScreenshotAsTextureRGBA(Camera camera)
            {
                return CameraScreenshotAsTexture(camera, TextureFormat.RGBA32);
            }
            public static Texture2D CameraScreenshotAsTexture(Camera camera, TextureFormat textureFormat)
            {
                var oldRenderTexture = camera.targetTexture;
                RenderTexture renderTexture;
                renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
                camera.targetTexture = renderTexture;
                camera.Render();
                Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, textureFormat, false);
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
            public static Sprite CameraScreenshotAsSpriteRGBA(Camera camera)
            {
                var texture2D = CameraScreenshotAsTextureRGBA(camera);
                var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                return sprite;
            }
            public static Sprite CameraScreenshotAsSpriteRGB(Camera camera)
            {
                var texture2D = CameraScreenshotAsTextureRGB(camera);
                var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                return sprite;
            }
            public static Sprite CameraScreenshotAsSprite(Camera camera, TextureFormat textureFormat)
            {
                var texture2D = CameraScreenshotAsTexture(camera, textureFormat);
                var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                return sprite;
            }
            public static Texture2D BytesToTexture2D(byte[] bytes, int width, int height)
            {
                Texture2D texture2D = new Texture2D(width,height);
                texture2D.LoadImage(bytes);
                return texture2D;
            }
            #endregion

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
            public static Coroutine StartCoroutine(Action handler, Action callback)
            {
                return CoroutineHelper.StartCoroutine(handler, callback);
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

            #region UnityWebRequest
            public static Coroutine DownloadTextAsync(string url, Action<float> progress, Action<string> downloadedCallback)
            {
                return Utility.Unity.StartCoroutine(EnumUnityWebRequest(UnityWebRequest.Get(url), progress, (UnityWebRequest req) =>
                 {
                     downloadedCallback?.Invoke(req.downloadHandler.text);
                 }));
            }
            public static Coroutine DownloadTextsAsync(string[] urls, Action<float> overallProgress, Action<float> progress, Action<string[]> downloadedCallback)
            {
                var length = urls.Length;
                var requests = new List<UnityWebRequest>();
                for (int i = 0; i < length; i++)
                {
                    requests.Add(UnityWebRequest.Get(urls[i]));
                }
                return Utility.Unity.StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
                {
                    var reqLength = reqs.Length;
                    var texts = new string[reqLength];
                    for (int i = 0; i < reqLength; i++)
                    {
                        texts[i] = reqs[i].downloadHandler.text;
                    }
                    downloadedCallback?.Invoke(texts);
                }));
            }
            public static Coroutine DownloadTextureAsync(string url, Action<float> progress, Action<Texture2D> downloadedCallback)
            {
                return Utility.Unity.StartCoroutine(EnumUnityWebRequest(UnityWebRequestTexture.GetTexture(url), progress, (UnityWebRequest req) =>
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(req);
                    //var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    downloadedCallback?.Invoke(texture);
                }));
            }
            public static Coroutine DownloadTexturesAsync(string[] urls, Action<float> overallProgress, Action<float> progress, Action<Texture2D[]> downloadedCallback)
            {
                var length = urls.Length;
                var requests = new List<UnityWebRequest>();
                for (int i = 0; i < length; i++)
                {
                    requests.Add(UnityWebRequestTexture.GetTexture(urls[i]));
                }
                return Utility.Unity.StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
                {
                    var reqLength = reqs.Length;
                    var textures = new Texture2D[reqLength];
                    for (int i = 0; i < reqLength; i++)
                    {
                        textures[i] = DownloadHandlerTexture.GetContent(reqs[i]);
                    }
                    downloadedCallback?.Invoke(textures);
                }));
            }
            public static Coroutine DownloadAudioAsync(string url, AudioType audioType, Action<float> progress, Action<AudioClip> downloadedCallback)
            {
                return Utility.Unity.StartCoroutine(EnumUnityWebRequest(UnityWebRequestMultimedia.GetAudioClip(url, audioType), progress, (UnityWebRequest req) =>
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                    downloadedCallback?.Invoke(clip);
                }));
            }
            public static Coroutine DownloadAudiosAsync(IDictionary<string, AudioType> urlDict, Action<float> overallProgress, Action<float> progress, Action<AudioClip[]> downloadedCallback)
            {
                var length = urlDict.Count;
                var requests = new List<UnityWebRequest>();
                foreach (var url in urlDict)
                {
                    requests.Add(UnityWebRequestMultimedia.GetAudioClip(url.Key, url.Value));
                }
                return Utility.Unity.StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
                {
                    var reqLength = reqs.Length;
                    var audios = new AudioClip[reqLength];
                    for (int i = 0; i < reqLength; i++)
                    {
                        audios[i] = DownloadHandlerAudioClip.GetContent(reqs[i]);
                    }
                    downloadedCallback?.Invoke(audios);
                }));
            }
            public static Coroutine DownloadAssetBundleAsync(string url, Action<float> progress, Action<AssetBundle> downloadedCallback)
            {
                return Utility.Unity.StartCoroutine(EnumUnityWebRequest(UnityWebRequestAssetBundle.GetAssetBundle(url), progress, (UnityWebRequest req) =>
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
                    if (bundle)
                    {
                        downloadedCallback?.Invoke(bundle);
                    }
                }));
            }
            public static Coroutine DownloadAssetBundlesAsync(string[] urls, Action<float> overallProgress, Action<float> progress, Action<AssetBundle[]> downloadedCallback)
            {
                var length = urls.Length;
                var requests = new List<UnityWebRequest>();
                for (int i = 0; i < length; i++)
                {
                    requests.Add(UnityWebRequestAssetBundle.GetAssetBundle(urls[i]));
                }
                return Utility.Unity.StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
                {
                    var reqLength = reqs.Length;
                    var assetbundles = new AssetBundle[reqLength];
                    for (int i = 0; i < reqLength; i++)
                    {
                        assetbundles[i] = DownloadHandlerAssetBundle.GetContent(reqs[i]);
                    }
                    downloadedCallback?.Invoke(assetbundles);
                }));
            }
            public static Coroutine DownloadAssetBundleBytesAsync(string url, Action<float> progress, Action<byte[]> downloadedCallback)
            {
                return Utility.Unity.StartCoroutine(EnumUnityWebRequest(UnityWebRequest.Get(url), progress, (UnityWebRequest req) =>
                {
                    var bundleBytes = req.downloadHandler.data;
                    if (bundleBytes != null)
                    {
                        downloadedCallback?.Invoke(bundleBytes);
                    }
                }));
            }
            public static Coroutine DownloadAssetBundlesBytesAsync(string[] urls, Action<float> overallProgress, Action<float> progress, Action<IList<byte[]>> downloadedCallback)
            {
                var length = urls.Length;
                var requests = new List<UnityWebRequest>();
                for (int i = 0; i < length; i++)
                {
                    requests.Add(UnityWebRequest.Get(urls[i]));
                }
                return Utility.Unity.StartCoroutine(EnumBytesUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
                {
                    var reqLength = reqs.Count;
                    var bundleByteList = new List<byte[]>();
                    for (int i = 0; i < reqLength; i++)
                    {
                        bundleByteList.Add(reqs[i]);
                    }
                    downloadedCallback?.Invoke(bundleByteList);
                }));
            }
            static IEnumerator EnumUnityWebRequest(UnityWebRequest unityWebRequest, Action<float> progress, Action<UnityWebRequest> downloadedCallback)
            {
                using (UnityWebRequest request = unityWebRequest)
                {
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        progress?.Invoke(request.downloadProgress);
                        yield return null;
                    }
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        if (request.isDone)
                        {
                            progress?.Invoke(1);
                            downloadedCallback(request);
                        }
                    }
                    else
                    {
                        //throw new ArgumentException($"UnityWebRequest：{request.url } : {request.error } ！");
                    }
                }
            }
            static IEnumerator EnumUnityWebRequests(UnityWebRequest[] unityWebRequests, Action<float> overallProgress, Action<float> progress, Action<UnityWebRequest[]> downloadedCallback)
            {
                var length = unityWebRequests.Length;
                var count = length - 1;
                var requestList = new List<UnityWebRequest>();
                for (int i = 0; i < length; i++)
                {
                    overallProgress?.Invoke((float)i / (float)count);
                    yield return EnumUnityWebRequest(unityWebRequests[i], progress, (request) => { requestList.Add(request); });
                }
                downloadedCallback.Invoke(requestList.ToArray());
            }
            static IEnumerator EnumBytesUnityWebRequests(UnityWebRequest[] unityWebRequests, Action<float> overallProgress, Action<float> progress, Action< IList< byte[]>> downloadedCallback)
            {
                var length = unityWebRequests.Length;
                var count = length - 1;
                var requestBytesList = new List<byte[]>();
                for (int i = 0; i < length; i++)
                {
                    overallProgress?.Invoke((float)i / (float)count);
                    yield return EnumUnityWebRequest(unityWebRequests[i], progress, (request) => { requestBytesList.Add(request.downloadHandler.data); });
                }
                downloadedCallback.Invoke(requestBytesList);
            }
            #endregion
        }
    }
}