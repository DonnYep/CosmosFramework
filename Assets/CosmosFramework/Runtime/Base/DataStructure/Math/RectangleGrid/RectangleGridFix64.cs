using System;
using FixMath.NET;

namespace Cosmos
{
    /// <summary>
    /// https://www.digitalocean.com/community/tutorials/two-dimensional-array-in-c-plus-plus
    /// 存储位参考二维数组
    /// </summary>
    public struct RectangleGridFix64 : IEquatable<RectangleGridFix64>
    {
        RectangleFix64[,] rectangle2d;
        RectangleFix64[] rectangle1d;
        public RectangleFix64 GridArea { get; private set; }
        public uint RowCount { get; private set; }
        public uint ColumnCount { get; private set; }
        public Fix64 CellWidth { get; private set; }
        public Fix64 CellHeight { get; private set; }
        public Fix64 CenterX { get { return GridArea.CenterX; } }
        public Fix64 CenterY { get { return GridArea.CenterY; } }
        /// <summary>
        /// 宽度的缓冲区范围；
        /// 缓冲区=bufferRange+border;
        /// </summary>
        public Fix64 WidthBufferZoneRange { get; private set; }
        /// <summary>
        /// 高度的缓冲区范围；
        /// 缓冲区=bufferRange+border;
        /// </summary>
        public Fix64 HeightBufferZoneRange { get; private set; }
        Fix64 OffsetX;
        Fix64 OffsetY;
        public RectangleGridFix64(Fix64 cellWidth, Fix64 cellHeight, uint rowCount, uint columnCount)
    : this(cellWidth, cellHeight, rowCount, columnCount, Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero) { }
        public RectangleGridFix64(Fix64 cellWidth, Fix64 cellHeight, uint rowCount, uint columnCount, Fix64 centerX, Fix64 centerY)
            : this(cellWidth, cellHeight, rowCount, columnCount, centerX, centerY, Fix64.Zero, Fix64.Zero) { }
        public RectangleGridFix64(Fix64 cellWidth, Fix64 cellHeight, uint rowCount, uint columnCount, Fix64 centerX, Fix64 centerY, Fix64 widthBufferRange, Fix64 heightBufferRange)
        {
            if (cellWidth <= Fix64.Zero)
                throw new OverflowException("cellWidth can not less or equal than zero !");
            if (cellHeight <= Fix64.Zero)
                throw new OverflowException("cellHeight  can not less or equal than zero !");
            RowCount = rowCount;
            ColumnCount = columnCount;

            var width = cellWidth * (Fix64)rowCount;
            var height = cellHeight * (Fix64)columnCount;
            CellHeight = cellHeight;
            CellWidth = cellWidth;
            rectangle2d = new RectangleFix64[rowCount, columnCount];
            WidthBufferZoneRange = widthBufferRange >= Fix64.Zero ? widthBufferRange : Fix64.Zero;
            HeightBufferZoneRange = heightBufferRange >= Fix64.Zero ? heightBufferRange : Fix64.Zero;
            GridArea = new RectangleFix64(centerX, centerY, width, height);
            rectangle1d = new RectangleFix64[RowCount * ColumnCount];

            this.OffsetX = centerX - GridArea.HalfWidth;
            this.OffsetY = centerY - GridArea.HalfHeight;

            var centerOffsetX = cellWidth / (Fix64)2 + OffsetX;
            var centerOffsetY = cellHeight / (Fix64)2 + OffsetY;
            for (int y = 0; y < columnCount; y++)
            {
                for (int x = 0; x < rowCount; x++)
                {
                    var xCenter = (Fix64)x * CellWidth + centerOffsetX;
                    var yCenter = (Fix64)y * CellWidth + centerOffsetY;
                    rectangle2d[y, x] = new RectangleFix64(xCenter, yCenter, CellWidth, CellHeight);
                    rectangle1d[y * ColumnCount + x] = rectangle2d[y, x];
                }
            }
        }
        /// <summary>
        /// 获取与位置重合的块；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <returns>位置所在的块</returns>
        public RectangleFix64 GetRectangle(Fix64 posX, Fix64 posY)
        {
            if (!IsOverlapping(posX, posY))
                return RectangleFix64.Zero;
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
        public RectangleFix64[] GetRectangles(Fix64 posX, Fix64 posY)
        {
            if (!IsOverlappingBufferZone(posX, posY))
                return new RectangleFix64[0];
            if (!IsOverlapping(posX, posY))
                return new RectangleFix64[0];
            var row = (int)((posX - OffsetX) / CellWidth);
            var col = (int)((posY - OffsetY) / CellHeight);
            RectangleFix64[] neighborSquares = new RectangleFix64[9];
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
            var srcSquares = new RectangleFix64[idx];
            int dstIdx = 0;
            for (int i = 0; i < idx; i++)
            {
                if (IsOverlappingCellBufferZone(neighborSquares[i], posX, posY))
                {
                    srcSquares[dstIdx] = neighborSquares[i];
                    dstIdx++;
                }
            }
            var dstSquares = new RectangleFix64[dstIdx + 1];
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
        public RectangleFix64[] GetNearbyRectangles(Fix64 posX, Fix64 posY, int level = 0)
        {
            if (!IsOverlapping(posX, posY))
                return new RectangleFix64[0];
            if (!IsOverlapping(posX, posY))
                return new RectangleFix64[0];
            var row = (int)((posX - OffsetX) / CellWidth);
            var col = (int)((posY - OffsetY) / CellHeight);
            level = level >= 0 ? level : 0;
            if (level == 0)
                return new RectangleFix64[] { rectangle2d[col, row] };
            int sideCellCount = level * 2 + 1;
            int rectCount = sideCellCount * sideCellCount;
            RectangleFix64[] neabySquares = new RectangleFix64[rectCount + 1];
            int idx = 0;
            for (int x = -level; x <= level; x++)
            {
                for (int y = -level; y <= level; y++)
                {
                    int idxX = row + x;
                    int idxY = col + y;
                    if (idxX < RowCount - 1 && idxX >= 0 && idxY < ColumnCount - 1 && idxY >= 0)
                    {
                        neabySquares[idx] = rectangle2d[idxY, idxX];
                        idx++;
                    }
                }
            }
            var dstSquares = new RectangleFix64[idx];
            dstSquares[0] = rectangle2d[col, row];
            Array.Copy(neabySquares, 0, dstSquares, 0, idx);
            return dstSquares;
        }
        /// <summary>
        /// 获取所有单元格方块
        /// </summary>
        /// <returns>所有单元格方块</returns>
        public RectangleFix64[] GetAllRectangle()
        {
            return rectangle1d;
        }
        /// <summary>
        /// 是否与整个大方块重叠；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <returns>是否重叠</returns>
        public bool IsOverlapping(Fix64 posX, Fix64 posY)
        {
            if (posX < GridArea.Left || posX > GridArea.Right) return false;
            if (posY < GridArea.Bottom || posY > GridArea.Top) return false;
            return true;
        }
        public static bool operator ==(RectangleGridFix64 lhs, RectangleGridFix64 rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(RectangleGridFix64 lhs, RectangleGridFix64 rhs)
        {
            return !lhs.Equals(rhs);
        }
        public bool Equals(RectangleGridFix64 other)
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
            return obj is RectangleGridFix64 && (Equals((RectangleGridFix64)obj));
        }
        public override int GetHashCode()
        {
            var hashStr = $"{GridArea}{RowCount}{ColumnCount}{CellWidth}{CellHeight}{WidthBufferZoneRange}{HeightBufferZoneRange}";
            return hashStr.GetHashCode();
        }
        bool IsOverlappingBufferZone(Fix64 posX, Fix64 posY)
        {
            if (posX < GridArea.Left - WidthBufferZoneRange || posX > GridArea.Right + WidthBufferZoneRange) return false;
            if (posY < GridArea.Bottom - HeightBufferZoneRange || posY > GridArea.Top + HeightBufferZoneRange) return false;
            return true;
        }
        bool IsOverlappingCellBufferZone(RectangleFix64 rectangle, Fix64 posX, Fix64 posY)
        {
            if (posX < rectangle.Left - WidthBufferZoneRange || posX > rectangle.Right + WidthBufferZoneRange) return false;
            if (posY < rectangle.Bottom - HeightBufferZoneRange || posY > rectangle.Top + HeightBufferZoneRange) return false;
            return true;
        }
    }
}
