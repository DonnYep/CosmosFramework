using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    [DefaultExecutionOrder(2000)]
    public class CosmosEntry : MonoBehaviour
    {
        private void Awake()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var length = assemblies.Length;
            for (int i = 0; i < length; i++)
            {
                var helper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IDebugHelper>(assemblies[i]);
                if (helper != null)
                {
                    Utility.Debug.SetHelper(helper);
                    break;
                }
            }
            for (int i = 0; i < length; i++)
            {
                var helper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IJsonHelper>(assemblies[i]);
                if (helper != null)
                {
                    Utility.Json.SetHelper(helper);
                    break;
                }
            }
            for (int i = 0; i < length; i++)
            {
                var helper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IMessagePackHelper>(assemblies[i]);
                if (helper != null)
                {
                    Utility.MessagePack.SetHelper(helper);
                    break;
                }
            }
            GameManager.PreparatoryModule();
        }
    }
}