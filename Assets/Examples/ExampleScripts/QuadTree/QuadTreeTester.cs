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
    QuadTree<GameObject> quadTree;
    [SerializeField] Vector2 rectRange = new Vector2(400, 400);
    [SerializeField] Vector2 randomPosRange = new Vector2(-10, 10);
    [SerializeField] GameObject resPrefab;
    [SerializeField] int objectCount = 30;
    [SerializeField] int maxNodeObject = 4;
    [SerializeField] int maxDepth = 5;
    [SerializeField] GameObject supervisor;
    [SerializeField] bool drawGridGizmos;
    [SerializeField] bool runUpdate;
    [SerializeField] float objectMoveSpeed = 5;
    [SerializeField] Color gridGizmosColor = new Color(1, 1, 1, 1);
    [SerializeField] Transform activeTrans;
    [SerializeField] Transform deactiveTrans;
    QuadObjectSpawner objectSapwner;
    List<GameObject> objectInfos = new List<GameObject>();
    void Start()
    {
        DateTime startTime = DateTime.UtcNow;
        quadTree = QuadTree<GameObject>.Create(0, 0, rectRange.x, rectRange.y, new SpawnObjectBound(), maxNodeObject, maxDepth);
        quadTree.OnObjectOutQuadRectangle += OnObjectOutQuadRectangle;
        objectSapwner = new QuadObjectSpawner(resPrefab);
        int index = 0;
        for (int i = 0; i < objectCount; i++)
        {
            var position = new Vector3(UnityEngine.Random.Range(-rectRange.x * 0.5f, rectRange.x * 0.5f), 1, UnityEngine.Random.Range(-rectRange.y * 0.5f, rectRange.y * 0.5f));
            var info = objectSapwner.Spawn();
            info.transform.position = position;
            if (quadTree.Insert(info))
            {
                index++;
                info.name = resPrefab.name + index;
                info.transform.SetParent(activeTrans);
                objectInfos.Add(info);
            }
            else
            {
                Destroy(info);
            }
        }
        DateTime endTime = DateTime.UtcNow;
        Debug.Log(index);
        Debug.Log(endTime - startTime);
    }
    void OnObjectOutQuadRectangle(GameObject obj)
    {
        obj.transform.SetParent(deactiveTrans);
        if (objectInfos.Remove(obj))
        {
            //Utility.Debug.LogInfo(obj.name + "-Out");
        }
    }

    void Update()
    {
        quadTree.CheckObjectRect();
        if (runUpdate)
        {
            DrawSpawnInfo();
        }
    }
    void DrawSpawnInfo()
    {
        var objArr = objectInfos.ToArray();
        var length = objArr.Length;
        for (int i = 0; i < length; i++)
        {
            var valueX = UnityEngine.Random.Range(randomPosRange.x, randomPosRange.y);
            var valueZ = UnityEngine.Random.Range(randomPosRange.x, randomPosRange.y);
            var symbol = UnityEngine.Random.Range(0, 16);
            var pos = new Vector3(valueX, 0, valueZ);
            if (symbol % 2 == 0)
            {
                objArr[i].transform.position = Vector3.Lerp(objArr[i].transform.position, objArr[i].transform.position + pos, Time.deltaTime * objectMoveSpeed);
            }
            else
            {
                objArr[i].transform.position = Vector3.Lerp(objArr[i].transform.position, objArr[i].transform.position - pos, Time.deltaTime * objectMoveSpeed);
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
                    Gizmos.color = gridGizmosColor;
                    var pos = new Vector3(grids[i].X, 0, grids[i].Y);
                    var size = new Vector3(grids[i].Width, 5, grids[i].Height);
                    Gizmos.DrawWireCube(pos, size);
                }
            }
            var objs = quadTree.GetAllObjects();
            for (int i = 0; i < objs.Length; i++)
            {
                Gizmos.color = gridGizmosColor;
                var pos = objs[i].transform.position;
                var size = Vector3.one*0.5f;
                Gizmos.DrawWireCube(pos, size);
            }
        }
    }
}
