namespace Cosmos.QuadTree
{
    /// <summary>
    ///在四叉树中对象的边界获取接口
    /// </summary>
    public interface IObjecRectangletBound<in T>
    {
        /// <summary>
        /// 获取对象的X的位置；
        /// </summary>
        /// <param name="go">对象</param>
        /// <returns>位置数值</returns>
        float GetCenterX(T go);
        /// <summary>
        /// 获取对象的Y的位置；
        /// </summary>
        /// <param name="go">对象</param>
        /// <returns>位置数值</returns>
        float GetCenterY(T go);
        /// <summary>
        /// 获取对象的宽度；
        /// </summary>
        /// <param name="go">对象</param>
        /// <returns>位置数值</returns>
        float GetWidth(T go);
        /// <summary>
        /// 获取对象的长度；
        /// </summary>
        /// <param name="go">对象</param>
        /// <returns>位置数值</returns>
        float GetHeight(T go);
    }
}

