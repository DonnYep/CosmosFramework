using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public abstract class AudioPlayer: MonoBehaviour
    {
        public virtual AudioDataSet AudioDataSet { get; }
        public virtual AudioVariable AudioVariable { get; }
        public abstract void PlayAudio();
        public abstract void PauseAudio();
        public abstract void UnpauseAudio();
        public abstract void StopAudio();
    }
}