using Cosmos.QuadTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SpawnObjectBound : IObjecRectangletBound<ObjectSpawnInfo>
{
    public float GetHeight(ObjectSpawnInfo go)
    {
        return 1;
    }
    public float GetPositonX(ObjectSpawnInfo go)
    {
        return go.Position.x;
    }
    public float GetPositonY(ObjectSpawnInfo go)
    {
        return go.Position.z;
    }
    public float GetWidth(ObjectSpawnInfo go)
    {
        return 1;
    }
}
