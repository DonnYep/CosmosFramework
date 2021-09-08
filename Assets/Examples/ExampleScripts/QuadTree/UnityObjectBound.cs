using UnityEngine;
using Cosmos.QuadTree;
public class UnityObjectBound : IObjecRectangletBound<GameObject>
{
    public float GetHeight(GameObject go)
    {
          return 1;
    }
    public float GetPositonX(GameObject go)
    {
        return go.transform.position.x;
    }
    public float GetPositonY(GameObject go)
    {
        return go.transform.position.z;
    }
    public float GetWidth(GameObject go)
    {
        return 1;
    }
}
