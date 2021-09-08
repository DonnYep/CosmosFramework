using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.QuadTree
{
    public class QuadTreeRect : Rectangle
    {
        public bool IsOverlapped { get; set; }
        public QuadTreeRect(float x, float y, float width, float height) : base(x, y, width, height)
        {
            IsOverlapped = false;
        }
    }
}