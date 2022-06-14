using UnityEngine;
using Cosmos.Resource;

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
        const string NONE = "<NONE>";
        protected virtual void Awake()
        {
            if (!string.IsNullOrEmpty(debugHelperName)&&debugHelperName!= NONE)
            {
                var debugHelper = Utility.Assembly.GetTypeInstance(debugHelperName);
                if (debugHelper != null)
                {
                    Utility.Debug.SetHelper((Utility.Debug.IDebugHelper)debugHelper);
                }
            }
            if (!string.IsNullOrEmpty(jsonHelperName)&&jsonHelperName!=NONE)
            {
                var jsonHelper = Utility.Assembly.GetTypeInstance(jsonHelperName);
                if (jsonHelper != null)
                {
                    Utility.Json.SetHelper((Utility.Json.IJsonHelper)jsonHelper);
                }
            }
            if (!string.IsNullOrEmpty(messagePackHelperName)&&messagePackHelperName!=NONE)
            {
                var messagePackHelper = Utility.Assembly.GetTypeInstance(messagePackHelperName);
                if (messagePackHelper != null)
                {
                    Utility.MessagePack.SetHelper((Utility.MessagePack.IMessagePackHelper)messagePackHelper);
                }
            }
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
                        CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, new AssetBundleLoader());
                        break;
                    case ResourceLoadMode.AssetDatabase:
                        CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, new AssetDatabaseLoader());
                        break;
                    case ResourceLoadMode.CustomLoader:
                        {
                            var loadHelper = Utility.Assembly.GetTypeInstance(resourceLoaderName);
                            if (loadHelper != null)
                            {
                                CosmosEntry.ResourceManager.SetDefaultLoadHeper(resourceLoadMode, (IResourceLoadHelper)loadHelper);
                            }
                        }
                        break;
                }
            }
            Application.runInBackground = runInBackground;
        }
    }
}