using UnityEngine;
using Cosmos;
public class LRUTester: MonoBehaviour
{
    GameObject spawnItem;
    LRUCache<int, GameObject> goDict = new LRUCache<int, GameObject>(4);
    int index = 0;
    public void AddLRU()
    {
        goDict.Add(index++,  Facade.SpawnObject(this));
        if (index >= 16)
            goDict.ResetCapacity(8);
    }
    void Start()
    {
        spawnItem = new GameObject("LRUSpawnItem");
        Facade.RegisterObjcetSpawnPool(this, spawnItem, SpawnHandler, DespawnHandler);
        goDict.AddOverflowAction((x) =>Facade.DespawnObject(this,x));
    }
    public Transform ActiveObjectMount
    {
        get
        {
            Transform tran = Facade.GetObjectSpawnPoolActiveMount().transform;
            tran.SetParent(Facade.GetModule(CFModules.OBJECTPOOL).ModuleMountObject.transform);
            return tran;
        }
    }
    void SpawnHandler(GameObject go)
    {
        go.name = "LRU"+ index;
        go.transform.SetParent(ActiveObjectMount);
    }
    protected GameObject deactiveObjectMount;
    public Transform DeactiveObjectMount
    {
        get
        {
            if (deactiveObjectMount == null)
            {
                deactiveObjectMount = new GameObject(this.gameObject.name + "->>DeactiveObjectMount");
                deactiveObjectMount.transform.SetParent(Facade.GetModule(CFModules.OBJECTPOOL).ModuleMountObject.transform);
                deactiveObjectMount.transform.ResetLocalTransform();
            }
            return deactiveObjectMount.transform;
        }
    }
    void DespawnHandler(GameObject go)
    {
        if (go == null)
            return;
        go.transform.SetParent(DeactiveObjectMount);
        go.transform.ResetLocalTransform();
    }
}
