using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    public class SquareGridRender : MonoBehaviour
    {
        [SerializeField] float cellSideLength = 5;
        [SerializeField] uint divided;

        [SerializeField] Vector2 center;
        [SerializeField] float cellBufferZoneBound = 8;
        [SerializeField] Color cellZoneColor;
        [SerializeField] Color bufferZoneColor;
        [SerializeField] Color heightLightColor;
        [SerializeField] Transform supervisor;

        [SerializeField] GameObject squareTilePrefab;
        SquareGrid squareGrid;

        GameObject normalTileRoot;
        Dictionary<Square, SquareTileComponent> squareTileDict
            = new Dictionary<Square, SquareTileComponent>();

        List<Square> highlightCache = new List<Square>();
        private void Start()
        {
            normalTileRoot = new GameObject("NormalTileRoot");
            normalTileRoot.transform.SetParent(transform);
            normalTileRoot.transform.ResetLocalTransform();

            squareGrid = new SquareGrid(cellSideLength, divided, center.x, center.y, cellBufferZoneBound);

            var squares = squareGrid.GetAllSquares();
            var length = squares.Length;
            for (int i = 0; i < length; i++)
            {
                var go = GameObject.Instantiate(squareTilePrefab, normalTileRoot.transform);
                var pos = new Vector3(squares[i].CenterX, normalTileRoot.transform.position.y, squares[i].CenterY);
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
            highlightCache.AddRange(squareGrid.GetSquares(pos.x, pos.y));
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
            var slength = squares.Length;
            for (int i = 0; i < slength; i++)
            {
                Gizmos.color = cellZoneColor;
                var pos = new Vector3(squares[i].CenterX, 0, squares[i].CenterY);
                var size = new Vector3(squareGrid.CellSideLength, 6, squareGrid.CellSideLength);
                Gizmos.DrawWireCube(pos, size);
                Gizmos.color = bufferZoneColor;
                var cellSize = new Vector3(squareGrid.CellSideLength + cellBufferZoneBound * 2, 8, squareGrid.CellSideLength + cellBufferZoneBound * 2);
                Gizmos.DrawWireCube(pos, cellSize);
            }
            if (true)
            {
                var length = highlightCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var curSquare = highlightCache[i];
                    Gizmos.color = heightLightColor;
                    var hlSize = new Vector3(curSquare.SideLength, 1, curSquare.SideLength);
                    var hlPos = new Vector3(curSquare.CenterX, 0, curSquare.CenterY);
                    Gizmos.DrawCube(hlPos, hlSize);
                }
            }
        }
    }
}
