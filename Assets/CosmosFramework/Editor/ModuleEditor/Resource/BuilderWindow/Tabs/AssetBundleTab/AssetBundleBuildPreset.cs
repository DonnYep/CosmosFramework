using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class AssetBundleBuildPreset : ScriptableObject
    {
        /// <summary>
        /// 打包数据
        /// </summary>
        [SerializeField] AssetBundleBuildPresetData assetBundleSettingsData;
        public AssetBundleBuildPresetData AssetBundleSettingsData
        {
            get
            {
                if (assetBundleSettingsData == null)
                    assetBundleSettingsData = new AssetBundleBuildPresetData();
                return assetBundleSettingsData;
            }
            set { assetBundleSettingsData = value; }
        }
        public void Reset()
        {
            assetBundleSettingsData = new AssetBundleBuildPresetData();
        }
    }
}
