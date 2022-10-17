namespace Cosmos.Audio
{
    public class AudioRegisterFailureEventArgs : GameEventArgs
    {
        public string AudioName { get; private set; }
        public string AudioGroupName { get; private set; }
        
        public override void Release()
        {
            AudioName = string.Empty;
            AudioGroupName = string.Empty;
        }
        internal static AudioRegisterFailureEventArgs Create(string audioName,string audioGroupName)
        {
            var eventArgs = ReferencePool.Acquire<AudioRegisterFailureEventArgs>();
            eventArgs.AudioName = audioName;
            eventArgs.AudioGroupName = audioGroupName;
            return eventArgs;
        }
        internal static void Release(AudioRegisterFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
