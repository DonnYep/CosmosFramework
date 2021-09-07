using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cosmos.Awaitable;
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
