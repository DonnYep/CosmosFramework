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


        protected virtual void Awake()
        {
            var debugHelper = Utility.Assembly.GetTypeInstance(debugHelperName);
            if (debugHelper != null)
            {
                Utility.Debug.SetHelper((Utility.Debug.IDebugHelper)debugHelper);
            }
            var jsonHelper = Utility.Assembly.GetTypeInstance(jsonHelperName);
            if (jsonHelper != null)
            {
                Utility.Json.SetHelper((Utility.Json.IJsonHelper)jsonHelper);
            }
            var messagePackHelper = Utility.Assembly.GetTypeInstance(messagePackHelperName);
            if (messagePackHelper != null)
            {
                Utility.MessagePack.SetHelper((Utility.MessagePack.IMessagePackHelper)messagePackHelper);
            }
            CosmosEntry.PrintModulePreparatory = printModulePreparatory;
            if (launchAppDomainModules)
            {
                CosmosEntry.LaunchAppDomainModules();
                var loadHelper = Utility.Assembly.GetTypeInstance(resourceLoaderName);
                if (loadHelper != null)
                {
                    CosmosEntry.ResourceManager.AddOrUpdateLoadHelper(resourceLoadMode, (IResourceLoadHelper)loadHelper);
                    CosmosEntry.ResourceManager.SwitchLoadMode(resourceLoadMode);
                }
            }
        }
    }
}