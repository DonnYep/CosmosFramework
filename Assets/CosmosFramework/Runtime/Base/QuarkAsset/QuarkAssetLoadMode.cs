using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark
{
    public enum QuarkAssetLoadMode : byte
    {
        None = 0x0,
        AssetDatabase = 0x1,
        BuiltAssetBundle = 0x2
    }
}
