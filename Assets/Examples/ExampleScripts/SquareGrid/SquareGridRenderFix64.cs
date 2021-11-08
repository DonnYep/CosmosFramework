using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FixMath.NET;
namespace Cosmos.Test
{
    public class SquareGridRenderFix64 : MonoBehaviour
    {
        [SerializeField] float cellSideLength = 5;
        [SerializeField] uint cellSection;

        [SerializeField] Vector2 offset;
        [SerializeField] float cellBufferZoneBound = 8;
        [SerializeField] Color cellZoneColor;
        [SerializeField] Color bufferZoneColor;
        [SerializeField] Color heightLightColor;
        [SerializeField] Transform supervisor;

       SquareGridFix64 squareGrid;
        SquareGridFix64.Square[] bufferZoneSquares;


        private void Start()
        {
            squareGrid = new SquareGridFix64((Fix64)cellSideLength, cellSection, (Fix64)offset.x, (Fix64)offset.y, (Fix64)cellBufferZoneBound);
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
            bufferZoneSquares = squareGrid.GetRectangles((Fix64)pos.x, (Fix64)pos.y);
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
                    var pos = new Vector3((float)rects[i, j].CenterX, 0, (float)rects[i, j].CenterY);
                    var size = new Vector3((float)squareGrid.CellSideLength, 6, (float)squareGrid.CellSideLength);
                    Gizmos.DrawWireCube(pos, size);
                    Gizmos.color = bufferZoneColor;
                    var cellSize = new Vector3((float)squareGrid.CellSideLength+cellBufferZoneBound*2, 8, (float)squareGrid.CellSideLength+ cellBufferZoneBound*2);
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
                    var hlSize = new Vector3((float)tmpSquare.SideLength, 1, (float)tmpSquare.SideLength);
                    var hlPos = new Vector3((float)tmpSquare.CenterX, 0, (float)tmpSquare.CenterY);
                    Gizmos.DrawCube(hlPos, hlSize);
                }
            }
        }
    }
}
