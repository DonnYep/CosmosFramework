using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test{
    public class ModuleInitializer : MonoBehaviour
    {
        enum ModuleType : int
        {
            None,
            All,
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
            Reference
        }
        [Tooltip("模块初始化器，用于测试或者游戏中使用")]
        [SerializeField] ModuleType module;
        private void Start()
        {
            switch (module)
            {
                case ModuleType.None:
                    break;
                case ModuleType.All:
                    Facade.Instance.InitAllModule();
                    break;
                default:
                    var moduleResult= Facade.Instance.GetModule(module.ToString());
                    if(moduleResult!=null)
                        moduleResult.OnInitialization();
                    break;
            }
        }
    }
}