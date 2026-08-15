using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 立即失败的资产提供器，用于资产不存在等场景的错误传递。
    /// </summary>
    internal class FailedAssetProvider : ProviderBase
    {
        readonly string failReason;

        internal FailedAssetProvider(AssetInfo assetInfo, Type assetType, string failReason)
        {
            this.MainAssetInfo = assetInfo;
            this.AssetType = assetType;
            this.failReason = failReason;
        }

        protected override void OnStart()
        {
            InvokeCompletion(failReason, OperationStatus.Failed);
        }

        protected override void OnUpdate()
        {
        }

        internal override void WaitForCompletion()
        {
        }
    }
}
