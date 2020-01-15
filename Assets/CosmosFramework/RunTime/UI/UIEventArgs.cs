using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos.UI{
    /// <summary>
    /// UI事件 处理数据
    /// </summary>
    public class UIEventArgs :GameEventArgs
    {
        public string Message { get; set; }
        public float SliderValue { get; set; }
        public float SliderMaxValue { get; set; }
        public override void Reset()
        {
            Message = null;
            SliderMaxValue = 1;
            SliderValue = 0;
        }
    }
}