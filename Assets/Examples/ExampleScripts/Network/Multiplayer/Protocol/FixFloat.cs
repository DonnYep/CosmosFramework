using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Test
{
    /// <summary>
    /// float放大一千倍；
    /// </summary>
    [Serializable]
    public struct FixFloat
    {
        public FixFloat(float value)
        {
            Value = (int)value * 1000;
        }
        public int Value { get; set; }
        public static FixFloat SetFloat(float value)
        {
            return new FixFloat(value);
        }
        public float GetValue()
        {
            return (float)Value / 1000;
        }
    }
}
