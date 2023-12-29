using UnityEngine;
namespace Cosmos.Audio
{
    public class AudioRegisterSuccessEventArgs : GameEventArgs
    {
        public string AudioAssetName { get; private set; }
        public AudioClip AudioClip { get; private set; }
        public override void Release()
        {
            AudioAssetName = string.Empty;
            AudioClip = null;
        }
        internal static AudioRegisterSuccessEventArgs Create(string audioAssetName, AudioClip audioClip)
        {
            var eventArgs = ReferencePool.Acquire<AudioRegisterSuccessEventArgs>();
            eventArgs.AudioAssetName = audioAssetName;
            eventArgs.AudioClip = audioClip;
            return eventArgs;
        }
        internal static void Release(AudioRegisterSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
