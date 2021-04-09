using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
namespace Cosmos.Test
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkAnimator : NetworkBehaviour
    {
        public override NetworkdComponetType NetworkdComponetType { get; protected set; } = NetworkdComponetType.Animator;
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

        public override string OnSerialize()
        {
            return GetCurrentParameterValue();
        }
        public override void OnDeserialize(string compJson)
        {
            var animParams = Utility.Json.ToObject<List<FixAnimParameter>>(compJson);
            if (IsAuthority || !animator.enabled)
                return;
            for (int i = 0; i < animParams.Count; i++)
            {
                AnimatorControllerParameter par = parameters[i];

                if (par.type == AnimatorControllerParameterType.Int)
                {
                    var value = Convert.ToInt32(animParams[i].ParameterValue);
                    animator.SetInteger(animParams[i].NameHash, value);
                }
                else if (par.type == AnimatorControllerParameterType.Float)
                {
                    var value = (float)Convert.ToInt32(animParams[i].ParameterValue) / 1000;
                    animator.SetFloat(animParams[i].NameHash, value);
                }
                else if (par.type == AnimatorControllerParameterType.Bool)
                {
                    var value = Convert.ToBoolean(animParams[i].ParameterValue);
                    animator.SetBool(animParams[i].NameHash, value);
                }
                else if (par.type == AnimatorControllerParameterType.Trigger)
                {
                    var isTriggered = Convert.ToBoolean(animParams[i].ParameterValue);
                    if (isTriggered)
                        animator.SetTrigger(animParams[i].NameHash);
                    else
                        animator.ResetTrigger(animParams[i].NameHash);
                }
            }
        }
        string GetCurrentParameterValue()
        {
            List<FixAnimParameter> animParas = new List<FixAnimParameter>();

            for (int i = 0; i < parameters.Length; i++)
            {
                AnimatorControllerParameter par = parameters[i];
                if (par.type == AnimatorControllerParameterType.Int)
                {
                    var animParameter = new FixAnimParameter();
                    animParameter.Type = (byte)AnimatorControllerParameterType.Int;
                    int newIntValue = animator.GetInteger(par.nameHash);
                    animParameter.ParameterValue = newIntValue;
                    animParameter.NameHash = par.nameHash;
                    animParas.Add(animParameter);
                }
                else if (par.type == AnimatorControllerParameterType.Float)
                {
                    var animParameter = new FixAnimParameter();
                    animParameter.Type = (byte)AnimatorControllerParameterType.Float;
                    float newFloatValue = animator.GetFloat(par.nameHash);
                    animParameter.NameHash = par.nameHash;

                    animParameter.ParameterValue = Mathf.FloorToInt(newFloatValue * 1000);
                    animParas.Add(animParameter);
                }
                else if (par.type == AnimatorControllerParameterType.Bool)
                {
                    var animParameter = new FixAnimParameter();
                    animParameter.Type = (byte)AnimatorControllerParameterType.Bool;
                    bool newBoolValue = animator.GetBool(par.nameHash);
                    animParameter.NameHash = par.nameHash;
                    animParameter.ParameterValue = newBoolValue;
                    animParas.Add(animParameter);
                }
                else if (par.type == AnimatorControllerParameterType.Trigger)
                {
                    var animParameter = new FixAnimParameter();
                    animParameter.Type = (byte)AnimatorControllerParameterType.Trigger;
                    animParameter.NameHash = par.nameHash;
                    animParameter.ParameterValue= animator.GetBool(par.nameHash);
                    animParas.Add(animParameter);
                }
            }
            return Utility.Json.ToJson(animParas);
        }
        private void Awake()
        {
            animator = GetComponent<Animator>();
            parameters = animator.parameters
       .Where(par => !animator.IsParameterControlledByCurve(par.nameHash))
       .ToArray();

            lastIntParameters = new int[parameters.Length];
            lastFloatParameters = new float[parameters.Length];
            lastBoolParameters = new bool[parameters.Length];

            animationHash = new int[animator.layerCount];
            transitionHash = new int[animator.layerCount];
            layerWeight = new float[animator.layerCount];

        }
        private void FixedUpdate()
        {

            if (!animator.enabled)
                return;
            //for (int i = 0; i < animator.layerCount; i++)
            //{
            //    if (!CheckAnimStateChanged(out var stateHash, out var normalizedTime, i))
            //    {
            //        continue;
            //    }
            //}
            CheckSpeed();
        }
        void HandleAnimMsg(FixAnimParameter animParameter)
        {
            if (IsAuthority)
                return;
            if (animParameter.NameHash != 0 && animator.enabled)
            {
                animator.Play(animParameter.NameHash, animParameter.LayerId, animParameter.NormalizedTime.GetFloat());
            }
            animator.SetLayerWeight(animParameter.LayerId, animParameter.LayerWeight.GetFloat());
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
        void OnAnimatorSpeedChanged(float _, float value)
        {
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
                    transitionHash[layerId] = tt.fullPathHash;
                    animationHash[layerId] = 0;
                    return true;
                }
                return change;
            }

            AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(layerId);
            if (st.fullPathHash != animationHash[layerId])
            {
                if (animationHash[layerId] != 0)
                {
                    stateHash = st.fullPathHash;
                    normalizedTime = st.normalizedTime;
                }
                transitionHash[layerId] = 0;
                animationHash[layerId] = st.fullPathHash;
                return true;
            }
            return change;
        }

    }
}