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
            return other.Radius - Radius < dis && dis < other.Radius + Radius;
        }

        /// <summary>
        /// 是否相切；
        /// </summary>
        public bool IsIntersectWith(Circle other)
        {
            var dis = Math.Sqrt(Math.Pow(other.Center.X - Center.X, 2) + Math.Pow(other.Center.Y - Center.Y, 2));
            return Math.Abs(other.Radius - Radius - dis) < 1e-7 || Math.Abs(dis - (other.Radius + Radius)) < 1e-7;
        }

        /// <summary>
        /// 是否相离；
        /// </summary>
        public bool IsSeparateWith(Circle other)
        {
            return !IsCrossWith(other) && !IsIntersectWith(other);
        }
    }
}
