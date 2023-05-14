using Cosmos.Resource;
namespace Cosmos.Editor.Resource
{
    public interface IResourceBuildHandler
    {
        void OnBuildPrepared(AssetBundleBuildParams buildParams);
        void OnBuildComplete(AssetBundleBuildParams buildParams);
    }
}
