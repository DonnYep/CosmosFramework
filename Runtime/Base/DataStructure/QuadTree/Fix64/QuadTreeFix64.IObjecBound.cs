using FixMath.NET;
namespace Cosmos
{
    public partial class QuadTreeFix64<T>
    {
        /// <summary>
        ///在四叉树中对象的边界获取接口
        /// </summary>
        public interface IObjecBound
        {
            /// <summary>
            /// 获取对象的X的位置；
            /// </summary>
            /// <param name="go">对象</param>
            /// <returns>位置数值</returns>
            Fix64 GetCenterX(T go);
            /// <summary>
            /// 获取对象的Y的位置；
            /// </summary>
            /// <param name="go">对象</param>
            /// <returns>位置数值</returns>
            Fix64 GetCenterY(T go);
            /// <summary>
            /// 获取对象的宽度；
            /// </summary>
            /// <param name="go">对象</param>
            /// <returns>位置数值</returns>
            Fix64 GetWidth(T go);
            /// <summary>
            /// 获取对象的长度；
            /// </summary>
            /// <param name="go">对象</param>
            /// <returns>位置数值</returns>
            Fix64 GetHeight(T go);
        }
    }
}