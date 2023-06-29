using UnityEngine;
using Cosmos.FSM;
public class ExitTestTransition : FSMTransition<Transform>
{
    public override bool Handler(IFSM<Transform> fsm)
    {
        float distance = Vector3.Distance(fsm.Owner.transform.position, FSMTester.Instance.Target.position);
        if (distance > FSMTester.Instance.ObjectDetectionRange)
            return true;
        else
            return false;
    }
}
