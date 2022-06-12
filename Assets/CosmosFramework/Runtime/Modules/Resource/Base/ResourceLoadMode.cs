namespace Cosmos.Resource
{
    public enum ResourceLoadMode : byte
    {
        Resource = 0,
        AssetBundle = 1,
        AssetDatabase= 2,
        CustomLoader=3 //自定义加载方案
    }
}
