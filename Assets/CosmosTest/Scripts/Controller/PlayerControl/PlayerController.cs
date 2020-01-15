using UnityEngine;
using System.Collections;
using Cosmos.Event;

namespace Cosmos
{
    public class PlayerController : CharacterInputController
    {
        int moveSpeed = Animator.StringToHash("MoveSpeed");

        private void OnEnable()
        {
            Facade.Instance.AddEventListener(ApplicationConst._InputEventKey, Handler);
        }
        protected override void Handler(object sender, GameEventArgs arg)
        {
            throw new System.NotImplementedException();
        }
    }
}