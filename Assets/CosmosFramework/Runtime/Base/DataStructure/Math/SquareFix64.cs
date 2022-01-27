using FixMath.NET;
using System;
namespace Cosmos
{
    public struct SquareFix64 : IEquatable<SquareFix64>
    {
        public Fix64 CenterX { get; private set; }
        public Fix64 CenterY { get; private set; }
        public Fix64 SideLength { get; private set; }
        public Fix64 Top { get; private set; }
        public Fix64 Bottom { get; private set; }
        public Fix64 Left { get; private set; }
        public Fix64 Right { get; private set; }
        public Fix64 HalfSideLength { get; private set; }
        public SquareFix64(Fix64 centerX, Fix64 centerY, Fix64 sideLength)
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
        public static readonly SquareFix64 Zero = new SquareFix64 (Fix64.Zero, Fix64.Zero, Fix64.Zero);
        public static readonly SquareFix64 One = new SquareFix64(Fix64.One, Fix64.One, Fix64.One);
        public static bool operator ==(SquareFix64 a, SquareFix64 b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(SquareFix64 a, SquareFix64 b)
        {
            return !a.Equals(b);
        }
        public override bool Equals(object obj)
        {
            return obj is SquareFix64  && Equals((SquareFix64)obj);
        }
        public bool Equals(SquareFix64 other)
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
            return $"[ X:{CenterX.RawValue} ,Y:{CenterY.RawValue} ],[ SideLength:{SideLength.RawValue} ]";
        }
    }
}
