using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Cosmos.FSM;
public class ExitRangeState : FSMState<FSMTester>
{
    public override void Action(IFSM<FSMTester> fsm)
    {
        Utility.DebugLog("ExitRangeState Action", MessageColor.INDIGO, fsm.Owner.gameObject);
    }
    public override void OnEnter(IFSM<FSMTester> fsm)
    {
        Utility.DebugLog("ExitRangeState OnEnter", MessageColor.INDIGO);
    }
    public override void OnExit(IFSM<FSMTester> fsm)
    {
        Utility.DebugLog("ExitRangeState OnExit", MessageColor.INDIGO);
    }
    public override void OnInitialization(IFSM<FSMTester> fsm)
    {
        Utility.DebugLog("ExitRangeState OnInitialization", MessageColor.INDIGO);
    }
    public override void OnTermination(IFSM<FSMTester> fsm)
    {
        Utility.DebugLog("ExitRangeState OnTermination", MessageColor.INDIGO);
    }
}
