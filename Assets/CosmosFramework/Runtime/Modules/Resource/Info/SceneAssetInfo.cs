﻿namespace Cosmos.Resource
{
    /// <summary>
    /// AB包中场景资源的信息；
    /// </summary>
    public sealed class SceneAssetInfo : AssetInfo
    {
        public int Priority { get; private set; }
        public string SceneName { get; private set; }
        public bool Additive { get; private set; }
        public SceneAssetInfo(string assetBundleName, string sceneName,int priority=100) : base(assetBundleName, sceneName)
        {
            Priority = priority;
        }
        public SceneAssetInfo(string sceneName, bool addtive, int priority = 100) : this(null, sceneName, priority)
        {
            SceneName = sceneName;
            Additive = addtive;
        }
    }
}