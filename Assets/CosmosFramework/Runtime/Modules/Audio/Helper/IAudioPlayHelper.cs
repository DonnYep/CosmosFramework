using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Audio
{
    public interface IAudioPlayHelper
    {
        void PlayAudio(AudioObject audioObject,AudioParams audioParams);
        void StopAudio(AudioObject audioObject);
        void PauseAudio(AudioObject audioObject);
        void ResumeAudio(AudioObject audioObject);
    }
}
