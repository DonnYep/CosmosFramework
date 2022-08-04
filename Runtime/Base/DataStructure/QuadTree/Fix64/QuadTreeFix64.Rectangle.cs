using FixMath.NET;
using System;
namespace Cosmos
{
    public partial class QuadTreeFix64<T>
    {
        public struct Rectangle : IEquatable<Rectangle>
        {
            public Fix64 CenterX { get; set; }
            public Fix64 CenterY { get; set; }
            public Fix64 Width { get; set; }
            public Fix64 Height { get; set; }
            public Fix64 Top { get { return CenterY + HalfHeight; } }
            public Fix64 Bottom { get { return CenterY - HalfHeight; } }
            public Fix64 Left { get { return CenterX - HalfWidth; } }
            public Fix64 Right { get { return CenterX + HalfWidth; } }
            public Fix64 HalfWidth { get { return Width * (Fix64)0.5f; } }
            public Fix64 HalfHeight { get { return Height * (Fix64)0.5f; } }
            public Rectangle(Fix64 centerX, Fix64 centerY, Fix64 width, Fix64 height)
            {
                CenterX = centerX;
                CenterY = centerY;
                Width = width;
                Height = height;
            }
            public bool Contains(Fix64 x, Fix64 y)
            {
                if (x < Left || x > Right) return false;
                if (y > Top || y < Bottom) return false;
                return true;
            }
            public static bool operator ==(Rectangle lhs, Rectangle rhs)
            {
                return lhs.Equals(rhs);
            }
            public static bool operator !=(Rectangle lhs, Rectangle rhs)
            {
                return !lhs.Equals(rhs);
            }
            public override bool Equals(object obj)
            {
                return obj is Rectangle && Equals((Rectangle)obj);
            }
            public bool Equals(Rectangle other)
            {
                return this.CenterX == other.CenterX && this.CenterY == other.CenterY &&
                      this.Width == other.Width && this.Height == other.Height;
            }
            public override int GetHashCode()
            {
                var hashStr = $"{CenterX}{CenterY}{Width}{Height}";
                return hashStr.GetHashCode();
            }
            public override string ToString()
            {
                return $"[ X:{CenterX} ,Y:{CenterY} ],[ Width:{Width},Height:{Height} ]";
            }
            public static readonly Rectangle Zero = new Rectangle(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero);
            public static readonly Rectangle One = new Rectangle(Fix64.One, Fix64.One, Fix64.One, Fix64.One);
        }
    }
}