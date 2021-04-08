using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
#endif
namespace Cosmos.Test
{
    [Serializable]

    public class FixAnimParameter
    {
        public byte Type { get; set; }
        public  int StringHash{ get; set; }
        public object ParameterValue { get; set; }
        public int LayerIndex { get; set; }
        public FixFloat CurrentSpeed{ get; set; }
        public FixFloat LayerWeight{ get; set; }
        public FixFloat NormalizedTime { get; set; }
    }
}
