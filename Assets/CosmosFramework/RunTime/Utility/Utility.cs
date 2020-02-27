using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using Object = UnityEngine.Object;
namespace Cosmos
{
    public enum MessageColor
    {
        black,
        blue,
        brown,
        darkblue,
        green,//原谅色
        lime,//力量色
        grey,
        fuchsia,//洋红色
        navy,//海军蓝
        orange,
        red,
        teal,//蓝绿色
        yellow,
        maroon,//褐红色
        purple
    }
    /// <summary>
    /// 通用工具类：
    /// 数组工具，反射工具，文字工具，加密工具，
    /// 数学工具，持久化数据工具，Debug工具……
    /// </summary>
    public sealed partial class Utility
    {
        //Log打印是否开启，默认开启
        static bool enableDebugLog = true;
        public static bool EnableDebugLog { get { return enableDebugLog; } set { enableDebugLog = value; } }
        public static void DebugLog(object o, Object context = null)
        {
            if (!EnableDebugLog)
                return;
            if (context == null)
                Debug.Log("<b>-->><color=#254FDB>" + o + "</color></b>");
            else
                Debug.Log("<b>-->><color=#254FDB>" + o + "</color></b>", context);
        }
        public static void DebugLog(object o, MessageColor messageColor,Object context = null)
        {
            //Debug.Log("<b>-->><size=16><color=" + messageColor.ToString() + ">" + o + "</color></size></b>");

            if (!EnableDebugLog)
                return;
            if (context == null)
                Debug.Log("<b>-->><color="+messageColor.ToString()+">" + o + "</color></b>");
            else
                Debug.Log("<b>-->><color=" + messageColor.ToString() + ">" + o + "</color></b>", context);
        }
        public static void DebugWarning(object o, Object context = null)
        {
            if (!EnableDebugLog)
                return;
            if (context == null)
                Debug.LogWarning("<b>-->><color=#FF5E00>" + o + "</color></b>");
            else
                Debug.LogWarning("<b>-->><color=#FF5E00>" + o + "</color></b>", context);
        }
        public static void DebugError(object o, Object context = null)
        {
            if (!EnableDebugLog)
                return;
            if (context == null)
                Debug.LogError("<b>-->><color=#FF0000>" + o + "</color></b>");
            else
                Debug.LogError("<b>-->><color=#FF0000>" + o + "</color></b>", context);
        }
        public static int Int(object arg)
        {
            return Convert.ToInt32(arg);
        }
        public static float Float(object arg)
        {
            return (float)Math.Round(Convert.ToSingle(arg));
        }
        public static long Long(object arg)
        {
            return Convert.ToInt64(arg);
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
        public static T Get<T>(Component comp, string subNode) where T : Component
        {
            return comp.transform.Find(subNode).GetComponent<T>();
        }
        /// <summary>
        /// 如果旧的对象没有目标组件，则删除旧对象。
        /// 返回新创建添加组件后的对象
        /// </summary>
        public static T Add<T>(GameObject go) where T : Component
        {
            if (go != null)
            {
                T[] ts = go.GetComponents<T>();
                for (int i = 0; i < ts.Length; i++)
                {
                    if (ts[i] != null)
                        GameManager.KillObject(go);
                }
                return go.gameObject.AddComponent<T>();
            }
            return null;
        }
        /// <summary>
        /// 如果旧的对象没有目标组件，则删除旧对象。
        /// 返回新创建添加组件后的对象
        /// </summary>
        public static T Add<T>(Transform go) where T : Component
        {
            return Add<T>(go.transform);
        }
        /// <summary>
        /// 升序排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        public static void SortByOrder<T, K>(T[] array, CFResultAction<T, K> handler)
            where K : IComparable<K>
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                    {
                        T temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
        }
        /// <summary>
        /// 降序排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        public static void SortByDescending<T, K>(T[] array, CFResultAction<T, K> handler)
            where K : IComparable<K>
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                    {
                        T temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
        }
        /// <summary>
        /// 获取最小
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="array"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static T Min<T, K>(T[] array, CFResultAction<T, K> handler)
        where K : IComparable<K>
        {
            T temp = default(T);
            temp = array[0];
            foreach (var arr in array)
            {
                if (handler(temp).CompareTo(handler(arr)) > 0)
                {
                    temp = arr;
                }
            }
            return temp;
        }
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="array"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static T Max<T, K>(T[] array, CFResultAction<T, K> handler)
        where K : IComparable<K>
        {
            T temp = default(T);
            temp = array[0];
            foreach (var arr in array)
            {
                if (handler(temp).CompareTo(handler(arr)) < 0)
                {
                    temp = arr;
                }
            }
            return temp;
        }
        /// <summary>
        /// 获得传入元素某个符合条件的所有对象
        /// </summary>
        public static T Find<T>(T[] array, CFPredicateAction<T> handler)
        {
            T temp = default(T);
            for (int i = 0; i < array.Length; i++)
            {
                if (handler(array[i]))
                {
                    return array[i];
                }
            }
            return temp;
        }
        /// <summary>
        /// 获得传入元素某个符合条件的所有对象
        /// </summary>
        public static T[] FindAll<T, K>(T[] array, CFPredicateAction<T> handler)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                if (handler(array[i]))
                    list.Add(array[i]);
            }
            return list.ToArray();
        }
        /// <summary>
        ///这里定义仅仅作为开发工具，runtime有runtime的resourceMgr 
        /// </summary>
        public static T Load<T>(string path)
            where T : UnityEngine.Object
        {
            T res = Resources.Load<T>(path);
            if(res!=null)
                return res;
            else
            {
                DebugError("Path:" + path + "\n not exist, check your path");
                return null;
            }
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
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Encode(string message)
        {
            byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(message);
            return Convert.ToBase64String(bytes);
        }
        public static string Decode(string message)
        {
            byte[] bytes = Convert.FromBase64String(message);
            return Encoding.GetEncoding("utf-8").GetString(bytes);
        }
        /// <summary>
        /// 是否是一串数字类型的string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsNumber(str[i])) return false;
            }
            return true;
        }
        public static void ClearChild(Transform go)
        {
            if (go != null)
                for (int i = 0; i < go.childCount; i++)
                {
                    GameManager.KillObject(go.GetChild(i).gameObject);
                }
        }
        public static void ClearMemory()
        {
            GC.Collect(); Resources.UnloadUnusedAssets();
        }
        /// <summary>
        /// PlayerPrefs 是否存在key，否则报错显示信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasPrefsKey(string key)
        {
            string fullKey = GetPrefsKey(key);
            if (PlayerPrefs.HasKey(fullKey))
                return true;
            else
            {
                DebugError("PlayerPrefs key\t" + key + "\t not exist!");
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
            return ApplicationConst._AppPerfix + "_" + key;
        }
        public static int GetPrefsInt(string key)
        {
            string fullKey = GetPrefsKey(key);
            return PlayerPrefs.GetInt(fullKey);
        }
        public static string GetString(string key)
        {
            string fullKey = GetPrefsKey(key);
            return PlayerPrefs.GetString(fullKey);
        }
        public static float GetPrefsFloat(string key)
        {
            string fullKey = GetPrefsKey(key);
            return PlayerPrefs.GetFloat(fullKey);
        }
        public static void SetPrefsString(string key, string value)
        {
            string fullKey = GetPrefsKey(key);
            PlayerPrefs.DeleteKey(fullKey);
            PlayerPrefs.SetString(key, value);
        }
        public static void SetPrefsInt(string key, int value)
        {
            string fullKey = GetPrefsKey(key);
            PlayerPrefs.DeleteKey(fullKey);
            PlayerPrefs.SetInt(key, value);
        }
        public static void SetPrefsFloat(string key, int value)
        {
            string fullKey = GetPrefsKey(key);
            PlayerPrefs.DeleteKey(fullKey);
            PlayerPrefs.SetFloat(key, value);
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
        ///T是一个类的对象，由object转换成class对象 
        /// </summary>
        public static T ConvertToObject<T>(object arg)
            where T : class
        {
            return arg as T;
        }
        /// <summary>
        ///   /// 反射工具，得到反射类的对象；
        /// 不可反射Mono子类，被反射对象必须是具有无参公共构造
        /// </summary>
        /// <typeparam name="T">类型目标</typeparam>
        /// <param name="assembly">目标程序集</param>
        /// <param name="typeFullName">完全限定名</param>
        /// <returns>返回T类型的目标类型对象</returns>
        public static T GetTypeInstance<T>(Assembly assembly,string typeFullName)
            where T : class
        {
            Type type = assembly.GetType(typeFullName);
            if (type != null)
            {
                var obj = Activator.CreateInstance(type) as T;
                return obj;
            }
            else
            {
                DebugError("Type :" + typeFullName + "\n not exist,check your fullName !");
                return null;
            }
        }
        /// <summary>
        ///   /// 反射工具，得到反射类的对象；
        /// 不可反射Mono子类，被反射对象必须是具有无参公共构造
        /// </summary>
        /// <typeparam name="T">类型目标</typeparam>
        /// <param name="type">目标对象程序集中的某个对象类型:GetType()</param>
        /// <param name="typeFullName">完全限定名</param>
        /// <returns>返回T类型的目标类型对象</returns>
        public static T GetTypeInstance<T>(Type type, string typeFullName)
            where T:class
        {
            return GetTypeInstance<T>(type.Assembly, typeFullName);
        }
        /// <summary>
        /// 反射工具，得到反射类的对象；
        /// 不可反射Mono子类，被反射对象必须是具有无参公共构造
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="arg">目标类型的对象</param>
        /// <param name="typeFullName">完全限定名</param>
        /// <returns>返回T类型的目标类型对象</returns>
        public static T GetTypeInstance<T>(T arg,string typeFullName)
            where T:class
        {
            return GetTypeInstance<T>(typeof(T).Assembly, typeFullName);
        }
        #region Text Tool
        static StringBuilder stringBuilderCache;
        static StringBuilder StringBuilderCache
        {
            get
            {
                if (stringBuilderCache == null)
                    stringBuilderCache = new StringBuilder(1024);
                return stringBuilderCache;
            }
            set { stringBuilderCache = value; }
        }
        public static string GetTypeFullName<T>(T arg)
        {
            return typeof(T).ToString();
        }
        public static string GetTypeFullName<T>(string name)
            where T:class
        {
            return GetTypeFullName(typeof(T), name);
        }
        public static string GetTypeFullName(Type type,string name)
        {
            if (type == null)
            {
                DebugError("Type is invalid.无效类");
                return null;
            }
            string typeName = type.FullName;
            return string.IsNullOrEmpty(name) ? typeName : Format(typeName, name);
        }
        public static string Format(string format,params object[] args)
        {
            if (string.IsNullOrEmpty(format))
            {
                DebugError("Format is invaild");
                return null;
            }
            if (args == null)
            {
                DebugError("Arg is invaild");
                return null;
            }
            stringBuilderCache.Length = 0;
            StringBuilderCache.AppendFormat(format, args);
            return StringBuilderCache.ToString();
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="fullString">完整的字符串</param>
        /// <param name="separator">分割符</param>
        /// <param name="options">分割操作</param>
        /// <param name="subStringIndex">分割后的字段角标</param>
        /// <returns></returns>
        public static string StringSplit(string fullString, String[] separator, StringSplitOptions options,int subStringIndex)
        {
            string[] stringArray = fullString.Split(separator,options);
            string subString = stringArray[subStringIndex];
            return subString;
        }
        public static string StringSplit(string fullString, String[] separator, int count,StringSplitOptions options)
        {
            string[] stringArray = fullString.Split(separator,count, options);
            return stringArray.ToString();
        }
        public static int CharCount(string fullString,char separator)
        {
            if (string.IsNullOrEmpty(fullString)||string.IsNullOrEmpty(separator.ToString()))
            {
                DebugError("charCount \n string invaild!");
                return 0;
            }
            int count = 0;
            for (int i = 0; i < fullString.Length; i++)
            {
                if (fullString[i] == separator)
                {
                    count++;
                }
            }
            return count;
        }
        #endregion
        /// <summary>
        /// 合并地址,返回绝对路径
        /// UnityEditor环境下使用
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>返回绝对路径</returns>
        public static string CombineAbsolutePath(params string[] relativePath)
        {
            string fullPath = null;
            for (int i = 0; i < relativePath.Length; i++)
            {
                fullPath += (relativePath[i] + "\\");
            }
            return ApplicationConst.ApplicationDataPath + "\\" + fullPath;
        }
        /// <summary>
        /// 合并地址,返回绝对路径
        /// UnityEditor环境下使用
        /// </summary>
        /// <param name="fileFullName">文件的完整名称（包括文件扩展名）</param>
        /// <param name="relativePath">相对路径</param>
        /// <returns></returns>
        public static string CombineAbsoluteFilePath(string fileFullName, params string[] relativePath)
        {
            return CombineAbsolutePath(relativePath) + fileFullName;
        }
        /// <summary>
        /// 合并地址,返回相对路径
        /// 参考示例：Resources\JsonData\
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns></returns>
        public static string CombineRelativePath(params string[] relativePath)
        {
            string fullPath = null;
            for (int i = 0; i < relativePath.Length; i++)
            {
                fullPath += (relativePath[i]+"\\");
            }
            return fullPath;
        }
        /// <summary>
        /// 合并地址,返回相对路径
        /// 参考示例：Resources\JsonData\CF.json
        /// </summary>
        /// <param name="fileFullName">文件的完整名称（包括文件扩展名）</param>
        /// <param name="relativePath">相对路径</param>
        /// <returns></returns>
        public static string CombineRelativeFilePath(string fileFullName, params string[] relativePath)
        {
            return CombineRelativePath(relativePath) + fileFullName;
        }
        /// <summary>
        /// 分解绝对路径，返回相对路径
        /// </summary>
        /// <param name="absolutePath">绝对路径</param>
        /// <returns>相对路径</returns>
        public static string DecomposeAbsolutePath(string absolutePath)
        {
            return  absolutePath.Remove(0,ApplicationConst.ApplicationDataPath.Length);
        }
        /// <summary>
        /// 刷新unity编辑器
        /// </summary>
        public static void Refresh()
        {
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}