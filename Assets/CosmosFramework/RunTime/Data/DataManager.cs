using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace Cosmos.Data {
    //TODO 更改Litjson为JsonFX，跨平台处理上fx更加适用，尤其是ios
    public sealed class DataManager : Module<DataManager>
    {
        //数据处理
        DataProcess dataProcess = new DataProcess();
        public void ParseData(string jsonFullPath,CFAction<JsonData>callBack=null)
        {
            TextAsset ta =Facade.Instance.Load<TextAsset>(jsonFullPath);
            dataProcess.ParseJosn(ta, callBack);
        }
        public void ParseData<T>(string jsonFullPath,CFAction<T> callBack=null)
            where T:class, new()
        {
            TextAsset ta = Facade.Instance.Load<TextAsset>(jsonFullPath);
            dataProcess.ParseJson<T>(ta, callBack);
        }
        /// <summary>
        /// 保存Json数据到本地的绝对路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="dataSet">装箱后的数据</param>
        /// <param name="callBack">回调函数，当写入成功后调用</param>
        public void SaveJsonDataToLocal(string relativePath,string fileName,object dataSet,CFAction callBack)
        {
            string absoluteFullpath = Utility.CombineAbsolutePath(relativePath);
            if (!Directory.Exists(absoluteFullpath))
                Directory.CreateDirectory(absoluteFullpath);
            using (FileStream stream = File.Create(Utility.CombineRelativeFilePath(fileName, absoluteFullpath)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                var json = JsonMapper.ToJson(dataSet);
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
        public void LoadJsonDataFromLocal<T>(string relativePath,string fileName,out T dataSet,CFAction<T>callBack=null)
        {
            using (FileStream stream =File.Open(Utility.CombineAbsoluteFilePath(fileName, relativePath), FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                string json = (string)bf.Deserialize(stream);
                dataSet = JsonMapper.ToObject<T>(json);
                callBack?.Invoke(dataSet);
                stream.Close();
            }
        }
        public void LoadJsonDataFromLocal<T>(string fullRelativeFilePath, out T dataSet, CFAction<T> callBack = null)
        {
            using (FileStream stream = File.Open(Utility.CombineAbsoluteFilePath(fullRelativeFilePath), FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                string json = (string)bf.Deserialize(stream);
                dataSet = JsonMapper.ToObject<T>(json);
                callBack?.Invoke(dataSet);
                stream.Close();
            }
        }
    }
}