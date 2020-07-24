using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PrefabUnitAttribute : ResourceUnitAttribute
    {
        public string PrefabName { get; set; }
        public PrefabUnitAttribute(string resourcePath) : base(resourcePath)
        {
        }
        public PrefabUnitAttribute(string assetBundleName, string assetPath, string resourcePath) : base(assetBundleName, assetPath, resourcePath)
        {
        }
    }
}