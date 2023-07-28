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

        [SerializeField] ResourceDataset resourceDataset;
        [SerializeField] ResourceBundlePathType resourceBundlePathType;
        [SerializeField] string relativeBundlePath;

        [SerializeField] bool assetBundleEncrytion = false;
        [SerializeField] ulong assetBundleEncrytionOffset = 16;

        [SerializeField] bool manifestEncrytion = false;
        [SerializeField] string manifestEncrytionKey = "CosmosBundlesKey";

        [SerializeField] bool drawDebugWindow;

        [SerializeField] int inputHelperIndex;
        [SerializeField] string inputHelperName;
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
                        string prefix = ResourceUtility.Prefix;
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
                        //webrequest需要加file://，System.IO不需要加。加载器使用的是unity原生的assetbundle.loadxxxx，属于IO，因此无需加前缀；
                        if (assetBundleEncrytion)
                            ResourceDataProxy.BundleEncryptionOffset = assetBundleEncrytionOffset;
                        else
                            ResourceDataProxy.BundleEncryptionOffset = 0;
                        if (manifestEncrytion)
                            ResourceDataProxy.ManifestEncryptionKey = manifestEncrytionKey;
                        else
                            ResourceDataProxy.ManifestEncryptionKey = string.Empty;
                        var assetBundleLoader = new AssetBundleLoader();
                        CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, assetBundleLoader);
                        CosmosEntry.ResourceManager.StartRequestManifest(manifestPath, ResourceDataProxy.ManifestEncryptionKey);
                    }
                    break;
                case ResourceLoadMode.AssetDatabase:
                    var assetDatabaseLoader = new AssetDatabaseLoader();
                    assetDatabaseLoader.SetResourceDataset(resourceDataset);
                    CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, assetDatabaseLoader);
                    break;
                case ResourceLoadMode.CustomLoader:
                    {
                        if (string.IsNullOrEmpty(resourceLoaderName) || resourceLoaderName == Constants.NONE)
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
                LoadResource();
                SetInputHelper();
            }
        }
        void LoadHelpers()
        {
            if (!string.IsNullOrEmpty(debugHelperName) && debugHelperName != Constants.NONE)
            {
                var debugHelper = Utility.Assembly.GetTypeInstance(debugHelperName);
                if (debugHelper != null)
                    Utility.Debug.SetHelper((Utility.Debug.IDebugHelper)debugHelper);
            }
            if (!string.IsNullOrEmpty(jsonHelperName) && jsonHelperName != Constants.NONE)
            {
                var jsonHelper = Utility.Assembly.GetTypeInstance(jsonHelperName);
                if (jsonHelper != null)
                    Utility.Json.SetHelper((Utility.Json.IJsonHelper)jsonHelper);
            }
            if (!string.IsNullOrEmpty(messagePackHelperName) && messagePackHelperName != Constants.NONE)
            {
                var messagePackHelper = Utility.Assembly.GetTypeInstance(messagePackHelperName);
                if (messagePackHelper != null)
                    Utility.MessagePack.SetHelper((Utility.MessagePack.IMessagePackHelper)messagePackHelper);
            }
        }
        void SetInputHelper()
        {
            if (!string.IsNullOrEmpty(inputHelperName) && inputHelperName != Constants.NONE)
            {
                var inputHelper = Utility.Assembly.GetTypeInstance<Cosmos.Input.IInputHelper>(inputHelperName);
                if (inputHelper != null)
                    CosmosEntry.InputManager.SetInputHelper(inputHelper);
            }
        }
    }
}