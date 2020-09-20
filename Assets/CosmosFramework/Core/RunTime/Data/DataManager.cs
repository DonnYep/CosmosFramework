using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace Cosmos.Data
{
    internal sealed class DataManager : Module<DataManager>
    {
        #region Methods
        /// <summary>
        /// 从Resource文件夹下读取Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="dataSet">存储json的类模型</param>
        /// <param name="callback">回调函数</param>
        internal void ParseDataFromResource<T>(string relativePath, string fileName, ref T dataSet, Action<T> callback = null)
            where T : class, new()
        {
            string relativeFullPath = Utility.IO.CombineRelativeFilePath(fileName, relativePath);
            TextAsset ta = Facade.LoadResAsset<TextAsset>(relativeFullPath);
            dataSet = Utility.Json.ToObject<T>(ta.text);
            callback?.Invoke(dataSet);
        }
        /// <summary>
        /// 保存Json数据到本地的绝对路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="dataSet">装箱后的数据</param>
        /// <param name="binary">文件是否为二进制</param>
        /// <param name="callback">回调函数，当写入成功后调用</param>
        internal void SaveJsonDataToLocal<T>(string relativePath, string fileName, T dataSet, bool binary = false, Action callback = null)
            where T : class, new()
        {
            string absoluteFullpath = Utility.Unity.CombineAppPersistentPath(relativePath);
            if (!Directory.Exists(absoluteFullpath))
                Directory.CreateDirectory(absoluteFullpath);
            using (FileStream stream = File.Create(Utility.IO.CombineRelativeFilePath(fileName, absoluteFullpath)))
            {
                Utility.Debug.LogInfo("Save local path：\n" + Utility.IO.CombineRelativeFilePath(fileName, absoluteFullpath), MessageColor.GREEN);
                if (binary)
                {
                    BinaryFormatter bf = new BinaryFormatter();//二进制
                    var json = Utility.Json.ToJson(dataSet);
                    bf.Serialize(stream, json);//二进制
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        var json = Utility.Json.ToJson(dataSet);
                        writer.Write(json);
                        writer.Close();
                    }
                }
                callback?.Invoke();
                stream.Close();
            }
        }
        /// <summary>
        /// 从本地的绝对路径读取Json数据
        /// </summary>
        /// <typeparam name="T">反序列化的目标类型</typeparam>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="binary">文件是否为二进制</param>
        /// <param name="callback">回调函数，当读取成功后调用</param>
       internal string LoadJsonDataFromLocal(string relativePath, string fileName, bool binary=false, Action callback = null)
        {
            string absoluteFullpath = Utility.Unity.CombineAppPersistentPath(relativePath);
            if (!Directory.Exists(absoluteFullpath))
                throw new IOException("DataManager-->> Json floder not exist!");
            string json = "";
            using (FileStream stream = File.Open(Utility.IO.CombineRelativeFilePath(fileName, absoluteFullpath), FileMode.Open))
            {
                Utility.Debug.LogInfo("Load local path : \n" + Utility.IO.CombineRelativeFilePath(fileName, absoluteFullpath), MessageColor.GREEN);
                if (binary)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                   json= (string)bf.Deserialize(stream);
                }
                else
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                         json = reader.ReadToEnd();
                        reader.Close();
                    }
                }
                callback?.Invoke();
                stream.Close();
                return json;
            }
        }
        /// <summary>
        /// 从本地的绝对路径读取Json数据
        /// </summary>
        /// <typeparam name="T">反序列化的目标类型</typeparam>
        /// <param name="fullRelativeFilePath">完整的相对路径，具体到文件名</param>
        /// <param name="dataSet">装箱后的数据</param>
        /// <param name="binary">文件是否为二进制</param>
        /// <param name="callback">回调函数，读取成功后调用</param>
        internal string LoadJsonDataFromLocal(string fullRelativeFilePath,  bool binary=false, Action callback = null)
        {
            string absoluteFullpath = Utility.Unity.CombineAppPersistentPath(fullRelativeFilePath);
            if (!File.Exists(absoluteFullpath))
                throw new IOException("DataManager-->> Json file not exist!");
            string json = "";
            using (FileStream stream = File.Open(Utility.Unity.CombineAppPersistentPath(absoluteFullpath), FileMode.Open))
            {
                Utility.Debug.LogInfo("Load local path：\n" + Utility.IO.CombineRelativeFilePath(absoluteFullpath), MessageColor.GREEN);
                if (binary)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    json = (string)bf.Deserialize(stream);
                }
                else
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                        reader.Close();
                    }
                }
                callback?.Invoke();
                stream.Close();
                return json;
            }
        }
        #endregion
    }
}