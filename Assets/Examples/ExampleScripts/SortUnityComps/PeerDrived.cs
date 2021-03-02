using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    public class PeerDrived : PeerBase
    {
        public override void PeerMsg()
        {
            Debug.Log(gameObject.name);
        }
    }
}