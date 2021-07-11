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
        Type uiFromBaseType = typeof(UIForm);
        public UIForm InstanceUIForm(UIAssetInfo assetInfo, Type uiType)
        {
            if (assetInfo == null)
                throw new ArgumentNullException("UIAssetInfo is invalid !");
            if (!uiFromBaseType.IsAssignableFrom(uiType))
                throw new NotImplementedException($"Type:{uiType} is not inherit form UIFormBase");
            if (string.IsNullOrEmpty(assetInfo.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            var panel = ResourceManager.LoadPrefab(assetInfo);
            var comp = panel.GetOrAddComponent(uiType) as UIForm;
            return comp;
        }
        public Coroutine InstanceUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<UIForm> doneCallback)
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
                var comp = go.GetOrAddComponent(uiType) as UIForm;
                doneCallback?.Invoke(comp);
            }, null, true);
        }
        public void ReleaseUIForm(UIForm uiForm)
        {
            GameObject.Destroy(uiForm);
        }
    }
}
