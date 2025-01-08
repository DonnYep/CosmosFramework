using System;
using UnityEngine;
namespace Hydrogen
{
    public static class Log
    {
        public static Action<object> Info = (obj) => Debug.Log(obj);
        public static Action<object> Warning = (obj) => Debug.LogWarning(obj);
        public static Action<object> Error = (obj) => Debug.LogError(obj);
    }
}
