namespace Cosmos.QuadTree
{
    public class Rectangle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Top { get { return Y; } }
        public float Bottom { get { return Y + Height; } }
        public float Left { get { return X; } }
        public float Right { get { return X + Width; } }
        public float HalfWidth { get { return Width * 0.5f; } }
        public float HalfHeight { get { return Height * 0.5f; } }
        public float CenterX { get { return X + HalfWidth; } }
        public float CenterY { get { return Y + HalfHeight; } }
        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public bool Contains(float x, float y)
        {
            if (x < Left || x > Right) return false;
            if (y < Top || y > Bottom) return false;
            return true;
        }
    }
}
