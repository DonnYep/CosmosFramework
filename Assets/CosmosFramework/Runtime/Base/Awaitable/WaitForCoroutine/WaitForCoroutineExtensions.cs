using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos
{
     public static class WaitForCoroutineExtensions
    {
        public static WaitForCoroutineAwaiter GetAwaiter(this Coroutine coroutine)
        {
            return new WaitForCoroutineAwaiter(coroutine);
        }
    }
}
