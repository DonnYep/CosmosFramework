using System;
using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// editor资源加载器，这里需要统一进行释放处理
    /// </summary>
    internal class DatabaseAssetProvider : ProviderBase
    {
        internal DatabaseAssetProvider(AssetInfo assetInfo,Type assetType)
        {
            this.MainAssetInfo = assetInfo;
            this.AssetType = assetType;
        }

        internal override void InternalOnStart()
        {
        }

        internal override void InternalOnUpdate()
        {
#if UNITY_EDITOR
            if (IsDone)
                return;
            if (currentStep == ProviderStep.None)
            {
                string guid = UnityEditor.AssetDatabase.AssetPathToGUID(MainAssetInfo.AssetPath);
                if (string.IsNullOrEmpty(guid))
                {
                    string error = $"Asset not found - {MainAssetInfo.AssetPath}";
                    InvokeCompletion(error, OperationStatus.Failed);
                    return;
                }
                currentStep = ProviderStep.CheckBundle;
            }
            if (!IsWaitForAsyncComplete)
                return;
            if (currentStep == ProviderStep.CheckBundle)
            {
                if (IsWaitForAsyncComplete)
                {
                    //暂时不处理
                }
            }
            currentStep = ProviderStep.Loading;

            if (currentStep == ProviderStep.Loading)
            {
                if (AssetType == null)
                    AssetObject = UnityEditor.AssetDatabase.LoadMainAssetAtPath(MainAssetInfo.AssetPath);
                else
                    AssetObject = UnityEditor.AssetDatabase.LoadAssetAtPath(MainAssetInfo.AssetPath, AssetType);
                currentStep = ProviderStep.Checking;
            }

            if (currentStep == ProviderStep.Checking)
            {
                if (AssetObject == null)
                {
                    string error;
                    if (AssetType == null)
                        error = $"Failed to load asset object : {MainAssetInfo.AssetPath} AssetType : null";
                    else
                        error = $"Failed to load asset object : {MainAssetInfo.AssetPath} AssetType : {AssetType}";
                    InvokeCompletion(error, OperationStatus.Failed);
                }
                else
                {
                    InvokeCompletion(string.Empty, OperationStatus.Succeeded);
                }
            }
#endif
        }

        internal override void WaitForCompletion()
        {
            //一千帧，覆盖大部分刷新率的设备
            int frame = 1000;
            while (true)
            {
                frame--;
                if (frame == 0)
                {
                    if (!IsDone == false)
                    {
                        Status = OperationStatus.Failed;
                        Debug.LogError("加载错误");
                    }
                    break;
                }

                // 主线程阻塞状态下刷新
                InternalOnUpdate();

                // 完成后退出
                if (IsDone)
                    break;
            }
        }
    }
}
