using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
public class AStartManager : MonoBehaviour
{
    [SerializeField] int xSize = 20;
    [SerializeField] int ySize = 20;
    [SerializeField] float nodeSideLength = 1;
    AStartGrid aStartGrid;
    [SerializeField] GameObject player;
    [SerializeField] GameObject defaultTile;
    [SerializeField] GameObject hightlightTile;
    GameObject playerInst;
    Pool<GameObject> hightlightPool;
    List<GameObject> spawnedHightlightTiles;
    void Awake()
    {
        aStartGrid = new AStartGrid(0,0, xSize, ySize, nodeSideLength);
        hightlightPool = new Pool<GameObject>(() => { return Instantiate(hightlightTile); },
            go => go.gameObject.SetActive(true),
            go => go.gameObject.SetActive(false)
            );
        spawnedHightlightTiles = new List<GameObject>();
    }
    void Start()
    {
        playerInst = GameObject.Instantiate(player);
        playerInst.transform.position = transform.position;
        var nodes = aStartGrid.GetAllNodes();
        var length = nodes.Count;
        for (int i = 0; i < length; i++)
        {
            var go = GameObject.Instantiate(defaultTile, transform);
            var pos = new Vector3(nodes[i].CenterX, transform.position.y, nodes[i].CenterY);
            go.transform.position = pos;
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            if (Physics.Raycast(castPoint, out var hit))
            {
                if (hit.collider.gameObject.name == "AStartQuad")
                {
                    DrawPath(hit.transform.parent);
                    //var hitPos = hit.collider.transform.parent.position;
                    //var neighbors = aStartGrid.GetNeighboringNodes(hitPos.x, hitPos.z);
                    //foreach (var n in neighbors)
                    //{
                    //    UnityEngine.Debug.Log($"Node :{n.CenterX},{n.CenterY}");
                    //}
                }
            }
        }
    }
    void DrawPath(Transform dst)
    {
        var playerInstPos = playerInst.transform.position;
        var dstPos = dst.transform.position;
        var pathNodes = aStartGrid.FindPath(playerInstPos.x, playerInstPos.z, dstPos.x, dstPos.z);
        if (pathNodes == null)
        {
            Debug.Log("pathNodes  is empty");
            return;
        }
        var pathLength = pathNodes.Count;
        for (int i = 0; i < spawnedHightlightTiles.Count; i++)
        {
            hightlightPool.Despawn(spawnedHightlightTiles[i]);
        }
        spawnedHightlightTiles.Clear();
        for (int i = 0; i < pathLength; i++)
        {
            var node = pathNodes[i];
            var go= hightlightPool.Spawn();
            var pos = new Vector3(node.CenterX,transform.position.y,node.CenterY);
            go.transform.position = pos;
            spawnedHightlightTiles.Add(go);
        }
    }
    private void OnDrawGizmos()
    {
        if (aStartGrid != null)
        {
            var gridPos = new Vector3(aStartGrid.GridCenterX, transform.position.y, aStartGrid.GridCenterY);
            var gridSize = new Vector3(aStartGrid.GridWidth, 5, aStartGrid.GridHeight);
            Gizmos.DrawWireCube(gridPos, gridSize);
        }
    }
}
