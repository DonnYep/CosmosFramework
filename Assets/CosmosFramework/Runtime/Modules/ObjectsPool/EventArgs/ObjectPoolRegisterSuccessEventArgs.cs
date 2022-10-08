namespace Cosmos.ObjectPool
{
    public class ObjectPoolRegisterSuccessEventArgs : GameEventArgs
    {
        public string ObjectPoolName { get; private set; }
        public IObjectPool ObjectPool { get; private set; }
        public override void Release()
        {
            ObjectPoolName = string.Empty;
            ObjectPool = default;
        }
        internal static ObjectPoolRegisterSuccessEventArgs Create(string objectPoolName, IObjectPool objectPool)
        {
            var eventArgs = ReferencePool.Acquire<ObjectPoolRegisterSuccessEventArgs>();
            eventArgs.ObjectPoolName = objectPoolName;
            eventArgs.ObjectPool = objectPool;
            return eventArgs;
        }
        internal static void Release(ObjectPoolRegisterSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
