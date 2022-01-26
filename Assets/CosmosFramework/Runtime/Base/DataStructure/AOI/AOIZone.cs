using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public partial class AOIZone<T>
        where T : IComparable
    {
        public Square ZoneSquare { get; private set; }
        /// <summary>
        /// 横向X的均等分数；
        /// </summary>
        public int RowDivide { get; private set; }
        /// <summary>
        /// 纵向Y的均等分数；
        /// </summary>
        public int ColumnDivide { get; private set; }
        /// <summary>
        /// 中心X轴偏移量；
        /// </summary>
        public float OffsetX { get; private set; }
        /// <summary>
        /// 中心Y轴偏移量；
        /// </summary>
        public float OffsetY { get; private set; }

        public float SquareSideLength { get; private set; }
        IObjectHelper objectHelper;

        List<Square> squareList = new List<Square>();

        public AOIZone(int widthCount, int heightCount, float squareSideLentgh, float offsetX, float offsetY, IObjectHelper objectHelper)
        {
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
         var   HalfSideLength = SquareSideLength / 2;

            CreateSquare(offsetX,offsetY);
        }
        public Square GetArea(T obj)
        {
            return Square.Zero;
        }
        public bool IsOverlapping(T obj)
        {
            var posX = objectHelper.GetCenterX(obj);
            var posY = objectHelper.GetCenterY(obj);
            return IsOverlapping(posX, posY);
        }
        public bool IsOverlapping(float posX, float posY)
        {
            if (posX < ZoneSquare.Left || posX > ZoneSquare.Right) return false;
            if (posY < ZoneSquare.Bottom || posY > ZoneSquare.Top) return false;
            return true;
        }
        void CreateSquare(float offsetX, float offsetY)
        {
            var centerOffsetX = SquareSideLength / 2 + offsetX;
            var centerOffsetY = SquareSideLength / 2 + offsetY;
            for (int x = 0; x < RowDivide; x++)
            {
                for (int y = 0; y < ColumnDivide; y++)
                {
                    var square = new Square(x * SquareSideLength + centerOffsetX, y * SquareSideLength + centerOffsetY, SquareSideLength);
                    squareList.Add(square);
                }
            }
        }
    }
}
