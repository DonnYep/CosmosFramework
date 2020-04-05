using UnityEngine;
using System.Collections;
using System;
namespace Cosmos.Resource
{
    //TODO  资源单位专门供assetBundle使用，待完善方法
    /// <summary>
    /// 资源单元
    /// </summary>
    public class ResourceUnit : IDisposable
    {
        /// <summary>
        /// AssetBundle的名称
        /// </summary>
        string assetBundleName;
        public string AssetBundleName { get { return assetBundleName; } }
        /// <summary>
        /// 资产的路径
        /// </summary>
        string assetPath;
        public string AssetPath { get { return assetPath; } }
        ///// <summary>
        ///// Resources文件夹中的路径
        ///// </summary>
        //string resourcePath;
        //public string RessourcePath { get { return resourcePath; } }


        public ResourceUnit(string assetBundleName, string assetPath)
        {
            this.assetBundleName = assetBundleName;
            this.assetPath = assetPath;
        }
        public void Dispose(){ }

    }
}