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
        public byte ParameterType { get; set; }
        public  string ParameterKey { get; set; }
        public object ParameterValue { get; set; }

    }
}
