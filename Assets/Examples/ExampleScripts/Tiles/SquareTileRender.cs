using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Test
{
    public class SquareTileRender : MonoBehaviour
    {
        [SerializeField] float cellSideLength = 5;
        [SerializeField] uint divided;

        [SerializeField] Vector2 offset;
        [SerializeField] float cellBufferZoneBound = 8;
        [SerializeField] Color cellZoneColor;
        [SerializeField] Color bufferZoneColor;
        [SerializeField] Color heightLightColor;
        [SerializeField] Transform supervisor;

        SquareGrid squareGrid;
        SquareGrid.Square[] bufferZoneSquares;


        private void Start()
        {
            squareGrid = new SquareGrid(cellSideLength, divided, offset.x, offset.y,cellBufferZoneBound);
        }
        private void Update()
        {
            if (supervisor != null)
            {
                UpdateSquare();
            }
        }
        void UpdateSquare()
        {
            var pos = new Vector2(supervisor.position.x, supervisor.position.z);
            bufferZoneSquares = squareGrid.GetRectanglesByBufferZone(pos.x, pos.y);
        }
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                DrawSquareGrid();
            }
        }
        void DrawSquareGrid()
        {
            var rects = squareGrid.GetAllRectangle();
            var x = squareGrid.CellSection;
            var y = squareGrid.CellSection;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Gizmos.color = cellZoneColor;
                    var pos = new Vector3(rects[i, j].CenterX, 0, rects[i, j].CenterY);
                    var size = new Vector3(squareGrid.CellSideLength, 6, squareGrid.CellSideLength);
                    Gizmos.DrawWireCube(pos, size);
                    Gizmos.color = bufferZoneColor;
                    var cellSize = new Vector3(squareGrid.CellSideLength+cellBufferZoneBound*2, 8, squareGrid.CellSideLength+ cellBufferZoneBound*2);
                    Gizmos.DrawWireCube(pos, cellSize);
                }
            }
            if (true)
            {
                var length = bufferZoneSquares.Length;
                for (int i = 0; i < length; i++)
                {
                    var tmpSquare = bufferZoneSquares[i];
                    Gizmos.color = heightLightColor;
                    var hlSize = new Vector3(tmpSquare.SideLength, 1, tmpSquare.SideLength);
                    var hlPos = new Vector3(tmpSquare.CenterX, 0, tmpSquare.CenterY);
                    Gizmos.DrawCube(hlPos, hlSize);
                }
            }
        }
    }
}
