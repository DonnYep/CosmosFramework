using System;

namespace Cosmos
{
    /// <summary>
    /// 圆形；
    /// </summary>
    public class Circle
    {
        /// <summary>
        /// 圆形
        /// </summary>
        /// <param name="center">圆心坐标</param>
        /// <param name="radius">半径</param>
        public Circle(Point2D center, double radius)
        {
            Center = center;
            Radius = radius;
            if (radius < 0)
            {
                throw new ArgumentException("半径不能为负数", nameof(radius));
            }
        }

        /// <summary>
        /// 圆心坐标；
        /// </summary>
        public Point2D Center { get; }

        /// <summary>
        /// 半径；
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// 是否相交；
        /// </summary>
        public bool IsCrossWith(Circle other)
        {
            var dis = Math.Sqrt(Math.Pow(other.Center.X - Center.X, 2) + Math.Pow(other.Center.Y - Center.Y, 2));
            //圆心距必须大于半径差（否则是包含/同心），且小于半径和（否则是相离/外切）
            return Math.Abs(other.Radius - Radius) < dis && dis < other.Radius + Radius;
        }

        /// <summary>
        /// 是否相切；
        /// </summary>
        public bool IsIntersectWith(Circle other)
        {
            var dis = Math.Sqrt(Math.Pow(other.Center.X - Center.X, 2) + Math.Pow(other.Center.Y - Center.Y, 2));
            //内切：圆心距等于半径差的绝对值；重合的等半径圆不算相切
            var internalTangent = dis > 0 && Math.Abs(Math.Abs(other.Radius - Radius) - dis) < 1e-7;
            var externalTangent = Math.Abs(dis - (other.Radius + Radius)) < 1e-7;
            return internalTangent || externalTangent;
        }

        /// <summary>
        /// 是否相离；
        /// </summary>
        public bool IsSeparateWith(Circle other)
        {
            var dis = Math.Sqrt(Math.Pow(other.Center.X - Center.X, 2) + Math.Pow(other.Center.Y - Center.Y, 2));
            //同心、包含、相交、相切都不算相离
            return dis > other.Radius + Radius;
        }
    }
}
