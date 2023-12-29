namespace Cosmos.Audio
{
    public class AudioRegisterFailureEventArgs : GameEventArgs
    {
        public string AudioAssetName { get; private set; }
        public string ErrorMessage { get; private set; }
        public override void Release()
        {
            AudioAssetName = string.Empty;
            ErrorMessage = string.Empty;
        }
        internal static AudioRegisterFailureEventArgs Create(string audioAssetName, string errorMessage)
        {
            var eventArgs = ReferencePool.Acquire<AudioRegisterFailureEventArgs>();
            eventArgs.AudioAssetName = audioAssetName;
            eventArgs.ErrorMessage = errorMessage;
            return eventArgs;
        }
        internal static void Release(AudioRegisterFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
