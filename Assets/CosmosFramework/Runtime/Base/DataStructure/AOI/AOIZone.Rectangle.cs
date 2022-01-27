using System;
namespace Cosmos
{
    public partial class AOIZone<T>
    {
        public struct Rectangle : IEquatable<Rectangle>
        {
            public float CenterX { get; set; }
            public float CenterY { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public float Top { get { return CenterY + HalfHeight; } }
            public float Bottom { get { return CenterY - HalfHeight; } }
            public float Left { get { return CenterX - HalfWidth; } }
            public float Right { get { return CenterX + HalfWidth; } }
            public float HalfWidth { get { return Width * 0.5f; } }
            public float HalfHeight { get { return Height * 0.5f; } }
            public Rectangle(float centerX, float centerY, float width, float height)
            {
                CenterX = centerX;
                CenterY = centerY;
                Width = width;
                Height = height;
            }
            public bool Contains(float x, float y)
            {
                if (x < Left || x > Right) return false;
                if (y > Top || y < Bottom) return false;
                return true;
            }
            public static bool operator ==(Rectangle a, Rectangle b)
            {
                return a.Equals(b);
            }
            public static bool operator !=(Rectangle a, Rectangle b)
            {
                return !a.Equals(b);
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
            public static readonly Rectangle Zero = new Rectangle(0, 0, 0, 0);
            public static readonly Rectangle One = new Rectangle(1, 1, 1, 1);
        }
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
    }
}