using UnityEngine;
using Cosmos;
using Cosmos.ObjectPool;
public class LRUTester: MonoBehaviour
{
    IObjectPoolManager objectPoolManager;
    GameObject spawnItem;
    LRUCache<int, GameObject> goDict = new LRUCache<int, GameObject>(4);
    int index = 0;
    protected GameObject deactiveObjectMount;
    public Transform DeactiveObjectMount
    {
        get
        {
            if (deactiveObjectMount == null)
            {
                deactiveObjectMount = new GameObject(this.gameObject.name + "->>DeactiveObjectMount");
                deactiveObjectMount.transform.SetParent(GameManagerAgent.Instance.transform);
                deactiveObjectMount.transform.ResetLocalTransform();
            }
            return deactiveObjectMount.transform;
        }
    }
    private void OnEnable()
    {
        objectPoolManager = GameManager.GetModule<IObjectPoolManager>();
    }
    public void AddLRU()
    {
        goDict.Add(index++, objectPoolManager.Spawn(this));
        if (index >= 16)
            goDict.ResetCapacity(8);
    }
    public Transform ActiveObjectMount
    {
        get
        {
            Transform tran = GameManager.GetModuleMount<IObjectPoolManager>().transform;
            tran.SetParent(GameManagerAgent.Instance.transform);
            return tran;
        }
    }
    void SpawnHandler(GameObject go)
    {
        go.name = "LRU"+ index;
        go.transform.SetParent(ActiveObjectMount);
    }
    void DespawnHandler(GameObject go)
    {
        if (go == null)
            return;
        go.transform.SetParent(DeactiveObjectMount);
        go.transform.ResetLocalTransform();
    }
    void Start()
    {
        spawnItem = new GameObject("LRUSpawnItem");
        objectPoolManager.RegisterSpawnPool(this, spawnItem, SpawnHandler, DespawnHandler);
        goDict.AddOverflowAction((x) => objectPoolManager.Despawn(this, x));
    }


}
