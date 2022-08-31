using UnityEngine;
namespace Cosmos
{
    public class RandomStateSMB : StateMachineBehaviour
    {
        public int numberOfStates = 3;
        public float minNormTime = 0f;
        public float maxNormTime = 5f;

        protected float m_RandomNormTime;

        readonly int m_HashRandomIdle = Animator.StringToHash("RandomIdle");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_RandomNormTime = Random.Range(minNormTime, maxNormTime);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
            {
                animator.SetInteger(m_HashRandomIdle, -1);
            }

            if (stateInfo.normalizedTime > m_RandomNormTime && !animator.IsInTransition(0))
            {
                animator.SetInteger(m_HashRandomIdle, Random.Range(0, numberOfStates));
            }
        }
    }
}