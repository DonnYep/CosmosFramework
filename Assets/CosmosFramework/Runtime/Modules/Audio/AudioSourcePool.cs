using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Audio
{
    public class AudioSourcePool
    {
        Pool<AudioSource> audioSourcePool;

        public AudioSourcePool()
        {
            audioSourcePool = new Pool<AudioSource>(OnGenerate, OnDespawn);
        }
        public void Despawn(AudioSource audioSource)
        {
            audioSourcePool.Despawn(audioSource);
        }
        public AudioSource Spawn()
        {
            return audioSourcePool.Spawn();
        }
        AudioSource OnGenerate()
        {
            var go = new GameObject();
            go.transform.position = Vector3.zero;
            return go.AddComponent<AudioSource>();
        }
        void OnDespawn(AudioSource audioSource)
        {
            audioSource.Reset();
            audioSource.transform.position = Vector3.zero;
        }
    }
}
