using UnityEditor;
namespace Cosmos.Editor.Resource
{
    public interface IResourceBuildHandler
    {
        void OnBuildStart(BuildTarget buildTarget, string assetBundleBuildPath);
        void OnBuildComplete(BuildTarget buildTarget, string assetBundleBuildPath);
    }
}
