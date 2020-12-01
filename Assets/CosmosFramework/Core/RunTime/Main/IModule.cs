using System.Collections;
using System;
namespace Cosmos
{
    internal interface IModule
    {
        string ModuleFullName { get; }
        void OnFixRefresh();
        void OnLateRefresh();
    }
}
