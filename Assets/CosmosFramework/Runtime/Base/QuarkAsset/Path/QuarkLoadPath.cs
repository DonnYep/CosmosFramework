using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark
{
    public enum QuarkLoadPath:byte
    {
        PersistentDataPath=0x0,
        StreamingAssets=0x1,
        Custome = 0x2
    }
}
