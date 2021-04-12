using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Cosmos.Test
{
    [Serializable]
    public struct FixTransportData
    {
        public int Conv { get; set; }
        public Dictionary<byte,string> CompData { get; set; }
    }
}
