using System;
using System.Collections.Generic;

namespace Cosmos
{
    public class AStarDiagonal : AStar
    {
        public AStarDiagonal(float gridCenterX, float gridCenterY, int xCount, int yCount, float nodeSideLength)
            : base(gridCenterX, gridCenterY, xCount, yCount, nodeSideLength){}

        protected override int GetDistance(Node a, Node b)
        {
            return GetDiagonalDistance(a, b);
        }
    }
}
