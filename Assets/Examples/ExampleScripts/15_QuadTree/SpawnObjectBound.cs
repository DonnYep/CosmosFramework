using Cosmos;
using UnityEngine;

public class SpawnObjectBound :QuadTree<GameObject> .IObjecBound
{
    public float GetHeight(GameObject go)
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
    public float GetWidth(GameObject go)
    {
        return 1;
    }
}
