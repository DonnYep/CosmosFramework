namespace Cosmos.Scene
{
    public struct SceneInfo: ISceneInfo
    {
        readonly string sceneName;
        readonly bool additive;
        public string SceneName { get { return sceneName; } }
        public bool Additive { get { return additive; } }
        public SceneInfo(string sceneName,bool addtive)
        {
            this.sceneName = sceneName;
            additive = addtive;
        }
        public SceneInfo(string sceneName):this(sceneName,false){}
    }
}
