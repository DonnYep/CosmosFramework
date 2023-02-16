namespace Cosmos.Editor.Resource
{
    public class ResourceEditorConstant
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
            ".flare",".playable",".physicmaterial",".signal",".guiskin",".dds",".otf"
        };
        public const string VALID = "VALID";
        public const string INVALID = "INVALID";
        public const string UNKONM = "< UNKONW > ";
        public const int MULTIPLE_VALUE = 10000;
        public const string SERACH = "Search";
        public const float MAX_WIDTH = 0.618f;
        public const float MIN_WIDTH = 0.382f;
    }
}
