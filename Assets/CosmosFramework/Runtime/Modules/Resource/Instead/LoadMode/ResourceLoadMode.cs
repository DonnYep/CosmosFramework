namespace Cosmos.Resource
{
    public enum ResourceLoadMode : byte
    {
        NONE=0,
        Resource = 1,
        AssetBundle = 2,
        AssetDatabase= 3,
        CustomLoader=4 //自定义加载方案
    }
}
