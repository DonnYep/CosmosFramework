using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    public class ControllerEventArgs : GameEventArgs
    {
        public CameraTarget CameraTarget { get; set; }
        public override void Clear()
        {
            CameraTarget = null;
        }
    }
}