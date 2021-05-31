using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.CosmosEditor
{
    public class QuarkAssetConst
    {
        public static string[] Extensions { get { return extensions; } }
        readonly static string[] extensions = new string[]
        {
        ".3ds",".bmp",".blend",".eps",".exif",".gif",".icns",".ico",".jpeg",
        ".jpg",".ma",".max",".mb",".pcx",".png",".psd",".svg",".controller",
        ".wav",".txt",".prefab",".xml",".shadervariants",".shader",".anim",
        ".unity",".mat",".mask",".overrideController",".tif",".spriteatlas",".mp3",
        ".ogg",".aiff",".tga",".dds"
        };
    }
}
