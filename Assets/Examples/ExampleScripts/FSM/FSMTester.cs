using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cosmos;
    public class FSMTester: MonoBehaviour {
        [SerializeField]
        int lastFrame = -5;
        public void PrintFrameCount()
        {
            lastFrame = Time.frameCount;
            Utility.DebugLog("framecount:" + lastFrame);
        }
        private void Start()
        {
            Type type = this.GetType();
            Utility.DebugLog( type.ToString());

        }
    }
