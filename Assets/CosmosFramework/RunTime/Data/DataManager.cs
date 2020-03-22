using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace Cosmos.Data {
    //TODO 逐渐弃用Litjson，跨平台处理上fx更加适用，尤其是ios
    public sealed class DataManager : Module<DataManager>
    {
        //数据处理
        //DataProcess dataProcess = new DataProcess();
        public void ParseDataFromResource<T>(string relativePath, string fileName,ref T dataSet,CFAction<T> callBack=null)
            where T:class, new()
        {
            string relativeFullPath = Utility.CombineRelativeFilePath(fileName, relativePath);
            TextAsset ta = Facade.Instance.LoadResAsset<TextAsset>(relativeFullPath);
            JsonUtility.FromJsonOverwrite(ta.text, dataSet);
            callBack?.Invoke(dataSet);
        }
        /// <summary>
        /// 保存Json数据到本地的绝对路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="dataSet">装箱后的数据</param>
        /// <param name="callBack">回调函数，当写入成功后调用</param>
        public void SaveJsonDataToLocal<T>(string relativePath,string fileName,T dataSet,CFAction callBack=null)
        {
            string absoluteFullpath = Utility.CombinePersistentPath(relativePath);
            if (!Directory.Exists(absoluteFullpath))
                Directory.CreateDirectory(absoluteFullpath);
            using (FileStream stream = File.Create(Utility.CombineRelativeFilePath(fileName, absoluteFullpath)))
            {
                Utility.DebugLog("Save local path：\n"+ Utility.CombineRelativeFilePath(fileName, absoluteFullpath),MessageColor.green);
                BinaryFormatter bf = new BinaryFormatter();
                var json = JsonUtility.ToJson(dataSet);
                bf.Serialize(stream, json);
                callBack?.Invoke();
                stream.Close();
            }
        }
        /// <summary>
        /// 从本地的绝对路径读取Json数据
        /// </summary>
        /// <typeparam name="T">反序列化的目标类型</typeparam>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="dataSet">装箱后的数据</param>
        /// <param name="callBack">回调函数，当读取成功后调用</param>
        public void LoadJsonDataFromLocal<T>(string relativePath,string fileName,ref T dataSet,CFAction<T>callBack=null)
        {
            string absoluteFullpath = Utility.CombinePersistentPath(relativePath);
            if (!Directory.Exists(absoluteFullpath))
                return;
            using (FileStream stream =File.Open(Utility.CombineRelativeFilePath(fileName, absoluteFullpath), FileMode.Open))
            {
                Utility.DebugLog("Load local path：\n" + Utility.CombineRelativeFilePath(fileName, absoluteFullpath), MessageColor.green);
                BinaryFormatter bf = new BinaryFormatter();
                string json = (string)bf.Deserialize(stream);
                JsonUtility.FromJsonOverwrite(json, dataSet);
                callBack?.Invoke(dataSet);
                stream.Close();
            }
        }
        public void LoadJsonDataFromLocal<T>(string fullRelativeFilePath, ref T dataSet, CFAction<T> callBack = null)
        {
            string absoluteFullpath = Utility.CombinePersistentPath(fullRelativeFilePath);
            if (!File.Exists(absoluteFullpath))
                return;
            using (FileStream stream = File.Open(Utility.CombinePersistentPath(absoluteFullpath), FileMode.Open))
            {
                Utility.DebugLog("Load local path：\n" + Utility.CombineRelativeFilePath(absoluteFullpath), MessageColor.green);
                BinaryFormatter bf = new BinaryFormatter();
                string json = (string)bf.Deserialize(stream);
                JsonUtility.FromJsonOverwrite(json, dataSet);
                callBack?.Invoke(dataSet);
                stream.Close();
            }
        }
    }
}