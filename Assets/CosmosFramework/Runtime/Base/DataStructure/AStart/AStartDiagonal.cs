using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class AStartDiagonal : AStart
    {
        public AStartDiagonal(float gridCenterX, float gridCenterY, int xCount, int yCount, float nodeSideLength)
            : base(gridCenterX, gridCenterY, xCount, yCount, nodeSideLength){}
        protected override int GetDistance(Node a, Node b)
        {
            return GetDiagonalDistance(a, b);
        }
    }
}
