using UnityEngine;
namespace Cosmos.Audio
{
    /// <summary>
    /// AudioSource对象池
    /// <para><see cref="UnityEngine.AudioSource"/>的对象池</para>
    /// <para>池内对象一律挂在 DontDestroyOnLoad 场景根节点：绑定到场景物体的播放器归还时会先被"摘下来"，
    /// 否则场景卸载会把池里的对象一起销毁，下一次 Spawn 拿到的就是已销毁对象（MissingReferenceException）。</para>
    /// </summary>
    public static class AudioSourcePool
    {
        readonly static Pool<AudioSource> audioSourcePool = new Pool<AudioSource>(OnGenerate, OnSpawn, OnDespawn);
        /// <summary>
        /// 归还播放器
        /// </summary>
        /// <param name="audioSource">播放器</param>
        public static void Despawn(AudioSource audioSource)
        {
            if (audioSource == null)
                return;
            audioSourcePool.Despawn(audioSource);
        }
        /// <summary>
        /// 取用播放器
        /// </summary>
        /// <returns>播放器</returns>
        public static AudioSource Spawn()
        {
            return audioSourcePool.Spawn();
        }
        /// <summary>
        /// 销毁池中所有播放器（模块终止时调用）
        /// </summary>
        public static void Clear()
        {
            foreach (var audioSource in audioSourcePool)
            {
                if (audioSource == null)
                    continue;
                //注意销毁的是 GameObject：只销毁组件会在场景里留下空物体
                GameObject.Destroy(audioSource.gameObject);
            }
            audioSourcePool.Clear();
        }
        static AudioSource OnGenerate()
        {
            var go = new GameObject(AudioConstant.PREFIX + "AudioSource");
            go.transform.position = Vector3.zero;
            GameObject.DontDestroyOnLoad(go);
            return go.AddComponent<AudioSource>();
        }
        static void OnSpawn(AudioSource audioSource)
        {
            audioSource.gameObject.SetActive(true);
        }
        static void OnDespawn(AudioSource audioSource)
        {
            if (audioSource == null)
                return;
            //先脱离可能被卸载的场景物体，再挂到常驻场景，最后失活
            audioSource.transform.SetParent(null);
            GameObject.DontDestroyOnLoad(audioSource.gameObject);
            audioSource.Reset();
            audioSource.transform.position = Vector3.zero;
            audioSource.gameObject.SetActive(false);
        }
    }
}
