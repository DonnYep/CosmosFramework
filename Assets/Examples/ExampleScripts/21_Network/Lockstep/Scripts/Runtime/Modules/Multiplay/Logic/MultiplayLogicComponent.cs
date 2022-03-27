using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using UnityEngine;
using Cosmos;
namespace Cosmos.Lockstep
{
    /// <summary>
    /// 逻辑Tick层；
    /// </summary>
    public class MultiplayLogicComponent : MonoBehaviour
    {
        [SerializeField] int netId;
        [SerializeField] float rotSpeed = 20;
        [SerializeField] [Range(0, 1)] float forwardDampTime = 0.1f;
        int verticalHash = Animator.StringToHash("Vertical");
        int inputHash = Animator.StringToHash("Input");
        int attackIndexHash = Animator.StringToHash("AttackIndex");
        Animator animator;
        int hitCount = 0;
        OpInput opInput;
        bool isFirstTick = true;
        public int NetId { get { return netId; } }
        public bool IsAuthority { get; private set; }
        public void Init(int netId, bool isAuthority = false)
        {
            this.netId = netId;
            IsAuthority = isAuthority;
        }
        public void OnTickInput(OpInput opInput)
        {
            this.opInput = opInput;
            if (isFirstTick)
            {
                var targetPos = opInput.Position;
                var targetRot = opInput.Rotation;
                transform.position = targetPos;
                transform.eulerAngles = targetRot;
                isFirstTick = false;
            }
        }
        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        void Update()
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if ((stateInfo.IsName("Attack_00") || stateInfo.IsName("Attack_01") || stateInfo.IsName("Attack_02")) && stateInfo.normalizedTime > 1f)
            {
                hitCount = 0;
                animator.SetInteger(attackIndexHash, 0);
            }
            if (opInput.MouseLeftDown)
            {
                if (stateInfo.IsName("Ground") && hitCount == 0)
                {
                    hitCount = 1;
                }
                else if (stateInfo.IsName("Attack_00") && hitCount == 1 && stateInfo.normalizedTime < 0.7f)
                {
                    hitCount = 2;
                }
                else if (stateInfo.IsName("Attack_01") && hitCount == 2 && stateInfo.normalizedTime < 0.7f)
                {
                    hitCount = 3;
                }
                animator.SetInteger(attackIndexHash, hitCount);
            }
            if (hitCount == 0)
            {
                var targetForward = opInput.Forward;
                var targetPos = opInput.Position+Vector3.Normalize(targetForward)*MultiplayConstant.SyncInterval;
                var targetInput = opInput.Input;
                if (targetInput != Vector3.zero)
                {
                    animator.SetBool(inputHash, true);
                    transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
                    //transform.position += MultiplayConstant.SyncInterval * transform.forward;
                    transform.forward = Vector3.Slerp(transform.forward, targetForward, Time.deltaTime * rotSpeed);
                }
                else
                {
                    animator.SetBool(inputHash, false);
                }
                var moveMagnitude = targetInput.normalized.magnitude;
                if (opInput.LeftShiftDown)
                {
                    moveMagnitude *= 2;
                }
                animator.SetFloat(verticalHash, moveMagnitude, forwardDampTime, Time.deltaTime);
            }
        }
    }
}
