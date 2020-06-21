using UnityEngine;
using Cosmos;
public class LRUTester: MonoBehaviour
{
    [SerializeField]
    LRUObject spawnItem;
    LRUCache<int, IObject> goDict = new LRUCache<int, IObject>(4);
    int index = 0;
    ObjectKey<LRUObject> objectKey;
    GameObject activeObjectMount;
    ObjectPoolVariable poolData;
    public void AddLRU()
    {
        goDict.Add(index++,  Facade.SpawnObject(objectKey));
        if (index >= 16)
            goDict.ResetCapacity(8);
    }
    private void Awake()
    {
        activeObjectMount = new GameObject("ActiveObjectMount");
        poolData = new ObjectPoolVariable(spawnItem, SpawnHandler, DespawnHandler,CreateHandler);
        objectKey = new ObjectKey<LRUObject>();
    }
    void Start()
    {
        Facade.RegisterObjcetSpawnPool(objectKey,poolData);
        goDict.AddOverflowAction((x) =>Facade.DespawnObject(objectKey,x));
    }
    public Transform ActiveObjectMount
    {
        get
        {
            Transform tran = activeObjectMount.transform;
            tran.SetParent(Facade.GetModule(CFModules.OBJECTPOOL).ModuleMountObject.transform);
            return tran;
        }
    }
    void SpawnHandler(IObject obj)
    {
        var go = obj as LRUObject;
        go.gameObject.SetActive(true);
        go.name = "LRU"+ index;
        go.transform.SetParent(ActiveObjectMount);
    }
    LRUObject  CreateHandler()
    {
        var go = GameObject.Instantiate(spawnItem);
        return go;
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
    void DespawnHandler(IObject obj)
    {
        var go = obj as LRUObject;
        if (go == null)
            return;
        go.transform.SetParent(DeactiveObjectMount);
        go.gameObject.SetActive(false);
        go.transform.ResetLocalTransform();
    }
}
