using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
public class AStartManager : MonoBehaviour
{
    [SerializeField] int xSize = 20;
    [SerializeField] int ySize = 20;
    [SerializeField] float nodeSideLength = 1;
    AStart aStartGrid;
    [SerializeField] GameObject player;
    [SerializeField] GameObject defaultTile;
    [SerializeField] GameObject hightlightTile;
    [SerializeField] AStartDistanceType aStartDistanceType;
    GameObject playerInst;
    Pool<GameObject> hightlightPool;
    List<GameObject> spawnedHightlightTiles;
    GameObject defaultTileRoot;
    GameObject hightligntTileRoot;

    Transform latestQuad;
    void Awake()
    {
        defaultTileRoot = new GameObject("DefaultTileRoot");
        hightligntTileRoot = new GameObject("HightligntTileRoot");
        defaultTileRoot.transform.SetParent(transform);
        hightligntTileRoot.transform.SetParent(transform);
        defaultTileRoot.transform.ResetLocalTransform();
        hightligntTileRoot.transform.ResetLocalTransform();

        switch (aStartDistanceType)
        {
            case AStartDistanceType.Euclidean:
                aStartGrid = new AStartEuclidean(0, 0, xSize, ySize, nodeSideLength);
                break;
            case AStartDistanceType.Manhattan:
                aStartGrid = new AStartManhattan(0, 0, xSize, ySize, nodeSideLength);
                break;
            case AStartDistanceType.Diagonal:
                aStartGrid = new AStartDiagonal(0, 0, xSize, ySize, nodeSideLength);
                break;
        }
        hightlightPool = new Pool<GameObject>(() => { return Instantiate(hightlightTile, hightligntTileRoot.transform); },
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
        for (int i = 0; i < spawnedHightlightTiles.Count; i++)
        {
            hightlightPool.Despawn(spawnedHightlightTiles[i]);
        }
        spawnedHightlightTiles.Clear();
        for (int i = 0; i < pathLength; i++)
        {
            var node = pathNodes[i];
            var go = hightlightPool.Spawn();
            var pos = new Vector3(node.CenterX, transform.position.y, node.CenterY);
            go.transform.position = pos;
            spawnedHightlightTiles.Add(go);
        }
    }
}
