using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using LitJson;
namespace Cosmos.IO{
    /// <summary>
    /// 这里使用LitJson
    /// </summary>
    public class JsonOperator
    {
        /// <summary>
        /// 解析Json
        /// </summary>
        /// <typeparam name="T">泛型，具有约束</typeparam>
        /// <param name="ta">载入的TextAsset类型对象</param>
        /// <param name="parsedCallBack">回调函数，这里是直接映射</param>
        public void ParseJson<T>(TextAsset ta, CFAction<T> parsedCallBack)
            where T : new()
        {
            string jsonStr = ta.text;
            JsonData jsonObj = JsonMapper.ToObject(jsonStr);
            if (parsedCallBack != null)
            {
                T tempObj = new T();
                tempObj = JsonMapper.ToObject<T>(jsonObj.ToJson());
                parsedCallBack(tempObj);
            }
        }
        /// <summary>
        /// 解析Json
        /// </summary>
        /// <param name="ta">载入的TextAsset类型对象</param>
        /// <param name="parsedCallBack">回调函数，这里返回JsonData，数据由回调函数解析</param>
        public void ParseJson(TextAsset ta, CFAction<JsonData> parsedCallBack)
        {
            string jsonStr = ta.text;
            JsonData jsonObj = JsonMapper.ToObject(jsonStr);
            if (parsedCallBack != null)
            {
                parsedCallBack(jsonObj);
            }
        }
        public void CreateEmptyJson(string jsonFullPath)
        {
            if (File.Exists(jsonFullPath))
            {
                File.Delete(jsonFullPath);
            }
            FileStream fileStream = new FileStream(jsonFullPath,
                     FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fileStream);
            sw.WriteLine("EmptyJsonFile");
            sw.Close();
            fileStream.Close();
            fileStream.Dispose();
        }
    }
}