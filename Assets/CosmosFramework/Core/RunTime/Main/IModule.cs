using System.Collections;
using System;
namespace Cosmos
{
    public interface IModule: IControllableBehaviour,IMountPoint,IOperable
    {
        ModuleEnum ModuleEnum { get; }
        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        int Priority { get; }
    }
}
