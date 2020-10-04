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
            Reference,
            Exception
        }
        [Tooltip("模块初始化器，用于测试或者游戏中使用")]
        [SerializeField] ModuleType module;
        private void Awake()
        {
            Utility.Debug.SetHelper(new UnityDebugHelper());
        }
        private void Start()
        {
            switch (module)
            {
                case ModuleType.None:
                    break;
                case ModuleType.All:
                    //Facade.InitAllModule();
                    break;
                default:
                    //var moduleEnum = Utility.Framework.GetModuleEnum(module.ToString());
                    //var moduleResult= Facade.GetModule( moduleEnum );
                    //if (moduleResult != null)
                    //    Utility.Debug.LogInfo(moduleResult.MountPoint.name);
                    break;
            }
        }
    }
}