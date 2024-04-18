using Cosmos.Resource;
using Cosmos.UI;
using System;
using UnityEngine;
namespace Cosmos
{
    public class UGUIAssetHelper : IUIFormAssetHelper
    {
        IResourceManager resourceManager;
        Type uiFromBaseType;
        Transform uiRoot;
        public UGUIAssetHelper() : this(CosmosEntry.UIManager.InstanceObject().transform) { }
        public UGUIAssetHelper(Transform uiRoot)
        {
            this.uiRoot = uiRoot;
            resourceManager = CosmosEntry.ResourceManager;
            uiFromBaseType = typeof(IUIForm);
        }
        ///<inheritdoc/>
        public Coroutine InstanceUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<IUIForm> doneCallback)
        {
            if (!uiFromBaseType.IsAssignableFrom(uiType))
                throw new NotImplementedException($"Type:{uiType} is not inherit form UIFormBase");
            if (string.IsNullOrEmpty(assetInfo.UIFormName))
                throw new ArgumentException("UIFormName is invalid !");
            return resourceManager.LoadPrefabAsync(assetInfo.AssetName, go =>
            {
                go.transform.ResetLocalTransform();
                go.transform.SetAndAlignParent(uiRoot);
                var comp = go.GetOrAddComponent(uiType) as IUIForm;
                doneCallback?.Invoke(comp);
            }, null, true);
        }
        ///<inheritdoc/>
        public void ReleaseUIForm(IUIForm uiForm)
        {
            resourceManager.UnloadAsset(uiForm.UIAssetInfo.AssetName);
            GameObject.Destroy(uiForm.Handle.CastTo<GameObject>());
        }
    }
}
