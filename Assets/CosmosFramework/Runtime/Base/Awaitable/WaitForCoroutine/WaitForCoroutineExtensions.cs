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
