using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Xml;
namespace Cosmos.Data {
    public class DataManager : Module<DataManager>
    {
        //数据处理
        DataProcess dataProcess = new DataProcess();
        protected override void InitModule()
        {
            RegisterModule(CFModule.Data);
        }
        public void ParseJson(string jsonFullPath,CFAction<JsonData>callBack)
        {
            TextAsset ta =Facade.Instance.Load<TextAsset>(jsonFullPath);
            dataProcess.ParseJosn(ta, callBack);
        }
        public T ParseJson<T>(string jsonFullPath)
            where T : new()
        {
            TextAsset ta = Facade.Instance.Load<TextAsset>(jsonFullPath);
            return dataProcess.ParseJson<T>(ta);
        }
        public void ParseJson<T>(string jsonFullPath,CFAction<T> callBack)
            where T:new()
        {
            TextAsset ta = Facade.Instance.Load<TextAsset>(jsonFullPath);
            dataProcess.ParseJson<T>(ta, callBack);
        }
        public void ParseXml(string XmlFullPath,CFAction<XmlDocument> callBack)
        {
            TextAsset ta = Facade.Instance.Load<TextAsset>(XmlFullPath);
            dataProcess.ParseXml(ta, callBack);
        }
        public void CreateEmptyJson(string xmlFullPath)
        {
            dataProcess.CreateEmptyJson(xmlFullPath);
        }
    }
}