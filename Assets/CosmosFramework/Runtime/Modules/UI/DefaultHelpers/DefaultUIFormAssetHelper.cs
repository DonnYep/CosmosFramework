using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos.UI
{
    public class DefaultUIFormAssetHelper : IUIFormAssetHelper
    {
        IResourceManager ResourceManager { get { return CosmosEntry.ResourceManager; } }
        Type uiFromBaseType = typeof(IUIForm);
        public IUIForm InstanceUIForm(UIAssetInfo assetInfo, Type uiType)
        {
            if (assetInfo == null)
                throw new ArgumentNullException("UIAssetInfo is invalid !");
            if (!uiFromBaseType.IsAssignableFrom(uiType))
                throw new NotImplementedException($"Type:{uiType} is not inherit form UIFormBase");
            if (string.IsNullOrEmpty(assetInfo.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            var panel = ResourceManager.LoadPrefab(assetInfo);
            var comp = panel.GetOrAddComponent(uiType) as IUIForm;
            return comp;
        }
        public Coroutine InstanceUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<IUIForm> doneCallback)
        {
            if (assetInfo == null)
                throw new ArgumentNullException("UIAssetInfo is invalid !");
            if (!uiFromBaseType.IsAssignableFrom(uiType))
                throw new NotImplementedException($"Type:{uiType} is not inherit form UIFormBase");
            if (string.IsNullOrEmpty(assetInfo.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            return ResourceManager.LoadPrefabAsync(assetInfo, go =>
            {
                go.transform.ResetLocalTransform();
                var comp = go.GetOrAddComponent(uiType) as IUIForm;
                doneCallback?.Invoke(comp);
            }, null, true);
        }
        public void ReleaseUIForm(IUIForm uiForm)
        {
            GameObject.Destroy(uiForm.Handle.CastTo<GameObject>());
        }
        public void AttachTo(IUIForm src, IUIForm dst)
        {
            var srcTrans = src.Handle.CastTo<GameObject>().transform;
            var dstTrans=dst.Handle.CastTo<GameObject>().transform;
            srcTrans.SetParent(dstTrans);
            (srcTrans as RectTransform).ResetRectTransform();
        }
        public void DetachFrom(IUIForm src, IUIForm dst)
        {
            dst.Handle.CastTo<GameObject>().transform.SetParent(null);
        }
        public void AttachTo(IUIForm src,  Transform dst)
        {
           var trans= src.Handle.CastTo<GameObject>().transform;
            trans.SetParent(dst);
            (trans as RectTransform).ResetRectTransform();
        }
    }
}
