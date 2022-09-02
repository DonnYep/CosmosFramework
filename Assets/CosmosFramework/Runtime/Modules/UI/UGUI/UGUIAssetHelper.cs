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
        public UGUIAssetHelper() : this(CosmosEntry.UIManager.Instance().transform) { }
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
                go.transform.SetAlignParent(uiRoot);
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
        /// <summary>
        /// 附加UIForm到一个UIForm上；
        /// </summary>
        /// <param name="src">挂载到其他对象</param>
        /// <param name="dst">被其他对象挂载</param>
        public void AttachTo(IUIForm src, IUIForm dst)
        {
            var srcTrans = src.Handle.CastTo<GameObject>().transform;
            var dstTrans = dst.Handle.CastTo<GameObject>().transform;
            srcTrans.SetParent(dstTrans);
            (srcTrans as RectTransform).ResetRectTransform();
        }
        /// <summary>
        /// 移除挂载；
        /// </summary>
        /// <param name="src">原本挂在其他form的对象</param>
        /// <param name="dst">原本被其他对象挂着的form</param>
        public void DetachFrom(IUIForm src, IUIForm dst)
        {
            dst.Handle.CastTo<GameObject>().transform.SetParent(null);
        }
    }
}
