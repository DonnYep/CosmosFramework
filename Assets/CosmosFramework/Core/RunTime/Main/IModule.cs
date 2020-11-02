using System.Collections;
using System;
namespace Cosmos
{
    internal interface IModule: IControllableBehaviour,IMountPoint,IOperable
    {
        string ModuleFullName { get; }
        void OnFixRefresh();
        void OnLateRefresh();
    }
}
