using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.FSM;
using Cosmos;
public class EnterRangeState : FSMState<FSMTester>
{
    public override void Action(IFSM<FSMTester> fsm)
    {
        Utility.Debug.LogInfo("EnterRangeState Action", MessageColor.INDIGO,fsm.Owner.gameObject);
    }
    public override void OnEnter(IFSM<FSMTester> fsm)
    {
        Utility.Debug.LogInfo("EnterRangeState OnEnter", MessageColor.INDIGO);
    }
    public override void OnExit(IFSM<FSMTester> fsm)
    {
        Utility.Debug.LogInfo("EnterRangeState OnExit", MessageColor.INDIGO);
    }
    public override void OnInitialization(IFSM<FSMTester> fsm)
    {
        AddTrigger(new ExitTestTrigger(), new ExitRangeState());
        Utility.Debug.LogInfo("EnterRangeState OnInitialization", MessageColor.INDIGO);
    }
    public override void OnTermination(IFSM<FSMTester> fsm)
    {
        Utility.Debug.LogInfo("EnterRangeState OnTermination", MessageColor.INDIGO);
    }
}
