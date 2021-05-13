using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cosmos.UI;
using Cosmos.Mvvm;
namespace Cosmos.Test
{
    [DefaultExecutionOrder(1100)]
    public class Entry : MonoBehaviour
    {
        private void Awake()
        {
            CosmosEntry.LaunchAppDomainHelpers();
            CosmosEntry.LaunchAppDomainModules();
            CosmosEntry.InputManager.SetInputDevice(new StandardInputDevice());
        }
    }
}