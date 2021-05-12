using UnityEngine;
using Cosmos;
using Cosmos.ObjectPool;
public class LRUTester: MonoBehaviour
{
    IObjectPoolManager objectPoolManager;
    GameObject spawnItem;
    LRU<int, GameObject> goDict = new LRU<int, GameObject>(4);
    int index = 0;
    IObjectPool objectPool;
    string objectKey = "LRUTester";
    protected GameObject deactiveObjectMount;
    public Transform DeactiveObjectMount
    {
        get
        {
            if (deactiveObjectMount == null)
            {
                deactiveObjectMount = new GameObject(this.gameObject.name + "->>DeactiveObjectMount");
                deactiveObjectMount.transform.SetParent(MonoGameManager.Instance.transform);
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
        goDict.Add(index++, objectPool.Spawn().CastTo<GameObject>());
        if (index >= 16)
            goDict.ResetCapacity(8);
    }
    public Transform ActiveObjectMount
    {
        get
        {
            Transform tran = GameManager.GetModuleMount<IObjectPoolManager>().transform;
            tran.SetParent(MonoGameManager.Instance.transform);
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
        objectPool= objectPoolManager.RegisterObjectPool(objectKey,spawnItem);
        goDict.AddOverflowAction((x) => objectPool.Despawn(x));
    }


}
