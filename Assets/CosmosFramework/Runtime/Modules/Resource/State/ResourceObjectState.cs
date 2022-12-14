using System.Runtime.InteropServices;

namespace Cosmos.Resource.State
{
    /// <summary>
    /// 资源对象状态；
    /// 用于查看引用计数，加载信息等；
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct ResourceObjectState
    {
        /// <summary>
        /// 资源对象名；
        /// </summary>
        public string ResourceObjectName;
        /// <summary>
        /// 资源对象路径；
        /// </summary>
        public string ResourceObjectPath;
        /// <summary>
        /// 资源所属包体名；
        /// </summary>
        public string ResourceBundleName;
        /// <summary>
        /// 资源对象类型；
        /// </summary>
        public string ResourceObjectType;
        /// <summary>
        /// 资源扩展名；
        /// </summary>
        public string ResourceExtension;
        /// <summary>
        /// 引用次数；
        /// </summary>
        public int ResourceReferenceCount;
    }
}
