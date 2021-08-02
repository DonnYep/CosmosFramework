using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Quark
{
    public class QuarkConsts
    {
        /// <summary>
        /// Quark资源打包出来后信息表；
        /// </summary>
        public const string BuildInfoFileName = "BuildInfo.json";
        /// <summary>
        /// Quark自带的Manifest名称；
        /// </summary>
        public const string ManifestName = "Manifest.json";
        /// <summary>
        /// Quark旧的的Manifest名称；
        /// </summary>
        public const string ManifestOldName = "ManifestOld.json";
        /// <summary>
        /// Quark可识别的文件后缀名；
        /// </summary>
        public static string[] Extensions { get { return extensions; } }
        readonly static string[] extensions = new string[]
        {
        ".3ds",".bmp",".blend",".eps",".exif",".gif",".icns",".ico",".jpeg",
        ".jpg",".ma",".max",".mb",".pcx",".png",".psd",".svg",".controller",
        ".wav",".txt",".prefab",".xml",".shadervariants",".shader",".anim",
        ".unity",".mat",".mask",".overrideController",".tif",".spriteatlas",".mp3",
        ".ogg",".aiff",".tga",".dds",".bytes"
        };
    }
}
