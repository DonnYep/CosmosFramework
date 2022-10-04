using System;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using FixMath.NET;
public class QuadTreeFix64Tester : MonoBehaviour
{
    QuadTreeFix64<GameObject> quadTree;
    [SerializeField] Vector2 rectRange = new Vector2(400, 400);
    [SerializeField] GameObject resPrefab;
    [SerializeField] int objectCount = 30;
    /// <summary>
    /// 每个节点能够包含最多的对象数量；
    /// </summary>
    [SerializeField] int maxNodeObject = 4;
    [SerializeField] int maxDepth = 5;
    [SerializeField] bool drawGridGizmos;
    [SerializeField] bool drawObjectGridGizmos;
    [SerializeField] bool runUpdate;
    /// <summary>
    /// 当对象出四叉树根节点范围时，移除这个对象；
    /// </summary>
    [SerializeField] bool removeWhenOutBound;
    [SerializeField] float objectMoveSpeed = 5;
    [SerializeField] Color gridGizmosColor = new Color(1, 1, 1, 1);
    [SerializeField] Color objectGizmosColor = new Color(1, 1, 1, 1);
    [SerializeField] Transform activeTrans;
    [SerializeField] Transform deactiveTrans;
    QuadObjectSpawner objectSapwner;

    List<ObjectEntity> objectEntities = new List<ObjectEntity>();
    Dictionary<GameObject, ObjectEntity> goIdDict = new Dictionary<GameObject, ObjectEntity>();
    void Start()
    {
        DateTime startTime = DateTime.UtcNow;
        quadTree = new QuadTreeFix64<GameObject>((Fix64)0, (Fix64)0, (Fix64)rectRange.x, (Fix64)rectRange.y, new SpawnObjectBoundFix64(), maxNodeObject, maxDepth);
        quadTree.OnOutQuadBound += OnObjectOutQuadRectangle;
        objectSapwner = new QuadObjectSpawner(resPrefab);
        int index = 0;
        for (int i = 0; i < objectCount; i++)
        {
            var position = GetRandomPosition();
            var inst = objectSapwner.Spawn();
            inst.transform.position = position;
            try
            {
                quadTree.Insert(inst);
                index++;
                inst.name = resPrefab.name + index;
                inst.transform.SetParent(activeTrans);
                var entity = new ObjectEntity() { Inst = inst, MovePos = GetRandomPosition() };
                objectEntities.Add(entity);
                goIdDict.Add(inst, entity);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Destroy(inst);
            }
        }
        DateTime endTime = DateTime.UtcNow;
        Debug.Log(index);
        Debug.Log(endTime - startTime);
    }

    void OnObjectOutQuadRectangle(GameObject obj)
    {
        if (removeWhenOutBound)
        {
            if (!quadTree.IsOverlapping(obj))
            {
                obj.transform.SetParent(deactiveTrans);
                if (goIdDict.Remove(obj, out var entity))
                {
                    objectEntities.Remove(entity);
                }
            }
            else
            {
                quadTree.Insert(obj);
            }
        }
        else
        {
            quadTree.Insert(obj);
        }
    }
    void Update()
    {
        quadTree.CheckObjectBound();
        if (runUpdate)
        {
            DrawSpawnInfo();
        }
    }
    Vector3 GetRandomPosition()
    {
        var valueX = UnityEngine.Random.Range((float)quadTree.QuadTreeArea.Left, (float)quadTree.QuadTreeArea.Right);
        var valueZ = UnityEngine.Random.Range((float)quadTree.QuadTreeArea.Top, (float)quadTree.QuadTreeArea.Bottom);
        return new Vector3(valueX, 0, valueZ);
    }
    void DrawSpawnInfo()
    {
        var length = objectEntities.Count;
        for (int i = 0; i < length; i++)
        {
            var inst = objectEntities[i].Inst;
            var dir = objectEntities[i].MovePos - inst.transform.position;
            inst.transform.forward = Vector3.Lerp(inst.transform.forward, dir, Time.deltaTime);
            var dstPos = inst.transform.forward * Time.deltaTime * objectMoveSpeed + inst.transform.position;
            inst.transform.position = dstPos;
        }
    }
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (drawGridGizmos)
            {
                var grids = quadTree.GetAreaGrids();
                var length = grids.Length;
                for (int i = 0; i < length; i++)
                {
                    Gizmos.color = gridGizmosColor;
                    var pos = new Vector3((float)grids[i].CenterX, 0, (float)grids[i].CenterY);
                    var size = new Vector3((float)grids[i].Width, 5, (float)grids[i].Height);
                    Gizmos.DrawWireCube(pos, size);
                }
            }
            if (drawObjectGridGizmos)
            {
                var objs = quadTree.GetAllObjects();
                for (int i = 0; i < objs.Length; i++)
                {
                    Gizmos.color = objectGizmosColor;
                    var pos = objs[i].transform.position;
                    var size = Vector3.one * 0.5f;
                    Gizmos.DrawWireCube(pos, size);
                }
            }
        }
    }
    class ObjectEntity
    {
        public GameObject Inst;
        public Vector3 MovePos;
    }
}
