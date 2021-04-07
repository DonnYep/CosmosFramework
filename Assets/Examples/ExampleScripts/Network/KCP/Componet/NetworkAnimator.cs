using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Cosmos.Test {
    public class NetworkAnimator :NetworkBehaviour
    {
        public Animator animator;
        float animatorSpeed;
        float previousSpeed;
        AnimatorControllerParameter[] parameters;
        private void Awake()
        {
            parameters = animator.parameters
       .Where(par => !animator.IsParameterControlledByCurve(par.nameHash))
       .ToArray();
        }
    }
}