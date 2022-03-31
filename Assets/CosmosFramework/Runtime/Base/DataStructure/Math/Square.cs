using System;
namespace Cosmos
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
        public static bool operator ==(Square lhs, Square rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(Square lhs, Square rhs)
        {
            return !lhs.Equals(rhs);
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
}
