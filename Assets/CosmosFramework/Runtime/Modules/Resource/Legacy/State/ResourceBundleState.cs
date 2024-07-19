using System.Runtime.InteropServices;

namespace Cosmos.Resource.State
{
    /// <summary>
    /// 资源包状态；
    /// 用于查询引用状态，加载信息等；
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct ResourceBundleState
    {
        /// <summary>
        /// 资源包名。
        /// </summary>
        public string ResourceBundleName;
        /// <summary>
        /// 资源包引用计数。
        /// </summary>
        public int ResourceBundleReferenceCount;
        /// <summary>
        /// 包中对象的数量。
        /// </summary>
        public int ResourceObjectCount;
        public static readonly ResourceBundleState Default = new ResourceBundleState();
    }
}
