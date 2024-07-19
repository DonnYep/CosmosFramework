using System;
using System.Runtime.InteropServices;

namespace Cosmos.Resource.State
{
    /// <summary>
    /// 资源对象信息。
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct ResourceObjectState
    {
        /// <summary>
        /// 资源对象名。
        /// </summary>
        public string ResourceObjectName;
        /// <summary>
        /// 资源对象路径。
        /// </summary>
        public string ResourceObjectPath;
        /// <summary>
        /// 资源所属包体名。
        /// </summary>
        public string ResourceBundleName;
        /// <summary>
        /// 资源扩展名。
        /// </summary>
        public string ResourceExtension;
        /// <summary>
        /// 引用次数。
        /// </summary>
        public int ResourceReferenceCount;
        /// <summary>
        /// 资源类型。
        /// </summary>
        public Type ResourceType;
        public static readonly ResourceObjectState Default = new ResourceObjectState();
    }
}
