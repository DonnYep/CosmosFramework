using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Audio
{
    public class AudioRegistSuccessEventArgs : GameEventArgs
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
        internal static AudioRegistSuccessEventArgs Create(string audioName, string audioGroupName,AudioClip audioClip)
        {
            var eventArgs = ReferencePool.Acquire< AudioRegistSuccessEventArgs>();
            eventArgs.AudioName = audioName;
            eventArgs.AudioGroupName = audioGroupName;
            eventArgs.AudioClip = audioClip;
            return eventArgs;
        }
        internal static void Release(AudioRegistSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
