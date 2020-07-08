using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// 使用常量存储了常用的事件
    /// 分部类，可以进行后期拓展
    /// 常量名等于内容string
    /// </summary>
    public enum ModuleEnum
    {
        Audio,
        Mono,
        ObjectPool,
        Resource,
        UI,
        Event,
        Entity,
        Input,
        FSM,
        Network,
        Scene,
        Config,
        Data,
        Controller,
        ReferencePool,
        Exception,
        Hotfix
    }
}