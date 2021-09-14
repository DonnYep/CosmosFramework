using Cosmos.QuadTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnObjectBound : IObjecRectangletBound<GameObject>
{
    public float GetHeight(GameObject go)
    {
        return 0.5f;
    }
    public float GetCenterX(GameObject go)
    {
        return go.transform.position.x;
    }
    public float GetCenterY(GameObject go)
    {
        return go.transform.position.z;
    }
    public float GetWidth(GameObject go)
    {
        return 0.5f;
    }
}
