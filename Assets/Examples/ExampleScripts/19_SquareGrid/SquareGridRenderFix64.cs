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

        [SerializeField] GameObject squareTilePrefab;
        SquareGridFix64 squareGrid;

        GameObject normalTileRoot;
        Dictionary<SquareGridFix64.Square, SquareTileComponent> squareTileDict
            = new Dictionary<SquareGridFix64.Square, SquareTileComponent>();

        List<SquareGridFix64.Square> highlightCache = new List<SquareGridFix64.Square>();
        private void Start()
        {
            normalTileRoot = new GameObject("NormalTileRoot");
            normalTileRoot.transform.SetParent(transform);
            normalTileRoot.transform.ResetLocalTransform();
            squareGrid = new SquareGridFix64((Fix64)cellSideLength, cellSection, (Fix64)offset.x, (Fix64)offset.y, (Fix64)cellBufferZoneBound);
            var squares = squareGrid.GetAllSquares();
            var length = squareGrid.CellSection * squareGrid.CellSection;
            for (int i = 0; i < length; i++)
            {
                var go = GameObject.Instantiate(squareTilePrefab, normalTileRoot.transform);
                var pos = new Vector3((float)squares[i].CenterX, normalTileRoot.transform.position.y, (float)squares[i].CenterY);
                go.transform.position = pos;
                var comp = go.AddComponent<SquareTileComponent>();
                squareTileDict.Add(squares[i], comp);
            }
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
            highlightCache.Clear();
            highlightCache.AddRange(squareGrid.GetSquares((Fix64)pos.x, (Fix64)pos.y));
            foreach (var st in squareTileDict)
            {
                st.Value.SquareTileHighlight(highlightCache.Contains(st.Key));
            }
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
            var squares = squareGrid.GetAllSquares();
            var slength= squares.Length;
            for (int i = 0; i < slength; i++)
            {
                Gizmos.color = cellZoneColor;
                var pos = new Vector3((float)squares[i].CenterX, 0, (float)squares[i].CenterY);
                var size = new Vector3((float)squareGrid.CellSideLength, 6, (float)squareGrid.CellSideLength);
                Gizmos.DrawWireCube(pos, size);
                Gizmos.color = bufferZoneColor;
                var cellSize = new Vector3((float)squareGrid.CellSideLength + cellBufferZoneBound * 2, 8, (float)squareGrid.CellSideLength + cellBufferZoneBound * 2);
                Gizmos.DrawWireCube(pos, cellSize);
            }
            if (true)
            {
                var length = highlightCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var tmpSquare = highlightCache[i];
                    Gizmos.color = heightLightColor;
                    var hlSize = new Vector3((float)tmpSquare.SideLength, 1, (float)tmpSquare.SideLength);
                    var hlPos = new Vector3((float)tmpSquare.CenterX, 0, (float)tmpSquare.CenterY);
                    Gizmos.DrawCube(hlPos, hlSize);
                }
            }
        }
    }
}
