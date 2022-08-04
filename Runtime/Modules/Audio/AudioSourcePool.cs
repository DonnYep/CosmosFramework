﻿using UnityEngine;
namespace Cosmos.Audio
{
    public class AudioSourcePool
    {
        Pool<AudioSource> audioSourcePool;

        public AudioSourcePool()
        {
            audioSourcePool = new Pool<AudioSource>(OnGenerate, OnSpawn,OnDespawn);
        }
        public void Despawn(AudioSource audioSource)
        {
            audioSourcePool.Despawn(audioSource);
        }
        public AudioSource Spawn()
        {
            return audioSourcePool.Spawn();
        }
        public void Clear()
        {
            foreach (var au in audioSourcePool)
            {
                GameObject.Destroy(au);
            }
            audioSourcePool.Clear();
        }
        AudioSource OnGenerate()
        {
            var go = new GameObject();
            go.transform.position = Vector3.zero;
            return go.AddComponent<AudioSource>();
        }
        void OnSpawn(AudioSource audioSource)
        {
            audioSource.gameObject.SetActive(true);
        }
        void OnDespawn(AudioSource audioSource)
        {
            audioSource.Reset();
            audioSource.transform.position = Vector3.zero;
            audioSource.gameObject.SetActive(false);
        }
    }
}
