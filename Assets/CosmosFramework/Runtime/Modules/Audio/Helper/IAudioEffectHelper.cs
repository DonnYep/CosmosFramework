using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Audio
{
    public interface IAudioEffectHelper
    {
        void OnStart();
        void OnPlaying();
        void OnEnd();
    }
}
