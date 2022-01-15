using Cosmos.Resource;
namespace Cosmos.Audio
{
    public class AudioAssetInfo : AssetInfo
    {
        readonly string audioName;
        public string AudioName { get { return audioName; } }
        /// <summary>
        /// 声音组别；
        /// </summary>
        public string AudioGroupName{ get; set; }
        public AudioAssetInfo(string assetPath,string audioName) : base(assetPath)
        {
            this.audioName = audioName;
        }
        public AudioAssetInfo(string assetBundleName, string assetPath,string audioName) : base(assetBundleName, assetPath)
        {
            this.audioName = audioName;
        }
    }
}
