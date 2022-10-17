using UnityEngine;
namespace Cosmos.Audio
{
    public class AudioRegisterSuccessEventArgs : GameEventArgs
    {
        public string AudioName { get; private set; }
        public string AudioGroupName { get; private set; }
        public AudioClip AudioClip { get; private set; }
        public override void Release()
        {
            AudioName = string.Empty;
            AudioGroupName = string.Empty;
            AudioClip = null;
        }
        internal static AudioRegisterSuccessEventArgs Create(string audioName, string audioGroupName,AudioClip audioClip)
        {
            var eventArgs = ReferencePool.Acquire< AudioRegisterSuccessEventArgs>();
            eventArgs.AudioName = audioName;
            eventArgs.AudioGroupName = audioGroupName;
            eventArgs.AudioClip = audioClip;
            return eventArgs;
        }
        internal static void Release(AudioRegisterSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
