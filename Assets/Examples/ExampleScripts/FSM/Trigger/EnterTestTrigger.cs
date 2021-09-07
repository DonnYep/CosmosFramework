using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Cosmos.FSM;
public class EnterTestTrigger : FSMTrigger<FSMTester>
{
    public override bool Handler(IFSM<FSMTester> fsm)
    {
        Utility.Debug.LogInfo("EnterTestTrigger",MessageColor.INDIGO, fsm.Owner.gameObject);

        float distance = Vector3.Distance(fsm.Owner.transform.position, fsm.Owner.Target.position);
        if (distance <= fsm.Owner.Range)
            return true;
        else
            return false;
    }
    public override void OnInitialization()
    {
        Utility.Debug.LogInfo("EnterTestTrigger OnInitialization");
    }
}
