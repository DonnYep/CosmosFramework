using System;
namespace Cosmos
{
    /// <summary>
    /// 方形网格数据结构；
    /// </summary>
    public struct SquareGrid
    {
        public struct Square : IEquatable<Square>
        {
            public float CenterX { get; private set; }
            public float CenterY { get; private set; }
            public float SideLength { get; private set; }
            public float Top { get; private set; }
            public float Bottom { get; private set; }
            public float Left { get; private set; }
            public float Right { get; private set; }
            public float HalfSideLength { get; private set; }
            public Square(float centerX, float centerY, float sideLength)
            {
                CenterX = centerX;
                CenterY = centerY;
                SideLength = sideLength;
                HalfSideLength = sideLength * 0.5f;

                Top = centerY + HalfSideLength;
                Bottom = centerY - HalfSideLength;
                Left = centerX - HalfSideLength;
                Right = CenterX + HalfSideLength;
            }
            public bool Contains(float x, float y)
            {
                if (x < Left || x > Right) return false;
                if (y > Top || y < Bottom) return false;
                return true;
            }
            public static readonly Square Zero = new Square(0, 0, 0);
            public static readonly Square One = new Square(1, 1, 1);
            public static bool operator ==(Square a, Square b)
            {
                return a.Equals(b);
            }
            public static bool operator !=(Square a, Square b)
            {
                return !a.Equals(b);
            }
            public override bool Equals(object obj)
            {
                return obj is Square && Equals((Square)obj);
            }
            public bool Equals(Square other)
            {
                return this.CenterX == other.CenterX && this.CenterY == other.CenterY &&
                      this.SideLength == other.SideLength;
            }
            public override int GetHashCode()
            {
                var hashStr = $"{CenterX}{CenterY}{SideLength}";
                return hashStr.GetHashCode();
            }
            public override string ToString()
            {
                return $"[ X:{CenterX} ,Y:{CenterY} ],[ SideLength:{SideLength} ]";
            }
        }
        Square[,] square2d;
        Square[] square1d;
        public float CenterX { get; private set; }
        public float CenterY { get; private set; }
        public uint CellSection { get; private set; }
        public float SquareSideLength { get; private set; }
        public float SquareTop { get; private set; }
        public float SquareBottom { get; private set; }
        public float SquareLeft { get; private set; }
        public float SquareRight { get; private set; }
        public float HalfSideLength { get; private set; }
        public float CellSideLength { get; private set; }
        public float OffsetX { get; private set; }
        public float OffsetY { get; private set; }
        /// <summary>
        /// 缓冲去范围；
        /// 缓冲区=bufferRange+border;
        /// </summary>
        public float BufferZoneRange { get; private set; }
        public SquareGrid(float cellSideLength, uint cellSection, float offsetX, float offsetY) : this(cellSideLength, cellSection, offsetX, offsetY, 0) { }
        public SquareGrid(float cellSideLength, uint cellSection, float offsetX, float offsetY, float bufferRange)
        {
            if (cellSideLength < 0)
                throw new OverflowException("cellSideLength can not less than zero !");
            CellSection = cellSection;

            this.OffsetX = offsetX;
            this.OffsetY = offsetY;

            SquareSideLength = cellSideLength * cellSection;
            CellSideLength = cellSideLength;
            square2d = new Square[cellSection, cellSection];
            HalfSideLength = SquareSideLength / 2;

            CenterX = HalfSideLength + offsetX;
            CenterY = HalfSideLength + offsetY;

            SquareLeft = CenterX - HalfSideLength;
            SquareRight = CenterX + HalfSideLength;

            SquareTop = CenterY + HalfSideLength;
            SquareBottom = CenterY - HalfSideLength;

            BufferZoneRange = bufferRange >= 0 ? bufferRange : 0;

            square1d = new Square[CellSection * CellSection];
            CreateSquare(offsetX, offsetY);
        }
        /// <summary>
        /// 获取与位置重合的块；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <returns>位置所在的块</returns>
        public Square GetSquare(float posX, float posY)
        {
            if (!IsOverlapping(posX, posY))
                return Square.Zero;
            var col = (posX - OffsetX) / CellSideLength;
            var row = (posY - OffsetY) / CellSideLength;
            return square2d[(int)col, (int)row];
        }
        /// <summary>
        /// 获取位置所在缓冲区所有重叠的块；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <returns>缓冲所属的块</returns>
        public Square[] GetSquares(float posX, float posY)
        {
            if (!IsOverlappingBufferZone(posX, posY))
                return new Square[0];
            var col = (int)((posX - OffsetX) / CellSideLength);
            var row = (int)((posY - OffsetY) / CellSideLength);
            if (!IsOverlapping(posX, posY))
                return new Square[0];
            Square[] neighborSquares = new Square[9];
            int idx = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (y == 0 && x == 0)
                        continue;
                    int idxX = col + x;
                    int idxY = row + y;
                    if (idxX <= CellSection && idxX >= 0 && idxY <= CellSection && idxY >= 0)
                    {
                        neighborSquares[idx++] = square2d[idxX, idxY];
                    }
                }
            }
            var srcSquares = new Square[idx];
            int dstIdx = 0;
            for (int i = 0; i < idx; i++)
            {
                if (IsOverlappingCellBufferZone(neighborSquares[i], posX, posY))
                {
                    srcSquares[dstIdx] = neighborSquares[i];
                    dstIdx++;
                }
            }
            var dstSquares = new Square[dstIdx + 1];
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
        public Square[] GetNearbySquares(float posX, float posY, int level = 0)
        {
            if (!IsOverlapping(posX, posY))
                return new Square[0];
            var col = (int)((posX - OffsetX) / CellSideLength);
            var row = (int)((posY - OffsetY) / CellSideLength);
            if (!IsOverlapping(posX, posY))
                return new Square[0];
            level = level >= 0 ? level : 0;
            if (level == 0)
                return new Square[] { square2d[col, row] };
            if (level == CellSection)
                return square1d;
            int sideCellCount = level * 2 + 1;
            int squareCount = sideCellCount * sideCellCount;
            Square[] neabySquares = new Square[squareCount+1];
            int idx = 0;
            for (int x = -level; x <= level; x++)
            {
                for (int y = -level; y <= level; y++)
                {
                    int idxX = col + x;
                    int idxY = row + y;
                    if (idxX < CellSection && idxX >= 0 && idxY <CellSection && idxY >= 0)
                    {
                        neabySquares[idx] = square2d[idxX, idxY];
                        idx++;
                    }
                }
            }
            var dstSquares = new Square[idx];
            dstSquares[0] = square2d[col, row];
            Array.Copy(neabySquares, 0, dstSquares, 0, idx);
            return dstSquares;
        }
        /// <summary>
        /// 获取所有单元格方块
        /// </summary>
        /// <returns>所有单元格方块</returns>
        public Square[] GetAllSquares()
        {
            return square1d;
        }
        /// <summary>
        /// 是否与整个大方块重叠；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <returns>是否重叠</returns>
        public bool IsOverlapping(float posX, float posY)
        {
            if (posX < SquareLeft || posX > SquareRight) return false;
            if (posY < SquareBottom || posY > SquareTop) return false;
            return true;
        }
        bool IsOverlappingBufferZone(float posX, float posY)
        {
            if (posX < SquareLeft - BufferZoneRange || posX > SquareRight + BufferZoneRange) return false;
            if (posY < SquareBottom - BufferZoneRange || posY > SquareTop + BufferZoneRange) return false;
            return true;
        }
        bool IsOverlappingCellBufferZone(Square square, float posX, float posY)
        {
            if (posX < square.Left - BufferZoneRange || posX > square.Right + BufferZoneRange) return false;
            if (posY < square.Bottom - BufferZoneRange || posY > square.Top + BufferZoneRange) return false;
            return true;
        }
        void CreateSquare(float offsetX, float offsetY)
        {
            var centerOffsetX = CellSideLength / 2 + offsetX;
            var centerOffsetY = CellSideLength / 2 + offsetY;
            for (int i = 0; i < CellSection; i++)
            {
                for (int j = 0; j < CellSection; j++)
                {
                    square2d[i, j] = new Square(i * CellSideLength + centerOffsetX, j * CellSideLength + centerOffsetY, CellSideLength);
                    square1d[i * CellSection + j] = square2d[i, j];
                }
            }
        }
    }
}