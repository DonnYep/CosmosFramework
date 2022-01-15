using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.Audio
{
    public class AudioRegistFailureEventArgs : GameEventArgs
    {
        public string AudioName { get; private set; }
        public string AudioGroupName { get; private set; }
        
        public override void Release()
        {
            AudioName = string.Empty;
            AudioGroupName = string.Empty;
        }
        internal static AudioRegistFailureEventArgs Create(string audioName,string audioGroupName)
        {
            var eventArgs = ReferencePool.Acquire<AudioRegistFailureEventArgs>();
            eventArgs.AudioName = audioName;
            eventArgs.AudioGroupName = audioGroupName;
            return eventArgs;
        }
        internal static void Release(AudioRegistFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
