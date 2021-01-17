using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.FSM;
using Cosmos;
public class EnterRangeState : FSMState<FSMTester>
{
    public override void Action(IFSM<FSMTester> fsm)
    {
        Utility.DebugLog("EnterRangeState Action", MessageColor.INDIGO,fsm.Owner.gameObject);
    }
    public override void OnEnter(IFSM<FSMTester> fsm)
    {
        Utility.DebugLog("EnterRangeState OnEnter", MessageColor.INDIGO);
    }
    public override void OnExit(IFSM<FSMTester> fsm)
    {
        Utility.DebugLog("EnterRangeState OnExit", MessageColor.INDIGO);
    }
    public override void OnInitialization(IFSM<FSMTester> fsm)
    {
        Utility.DebugLog("EnterRangeState OnInitialization", MessageColor.INDIGO);
    }
    public override void OnTermination(IFSM<FSMTester> fsm)
    {
        Utility.DebugLog("EnterRangeState OnTermination", MessageColor.INDIGO);
    }
}
