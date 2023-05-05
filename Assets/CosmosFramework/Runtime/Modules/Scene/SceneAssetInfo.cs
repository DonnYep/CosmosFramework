namespace Cosmos
{
    /// <summary>
    /// AB包中场景资源的信息；
    /// </summary>
    public struct SceneAssetInfo
    {
        public int Priority { get; private set; }
        public string SceneName { get; private set; }
        public bool Additive { get; private set; }
        public SceneAssetInfo(string sceneName, int priority = 100) : this(sceneName, false, priority) { }
        public SceneAssetInfo(string sceneName, bool addtive, int priority = 100)
        {
            SceneName = sceneName;
            Additive = addtive;
            Priority = priority;
        }
    }
}