using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class AssetBundleBuildProfile : ScriptableObject
    {
        /// <summary>
        /// 打包数据
        /// </summary>
        [SerializeField] AssetBundleBuildProfileData assetBundleBuildProfileData;
        public AssetBundleBuildProfileData AssetBundleBuildProfileData
        {
            get
            {
                if (assetBundleBuildProfileData == null)
                    assetBundleBuildProfileData = new AssetBundleBuildProfileData();
                return assetBundleBuildProfileData;
            }
            set { assetBundleBuildProfileData = value; }
        }
        public void Reset()
        {
            assetBundleBuildProfileData = new AssetBundleBuildProfileData();
        }
    }
}
