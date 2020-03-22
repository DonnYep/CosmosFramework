using UnityEngine;
using System.Collections;
using System;
namespace Cosmos.Resource
{
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
        /// Asset的路径
        /// </summary>
        string assetPath;
        public string AssetPath { get { return assetPath; } }
        /// <summary>
        /// Resources文件夹中的路径
        /// </summary>
        string resourcePath;
        public string RessourcePath { get { return resourcePath; } }


        public ResourceUnit(string assetBundleName, string assetPath, string resourcePath)
        {
            this.assetBundleName = assetBundleName;
            this.assetPath = assetPath;
            this.resourcePath = resourcePath;
        }


        public void Dispose(){ }

    }
}