using System;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    /// <summary>
    /// float网络传输类型，防止传输时失真；
    /// </summary>
    [Serializable]
    public struct FixFloat
    {
        public FixFloat(float value)
        {
            Value= Mathf.FloorToInt(value * 1000);
        }
        public int Value { get; set; }
        public static FixFloat SetFloat(float value)
        {
            return new FixFloat(value);
        }
        public float GetFloat()
        {
            return (float)Value / 1000;
        }
        public override string ToString()
        {
            return $"Value :{(float)Value/1000}";
        }
    }
}
