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
    public sealed partial class CFModules
    {
        public const string AUDIO = "Audio";
        public const string MONO = "Mono";
        public const string OBJECTPOOL = "ObjectPool";
        public const string RESOURCE = "Resource";
        public const string UI= "UI";
        public const string EVENT = "Event";
        public const string ENTITY= "Entity";
        public const string INPUT = "Input";
        public const string FSM = "FSM";
        public const string NETWORK = "Network";
        public const string SCENE = "Scene";
        public const string CONFIG = "Config";
        public const string DATA= "Data";
        public const string CONTROLLER = "Controller";
        public const string REFERENCE= "Reference";
    }
}