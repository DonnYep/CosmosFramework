using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 初始化操作基类。
    /// </summary>
    public abstract class InitializationOperation : OperationBase
    {
        /// <summary>
        /// 资源版本
        /// </summary>
        public string Version { get; protected set; }
        /// <summary>
        /// 资源包裹名
        /// </summary>
        public string PackageName { get; protected set; }
    }

    /// <summary>
    /// 资源包裹初始化操作：加载文件清单并构建运行时寻址索引。
    /// </summary>
    internal class PackageInitializationOperation : InitializationOperation
    {
        readonly ResourcePackage package;
        readonly ResourceInitParameters parameters;
        string manifestContext;
        bool contextReady;

        internal PackageInitializationOperation(ResourcePackage package, ResourceInitParameters parameters)
        {
            this.package = package;
            this.parameters = parameters;
        }

        protected override void OnStart()
        {
            PackageName = parameters.PackageName;
            try
            {
                manifestContext = ResourceUtility.LoadManifestContext(parameters.ManifestPath);
                contextReady = true;
            }
            catch (Exception e)
            {
                Error = $"Failed to load manifest : {e.Message}";
                Status = OperationStatus.Failed;
            }
        }

        protected override void OnUpdate()
        {
            if (IsDone)
                return;
            if (!contextReady)
                return;
            try
            {
                var manifest = PackageManifest.Deserialize(manifestContext, parameters.ManifestEncryptionKey);
                if (manifest == null)
                {
                    Error = "PackageManifest deserialization failed !";
                    Status = OperationStatus.Failed;
                    return;
                }
                manifest.PackageName = parameters.PackageName;
                manifest.BuildIndex();
                package.SetManifest(manifest);
                Version = manifest.PackageVersion;
                Progress = 1;
                Status = OperationStatus.Succeeded;
            }
            catch (Exception e)
            {
                Error = $"PackageManifest deserialization exception : {e}";
                Status = OperationStatus.Failed;
            }
        }
    }
}
