using UnityEngine;
using Cosmos.Resource;
using System.IO;
using System;

namespace Cosmos
{
    [DefaultExecutionOrder(2000)]
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
        [SerializeField] string resourceBundlePath;
        [SerializeField] string customeResourceBundlePath;
        public const string NONE = "<NONE>";
        /// <summary>
        /// AB包所在位置；
        /// </summary>
        public string ResourceBundlePath
        {
            get { return resourceBundlePath; }
            set { resourceBundlePath = value; }
        }
        /// <summary>
        /// AB包所在位置的路径类型；
        /// </summary>
        public ResourceBundlePathType ResourceBundlePathType
        {
            get { return resourceBundlePathType; }
            set { resourceBundlePathType = value; }
        }
        /// <summary>
        /// 自定义AB地址
        /// </summary>
        public string CustomeResourceBundlePath
        {
            get { return customeResourceBundlePath; }
            set { customeResourceBundlePath = value; }
        }
        protected virtual void Awake()
        {
            LoadHelpers();
            CosmosEntry.PrintModulePreparatory = printModulePreparatory;
            if (launchAppDomainModules)
            {
                CosmosEntry.LaunchAppDomainModules();
                switch (resourceLoadMode)
                {
                    case ResourceLoadMode.Resource:
                        CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, new ResourcesLoader());
                        break;
                    case ResourceLoadMode.AssetBundle:
                        {
                            if (string.IsNullOrEmpty(resourceBundlePath))
                                throw new Exception("Relative Bundle Path is invalid !");
                            ResourceManifest resourceManifest = null;
                            string bundleFolderPath = string.Empty;
                            switch (resourceBundlePathType)
                            {
                                case ResourceBundlePathType.StreamingAssets:
                                    bundleFolderPath = Path.Combine(Application.streamingAssetsPath, resourceBundlePath);
                                    break;
                                case ResourceBundlePathType.PersistentDataPath:
                                    bundleFolderPath = Path.Combine(Application.persistentDataPath, resourceBundlePath);
                                    break;
                            }
                            if (!Directory.Exists(bundleFolderPath))
                                throw new DirectoryNotFoundException(string.Format("Path {0} not found !", bundleFolderPath));
                            var manifestPath = Path.Combine(bundleFolderPath, ResourceConstants.RESOURCE_MANIFEST);
                            if (!File.Exists(manifestPath))
                                throw new FileNotFoundException(string.Format("File {0} not found !", manifestPath));
                            var manifestJson = Utility.IO.ReadTextFileContent(manifestPath);
                            try
                            {
                                resourceManifest = Utility.Json.ToObject<ResourceManifest>(manifestJson);
                            }
                            catch { }
                            if (resourceManifest != null)
                                CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, new AssetBundleLoader(resourceManifest));
                            else
                                throw new Exception("ResourceManifest deserialization failed , check your file !");
                        }
                        break;
                    case ResourceLoadMode.AssetDatabase:
                        CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, new AssetDatabaseLoader(resourceDataset));
                        break;
                    case ResourceLoadMode.CustomLoader:
                        {
                            var loadHelper = Utility.Assembly.GetTypeInstance(resourceLoaderName);
                            if (loadHelper != null)
                                CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, (IResourceLoadHelper)loadHelper);
                        }
                        break;
                }
            }
            Application.runInBackground = runInBackground;
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