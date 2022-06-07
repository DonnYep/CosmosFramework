using Cosmos.Resource;
using Cosmos.UI;
using System;
using UnityEngine;
namespace Cosmos
{
    public class DefaultUIFormAssetHelper : IUIFormAssetHelper
    {
        IResourceManager ResourceManager { get { return CosmosEntry.ResourceManager; } }
        Type uiFromBaseType = typeof(IUIForm);
        ///<inheritdoc/>
        public Coroutine InstanceUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<IUIForm> doneCallback)
        {
            if (!uiFromBaseType.IsAssignableFrom(uiType))
                throw new NotImplementedException($"Type:{uiType} is not inherit form UIFormBase");
            if (string.IsNullOrEmpty(assetInfo.UIFormName))
                throw new ArgumentException("UIFormName is invalid !");
            return ResourceManager.LoadPrefabAsync(assetInfo.AssetName, go =>
            {
                go.transform.ResetLocalTransform();
                var comp = go.GetOrAddComponent(uiType) as IUIForm;
                doneCallback?.Invoke(comp);
            }, null, true);
        }
        ///<inheritdoc/>
        public void CloseUIForm(IUIForm uiForm)
        {
            ResourceManager.UnloadAsset(uiForm.UIAssetInfo.AssetName);
            GameObject.Destroy(uiForm.Handle.CastTo<GameObject>());
        }
        ///<inheritdoc/>
        public void AttachTo(IUIForm src, IUIForm dst)
        {
            var srcTrans = src.Handle.CastTo<GameObject>().transform;
            var dstTrans = dst.Handle.CastTo<GameObject>().transform;
            srcTrans.SetParent(dstTrans);
            (srcTrans as RectTransform).ResetRectTransform();
        }
        ///<inheritdoc/>
        public void DetachFrom(IUIForm src, IUIForm dst)
        {
            dst.Handle.CastTo<GameObject>().transform.SetParent(null);
        }
        ///<inheritdoc/>
        public void AttachTo(IUIForm src,  Transform dst)
        {
           var trans= src.Handle.CastTo<GameObject>().transform;
            trans.SetParent(dst);
            (trans as RectTransform).ResetRectTransform();
        }
    }
}
