using UnityEngine;
using Cosmos.Resource;
using System.IO;
using System;

namespace Cosmos
{
    [DisallowMultipleComponent]
    public class CosmosConfig : MonoBehaviour
    {
        [SerializeField] bool launchAppDomainModules = true;
        [SerializeField] bool printModulePreparatory = false;
        [SerializeField] ResourceLoadMode resourceLoadMode;
        [SerializeField] string resourceLoaderName;
        [SerializeField] int resourceLoaderIndex;

        [SerializeField] int debugHelperIndex;
        [SerializeField] int jsonHelperIndex;
        [SerializeField] int messagePackHelperIndex;

        [SerializeField] string debugHelperName;
        [SerializeField] string jsonHelperName;
        [SerializeField] string messagePackHelperName;

        [SerializeField] bool runInBackground;

        [SerializeField] bool autoInitResource = true;
        [SerializeField] ResourceDataset resourceDataset;
        [SerializeField] ResourceBundlePathType resourceBundlePathType;
        [SerializeField] string relativeBundlePath;

        [SerializeField] bool assetBundleEncrytion = false;
        [SerializeField] ulong assetBundleEncrytionOffset = 16;

        [SerializeField] bool buildInfoEncrytion = false;
        [SerializeField] string buildInfoEncrytionKey = "CosmosBundlesKey";

        public const string NONE = "<NONE>";
        public void LoadResource()
        {
            switch (resourceLoadMode)
            {
                case ResourceLoadMode.Resource:
                    CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, new ResourcesLoader());
                    break;
                case ResourceLoadMode.AssetBundle:
                    {
                        string manifestPath = string.Empty;
                        string bundlePath = string.Empty;
                        string prefix = string.Empty;
#if UNITY_EDITOR_WIN || UNITY_ANDROID || UNITY_STANDALONE
#elif UNITY_IOS ||UNITY_EDITOR_OSX||UNITY_STANDALONE_OSX
                        prefix=@"file://";
#endif
                        switch (resourceBundlePathType)
                        {
                            case ResourceBundlePathType.StreamingAssets:
                                bundlePath = Application.streamingAssetsPath;
                                break;
                            case ResourceBundlePathType.PersistentDataPath:
                                bundlePath = Application.persistentDataPath;
                                break;
                        }
                        ResourceDataProxy.ResourceBundlePathType = resourceBundlePathType;
                        if (!string.IsNullOrEmpty(relativeBundlePath))
                        {
                            manifestPath = Path.Combine(bundlePath, relativeBundlePath, ResourceConstants.RESOURCE_MANIFEST);
                            ResourceDataProxy.BundlePath = Path.Combine(bundlePath, relativeBundlePath);
                        }
                        else
                        {
                            manifestPath = Path.Combine(bundlePath, ResourceConstants.RESOURCE_MANIFEST);
                            ResourceDataProxy.BundlePath = bundlePath;
                        }
                        manifestPath = prefix + manifestPath;
                        //ResourceDataProxy.BundlePath = ResourceDataProxy.BundlePath;
                        //webrequest需要加file://，System.IO不需要加。加载器使用的是unity原生的assetbundle.loadxxxx，属于IO，因此无需加前缀；
                        if (assetBundleEncrytion)
                            ResourceDataProxy.EncryptionOffset = assetBundleEncrytionOffset;
                        if (buildInfoEncrytion)
                            ResourceDataProxy.BuildInfoEncryptionKey = buildInfoEncrytionKey;
                        var assetBundleLoader = new AssetBundleLoader();
                        CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, assetBundleLoader);
                        CosmosEntry.ResourceManager.StartRequestManifest(manifestPath);
                    }
                    break;
                case ResourceLoadMode.AssetDatabase:
                    var assetDatabaseLoader = new AssetDatabaseLoader();
                    assetDatabaseLoader.SetResourceDataset(resourceDataset);
                    CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, assetDatabaseLoader);
                    break;
                case ResourceLoadMode.CustomLoader:
                    {
                        if (string.IsNullOrEmpty(resourceLoaderName) || resourceLoaderName == NONE)
                        {
                            throw new Exception("CustomLoader is invalid !");
                        }
                        var loadHelper = Utility.Assembly.GetTypeInstance(resourceLoaderName);
                        if (loadHelper != null)
                            CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, (IResourceLoadHelper)loadHelper);
                    }
                    break;
            }
        }
        protected virtual void Awake()
        {
            LoadHelpers();
            CosmosEntry.PrintModulePreparatory = printModulePreparatory;
            Application.runInBackground = runInBackground;
            if (launchAppDomainModules)
            {
                CosmosEntry.LaunchAppDomainModules();
                if (autoInitResource)
                {
                    LoadResource();
                }
            }
        }
        void LoadHelpers()
        {
            if (!string.IsNullOrEmpty(debugHelperName) && debugHelperName != NONE)
            {
                var debugHelper = Utility.Assembly.GetTypeInstance(debugHelperName);
                if (debugHelper != null)
                    Utility.Debug.SetHelper((Utility.Debug.IDebugHelper)debugHelper);
            }
            if (!string.IsNullOrEmpty(jsonHelperName) && jsonHelperName != NONE)
            {
                var jsonHelper = Utility.Assembly.GetTypeInstance(jsonHelperName);
                if (jsonHelper != null)
                    Utility.Json.SetHelper((Utility.Json.IJsonHelper)jsonHelper);
            }
            if (!string.IsNullOrEmpty(messagePackHelperName) && messagePackHelperName != NONE)
            {
                var messagePackHelper = Utility.Assembly.GetTypeInstance(messagePackHelperName);
                if (messagePackHelper != null)
                    Utility.MessagePack.SetHelper((Utility.MessagePack.IMessagePackHelper)messagePackHelper);
            }
        }
    }
}