using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Cosmos.Test
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkAnimator : NetworkBehaviour
    {
        public Animator animator;
        float animatorSpeed;
        float previousSpeed;
        AnimatorControllerParameter[] parameters;
        int[] lastIntParameters;
        float[] lastFloatParameters;
        bool[] lastBoolParameters;

        // multiple layers
        int[] animationHash;
        int[] transitionHash;
        float[] layerWeight;
        float nextSendTime;
        private void Awake()
        {
            parameters = animator.parameters
       .Where(par => !animator.IsParameterControlledByCurve(par.nameHash))
       .ToArray();
        }
        private void FixedUpdate()
        {

            if (!animator.enabled)
                return;

            CheckSendRate();

            for (int i = 0; i < animator.layerCount; i++)
            {
                int stateHash;
                float normalizedTime;
                if (!CheckAnimStateChanged(out stateHash, out normalizedTime, i))
                {
                    continue;
                }
                //WriteParameters(writer);
                //SendAnimationMessage(stateHash, normalizedTime, i, layerWeight[i], writer.ToArray());
                //using (PooledNetworkWriter writer = NetworkWriterPool.GetWriter())
                //{
                //    WriteParameters(writer);
                //    SendAnimationMessage(stateHash, normalizedTime, i, layerWeight[i], writer.ToArray());
                //}
            }

            CheckSpeed();
        }
        void SendAnimationMessage(int stateHash, float normalizedTime, int layerId, float weight, byte[] parameters)
        {
            CmdOnAnimationServerMessage(stateHash, normalizedTime, layerId, weight, parameters);
        }
        void CmdOnAnimationServerMessage(int stateHash, float normalizedTime, int layerId, float weight, byte[] parameters)
        {
            if (IsAuthority)
                return;
            //HandleAnimMsg(stateHash, normalizedTime, layerId, weight, networkReader);
        }
        void CheckSpeed()
        {
            float newSpeed = animator.speed;
            if (Mathf.Abs(previousSpeed - newSpeed) > 0.001f)
            {
                previousSpeed = newSpeed;
                CmdSetAnimatorSpeed(newSpeed);
            }
        }
        void CmdSetAnimatorSpeed(float newSpeed)
        {
            animator.speed = newSpeed;
            animatorSpeed = newSpeed;
        }
        void CheckSendRate()
        {
            float now = Time.time;
            if (NetworkSimulateConsts.SyncInterval >= 0 && now > nextSendTime)
            {
                nextSendTime = now + NetworkSimulateConsts.SyncInterval;

                //using (PooledNetworkWriter writer = NetworkWriterPool.GetWriter())
                //{
                //    if (WriteParameters(writer))
                //        SendAnimationParametersMessage(writer.ToArray());
                //}
            }
        }
        void OnAnimatorSpeedChanged(float _, float value)
        {
            // skip if host or client with authority
            // they will have already set the speed so don't set again
            if (IsAuthority)
                return;

            animator.speed = value;
        }
        bool CheckAnimStateChanged(out int stateHash, out float normalizedTime, int layerId)
        {
            bool change = false;
            stateHash = 0;
            normalizedTime = 0;

            float lw = animator.GetLayerWeight(layerId);
            if (Mathf.Abs(lw - layerWeight[layerId]) > 0.001f)
            {
                layerWeight[layerId] = lw;
                change = true;
            }

            if (animator.IsInTransition(layerId))
            {
                AnimatorTransitionInfo tt = animator.GetAnimatorTransitionInfo(layerId);
                if (tt.fullPathHash != transitionHash[layerId])
                {
                    // first time in this transition
                    transitionHash[layerId] = tt.fullPathHash;
                    animationHash[layerId] = 0;
                    return true;
                }
                return change;
            }

            AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(layerId);
            if (st.fullPathHash != animationHash[layerId])
            {
                // first time in this animation state
                if (animationHash[layerId] != 0)
                {
                    // came from another animation directly - from Play()
                    stateHash = st.fullPathHash;
                    normalizedTime = st.normalizedTime;
                }
                transitionHash[layerId] = 0;
                animationHash[layerId] = st.fullPathHash;
                return true;
            }
            return change;
        }
        void HandleAnimMsg(int stateHash, float normalizedTime, int layerId, float weight, FixAnimator anim)
        {
            if (IsAuthority)
                return;
            if (stateHash != 0 && animator.enabled)
            {
                animator.Play(stateHash, layerId, normalizedTime);
            }
            animator.SetLayerWeight(layerId, weight);

        }
        void ReadParameters(NetworkReader reader)
        {
            bool animatorEnabled = animator.enabled;
            // need to read values from NetworkReader even if animator is disabled

            ulong dirtyBits = reader.ReadUInt64();
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((dirtyBits & (1ul << i)) == 0)
                    continue;

                AnimatorControllerParameter par = parameters[i];
                if (par.type == AnimatorControllerParameterType.Int)
                {
                    int newIntValue = reader.ReadInt32();
                    if (animatorEnabled)
                        animator.SetInteger(par.nameHash, newIntValue);
                }
                else if (par.type == AnimatorControllerParameterType.Float)
                {
                    float newFloatValue = reader.ReadSingle();
                    if (animatorEnabled)
                        animator.SetFloat(par.nameHash, newFloatValue);
                }
                else if (par.type == AnimatorControllerParameterType.Bool)
                {
                    bool newBoolValue = reader.ReadBoolean();
                    if (animatorEnabled)
                        animator.SetBool(par.nameHash, newBoolValue);
                }
            }
        }
    }
}