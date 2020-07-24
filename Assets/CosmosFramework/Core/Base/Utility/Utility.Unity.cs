using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos
{
    public sealed partial class Utility
    {
        /// <summary>
        /// 这个类封装了所有跟Unity相关的工具函数，是所有Utiltiy中需要引入UnityEngine的类
        /// </summary>
        public static class Unity
        {
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
            public static T Get<T>(Component comp, string subNode) where T : Component
            {
                return comp.transform.Find(subNode).GetComponent<T>();
            }
            /// <summary>
            /// 添加目标组件；默认不移除组件;
            /// 若removeExistComp为true，则移除已经存在的，并重新赋予；
            /// </summary>
            /// <typeparam name="T">目标组件</typeparam>
            /// <param name="go">目标对象</param>
            /// <param name="removeExistComp">是否移除已经存在的组件</param>
            /// <returns>返回添加的目标组件</returns>
            public static T Add<T>(GameObject go, bool removeExistComp = false) where T : Component
            {
                T t = default;
                if (go != null)
                {
                    if (removeExistComp)
                    {
                        T[] ts = go.GetComponents<T>();
                        for (int i = 0; i < ts.Length; i++)
                        {
                            if (ts[i] != null)
                                GameManager.KillObject(go);
                        }
                        t = go.AddComponent<T>();
                    }
                    else
                    {
                        T[] ts = go.GetComponents<T>();
                        if (ts.Length== 0)
                        {
                            t = go.AddComponent<T>();
                            return t;
                        }
                        for (int i = 0; i < ts.Length; i++)
                        {
                            if (ts[i] != null)
                            {
                                t = go.gameObject.GetComponent<T>();
                                return t;
                            }
                        }
                    }

                }
                return t;
            }
            /// <summary>
            /// 添加目标组件；默认不移除组件;
            /// 若removeExistComp为true，则移除已经存在的，并重新赋予；
            /// </summary>
            /// <typeparam name="T">目标组件</typeparam>
            /// <param name="go">目标对象</param>
            /// <param name="removeExistComp">是否移除已经存在的组件</param>
            /// <returns>返回添加的目标组件</returns>
            public static T Add<T>(Transform go, bool removeExistComp = false) where T : Component
            {
                return Add<T>(go.gameObject);
            }
            /// <summary>
            /// 实例化对象；
            /// 默认不移除原本就存在的T组件对象，若默认参数未true，则会移除本就存在的组件，再重新添加；
            /// </summary>
            /// <typeparam name="T">mono脚本</typeparam>
            /// <param name="spawnItem">生成的对象</param>
            /// <param name="removeExistComp">是否移除原本存在的T类型脚本组件</param>
            /// <returns>返回生成成功后的组件对象</returns>
            public static T Instantiate<T>(GameObject spawnItem, bool removeExistComp = false) where T : Component
            {
                if (spawnItem == null)
                    throw new ArgumentNullException("ObjectSpawner : spawnItem not exist !");
                var go = GameObject.Instantiate(spawnItem);
                T comp = default;
                if (!removeExistComp)
                {
                    comp = go.GetComponent<T>();
                    if (comp == null)
                        comp = go.AddComponent<T>();
                }
                else
                {
                    comp = Add<T>(go,true);
                }
                return comp;
            }
            public static GameObject Child(Transform go, string subNode)
            {
                Transform tran = go.Find(subNode);
                if (tran != null)
                    return tran.gameObject;
                else
                    return null;
            }
            public static GameObject Child(GameObject go, string subNode)
            {
                return Child(go.transform, subNode);
            }
            /// <summary>
            /// 查找同级别
            /// </summary>
            /// <param name="go">同级别当前对象</param>
            /// <param name="subNode">同级别目标对象名称</param>
            /// <returns></returns>
            public static GameObject Peer(Transform go, string subNode)
            {
                Transform tran = go.parent.Find(subNode);
                if (tran != null)
                    return tran.gameObject;
                else
                    return null;
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
                    DebugError("PlayerPrefs key " + key + "  not exist!");
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
                return Utility.Text.Format(ApplicationConst.APPPERFIX + "_" + key);
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
                return ApplicationConst.ApplicationDataPath + "/" + Utility.Text.StringBuilderCache.ToString();
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
                return absolutePath.Remove(0, ApplicationConst.ApplicationDataPath.Length);
            }
        }
    }
}
