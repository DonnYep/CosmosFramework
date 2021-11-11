using System;
using FixMath.NET;
namespace Cosmos
{
    /// <summary>
    /// 方形网格数据结构；
    /// </summary>
    public struct SquareGridFix64
    {
        public struct Square
        {
            public Fix64 CenterX { get; private set; }
            public Fix64 CenterY { get; private set; }
            public Fix64 SideLength { get; private set; }
            public Fix64 Top { get; private set; }
            public Fix64 Bottom { get; private set; }
            public Fix64 Left { get; private set; }
            public Fix64 Right { get; private set; }
            public Fix64 HalfSideLength { get; private set; }
            public Square(Fix64 centerX, Fix64 centerY, Fix64 sideLength)
            {
                CenterX = centerX;
                CenterY = centerY;
                SideLength = sideLength;
                HalfSideLength = sideLength / (Fix64)2;

                Top = centerY + HalfSideLength;
                Bottom = centerY - HalfSideLength;
                Left = centerX - HalfSideLength;
                Right = CenterX + HalfSideLength;
            }
            public bool Contains(Fix64 x, Fix64 y)
            {
                if (x < Left || x > Right) return false;
                if (y > Top || y < Bottom) return false;
                return true;
            }
            public static readonly Square Zero = new Square(Fix64.Zero, Fix64.Zero, Fix64.Zero);
            public static readonly Square One = new Square(Fix64.One, Fix64.One, Fix64.One);
        }
        Square[,] square2d;
        Square[] square1d;
        public Fix64 CenterX { get; private set; }
        public Fix64 CenterY { get; private set; }
        public uint CellSection { get; private set; }
        public Fix64 SquareSideLength { get; private set; }
        public Fix64 SquareTop { get; private set; }
        public Fix64 SquareBottom { get; private set; }
        public Fix64 SquareLeft { get; private set; }
        public Fix64 SquareRight { get; private set; }
        public Fix64 HalfSideLength { get; private set; }
        public Fix64 CellSideLength { get; private set; }
        public Fix64 OffsetX { get; private set; }
        public Fix64 OffsetY { get; private set; }
        /// <summary>
        /// 缓冲去范围；
        /// 缓冲区=bufferRange+border;
        /// </summary>
        public Fix64 BufferZoneRange { get; private set; }
        public SquareGridFix64(Fix64 cellSideLength, uint cellSection, Fix64 offsetX, Fix64 offsetY) : this(cellSideLength, cellSection, offsetX, offsetY, Fix64.Zero) { }
        public SquareGridFix64(Fix64 cellSideLength, uint cellSection, Fix64 offsetX, Fix64 offsetY, Fix64 bufferRange)
        {
            if (cellSideLength < Fix64.Zero)
                throw new OverflowException("cellSideLength can not less than zero !");
            CellSection = cellSection;

            this.OffsetX = offsetX;
            this.OffsetY = offsetY;

            SquareSideLength = cellSideLength * (Fix64)cellSection;
            CellSideLength = cellSideLength;
            square2d = new Square[cellSection, cellSection];
            HalfSideLength = SquareSideLength / (Fix64)2;

            CenterX = HalfSideLength + offsetX;
            CenterY = HalfSideLength + offsetY;

            SquareLeft = CenterX - HalfSideLength;
            SquareRight = CenterX + HalfSideLength;

            SquareTop = CenterY + HalfSideLength;
            SquareBottom = CenterY - HalfSideLength;

            BufferZoneRange = bufferRange;

            square1d = new Square[CellSection * CellSection];
            CreateSquare(offsetX, offsetY);
        }
        /// <summary>
        /// 获取与位置重合的块；
        /// </summary>
        /// <param name="posX">位置X</param>
        /// <param name="posY">位置Y</param>
        /// <returns>位置所在的块</returns>
        public Square GetSquare(Fix64 posX, Fix64 posY)
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
        public Square[] GetSquares(Fix64 posX, Fix64 posY)
        {
            if (!IsOverlappingBufferZone(posX, posY))
                return new Square[0];
            var col = (int)((posX - OffsetX) / CellSideLength);
            var row = (int)((posY - OffsetY) / CellSideLength);
            if (!IsOverlapping(posX, posY))
                return new Square[0];
            Square[] neighborSquares = new Square[9];

            int idx = 0;
            var leftCol = col - 1;
            var rightCol = col + 1;
            var upRow = row + 1;
            var downRow = row - 1;

            if (leftCol >= 0)
            {
                neighborSquares[idx++] = square2d[leftCol, row];
                if (upRow < CellSection)
                    neighborSquares[idx++] = square2d[leftCol, upRow];
                if (downRow >= 0)
                    neighborSquares[idx++] = square2d[leftCol, downRow];
            }
            if (rightCol < CellSection)
            {
                neighborSquares[idx++] = square2d[rightCol, row];
                if (upRow < CellSection)
                    neighborSquares[idx++] = square2d[rightCol, upRow];
                if (downRow >= 0)
                    neighborSquares[idx++] = square2d[rightCol, downRow];
            }
            if (upRow < CellSection)
                neighborSquares[idx++] = square2d[col, upRow];
            if (downRow >= 0)
                neighborSquares[idx++] = square2d[col, downRow];

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
        public bool IsOverlapping(Fix64 posX, Fix64 posY)
        {
            if (posX < SquareLeft || posX > SquareRight) return false;
            if (posY < SquareBottom || posY > SquareTop) return false;
            return true;
        }
        bool IsOverlappingBufferZone(Fix64 posX, Fix64 posY)
        {
            if (posX < SquareLeft - BufferZoneRange || posX > SquareRight + BufferZoneRange) return false;
            if (posY < SquareBottom - BufferZoneRange || posY > SquareTop + BufferZoneRange) return false;
            return true;
        }
        bool IsOverlappingCellBufferZone(Square square, Fix64 posX, Fix64 posY)
        {
            if (posX < square.Left - BufferZoneRange || posX > square.Right + BufferZoneRange) return false;
            if (posY < square.Bottom - BufferZoneRange || posY > square.Top + BufferZoneRange) return false;
            return true;
        }
        void CreateSquare(Fix64 offsetX, Fix64 offsetY)
        {
            var centerOffsetX = CellSideLength / (Fix64)2 + offsetX;
            var centerOffsetY = CellSideLength / (Fix64)2 + offsetY;
            for (int i = 0; i < CellSection; i++)
            {
                for (int j = 0; j < CellSection; j++)
                {
                    square2d[i, j] = new Square((Fix64)i * CellSideLength + centerOffsetX, (Fix64)j * CellSideLength + centerOffsetY, CellSideLength);
                    square1d[i * (CellSection - 1) + j] = square2d[i, j];
                }
            }
        }
    }
}