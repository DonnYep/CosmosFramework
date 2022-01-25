using System.Collections.Generic;
using UnityEngine;
using Cosmos;
public class AStarManager : MonoBehaviour
{
    [SerializeField] int xSize = 20;
    [SerializeField] int ySize = 20;
    AStar aStartGrid;
    [SerializeField] GameObject player;
    [SerializeField] GameObject defaultTile;
    [SerializeField] GameObject hightlightTile;
    [Header("距离公式类型")]
    [SerializeField] AStarDistanceType distanceType;
    float nodeSideLength = 1;
    GameObject playerInst;
    Pool<GameObject> highlightPool;
    List<GameObject> spawnedHighlightTiles;
    GameObject defaultTileRoot;
    GameObject highligntTileRoot;

    Transform latestQuad;
    void Awake()
    {
        defaultTileRoot = new GameObject("DefaultTileRoot");
        highligntTileRoot = new GameObject("HighligntTileRoot");
        defaultTileRoot.transform.SetParent(transform);
        highligntTileRoot.transform.SetParent(transform);
        defaultTileRoot.transform.ResetLocalTransform();
        highligntTileRoot.transform.ResetLocalTransform();

        switch (distanceType)
        {
            case AStarDistanceType.Euclidean:
                aStartGrid = new AStarEuclidean(0, 0, xSize, ySize, nodeSideLength);
                break;
            case AStarDistanceType.Manhattan:
                aStartGrid = new AStarManhattan(0, 0, xSize, ySize, nodeSideLength);
                break;
            case AStarDistanceType.Diagonal:
                aStartGrid = new AStarDiagonal(0, 0, xSize, ySize, nodeSideLength);
                break;
        }
        highlightPool = new Pool<GameObject>(() => { return Instantiate(hightlightTile, highligntTileRoot.transform); },
            go => go.gameObject.SetActive(true),
            go => go.gameObject.SetActive(false)
            );
        spawnedHighlightTiles = new List<GameObject>();

    }
    void Start()
    {
        playerInst = GameObject.Instantiate(player);
        playerInst.transform.position = transform.position;
        var nodes = aStartGrid.GetAllNodes();
        var length = nodes.Count;
        for (int i = 0; i < length; i++)
        {
            var go = GameObject.Instantiate(defaultTile, defaultTileRoot.transform);
            var pos = new Vector3(nodes[i].CenterX, transform.position.y, nodes[i].CenterY);
            go.transform.position = pos;
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            if (Physics.Raycast(castPoint, out var hit))
            {
                if (hit.collider.gameObject.name == "AStartQuad")
                {
                    playerInst.transform.position = hit.transform.parent.position;
                    if (latestQuad != null)
                        DrawPath(latestQuad);
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Input.mousePosition;
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);
            if (Physics.Raycast(castPoint, out var hit))
            {
                if (hit.collider.gameObject.name == "AStartQuad")
                {
                    latestQuad = hit.transform.parent;
                    DrawPath(latestQuad);
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
        for (int i = 0; i < spawnedHighlightTiles.Count; i++)
        {
            highlightPool.Despawn(spawnedHighlightTiles[i]);
        }
        spawnedHighlightTiles.Clear();
        for (int i = 0; i < pathLength; i++)
        {
            var node = pathNodes[i];
            var go = highlightPool.Spawn();
            var pos = new Vector3(node.CenterX, transform.position.y, node.CenterY);
            go.transform.position = pos;
            spawnedHighlightTiles.Add(go);
        }
    }
}
