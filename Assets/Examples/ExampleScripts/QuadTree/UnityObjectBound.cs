using UnityEngine;
using Cosmos.QuadTree;
public class UnityObjectBound : IObjecBound<GameObject>
{
    public float GetHeight(GameObject go)
    {
          return 1;
    }
    public float GetWidth(GameObject go)
    {
        return 1;
    }
    public float GetCenterX(GameObject go)
    {
        return go.transform.position.x;
    }
    public float GetCenterY(GameObject go)
    {
        return go.transform.position.z;
    }
}
