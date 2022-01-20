namespace Cosmos
{
    public struct SceneInfo: ISceneInfo
    {
        public string SceneName { get; private set; }
        public bool Additive { get; private set; }
        public SceneInfo(string sceneName,bool addtive)
        {
            SceneName = sceneName;
            Additive = addtive;
        }
        public SceneInfo(string sceneName):this(sceneName,false){}
    }
}
