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
            Utility.Debug.SetHelper(new UnityDebugHelper());
            GameManager.PreparatoryModule();
        }
    }
}