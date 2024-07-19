using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.Resource
{
    internal class ResourceMap
    {
        /// <summary>
        /// bundleKey===bundleName
        /// </summary>
        readonly Dictionary<string, string> resourceBundleKeyDict;
        /// <summary>
        /// assetPath===resourceObjectWarpper
        /// <para>理论上资源地址在unity中应该是唯一的</para> 
        /// <para>资源地址相同但文件bytes内容改变，打包时生成的hash也会与之不同。因此理论上应该是assetPath是唯一的</para>
        /// </summary>
        readonly Dictionary<string, ResourceObjectWarpper> resourceObjectWarpperDict;
        /// <summary>
        /// bundleName===resourceBundleWarpper
        /// <para>从框架的角度出发，资源bundle设计上就是以文件夹做包体单位。且编辑器做了限制。因此在原生的模块中，理论上bundleName是唯一的</para> 
        /// </summary>
        readonly Dictionary<string, ResourceBundleWarpper> resourceBundleWarpperDict;
        /// <summary>
        /// 资源寻址地址
        /// </summary>
        readonly ResourceAddress resourceAddress;
        public ResourceMap()
        {
            resourceAddress = new ResourceAddress();
            resourceBundleKeyDict = new Dictionary<string, string>();
            resourceBundleWarpperDict = new Dictionary<string, ResourceBundleWarpper>();
            resourceObjectWarpperDict = new Dictionary<string, ResourceObjectWarpper>();
        }
        public void Initialize(IEnumerable<ResourceBundle> bundles)
        {
            //foreach (var resourceBundle in bundles)
            //{
            //    var resourceBundleWarpper = new ResourceBundleWarpper(resourceBundle, bundlePath, bundleBuildInfo.BudleExtension, resourceManifest.BundleOffset);
            //    resourceBundleWarpperDict.TryAdd(resourceBundle.BundleName, resourceBundleWarpper);
            //    resourceBundleKeyDict.TryAdd(resourceBundle.BundleKey, resourceBundle.BundleName);
            //    var resourceObjectList = resourceBundle.ResourceObjectList;
            //    var objectLength = resourceObjectList.Count;
            //    for (int i = 0; i < objectLength; i++)
            //    {
            //        var resourceObject = resourceObjectList[i];
            //        resourceObjectWarpperDict.TryAdd(resourceObject.ObjectPath, new ResourceObjectWarpper(resourceObject));
            //    }
            //    resourceAddress.AddResourceObjects(resourceObjectList);
            //}
        }
    }
}
