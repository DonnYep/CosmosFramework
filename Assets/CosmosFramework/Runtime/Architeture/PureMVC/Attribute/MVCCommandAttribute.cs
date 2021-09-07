using System.Collections;
using System.Collections.Generic;
using System;
namespace PureMVC
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =true,Inherited =false)]
    public class MVCCommandAttribute : Attribute 
    {
        public string ActionKey { get; private set; }
        public MVCCommandAttribute(string actionKey)
        {
            ActionKey = actionKey;
        }
    }
}