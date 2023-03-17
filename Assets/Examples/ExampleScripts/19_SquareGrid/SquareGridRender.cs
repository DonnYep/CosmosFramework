using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    public class SquareGridRender : MonoBehaviour
    {
        [SerializeField] float cellSideLength = 5;
        [SerializeField] Vector2Int divided;

        [SerializeField] Vector2 center;
        [SerializeField] float cellBufferZoneBound = 8;
        [SerializeField] Color cellZoneColor;
        [SerializeField] Color bufferZoneColor;
        [SerializeField] Color heightLightColor;
        [SerializeField] Transform supervisor;

        [SerializeField] GameObject squareTilePrefab;
        RectangleGrid squareGrid;

        GameObject normalTileRoot;
        Dictionary<Rectangle, SquareTileComponent> squareTileDict
            = new Dictionary<Rectangle, SquareTileComponent>();

        List<Rectangle> highlightCache = new List<Rectangle>();
        private void Start()
        {
            normalTileRoot = new GameObject("NormalTileRoot");
            normalTileRoot.transform.SetParent(transform);
            normalTileRoot.transform.ResetLocalTransform();

            squareGrid = new RectangleGrid(cellSideLength, cellSideLength, (uint)divided.x, (uint)divided.y, center.x, center.y, cellBufferZoneBound, cellBufferZoneBound);

            var squares = squareGrid.GetAllRectangles();
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
            highlightCache.AddRange(squareGrid.GetRectangles(pos.x, pos.y));
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
            var squares = squareGrid.GetAllRectangles();
            var slength = squares.Length;
            for (int i = 0; i < slength; i++)
            {
                Gizmos.color = cellZoneColor;
                var pos = new Vector3(squares[i].CenterX, 0, squares[i].CenterY);
                var size = new Vector3(squareGrid.CellWidth, 6, squareGrid.CellHeight);
                Gizmos.DrawWireCube(pos, size);
                Gizmos.color = bufferZoneColor;
                var cellSize = new Vector3(squareGrid.CellWidth + cellBufferZoneBound * 2, 8, squareGrid.CellHeight + cellBufferZoneBound * 2);
                Gizmos.DrawWireCube(pos, cellSize);
            }
            if (true)
            {
                var length = highlightCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var curSquare = highlightCache[i];
                    Gizmos.color = heightLightColor;
                    var hlSize = new Vector3(curSquare.Width, 1, curSquare.Height);
                    var hlPos = new Vector3(curSquare.CenterX, 0, curSquare.CenterY);
                    Gizmos.DrawCube(hlPos, hlSize);
                }
            }
        }
    }
}
