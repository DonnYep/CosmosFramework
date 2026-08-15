using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包加载器。
    /// <para>同一资源包只会存在一个加载器，多路并发加载共享同一个异步请求，避免瞬时大量加载造成的卡顿。</para>
    /// <para>加载器通过引用计数管理包体生命周期，所有依赖包会先于本体加载。</para>
    /// </summary>
    internal class BundleLoader : OperationBase
    {
        /// <summary>
        /// 包体名称
        /// </summary>
        public string BundleName { get; private set; }
        /// <summary>
        /// 加载完成的AssetBundle
        /// </summary>
        public AssetBundle AssetBundle { get; private set; }
        /// <summary>
        /// 引用计数
        /// </summary>
        public int RefCount { get; private set; }
        /// <summary>
        /// 依赖的包加载器集合
        /// </summary>
        public IReadOnlyList<BundleLoader> DependLoaders
        {
            get { return dependLoaders; }
        }

        readonly string bundleFilePath;
        readonly List<BundleLoader> dependLoaders = new List<BundleLoader>();
        AssetBundleCreateRequest createRequest;
        bool dependenciesChecked;

        internal BundleLoader(string bundleName, string bundleFilePath)
        {
            BundleName = bundleName;
            this.bundleFilePath = bundleFilePath;
        }

        public void AddRef()
        {
            RefCount++;
        }
        public void Release()
        {
            if (RefCount > 0)
                RefCount--;
        }
        /// <summary>
        /// 添加依赖加载器，并增加一个引用计数
        /// </summary>
        public void AddDependency(BundleLoader loader)
        {
            if (loader == null)
                return;
            dependLoaders.Add(loader);
            loader.AddRef();
        }
        /// <summary>
        /// 依赖是否全部加载完毕
        /// </summary>
        public bool DependenciesDone
        {
            get
            {
                if (dependenciesChecked)
                    return true;
                var count = dependLoaders.Count;
                for (int i = 0; i < count; i++)
                {
                    if (dependLoaders[i].IsDone == false)
                        return false;
                }
                dependenciesChecked = true;
                return true;
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnUpdate()
        {
            if (IsDone)
                return;
            if (!DependenciesDone)
                return;
            if (AssetBundle != null)
            {
                Progress = 1;
                Status = OperationStatus.Succeeded;
                return;
            }
            if (createRequest == null)
            {
                createRequest = AssetBundle.LoadFromFileAsync(bundleFilePath);
                if (createRequest == null)
                {
                    InvokeFailure();
                    return;
                }
            }
            if (createRequest.isDone)
            {
                AssetBundle = createRequest.assetBundle;
                if (AssetBundle == null)
                {
                    InvokeFailure();
                    return;
                }
                Progress = 1;
                Status = OperationStatus.Succeeded;
            }
            else
            {
                // 依赖与自身各占一半进度
                Progress = createRequest.progress * 0.5f + 0.5f;
            }
        }

        void InvokeFailure()
        {
            Error = $"Failed to load bundle : {BundleName}";
            Status = OperationStatus.Failed;
        }

        /// <summary>
        /// 卸载包体
        /// </summary>
        public void Unload()
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(false);
                AssetBundle = null;
            }
            createRequest = null;
        }

        /// <summary>
        /// 销毁加载器
        /// </summary>
        public void Destroy()
        {
            Unload();
            dependLoaders.Clear();
            if (!IsDone)
                Abort();
        }
    }
}
