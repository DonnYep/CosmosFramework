using System.Collections.Generic;
using System;
namespace Cosmos.Resource
{
    /// <summary>
    /// 资源文件列表
    /// </summary>
    [Serializable]
    public class ResourceObjectList
    {
        /// <summary>
        /// ResourcePath===ResourceObject
        /// </summary>
        public Dictionary<string, ResourceObject> ResourceObjectDict;
    }
}
