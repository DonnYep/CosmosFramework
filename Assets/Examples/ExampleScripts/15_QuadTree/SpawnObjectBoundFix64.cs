using FixMath.NET;
using Cosmos;
using UnityEngine;

public class SpawnObjectBoundFix64 : QuadTreeFix64<GameObject>.IObjecBound
{
    public Fix64 GetHeight(GameObject go)
    {
        return (Fix64)1;
    }
    public Fix64 GetCenterX(GameObject go)
    {
        return (Fix64)go.transform.position.x;
    }
    public Fix64 GetCenterY(GameObject go)
    {
        return (Fix64)go.transform.position.z;
    }
    public Fix64 GetWidth(GameObject go)
    {
        return (Fix64)1;
    }
}
