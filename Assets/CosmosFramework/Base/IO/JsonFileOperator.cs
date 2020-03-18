using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
namespace Cosmos.IO{
    /// <summary>
    /// 这里使用LitJson
    /// 之所以不使用JsonUtility，是因为JU必须有一个类完全对应，不能根据键值对进行取值。
    /// LitJson能够存储字典，解析复杂Json上远超于JU
    /// </summary>
    public class JsonFileOperator
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