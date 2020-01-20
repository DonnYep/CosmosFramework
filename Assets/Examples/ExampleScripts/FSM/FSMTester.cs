using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Test{
    public class FSMTester: MonoBehaviour {
        [SerializeField]
        int lastFrame = -5;
        public void PrintFrameCount()
        {
            lastFrame = Time.frameCount;
            Utility.DebugLog("framecount:" + lastFrame);
        }
    }
}