namespace Cosmos.Editor.Resource
{
    public class ResourceEditorConstants
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
        public const string SERACH = "Search";
        /// <summary>
        /// 理论上bundle不会有上万个，因此依赖区间使用万位扩充
        /// </summary>
        public const int MULTIPLE_VALUE = 10000;
        public const int SBU_MULTIPLE_VALUE = 4000;

        public const float MAX_WIDTH = 0.618f;
        public const float MIN_WIDTH = 0.382f;

        /// <summary>
        /// 使用unity原生icon渲染的宽度
        /// </summary>
        public const int ICON_WIDTH = 28;
        /// <summary>
        /// 窗口页面按钮的长度
        /// </summary>
        public const int BUTTON_WIDTH = 128;
        /// <summary>
        /// 资源寻址文件创建地址
        /// </summary>
        public const string NEW_DATASET_PATH = "Assets/NewResourceDataset.asset";
        /// <summary>
        /// 默认寻址文件
        /// </summary>
        public const string DEFAULT_DATASET_PATH = "Assets/ResourceDataset.asset";
        /// <summary>
        /// 默认寻址文件名
        /// </summary>
        public const string DEFAULT_DATASET_NAME = "ResourceDataset";
        /// <summary>
        /// AssetBundle构建预设地址
        /// </summary>
        public const string NEW_BUILD_PROFILE_PATH = "Assets/Editor/NewAssetBundleBuildProfile.asset";
        /// <summary>
        /// 默认打包预设
        /// </summary>
        public const string DEFAULT_BUILD_PROFILE_PATH = "Assets/Editor/AssetBundleBuildProfile.asset";
        /// <summary>
        /// 项目默认相对地址:{WORKSPACE}/AssetBundles/Resource
        /// </summary>
        public const string DEFAULT_PROJECT_RELATIVE_BUILD_PATH = "AssetBundles/Resource";

        #region Editor
        /// <summary>
        /// 编辑器数据默认缓存相对地址
        /// {WORKSPACE}/Library/CosmosFramework/Resource
        /// </summary>
        public const string EDITOR_CACHE_RELATIVE_PATH = "Resource";
        /// <summary>
        /// 构建缓存文件名
        /// </summary>
        public const string RESOURCE_BUILD_CACHE = "ResourceBuildCache.json";
        /// <summary>
        /// 构建日志文件名
        /// </summary>
        public const string RESOURCE_BUILD_LOG = "ResourceBuildLog.json";
        #endregion

        #region Encryption
        /// <summary>
        /// 默认文件清单加密密钥
        /// </summary>
        public const string DEFAULT_MANIFEST_ENCRYPTION_KEY = "CosmosBundlesKey";
        /// <summary>
        /// 默认AB加密偏移量
        /// </summary>
        public const int DEFAULT_ASSETBUNDLE_OFFSET_VALUE = 16;
        #endregion
        /// <summary>
        /// assetbundle 默认后缀
        /// </summary>
        public const string DEFAULT_AB_EXTENSION = "ab";
    }
}
