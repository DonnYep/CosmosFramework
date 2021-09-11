using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cosmos.QuadTree;
using Cosmos;
using System.Collections;

public class QuadTreeTester : MonoBehaviour
{
    QuadTree<ObjectSpawnInfo> quadTree;
    [SerializeField] Vector2 rectRange = new Vector2(400, 400);
    [SerializeField] GameObject resPrefab;
    [SerializeField] int objectCount = 30;
    [SerializeField] int maxObject = 4;
    [SerializeField] int maxDepth = 5;
    [SerializeField] GameObject supervisor;
    [SerializeField] bool drawGridGizmos;
    [SerializeField] bool drawObjectGizmos;
    QuadObjectSpawner objectSapwner;
    List<ObjectSpawnInfo> objectInfos = new List<ObjectSpawnInfo>();

    Dictionary<ObjectSpawnInfo, GameObject> infoGoDict = new Dictionary<ObjectSpawnInfo, GameObject>();

    Dictionary<QuadRectangle, List<ObjectSpawnInfo>> rectSpawnInfoDict = new Dictionary<QuadRectangle, List<ObjectSpawnInfo>>();
    QuadRectangle latestRect;
    void Awake()
    {
        DateTime startTime=DateTime.UtcNow;
        quadTree = QuadTree<ObjectSpawnInfo>.Create(0, 0, rectRange.x, rectRange.y, new SpawnObjectBound(), maxObject, maxDepth);
        objectSapwner = new QuadObjectSpawner(resPrefab);
        for (int i = 0; i < objectCount; i++)
        {
            var position = new Vector3(UnityEngine.Random.Range(-rectRange.x * 0.5f, rectRange.x * 0.5f), 1, UnityEngine.Random.Range(-rectRange.y * 0.5f, rectRange.y * 0.5f));
            var info = new ObjectSpawnInfo(position);
            objectInfos.Add(info);
            quadTree.Insert(info);
        }
        var objs = quadTree.GetAllObjects();
        var objLength = objs.Length;
        for (int i = 0; i < objLength; i++)
        {
            var rect = quadTree.GetObjectGrid(objs[i]);
            if (rect != QuadRectangle.Zero)
            {
                if (!rectSpawnInfoDict.TryGetValue(rect, out var infos))
                {
                    infos = new List<ObjectSpawnInfo>();
                    rectSpawnInfoDict.Add(rect, infos);
                }
                infos.Add(objs[i]);
            }
        }
        Debug.Log("实际生成数量：" + objLength);
        DateTime endTime = DateTime.UtcNow;
        Debug.Log(endTime - startTime);
    }
    void Update()
    {
        var supGo = new ObjectSpawnInfo(supervisor.transform.position);
        var currentRect = quadTree.GetObjectGrid(supGo);
        if (currentRect != null)
        {
            //   Debug.Log($"[{currentRect.CenterX} , {currentRect.CenterY}]");
            if (latestRect != currentRect)
            {
                if (rectSpawnInfoDict.TryGetValue(latestRect, out var newInfos))
                {
                    for (int i = 0; i < newInfos.Count; i++)
                    {
                        if (infoGoDict.TryGetValue(newInfos[i], out var go))
                        {
                            objectSapwner.Despawn(go);
                            infoGoDict.Remove(newInfos[i]);
                        }
                    }
                }
                if (currentRect != QuadRectangle.Zero)
                {
                    if (rectSpawnInfoDict.TryGetValue(currentRect, out var latestInfos))
                    {
                        for (int i = 0; i < latestInfos.Count; i++)
                        {
                            if (!infoGoDict.TryGetValue(latestInfos[i], out var go))
                            {
                                go = objectSapwner.Spawn();
                                go.transform.position = latestInfos[i].Position;
                                go.transform.SetParent(transform);
                                infoGoDict.Add(latestInfos[i], go);
                            }
                        }
                    }
                }
                latestRect = currentRect;
            }
        }
    }
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (drawGridGizmos)
            {
                var grid = quadTree.GetGrid();
                HashSet<QuadRectangle> hs = new HashSet<QuadRectangle>(grid);
                var grids = hs.ToArray();
                var length = grids.Length;
                for (int i = 0; i < length; i++)
                {
                    Gizmos.color = new Color(0.1f, 1, 1, 0.4f);
                    var pos = new Vector3(grids[i].X, 0, grids[i].Y);
                    var size = new Vector3(grids[i].Width, 5, grids[i].Height);
                    Gizmos.DrawWireCube(pos, size);
                }
            }
            if (drawObjectGizmos)
            {
                var infos = objectInfos;
                for (int i = 0; i < infos.Count; i++)
                {
                    Gizmos.color = new Color(1, 0, 0.7f, 0.4f);
                    Gizmos.DrawWireSphere(infos[i].Position, 1);
                }
            }
        }
    }
}
