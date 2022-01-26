using Cosmos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AOIManager : MonoBehaviour
{
    [SerializeField] float cellSideLength = 5;
    [SerializeField] float cellBufferZoneBound = 8;
    [SerializeField] Vector2 offset;
    [SerializeField] uint cellCount;
    [SerializeField] int nearbyLevel = 1;
    [SerializeField] GameObject aoiTilePrefab;
    [SerializeField] Transform activeTrans;
    [SerializeField] Transform deactiveTrans;
    [SerializeField] Transform supervisor;
    [SerializeField] Color cellZoneColor;
    [SerializeField] Color aoiColor;

    [SerializeField] GameObject robot;
    [SerializeField] int robotCount = 100;
    [SerializeField] float robotMoveSpeed = 1;
    [SerializeField] int changeDirSec = 5;
    SquareGrid squareGrid;
    SquareGrid.Square[] nineSquareGrid;
    List<RobotEntity> robotInstList = new List<RobotEntity>();
    long latestTime;
    void Awake()
    {
        squareGrid = new SquareGrid(cellSideLength, cellCount, offset.x, offset.y, cellBufferZoneBound);
    }
    void Start()
    {
        for (int i = 0; i < robotCount; i++)
        {
            var go = GameObject.Instantiate(robot, activeTrans);
            robotInstList.Add(new RobotEntity() { RobotInst = go.transform, DstPos=GetRandomPosition() });
            var pos = GetRandomPosition();
            go.transform.localPosition = pos;
        }
    }
    private void Update()
    {
        var pos = supervisor.position;
        nineSquareGrid = squareGrid.GetNearbySquares(pos.x, pos.z, nearbyLevel);

        var now = Utility.Time.SecondNow();
        if (now > latestTime)
        {
            latestTime = now + changeDirSec;
        }
        RobotMove();
    }
    Vector3 GetRandomPosition()
    {
        var valueX = UnityEngine.Random.Range(squareGrid.SquareLeft, squareGrid.SquareRight);
        var valueZ = UnityEngine.Random.Range(squareGrid.SquareTop, squareGrid.SquareBottom);
        var pos = new Vector3(valueX, 0, valueZ);
        return pos;
    }
    void RobotMove()
    {
        var length = robotInstList.Count;
        for (int i = 0; i < length; i++)
        {
            var inst = robotInstList[i].RobotInst;
            var dir = robotInstList[i].DstPos - inst.position;
            inst.forward = Vector3.Lerp(inst.forward, dir, Time.deltaTime);
            var dstPos = inst.forward * Time.deltaTime * robotMoveSpeed + inst.position;
            if (!squareGrid.IsOverlapping(dstPos.x, dstPos.y))
            {
                robotInstList[i].DstPos = GetRandomPosition();
            }
            inst.position = dstPos;
        }
    }
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            DrawSquareGrid();
        }
    }
    void DrawSquareGrid()
    {
        var squares = squareGrid.GetAllSquares();
        var slength = squares.Length;
        for (int i = 0; i < slength; i++)
        {
            Gizmos.color = cellZoneColor;
            var pos = new Vector3(squares[i].CenterX, 0, squares[i].CenterY);
            var size = new Vector3(squareGrid.CellSideLength, 6, squareGrid.CellSideLength);
            Gizmos.DrawWireCube(pos, size);
        }
        var nsgLen = nineSquareGrid.Length;
        for (int i = 0; i < nsgLen; i++)
        {
            var pos = new Vector3(nineSquareGrid[i].CenterX, 0, nineSquareGrid[i].CenterY);
            var sideLength = squareGrid.CellSideLength;
            var size = new Vector3(sideLength, sideLength, sideLength);
            Gizmos.DrawWireCube(pos, size);
            Gizmos.color = aoiColor;
            Gizmos.DrawCube(pos, size);
        }
    }
    class RobotEntity
    {
        public Transform RobotInst;
        public Vector3 DstPos;
    }
}
