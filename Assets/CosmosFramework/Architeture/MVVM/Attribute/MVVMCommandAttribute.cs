using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.Mvvm
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =true,Inherited =false)]
    public class MVVMCommandAttribute : Attribute 
    {
        public string ActionKey { get; private set; }
        public MVVMCommandAttribute(string actionKey)
        {
            ActionKey = actionKey;
        }
    }
}