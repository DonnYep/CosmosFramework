using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark
{
    public enum QuarkPersistentPathType:byte
    {
        PersistentDataPath=0x0,
        StreamingAssets=0x1,
        CustomePersistentPath = 0x2
    }
}
