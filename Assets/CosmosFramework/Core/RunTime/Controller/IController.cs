using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    public interface  IController:IOperable,IControllable,IRefreshable
    {
        string ControllerName { get; set; }
    }
}