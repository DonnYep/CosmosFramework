using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Audio
{
    /// <summary>
    /// 音效组池；
    /// </summary>
    public class AudioGroupPool
    {
        Pool<AudioGroup> audioGroupPool;
        public AudioGroupPool()
        {
            audioGroupPool = new Pool<AudioGroup>(() => { return new AudioGroup(); },ag=> { ag.Release(); });
        }
        public void Despawn(AudioGroup audioGroup)
        {
            audioGroupPool.Despawn(audioGroup);
        }
        public AudioGroup Spawn()
        {
            return audioGroupPool.Spawn();
        }
    }
}
