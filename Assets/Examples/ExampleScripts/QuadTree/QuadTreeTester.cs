using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cosmos.QuadTree;
public class QuadTreeTester : MonoBehaviour
{
    QuadTree<ObjectSpawnInfo> quadTree;
    [SerializeField] GameObject resPrefab;
    [SerializeField] int objectCount = 30;
    [SerializeField] int maxObject = 4;
    [SerializeField] int maxDepth = 5;
    [SerializeField] GameObject supervisor;

    QuadObjectSpawner objectSapwner;
    List<ObjectSpawnInfo> objectInfos = new List<ObjectSpawnInfo>();

    void Awake()
    {
        //quadTree = new QuadTree<ObjectSpawnInfo>(-200, -200, 400, 400, new UnityObjectBound(), maxObject, maxDepth);
        quadTree = new QuadTree<ObjectSpawnInfo>(-200, -200, 400, 400, new SpawnObjectBound(), maxObject, maxDepth);
        objectSapwner = new QuadObjectSpawner(resPrefab);
    }
    void Start()
    {
        for (int i = 0; i < objectCount; i++)
        {
            var position = new Vector3(UnityEngine.Random.Range(-200, 200), 1, UnityEngine.Random.Range(-200, 200));
            var info = new ObjectSpawnInfo(position);
            objectInfos.Add(info);
            var go= objectSapwner.Spawn();
            go.transform.position = position;
            go.transform.SetParent(transform);
            quadTree.Insert(info);
        }
    }
    void Update()
    {

    }
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            var grid = quadTree.GetGrid();
            HashSet<QuadTreeRect> hs = new HashSet<QuadTreeRect>(grid);
            var grids = hs.ToArray();
            var length = grids.Length;
            for (int i = 0; i < length; i++)
            {
                Gizmos.color = new Color(0.1f, 1, 1, 0.4f);
                var pos = new Vector3(grids[i].CenterX, 0, grids[i].CenterY);
                var size = new Vector3(grids[i].Width, 5, grids[i].Height);
                Gizmos.DrawWireCube(pos, size);
            }
            var infos = quadTree.FindObjects(new ObjectSpawnInfo(supervisor.transform.position));
            for (int i = 0; i < infos.Length; i++)
            {
                Gizmos.color = new Color(1, 0, 0.7f, 0.4f);
                Gizmos.DrawWireSphere(infos[i].Position,3);
            }
        }
    }
}
