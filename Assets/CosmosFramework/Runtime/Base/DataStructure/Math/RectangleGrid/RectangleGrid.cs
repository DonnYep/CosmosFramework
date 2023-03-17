using System;
namespace Cosmos
{
    /// <summary>
    /// Cartesian coordinates
    /// 参考二维笛卡尔坐标系
    /// </summary>
    public struct RectangleGrid : IEquatable<RectangleGrid>
    {
        Rectangle[,] rectangle2d;
        Rectangle[] rectangle1d;
        public Rectangle GridArea { get; private set; }
        public uint RowCount { get; private set; }
        public uint ColumnCount { get; private set; }
        public float CellWidth { get; private set; }
        public float CellHeight { get; private set; }
        /// <summary>
        /// 宽度的缓冲区范围；
        /// 缓冲区=bufferRange+border;
        /// </summary>
        public float WidthBufferZoneRange { get; private set; }
        /// <summary>
        /// 高度的缓冲区范围；
        /// 缓冲区=bufferRange+border;
        /// </summary>
        public float HeightBufferZoneRange { get; private set; }
        public float CenterX { get { return GridArea.CenterX; } }
        public float CenterY { get { return GridArea.CenterY; } }
        float OffsetX;
        float OffsetY;
        public RectangleGrid(float cellWidth, float cellHeight, uint rowCount, uint columnCount)
    : this(cellWidth, cellHeight, rowCount, columnCount, 0, 0, 0, 0) { }
        public RectangleGrid(float cellWidth, float cellHeight, uint rowCount, uint columnCount, float centerX, float centerY)
            : this(cellWidth, cellHeight, rowCount, columnCount, centerX, centerY, 0, 0) { }
        public RectangleGrid(float cellWidth, float cellHeight, uint rowCount, uint columnCount, float centerX, float centerY, float widthBufferRange, float heightBufferRange)
        {
            if (cellWidth <= 0)
                throw new OverflowException("cellWidth can not less or equal than zero !");
            if (cellHeight <= 0)
                throw new OverflowException("cellHeight  can not less or equal than zero !");
            RowCount = rowCount;
            ColumnCount = columnCount;

            var width = cellWidth * rowCount;
            var height = cellHeight * columnCount;
            CellHeight = cellHeight;
            CellWidth = cellWidth;
            rectangle2d = new Rectangle[columnCount, rowCount];
            WidthBufferZoneRange = widthBufferRange >= 0 ? widthBufferRange : 0;
            HeightBufferZoneRange = heightBufferRange >= 0 ? heightBufferRange : 0;
            GridArea = new Rectangle(centerX, centerY, width, height);
            rectangle1d = new Rectangle[RowCount * ColumnCount];
            this.OffsetX = centerX - GridArea.HalfWidth;
            this.OffsetY = centerY - GridArea.HalfHeight;

            var centerOffsetX = cellWidth / 2 + OffsetX;
            var centerOffsetY = cellHeight / 2 + OffsetY;
            for (int y = 0; y < columnCount; y++)
            {
                for (int x = 0; x < rowCount; x++)
                {
                    var xCenter = x * CellWidth + centerOffsetX;
                    var yCenter = y * CellHeight + centerOffsetY;
                    rectangle2d[y, x] = new Rectangle(xCenter, yCenter, CellWidth, CellHeight);
                    rectangle1d[y * rowCount + x] = rectangle2d[y, x];
                }
            }
        }
        /// <summary>
        /// 获取与位置重合的块；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <returns>位置所在的块</returns>
        public Rectangle GetRectangle(float posX, float posY)
        {
            if (!IsOverlapping(posX, posY))
                return Rectangle.Zero;
            var row = (int)((posX - OffsetX) / CellWidth);
            var col = (int)((posY - OffsetY) / CellHeight);
            return rectangle2d[col, row];
        }
        /// <summary>
        /// 获取位置所在缓冲区所有重叠的块；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <returns>缓冲所属的块</returns>
        public Rectangle[] GetRectangles(float posX, float posY)
        {
            if (!IsOverlappingBufferZone(posX, posY))
                return new Rectangle[0];
            if (!IsOverlapping(posX, posY))
                return new Rectangle[0];
            var row = (int)((posX - OffsetX) / CellWidth);
            var col = (int)((posY - OffsetY) / CellHeight);
            Rectangle[] neighborSquares = new Rectangle[9];
            int idx = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (y == 0 && x == 0)
                        continue;
                    int idxX = row + x;
                    int idxY = col + y;
                    if (idxX <= RowCount - 1 && idxX >= 0 && idxY <= ColumnCount - 1 && idxY >= 0)
                    {
                        neighborSquares[idx++] = rectangle2d[idxY, idxX];
                    }
                }
            }
            var srcSquares = new Rectangle[idx];
            int dstIdx = 0;
            for (int i = 0; i < idx; i++)
            {
                if (IsOverlappingCellBufferZone(neighborSquares[i], posX, posY))
                {
                    srcSquares[dstIdx] = neighborSquares[i];
                    dstIdx++;
                }
            }
            var dstSquares = new Rectangle[dstIdx + 1];
            dstSquares[0] = rectangle2d[col, row];
            Array.Copy(srcSquares, 0, dstSquares, 1, dstIdx);
            return dstSquares;
        }
        /// <summary>
        /// 获取位置临近块；
        /// 若level为0，则表示当前所在的块；若level为1，则获取九宫格；依此类推；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <param name="level">临近的层级</param>
        /// <returns>获取到的临近地块</returns>
        public Rectangle[] GetNearbyRectangles(float posX, float posY, int level = 0)
        {
            if (!IsOverlapping(posX, posY))
                return new Rectangle[0];
            if (!IsOverlapping(posX, posY))
                return new Rectangle[0];
            var row = (int)((posX - OffsetX) / CellWidth);
            var col = (int)((posY - OffsetY) / CellHeight);
            level = level >= 0 ? level : 0;
            if (level == 0)
                return new Rectangle[] { rectangle2d[col, row] };
            int sideCellCount = level * 2 + 1;
            int rectCount = sideCellCount * sideCellCount;
            Rectangle[] neabySquares = new Rectangle[rectCount + 1];
            int idx = 0;
            for (int x = -level; x <= level; x++)
            {
                for (int y = -level; y <= level; y++)
                {
                    int idxX = row + x;
                    int idxY = col + y;
                    if (idxX <= RowCount - 1 && idxX >= 0 && idxY <= ColumnCount - 1 && idxY >= 0)
                    {
                        neabySquares[idx] = rectangle2d[idxY, idxX];
                        idx++;
                    }
                }
            }
            var dstSquares = new Rectangle[idx];
            dstSquares[0] = rectangle2d[col, row];
            Array.Copy(neabySquares, 0, dstSquares, 0, idx);
            return dstSquares;
        }
        /// <summary>
        /// 获取所有单元格方块
        /// </summary>
        /// <returns>所有单元格方块</returns>
        public Rectangle[] GetAllRectangles()
        {
            return rectangle1d;
        }
        /// <summary>
        /// 是否与整个大方块重叠；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <returns>是否重叠</returns>
        public bool IsOverlapping(float posX, float posY)
        {
            if (posX < GridArea.Left || posX > GridArea.Right) return false;
            if (posY < GridArea.Bottom || posY > GridArea.Top) return false;
            return true;
        }
        public static bool operator ==(RectangleGrid lhs, RectangleGrid rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(RectangleGrid lhs, RectangleGrid rhs)
        {
            return !lhs.Equals(rhs);
        }
        public bool Equals(RectangleGrid other)
        {
            return other.GridArea == this.GridArea &&
                other.CellWidth == this.CellWidth &&
                other.CellHeight == this.CellHeight &&
                other.RowCount == this.RowCount &&
                other.ColumnCount == this.ColumnCount &&
                other.WidthBufferZoneRange == this.WidthBufferZoneRange &&
                other.HeightBufferZoneRange == this.HeightBufferZoneRange;
        }
        public override bool Equals(object obj)
        {
            return obj is RectangleGrid && (Equals((RectangleGrid)obj));
        }
        public override int GetHashCode()
        {
            var hashStr = $"{GridArea}{RowCount}{ColumnCount}{CellWidth}{CellHeight}{WidthBufferZoneRange}{HeightBufferZoneRange}";
            return hashStr.GetHashCode();
        }
        bool IsOverlappingBufferZone(float posX, float posY)
        {
            if (posX < GridArea.Left - WidthBufferZoneRange || posX > GridArea.Right + WidthBufferZoneRange) return false;
            if (posY < GridArea.Bottom - HeightBufferZoneRange || posY > GridArea.Top + HeightBufferZoneRange) return false;
            return true;
        }
        bool IsOverlappingCellBufferZone(Rectangle rectangle, float posX, float posY)
        {
            if (posX < rectangle.Left - WidthBufferZoneRange || posX > rectangle.Right + WidthBufferZoneRange) return false;
            if (posY < rectangle.Bottom - HeightBufferZoneRange || posY > rectangle.Top + HeightBufferZoneRange) return false;
            return true;
        }
    }
}
