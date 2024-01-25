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
        [SerializeField] bool printLogWhenAssetNotExists;

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

        [SerializeField] bool manifestEncrytion = false;
        [SerializeField] string manifestEncrytionKey = "CosmosBundlesKey";

        [SerializeField] bool drawDebugWindow;
        [SerializeField] bool disableDebugLog = false;

        [SerializeField] int inputHelperIndex;
        [SerializeField] string inputHelperName;

        [SerializeField] bool moduleConfigFoldout;
        public void LoadResource()
        {
            ResourceDataProxy.PrintLogWhenAssetNotExists = printLogWhenAssetNotExists;
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
                        string formattedBundlePath = string.Empty;
                        if (!string.IsNullOrEmpty(relativeBundlePath))
                        {
                            manifestPath = Path.Combine(bundlePath, relativeBundlePath, ResourceConstants.RESOURCE_MANIFEST);
                            formattedBundlePath = Utility.IO.CombineURL(bundlePath, relativeBundlePath);
                        }
                        else
                        {
                            manifestPath = Path.Combine(bundlePath, ResourceConstants.RESOURCE_MANIFEST);
                            formattedBundlePath = bundlePath;
                        }
                        manifestPath = prefix + manifestPath;
                        //webrequest需要加file://，System.IO不需要加。加载器使用的是unity原生的assetbundle.loadxxxx，属于IO，因此无需加前缀；
                        string formattedManifestEncrytionKey = string.Empty;
                        if (manifestEncrytion)
                            formattedManifestEncrytionKey = manifestEncrytionKey;
                        else
                            formattedManifestEncrytionKey = string.Empty;
                        var assetBundleLoader = new AssetBundleLoader();

                        CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, assetBundleLoader);
                        var taskId = CosmosEntry.ResourceManager.StartRequestManifest(manifestPath, formattedManifestEncrytionKey, formattedBundlePath);
                        assetBundleLoader.TaskId = taskId;
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
            Utility.Debug.DisableDebugLog = disableDebugLog;
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