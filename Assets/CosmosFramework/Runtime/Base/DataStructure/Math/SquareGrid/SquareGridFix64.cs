using System;
using FixMath.NET;
namespace Cosmos
{
    /// <summary>
    /// 方形网格数据结构；
    /// </summary>
    public struct SquareGridFix64 : IEquatable<SquareGridFix64>
    {
        SquareFix64[,] square2d;
        SquareFix64[] square1d;
        public SquareFix64 GridArea { get; private set; }
        public uint CellSection { get; private set; }
        public Fix64 CellSideLength { get; private set; }
        public Fix64 CenterX { get { return GridArea.CenterX; } }
        public Fix64 CenterY { get { return GridArea.CenterY; } }
        /// <summary>
        /// 缓冲区范围；
        /// 缓冲区=bufferRange+border;
        /// </summary>
        public Fix64 BufferZoneRange { get; private set; }
        Fix64 OffsetX;
        Fix64 OffsetY;
        public SquareGridFix64(Fix64 cellSideLength, uint cellSection) : this(cellSideLength, cellSection, Fix64.Zero, Fix64.Zero, Fix64.Zero) { }
        public SquareGridFix64(Fix64 cellSideLength, uint cellSection, Fix64 centerX, Fix64 centerY) : this(cellSideLength, cellSection, centerX, centerY, Fix64.Zero) { }
        public SquareGridFix64(Fix64 cellSideLength, uint cellSection, Fix64 centerX, Fix64 centerY, Fix64 bufferRange)
        {
            if (cellSideLength < Fix64.Zero)
                throw new OverflowException("cellSideLength can not less than zero !");
            CellSection = cellSection;
            var squareSideLength = cellSideLength * (Fix64)cellSection;
            CellSideLength = cellSideLength;
            square2d = new SquareFix64[cellSection, cellSection];
            BufferZoneRange = bufferRange >= Fix64.Zero ? bufferRange : Fix64.Zero;
            GridArea = new SquareFix64(centerX, centerY, squareSideLength);
            square1d = new SquareFix64[CellSection * CellSection];
            this.OffsetX = centerX - GridArea.HalfSideLength;
            this.OffsetY = centerY - GridArea.HalfSideLength;

            var centerOffsetX = CellSideLength / (Fix64)2 + OffsetX;
            var centerOffsetY = CellSideLength / (Fix64)2 + OffsetY;
            for (int i = 0; i < CellSection; i++)
            {
                for (int j = 0; j < CellSection; j++)
                {
                    square2d[i, j] = new SquareFix64((Fix64)j * CellSideLength + centerOffsetX, (Fix64)i * CellSideLength + centerOffsetY, CellSideLength);
                    square1d[i * CellSection + j] = square2d[i, j];
                }
            }
        }
        /// <summary>
        /// 获取与位置重合的块；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <returns>位置所在的块</returns>
        public SquareFix64 GetSquare(Fix64 posX, Fix64 posY)
        {
            if (!IsOverlapping(posX, posY))
                return SquareFix64.Zero;
            var row = (int)((posX - OffsetX) / CellSideLength);
            var col = (int)((posY - OffsetY) / CellSideLength);
            return square2d[col, row];
        }
        /// <summary>
        /// 获取位置所在缓冲区所有重叠的块；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <returns>缓冲所属的块</returns>
        public SquareFix64[] GetSquares(Fix64 posX, Fix64 posY)
        {
            if (!IsOverlappingBufferZone(posX, posY))
                return new SquareFix64[0];
            if (!IsOverlapping(posX, posY))
                return new SquareFix64[0];
            var row = (int)((posX - OffsetX) / CellSideLength);
            var col = (int)((posY - OffsetY) / CellSideLength);
            SquareFix64[] neighborSquares = new SquareFix64[9];
            int idx = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (y == 0 && x == 0)
                        continue;
                    int idxX = col + x;
                    int idxY = row + y;
                    if (idxX <= CellSection - 1 && idxX >= 0 && idxY <= CellSection - 1 && idxY >= 0)
                    {
                        neighborSquares[idx++] = square2d[idxX, idxY];
                    }
                }
            }
            var srcSquares = new SquareFix64[idx];
            int dstIdx = 0;
            for (int i = 0; i < idx; i++)
            {
                if (IsOverlappingCellBufferZone(neighborSquares[i], posX, posY))
                {
                    srcSquares[dstIdx] = neighborSquares[i];
                    dstIdx++;
                }
            }
            var dstSquares = new SquareFix64[dstIdx + 1];
            dstSquares[0] = square2d[col, row];
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
        public SquareFix64[] GetNearbySquares(Fix64 posX, Fix64 posY, int level = 0)
        {
            if (!IsOverlapping(posX, posY))
                return new SquareFix64[0];
            if (!IsOverlapping(posX, posY))
                return new SquareFix64[0];
            var row = (int)((posX - OffsetX) / CellSideLength);
            var col = (int)((posY - OffsetY) / CellSideLength);
            level = level >= 0 ? level : 0;
            if (level == 0)
                return new SquareFix64[] { square2d[col, row] };
            if (level == CellSection)
                return square1d;
            int sideCellCount = level * 2 + 1;
            int squareCount = sideCellCount * sideCellCount;
            SquareFix64[] neabySquares = new SquareFix64[squareCount + 1];
            int idx = 0;
            for (int x = -level; x <= level; x++)
            {
                for (int y = -level; y <= level; y++)
                {
                    int idxX = col + x;
                    int idxY = row + y;
                    if (idxX < CellSection && idxX >= 0 && idxY < CellSection && idxY >= 0)
                    {
                        neabySquares[idx] = square2d[idxX, idxY];
                        idx++;
                    }
                }
            }
            var dstSquares = new SquareFix64[idx];
            dstSquares[0] = square2d[col, row];
            Array.Copy(neabySquares, 0, dstSquares, 0, idx);
            return dstSquares;
        }
        /// <summary>
        /// 获取所有单元格方块
        /// </summary>
        /// <returns>所有单元格方块</returns>
        public SquareFix64[] GetAllSquares()
        {
            return square1d;
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
        public static bool operator ==(SquareGridFix64 lhs, SquareGridFix64 rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(SquareGridFix64 lhs, SquareGridFix64 rhs)
        {
            return !lhs.Equals(rhs);
        }
        public bool Equals(SquareGridFix64 other)
        {
            return other.GridArea == this.GridArea &&
                      other.CellSideLength == this.CellSideLength &&
                      other.CellSection == this.CellSection &&
                      other.BufferZoneRange == this.BufferZoneRange;
        }
        public override bool Equals(object obj)
        {
            return obj is SquareGridFix64 && (Equals((SquareGridFix64)obj));
        }
        public override int GetHashCode()
        {
            var hashStr = $"{GridArea}{CellSection}{CellSideLength}{BufferZoneRange}";
            return hashStr.GetHashCode();
        }
        bool IsOverlappingBufferZone(Fix64 posX, Fix64 posY)
        {
            if (posX < GridArea.Left - BufferZoneRange || posX > GridArea.Right + BufferZoneRange) return false;
            if (posY < GridArea.Bottom - BufferZoneRange || posY > GridArea.Top + BufferZoneRange) return false;
            return true;
        }
        bool IsOverlappingCellBufferZone(SquareFix64 square, Fix64 posX, Fix64 posY)
        {
            if (posX < square.Left - BufferZoneRange || posX > square.Right + BufferZoneRange) return false;
            if (posY < square.Bottom - BufferZoneRange || posY > square.Top + BufferZoneRange) return false;
            return true;
        }
    }
}