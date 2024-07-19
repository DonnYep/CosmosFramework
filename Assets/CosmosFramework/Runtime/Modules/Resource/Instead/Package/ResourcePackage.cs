using Cosmos.Resource.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资产包
    /// </summary>
    public class ResourcePackage
    {
        readonly string packageName;
        public string PackageName
        {
            get { return packageName; }
        }
        /// <summary>
        /// 资源寻址地址
        /// </summary>
        readonly ResourceAddress resourceAddress;
        private ResourcePackage()
        {
        }
        internal ResourcePackage(string packageName)
        {
            this.packageName = packageName;
            resourceAddress=new ResourceAddress();
        }
        public ResourceVersion GetResourceVersion()
        {
            return default;
        }
        public ResourceBundleState GetBundleState()
        {
            return default;
        }
        public void UnloadAsset(string assetName)
        {
            UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(null);
        }
        public void LoadAssetAsync(string assetName)
        {

        }

    }
}
