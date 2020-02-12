using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using System.Xml;
namespace Cosmos.Data {
    public class DataManager : Module<DataManager>
    {
        //Dictionary<Type, CFScriptableObject> dataSet = new Dictionary<Type, CFScriptableObject>();
        //数据处理
        DataProcess dataProcess = new DataProcess();
        protected override void InitModule()
        {
            RegisterModule(CFModule.Data);
        }
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
        public void SaveJson( )
        {
            //runtime预留
            dataProcess.SaveJson();
        }
  
    }
}