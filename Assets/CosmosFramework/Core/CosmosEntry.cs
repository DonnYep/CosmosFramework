using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    [DefaultExecutionOrder(2000)]
    public class CosmosEntry : MonoBehaviour
    {
        private void Awake()
        {
            var debugHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IDebugHelper>();
            var jsonHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IJsonHelper>();
            var messagePackHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IMessagePackHelper>();
            Utility.Debug.SetHelper(debugHelper);
            Utility.Json.SetHelper(jsonHelper);
            Utility.MessagePack.SetHelper(messagePackHelper);
            GameManager.PreparatoryModule();
        }
    }
}