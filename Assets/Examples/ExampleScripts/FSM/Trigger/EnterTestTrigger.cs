using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Cosmos.FSM;
public class EnterTestTrigger : FSMTrigger<Transform>
{
    public override bool Handler(IFSM<Transform> fsm)
    {
        float distance = Vector3.Distance(fsm.Owner.transform.position, FSMTester.Instance.Target.position);
        if (distance <= FSMTester.Instance.ObjectDetectionRange)
            return true;
        else
            return false;
    }
}
