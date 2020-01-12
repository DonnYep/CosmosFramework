using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Test{
    public class FSMTester: MonoBehaviour {
        private void Start()
        {
            PrintEnun();
        }
        public void PrintEnun()
        {
            Utility.DebugLog(ContainerState.Empty.ToString());
        }
    }
}