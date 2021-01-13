using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PrefabAssetAttribute : AssetAttribute
    {
        public PrefabAssetAttribute(string resourcePath) : base(resourcePath){}
        public PrefabAssetAttribute(string assetBundleName, string assetPath, string resourcePath) : base(assetBundleName, assetPath, resourcePath){}
    }
}