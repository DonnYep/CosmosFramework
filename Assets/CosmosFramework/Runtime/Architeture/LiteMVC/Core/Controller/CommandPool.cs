using System;
using System.Collections.Generic;

namespace LiteMVC.Core
{
    internal class CommandPool
    {
        /// <summary>
        /// dataType===cmdPool
        /// </summary>
        readonly static Dictionary<Type, Pool<ICommand>> cmdPool = new Dictionary<Type, Pool<ICommand>>();
        public static ICommand Acquire(Type cmdType)
        {
            if (!cmdPool.TryGetValue(cmdType, out var pool))
            {
                pool = new Pool<ICommand>(() => (ICommand)Activator.CreateInstance(cmdType));
                cmdPool[cmdType] = pool;
            }
            return pool.Spawn();
        }
        public static void Release<T>(T command) where T : class, ICommand
        {
            var cmdType = typeof(T);
            if (!cmdPool.TryGetValue(cmdType, out var pool))
            {
                pool = new Pool<ICommand>(() => (ICommand)Activator.CreateInstance(cmdType));
                cmdPool[cmdType] = pool;
            }
            pool.Despawn(command);
        }
    }
}
