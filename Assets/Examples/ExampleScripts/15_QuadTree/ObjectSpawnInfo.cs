using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectSpawnInfo
{
    public Vector3 Position { get; set; }
    public ObjectSpawnInfo(Vector3 position)
    {
        Position = position;
    }
}
