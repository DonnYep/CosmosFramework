using UnityEngine;

namespace Cosmos.Audio
{
    internal class AudioAssetLoadTask : IReference
    {
        public string AudioAssetName { get; private set; }
        public Coroutine Coroutine { get; private set; }
        public void Release()
        {
            AudioAssetName = string.Empty;
            if (Coroutine != null)
                Utility.Unity.StopCoroutine(Coroutine);
            Coroutine = null;
        }
        public static AudioAssetLoadTask Create(string audioAssetName, Coroutine coroutine)
        {
            var task = ReferencePool.Acquire<AudioAssetLoadTask>();
            task.AudioAssetName = audioAssetName;
            task.Coroutine = coroutine;
            return task;
        }
        public static void Release(AudioAssetLoadTask loadTask)
        {
            ReferencePool.Release(loadTask);
        }
    }
}
