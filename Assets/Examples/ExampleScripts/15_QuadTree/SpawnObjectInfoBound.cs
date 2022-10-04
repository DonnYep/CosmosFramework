using Cosmos;
public class SpawnObjectInfoBound :  QuadTree<ObjectSpawnInfo>.IObjecBound
{
    public float GetHeight(ObjectSpawnInfo go)
    {
        return 1;
    }
    public float GetCenterX(ObjectSpawnInfo go)
    {
        return go.Position.x;
    }
    public float GetCenterY(ObjectSpawnInfo go)
    {
        return go.Position.z;
    }
    public float GetWidth(ObjectSpawnInfo go)
    {
        return 1;
    }
}
