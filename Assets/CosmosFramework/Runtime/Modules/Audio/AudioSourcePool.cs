using UnityEngine;
namespace Cosmos.Audio
{
    /// <summary>
    /// AudioSource对象池
    /// <para><see cref="UnityEngine.AudioSource"/>的对象池</para>
    /// </summary>
    public static class AudioSourcePool
    {
        readonly static Pool<AudioSource> audioSourcePool= new Pool<AudioSource>(OnGenerate, OnSpawn, OnDespawn);
        public static void Despawn(AudioSource audioSource)
        {
            audioSourcePool.Despawn(audioSource);
        }
        public static AudioSource Spawn()
        {
            return audioSourcePool.Spawn();
        }
        public static void Clear()
        {
            foreach (var au in audioSourcePool)
            {
                GameObject.Destroy(au);
            }
            audioSourcePool.Clear();
        }
        static AudioSource OnGenerate()
        {
            var go = new GameObject();
            go.transform.position = Vector3.zero;
            return go.AddComponent<AudioSource>();
        }
        static void OnSpawn(AudioSource audioSource)
        {
            audioSource.gameObject.SetActive(true);
        }
        static void OnDespawn(AudioSource audioSource)
        {
            audioSource.Reset();
            audioSource.transform.position = Vector3.zero;
            audioSource.gameObject.SetActive(false);
        }
    }
}
