namespace Cosmos.ObjectPool
{
    public class ObjectPoolRegisterFailureEventArgs : GameEventArgs
    {
        public string ObjectPoolName { get; private set; }
        public string ErrorMessage { get; private set; }
        public override void Release()
        {
            ObjectPoolName = string.Empty;
            ErrorMessage = string.Empty;
        }
        internal static ObjectPoolRegisterFailureEventArgs Create(string objectPoolName, string errorMessage)
        {
            var eventArgs = ReferencePool.Acquire<ObjectPoolRegisterFailureEventArgs>();
            eventArgs.ObjectPoolName = objectPoolName;
            eventArgs.ErrorMessage = errorMessage;
            return eventArgs;
        }
        internal static void Release(ObjectPoolRegisterFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
