using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    public abstract class AssetAttribute : Attribute
    {
        /// <summary>
        /// AB包名
        /// </summary>
        public string AssetBundleName { get; private set; }
        /// <summary>
        /// 基于AB包的资源的路径
        /// </summary>
        public string AssetPath { get; private set; }
        /// <summary>
        /// 基于Resource的资源路径
        /// </summary>
        public string ResourcePath { get; private set; }
        public AssetAttribute(string assetBundleName, string assetPath, string resourcePath)
        {
            AssetBundleName = assetBundleName;
            AssetPath = assetPath;
            ResourcePath = resourcePath;
        }
        public AssetAttribute(string resourcePath) : this(null, null, resourcePath)
        {
            ResourcePath = resourcePath;
        }
    }
}