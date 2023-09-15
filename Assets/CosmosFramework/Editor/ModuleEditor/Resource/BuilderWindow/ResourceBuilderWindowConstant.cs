namespace Cosmos.Editor.Resource
{
    public class ResourceBuilderWindowConstant
    {
        /// <summary>
        /// 资源编辑器能够识别的文件后缀名；
        /// </summary>
        public static string[] Extensions { get { return extensions; } }
        readonly static string[] extensions = new string[]
        {
            ".3ds",".bmp",".blend",".eps",".exif",".gif",".icns",".ico",".jpeg",
            ".jpg",".ma",".max",".mb",".pcx",".png",".psd",".svg",".controller",
            ".wav",".txt",".prefab",".xml",".shadervariants",".shader",".anim",
            ".unity",".mat",".mask",".overridecontroller",".tif",".spriteatlas",
            ".mp3",".ogg",".aiff",".tga",".dds",".bytes",".json",".asset",".mp4",
            ".xls",".xlsx",".docx",".doc",".mov",".rendertexture",".csv",".fbx",".mixer",
            ".flare",".playable",".physicmaterial",".signal",".guiskin",".otf",".ttf"
        };
        public const string VALID = "VALID";
        public const string INVALID = "INVALID";
        public const string UNKONM = "< UNKONW > ";
        /// <summary>
        /// 理论上bundle不会有上万个，因此依赖区间使用万位扩充
        /// </summary>
        public const int MULTIPLE_VALUE = 10000;
        public const int SBU_MULTIPLE_VALUE = 4000;
        public const string SERACH = "Search";
        public const float MAX_WIDTH = 0.618f;
        public const float MIN_WIDTH = 0.382f;
        public const string RESOURCE_BUILD_CACHE = "ResourceBuildCache.json";
        public const string RESOURCE_BUILD_LOG = "ResourceBuildLog.json";
        public const int TEXTURE_ICON_WIDTH = 28;

        /// <summary>
        /// 资源寻址文件创建地址
        /// </summary>
        public const string RESOURCE_NEW_DATASET_PATH = "Assets/NewResourceDataset.asset";
        /// <summary>
        /// AssetBundle构建预设地址
        /// </summary>
        public const string RESOURCE_NEW_BUILD_PRESET_PATH = "Assets/Editor/NewAssetBundleBuildPreset.asset";
    }
}
