using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class AssetBundleBuildPreset : ScriptableObject
    {
        /// <summary>
        /// 打包数据
        /// </summary>
        [SerializeField] AssetBundleBuildPresetData assetBundleBuildPresetData;
        public AssetBundleBuildPresetData AssetBundleBuildPresetData
        {
            get
            {
                if (assetBundleBuildPresetData == null)
                    assetBundleBuildPresetData = new AssetBundleBuildPresetData();
                return assetBundleBuildPresetData;
            }
            set { assetBundleBuildPresetData = value; }
        }
        public void Reset()
        {
            assetBundleBuildPresetData = new AssetBundleBuildPresetData();
        }
    }
}
