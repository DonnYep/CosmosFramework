using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace Cosmos.Test
{
    [Serializable]
    public struct FixTransform 
    {
        public FixTransform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Position = FixVector3.SetVector( position);
            Rotation = FixVector3.SetVector( rotation);
            Scale = FixVector3.SetVector( scale);
        }
        public FixVector3 Position { get; set; }
        public FixVector3 Rotation { get; set; }
        public FixVector3 Scale { get; set; }
        public override string ToString()
        {
            return $"Position :{Position} ; Rotation:{Rotation} ; Scale :{Scale}";
        }
    }
}