using UnityEngine;

namespace Cosmos
{
    public static class AnimatorExts
    {
        /// <summary>
        /// 获取动画组件切换进度
        /// </summary>
        public static float GetCrossFadeProgress(this Animator @this, int layer = 0)
        {
            if (@this.GetNextAnimatorStateInfo(layer).shortNameHash == 0)
            {
                return 1;
            }
            return @this.GetCurrentAnimatorStateInfo(layer).normalizedTime % 1;
        }
        /// <summary>
        /// 检测动画是否播放中 
        /// </summary>
        public static bool IsAnimationPlaying(this Animator @this, string animationName)
        {
            return @this.GetCurrentAnimatorStateInfo(0).IsName(animationName);
        }
        public static string GetCurrentClipName(this Animator @this)
        {
            AnimatorClipInfo[] clipInfo = @this.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                // 获取当前播放动画片段的名称
                string currentAnimation = clipInfo[0].clip.name;
                return currentAnimation;
            }
            return string.Empty;
        }
        public static float CurrentNormalizedTime(this Animator @this)
        {
            var stateInfo = @this.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime;
        }
    }
}
