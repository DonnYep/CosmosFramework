using UnityEngine;
namespace Cosmos
{
    public static class AudioSourceExts
    {
        /// <summary>
        /// 复位为默认状态；归还对象池前必须调用，停止播放也在这里完成，
        /// 否则失活的播放器仍可能继续出声。
        /// </summary>
        /// <param name="this">播放器</param>
        public static void Reset(this AudioSource @this)
        {
            if (@this == null)
                return;
            @this.Stop();
            @this.clip = null;
            @this.mute = false;
            @this.playOnAwake = true;
            @this.loop = false;
            @this.priority = 128;
            @this.volume = 1;
            @this.pitch = 1;
            @this.panStereo = 0;
            @this.spatialBlend = 0;
            @this.reverbZoneMix = 1;
            @this.dopplerLevel = 1;
            @this.spread = 0;
            @this.maxDistance = 500;
        }
    }
}
