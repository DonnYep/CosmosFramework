using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    public interface  IController:IBehaviour,IReference
    {
        int Id { get; }
        object Handle { get; }
        Type HandleType { get; }
        string ControllerName { get; }
    }
}