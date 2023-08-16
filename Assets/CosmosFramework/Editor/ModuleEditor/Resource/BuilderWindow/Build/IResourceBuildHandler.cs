namespace Cosmos.Editor.Resource
{
    public interface IResourceBuildHandler
    {
        void OnBuildPrepared(ResourceBuildParams buildParams);
        void OnBuildComplete(ResourceBuildParams buildParams);
    }
}
