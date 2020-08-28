using System.Collections;
using System;
namespace Cosmos
{
    public interface IModule: IControllableBehaviour,IMountPoint
    {
        string ModuleFullyQualifiedName { get; }
        ModuleEnum ModuleEnum { get; }
    }
}
