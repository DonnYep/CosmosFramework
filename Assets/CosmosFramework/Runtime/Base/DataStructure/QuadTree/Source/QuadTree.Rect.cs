using System;
namespace Cosmos
{
    public partial class QuadTree<T>
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
            public Rectangle(float x, float y, float width, float height)
            {
                CenterX = x;
                CenterY = y;
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
    }
}