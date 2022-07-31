namespace Cosmos.WebRequest
{
    public class WebRequestAllTaskCompleteEventArgs : GameEventArgs
    {
        public override void Release() { }
        internal static WebRequestAllTaskCompleteEventArgs Create()
        {
            var eventArgs = ReferencePool.Acquire<WebRequestAllTaskCompleteEventArgs>();
            return eventArgs;
        }
        internal static void Release(WebRequestAllTaskCompleteEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
