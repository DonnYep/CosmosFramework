//------------------------------------------------------------------------------
//      当前算一个早期版本，持续更新中。后期会做分类处理
//      当前资源管理需要精简，拆分功能
//----
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;
using Cosmos.Config;
//using Cosmos.Event;
using System.Collections;
namespace Cosmos.Resource
{
    public enum ResourceLoadMode : int
    {
        Resource = 0,
        AssetBundle = 1
    }
    public sealed class ResourceManager:Module<ResourceManager>
    {
        protected override void InitModule()
        {
            RegisterModule(CFModule.Resource);
        }
        #region 基于Resources
        /// <summary>
        /// 同步加载资源
        /// </summary>
        public T Load<T>(string path)
            where T:UnityEngine.Object
        {
            T res = Resources.Load<T>(path);
            if(res!=null)
            return res;
            else
            {
                Utility.DebugError("Assets: " + path + "\n not exist,check your path!");
                return null;
            }
        }
        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAysnc<T>(string path,CFAction<T> callBack=null)
            where T:UnityEngine.Object
        {
            Facade.Instance.StartCoroutine(EnumLoadAsync(path, callBack));
        }
         IEnumerator EnumLoadAsync<T>(string path, CFAction<T> callBack = null)
            where T : UnityEngine.Object
        {
            ResourceRequest req = Resources.LoadAsync<T>(path);
            yield return req;
            if (callBack != null)
                callBack(req.asset as T);
        }
        public  List<T> LoadFolderAssets<T>(string path)//通过路径载入资源
       where T : class
        {
            List<T> list = new List<T>();
            var resList = Resources.LoadAll(path, typeof(T)) as T[];
            for (int i = 0; i < resList.Length; i++)
            {
                list.Add(resList[i]);
            }
            return list;
        }
    }
    #endregion
}

