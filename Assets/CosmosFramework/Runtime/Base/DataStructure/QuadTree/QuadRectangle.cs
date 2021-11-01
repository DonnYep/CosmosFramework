using System;
namespace Cosmos.QuadTree
{
    /// <summary>
    /// (x,y) is center point
    /// </summary>
    public struct QuadRectangle : IEquatable<QuadRectangle>
    {
        /// <summary>
        /// CenterX;
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// CenterY;
        /// </summary>
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Top { get { return Y + HalfHeight; } }
        public float Bottom { get { return Y - HalfHeight; } }
        public float Left { get { return X - HalfWidth; } }
        public float Right { get { return X + HalfWidth; } }
        public float HalfWidth { get { return Width * 0.5f; } }
        public float HalfHeight { get { return Height * 0.5f; } }
        public QuadRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public bool Contains(float x, float y)
        {
            if (x < Left || x > Right) return false;
            if (y > Top || y < Bottom) return false;
            return true;
        }
        public static bool operator ==(QuadRectangle a, QuadRectangle b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(QuadRectangle a, QuadRectangle b)
        {
            return !a.Equals(b);
        }
        public override bool Equals(object obj)
        {
            return obj is QuadRectangle && Equals((QuadRectangle)obj);
        }
        public bool Equals(QuadRectangle other)
        {
            return this.X == other.X && this.Y == other.Y &&
                  this.Width == other.Width && this.Height == other.Height;
        }
        public override int GetHashCode()
        {
            var hashStr = $"{X}{Y}{Width}{Height}";
            return hashStr.GetHashCode();
        }
        public override string ToString()
        {
            return $"[ X:{X} ,Y:{Y} ],[ Width:{Width},Height:{Height} ]";
        }
        public static readonly QuadRectangle Zero = new QuadRectangle(0, 0, 0, 0);
        public static readonly QuadRectangle One = new QuadRectangle(1, 1, 1, 1);
    }
}
