namespace Cosmos
{
    public class AddressableAssetAttribute : AssetAttribute
    {
        public AddressableAssetAttribute(string addressablePath) : base(addressablePath){}
        public AddressableAssetAttribute(string assetBundleName, string assetPath, string resourcePath) 
            : base(assetBundleName, assetPath, resourcePath){}
    }
}