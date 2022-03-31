using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Test
{
    [Serializable]

    public struct FixAnimParameter
    {
        public byte Type { get; set; }
        public  int NameHash{ get; set; }
        public object ParameterValue { get; set; }
        public int LayerId { get; set; }
        public FixFloat CurrentSpeed{ get; set; }
        public FixFloat LayerWeight{ get; set; }
        public FixFloat NormalizedTime { get; set; }
    }
}
