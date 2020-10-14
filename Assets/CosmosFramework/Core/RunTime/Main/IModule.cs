using System.Collections;
using System;
namespace Cosmos
{
    internal interface IModule: IControllableBehaviour,IMountPoint
    {
        string ModuleFullName { get; }
        void OnFixRefresh();
        void OnLateRefresh();
    }
}
