using System.Collections.Generic;
using UnityEngine;
using Cosmos;

public class QuadTreeTesterSupervisor : MonoBehaviour
{
    QuadTree<ObjectSpawnInfo> quadTree;
    [SerializeField] Vector2 rectRange = new Vector2(400, 400);
    [SerializeField] Vector2 randomPosRange = new Vector2(-10, 10);
    [SerializeField] GameObject resPrefab;
    [SerializeField] int objectCount = 30;
    [SerializeField] int maxNodeObject = 4;
    [SerializeField] int maxDepth = 5;
    [SerializeField] GameObject supervisor;
    [SerializeField] bool drawGridGizmos;
    [SerializeField] bool drawObjectGridGizmos;
    [SerializeField] bool runUpdate;
    [SerializeField] float objectMoveSpeed = 5;
    [SerializeField] Color gridGizmosColor = new Color(1, 1, 1, 1);
    [SerializeField] Color objectGizmosColor = new Color(1, 1, 1, 1);
    [SerializeField] Transform activeTrans;
    [SerializeField] Transform deactiveTrans;
    QuadObjectSpawner objectSapwner;

    List<ObjectSpawnInfo> objectInfos = new List<ObjectSpawnInfo>();
    Dictionary<ObjectSpawnInfo, GameObject> infoGoDict = new Dictionary<ObjectSpawnInfo, GameObject>();

    Dictionary<QuadTree<ObjectSpawnInfo>.Rectangle, List<ObjectSpawnInfo>> rectSpawnInfoDict 
        = new Dictionary<QuadTree<ObjectSpawnInfo>.Rectangle, List<ObjectSpawnInfo>>();
    QuadTree<ObjectSpawnInfo>.Rectangle latestRect;
    void Start()
    {
        quadTree =new QuadTree<ObjectSpawnInfo>(0, 0, rectRange.x, rectRange.y, new SpawnObjectInfoBound(), maxNodeObject, maxDepth);
        objectSapwner = new QuadObjectSpawner(resPrefab);

        for (int i = 0; i < objectCount; i++)
        {
            var position = new Vector3(UnityEngine.Random.Range(-rectRange.x * 0.5f, rectRange.x * 0.5f), 1, UnityEngine.Random.Range(-rectRange.y * 0.5f, rectRange.y * 0.5f));
            var info = new ObjectSpawnInfo(position);
            objectInfos.Add(info);
            //var go = objectSapwner.Spawn();
            //go.transform.position = position;
            //go.transform.SetParent(transform);
            if (!quadTree.Insert(info))
            {
                Debug.Log(info);
            }
        }
        var objs = quadTree.GetAllObjects();
        var objLength = objs.Length;
        for (int i = 0; i < objLength; i++)
        {
            var rect = quadTree.GetAreaGrid(objs[i]);
            if (rect != null)
            {
                if (!rectSpawnInfoDict.TryGetValue(rect, out var infos))
                {
                    infos = new List<ObjectSpawnInfo>();
                    rectSpawnInfoDict.Add(rect, infos);
                }
                infos.Add(objs[i]);
            }
        }
    }

    void Update()
    {
        //quadTree.CheckObjectBound();
        //if (runUpdate)
        //{
        //    //DrawSpawnInfo();
        //}
        DrawObjectSpawn();
    }
    void DrawObjectSpawn()
    {
        var supGo = new ObjectSpawnInfo(supervisor.transform.position);
        var currentRect = quadTree.GetAreaGrid(supGo);
        if(currentRect== QuadTree<ObjectSpawnInfo>.Rectangle.Zero)
        {
            Debug.Log("ZERO");
        }
        if (latestRect != currentRect)
        {
            if (rectSpawnInfoDict.TryGetValue(latestRect, out var newInfos))
            {
                for (int i = 0; i < newInfos.Count; i++)
                {
                    if (infoGoDict.TryGetValue(newInfos[i], out var go))
                    {
                        objectSapwner.Despawn(go);
                        go.transform.SetParent(deactiveTrans);
                        infoGoDict.Remove(newInfos[i]);
                    }
                }
            }
        }
        if (currentRect != QuadTree<ObjectSpawnInfo>.Rectangle.Zero)
        {
            if (rectSpawnInfoDict.TryGetValue(currentRect, out var latestInfos))
            {
                for (int i = 0; i < latestInfos.Count; i++)
                {
                    if (!infoGoDict.TryGetValue(latestInfos[i], out var go))
                    {
                        go = objectSapwner.Spawn();
                        go.transform.position = latestInfos[i].Position;
                        go.transform.SetParent(activeTrans);
                        infoGoDict.Add(latestInfos[i], go);
                    }
                }
            }
            latestRect = currentRect;
        }
    }
    //void DrawSpawnInfo()
    //{
    //    var objArr = objectInfos.ToArray();
    //    var length = objArr.Length;
    //    for (int i = 0; i < length; i++)
    //    {
    //        var valueX = UnityEngine.Random.Range(randomPosRange.x, randomPosRange.y);
    //        var valueZ = UnityEngine.Random.Range(randomPosRange.x, randomPosRange.y);
    //        var symbol = UnityEngine.Random.Range(0, 16);
    //        var pos = new Vector3(valueX, 0, valueZ);
    //        if (symbol % 2 == 0)
    //        {
    //            objArr[i].transform.position = Vector3.Lerp(objArr[i].transform.position, objArr[i].transform.position + pos, Time.deltaTime * objectMoveSpeed);
    //        }
    //        else
    //        {
    //            objArr[i].transform.position = Vector3.Lerp(objArr[i].transform.position, objArr[i].transform.position - pos, Time.deltaTime * objectMoveSpeed);
    //        }
    //    }
    //}
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
                    var pos = new Vector3(grids[i].CenterX, 0, grids[i].CenterY);
                    var size = new Vector3(grids[i].Width, 5, grids[i].Height);
                    Gizmos.DrawWireCube(pos, size);
                }
            }
            if (drawObjectGridGizmos)
            {
                var objs = quadTree.GetAllObjects();
                for (int i = 0; i < objs.Length; i++)
                {
                    Gizmos.color = objectGizmosColor;
                    var pos = objs[i].Position;
                    var size = Vector3.one * 0.5f;
                    Gizmos.DrawWireCube(pos, size);
                }
            }
        }
    }
}
