// Stub facade for the legacy compile-check (Main module internals are out of scope).
namespace Cosmos
{
    internal static class GameManager
    {
        public static T GetModule<T>() where T : class, IModuleManager { return null; }
        public static int ModuleCount { get { return 0; } }
    }

    public static class CosmosEntry
    {
        public static Cosmos.Resource.IResourceManager ResourceManager { get { return null; } }
        public static Cosmos.WebRequest.IWebRequestManager WebRequestManager { get { return null; } }
        public static Cosmos.Download.IDownloadManager DownloadManager { get { return null; } }
    }
}
