using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.Resource
{
    public enum AssetLoadType:byte
    {
        Default=0x0,
        AssetDatabase=0x1,
        BuiltAssetBundle=0x2,
    }
}
