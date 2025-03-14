using Cosmos.Operation;
using Cosmos.UI;
using System;
using System.Collections.Generic;

namespace Cosmos.Resource
{
    /// <summary>
    ///  资源包裹，一个资源包含多个PackageBundle
    /// </summary>
    public class ResourcePackage
    {
        readonly PackageAddress packageAddress = new PackageAddress();

        readonly string packageName;
        public string PackageName
        {
            get { return packageName; }
        }
        private ResourcePackage()
        {
        }
        internal ResourcePackage(string packageName)
        {
            this.packageName = packageName;
        }

        public void UnloadAsset(string asset)
        {
            //UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(null);
        }
        /// <summary>
        /// 异步加载资产。
        /// <para>assetKey能够识别以下几种类型：assetName，assetName.ext，assetPath</para>
        /// <para>assetName: 资产文件名，e.g. myText</para>
        /// <para>assetName.ext: 资产文件名加上后缀，e.g. myText.txt</para>
        /// <para>assetPath: 资产路径，e.g. Assets/Text/myText.txt</para>
        /// </summary>
        /// <typeparam name="T">资产类型</typeparam>
        /// <param name="assetKey">资产识别标识</param>
        /// <returns>加载的handle</returns>
        public AssetHandle<T> LoadAssetAsync<T>(string assetKey)
            where T : UnityEngine.Object
        {
            var assetInfo = GetAssetInfo(assetKey);
            var provider = GetProvider(assetInfo, typeof(T));
            AssetHandle<T> handle = new AssetHandle<T>(provider);
            InternalStartOperation(provider);
            return handle;
        }

        internal InitializationOperation InitializeAsync()
        {
            //获取全局配置或者传入参数
            InitializationOperation operation = default;
            switch (CFResources.ResourcePlayMode)
            {
                case CFResourcePlayMode.AssetDatabase:

                    break;
                case CFResourcePlayMode.EditorAssetBundle:
                    break;
                case CFResourcePlayMode.AssetBundle:
                    break;
                case CFResourcePlayMode.Offline:
                    break;
            }
            operation.Completed += OperationCompleted;
            return operation;
        }

        private void OperationCompleted(OperationBase operation)
        {

        }

        internal void InternalStartOperation(OperationBase operationBase)
        {
            OperationSystem.StartOperation(operationBase);
        }
        ProviderBase GetProvider(AssetInfo assetInfo, Type assetType)
        {
            //这里获取时，可以从正在加载中的池中获取。获取的对象必须与assetInfo对应的资产一致。
            ProviderBase provider = default;
            if (CFResources.ResourcePlayMode == CFResourcePlayMode.AssetDatabase)
            {
                var assetProvider = new DatabaseAssetProvider(assetInfo, assetType);
                provider = assetProvider;
            }
            else
            {
            }
            return provider;
        }
        AssetInfo GetAssetInfo(string assetKey)
        {
            packageAddress.PeekAssetInfo(assetKey, out var assetInfo);
            return assetInfo;
        }
    }
}
