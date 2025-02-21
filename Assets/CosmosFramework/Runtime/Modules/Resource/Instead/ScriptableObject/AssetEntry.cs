using System;

namespace Cosmos.Resource
{
    [Serializable]
    /// <summary>
    /// 用于SO加载
    /// </summary>
    public class AssetEntry
    {
        public string Guid;
        public string AssetPath;
        public PackageBundleSO Parent;
    }
}
