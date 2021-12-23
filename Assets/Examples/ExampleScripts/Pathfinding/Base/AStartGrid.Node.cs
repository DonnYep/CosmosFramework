using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public partial class AStartGrid
    {
        public class Node
        {
            /// <summary>
            /// 网格中X位的序号；
            /// </summary>
            public int IndexX { get; set; }
            /// <summary>
            /// 网格中Y位的序号；
            /// </summary>
            public int IndexY { get; set; }
            /// <summary>
            /// X坐标中心位置
            /// </summary>
            public float CenterX { get; set; }
            /// <summary>
            /// Y坐标中心位置
            /// </summary>
            public float CenterY { get; set; }
            /// <summary>
            /// 边长；
            /// </summary>
            public float SideLength { get; set; }
            /// <summary>
            /// 是否是障碍物；
            /// </summary>
            public bool IsObstacle { get; set; }
            /// <summary>
            /// 父节点；
            /// </summary>
            public Node ParentNode { get; set; }
            /// <summary>
            /// 与初始点的距离；
            /// </summary>
            public int GCost { get; set; }
            /// <summary>
            ///与终点的距离；
            /// </summary>
            public int HCost { get; set; }
            /// <summary>
            /// 距离的代价；
            /// Manhattan or Euclidean distance；
            /// </summary>
            public int FCost { get { return GCost + HCost; } }
            /// <summary>
            /// 当前节点的代价；
            /// </summary>
            public int Cost { get; set; }
            public Node(int indexX, int indexY, float centerX, float centerY, float sideLength, bool isObstacle)
            {
                IndexX = indexX;
                IndexY = indexY;
                CenterX = centerX;
                CenterY = centerY;
                SideLength = sideLength;
                IsObstacle = isObstacle;
            }
        }
    }
}
