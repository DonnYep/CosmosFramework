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
            public static Transform FindChild(Transform go, string subNode)
            {
                var trans = go.GetComponentsInChildren<Transform>();
                var length = trans.Length;
                for (int i = 1; i < length; i++)
                {
                    if (trans[i].name.Equals(subNode))
                        return trans[i];
                }
                return null;
            }
            /// <summary>
            /// 查找所有符合名称的子节点
            /// </summary>
            /// <param name="go">目标对象</param>
            /// <param name="subNode">子级别目标对象名称</param>
            /// <returns>名字符合的对象数组</returns>
            public static Transform[] FindChilds(Transform go, string subNode)
            {
                var trans = go.GetComponentsInChildren<Transform>();
                var length = trans.Length;
                var dst = new Transform[length];
                int idx = 0;
                for (int i = 0; i < length; i++)
                {
                    if (trans[i].name.Contains(subNode))
                    {
                        dst[idx] = trans[i];
                        idx++;
                    }
                }
                Array.Resize(ref dst, idx);
                return dst;
            }
            public static T ChildComp<T>(Transform go, string subNode)
      where T : Component
            {
                var child = FindChild(go, subNode);
                if (child == null)
                    return null;
                return child.GetComponent<T>();
            }
            public static T ParentComp<T>(Transform go, string parentNode)
where T : Component
            {
                var parent = Parent(go, parentNode);
                if (parent == null)
                    return null;
                return parent.GetComponent<T>();
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
            /// <param name="peerNode">同级别目标对象名称</param>
            /// <returns>查找到的目标对象</returns>
            public static Transform Peer(Transform go, string peerNode)
            {
                Transform tran = go.parent.Find(peerNode);
                if (tran == null)
                    return null;
                return tran;
            }
            /// <summary>
            /// 查找同级别其他对象；
            /// </summary>
            /// <param name="go">同级别当前对象</param>
            /// <param name="includeSrc">是否包含本身</param>
            /// <returns>当前级别下除此对象的其他同级的对象</returns>
            public static Transform[] Peers(Transform go, bool includeSrc = false)
            {
                Transform parentTrans = go.parent;
                var childTrans = parentTrans.GetComponentsInChildren<Transform>();
                var length = childTrans.Length;
                if (!includeSrc)
                    return Utility.Algorithm.FindAll(childTrans, t => t.parent == parentTrans && t != go);
                else
                    return Utility.Algorithm.FindAll(childTrans, t => t.parent == parentTrans);
            }
            /// <summary>
            /// 查找同级别下所有目标组件；
            /// 略耗性能；
            /// </summary>
            /// <typeparam name="T">目标组件</typeparam>
            /// <param name="go">同级别当前对象</param>
            /// <param name="includeSrc">包含当前对象</param>
            /// <returns>同级别对象数组</returns>
            public static T[] PeerComponets<T>(Transform go, bool includeSrc = false) where T : Component
            {
                Transform parentTrans = go.parent;
                var childTrans = parentTrans.GetComponentsInChildren<Transform>();
                var length = childTrans.Length;
                Transform[] trans;
                if (!includeSrc)
                    trans = Utility.Algorithm.FindAll(childTrans, t => t.parent == parentTrans);
                else
                    trans = Utility.Algorithm.FindAll(childTrans, t => t.parent == parentTrans && t != go);
                var transLength = trans.Length;
                T[] src = new T[transLength];
                int idx = 0;
                for (int i = 0; i < transLength; i++)
                {
                    var comp = trans[i].GetComponent<T>();
                    if (comp != null)
                    {
                        src[idx] = comp;
                        idx++;
                    }
                }
                T[] dst = new T[idx];
                Array.Copy(src, 0, dst, 0, idx);
                return dst;
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
            public static Transform Parent(Transform go, string parentNode)
            {
                var par = go.GetComponentsInParent<Transform>();
                return Algorithm.Find(par, p => p.gameObject.name == parentNode);
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
                Texture2D texture2D = new Texture2D(width, height);
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
        }
    }
}