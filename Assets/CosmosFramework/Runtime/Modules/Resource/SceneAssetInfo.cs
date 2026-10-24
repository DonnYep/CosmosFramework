using System.Runtime.InteropServices;

namespace Cosmos
{
    /// <summary>
    /// 场景资源信息。
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct SceneAssetInfo
    {
        /// <summary>
        /// 加载优先级
        /// </summary>
        public int Priority { get; private set; }
        /// <summary>
        /// 场景标识（资源名 / 资源路径，原样透传给 Provider）
        /// </summary>
        public string SceneName { get; private set; }
        /// <summary>
        /// 是否叠加加载
        /// </summary>
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
