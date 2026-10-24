using System;
using System.Collections.Generic;

namespace Cosmos.Resource
{
    /// <summary>
    /// name-keyed 索引项：同一个资源标识 + 类型 + 加载方式共享同一个操作，句柄逐个入栈以支持配对释放。
    /// </summary>
    internal sealed class ResourceNameEntry
    {
        public readonly string EntryKey;
        public readonly string AssetKey;
        public readonly Type AssetType;
        public readonly AssetLoadMode LoadMode;
        public readonly ResourceProviderOperation Operation;
        public readonly List<HandleBase> Handles = new List<HandleBase>();

        public ResourceNameEntry(string entryKey, string assetKey, Type assetType, AssetLoadMode loadMode, ResourceProviderOperation operation)
        {
            EntryKey = entryKey;
            AssetKey = assetKey;
            AssetType = assetType;
            LoadMode = loadMode;
            Operation = operation;
        }
    }
}
