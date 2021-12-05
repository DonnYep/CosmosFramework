using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.FSM;
using Cosmos;
public class EnterRangeState : FSMState<FSMTester>
{
    public override void Action(IFSM<FSMTester> fsm)
    {
    }
    public override void OnEnter(IFSM<FSMTester> fsm)
    {
        var go = fsm.Owner.gameObject;
        Utility.Debug.LogInfo($"{go.name} EnterRangeState OnEnter", go);
    }
    public override void OnExit(IFSM<FSMTester> fsm)
    {
        var go = fsm.Owner.gameObject;
        Utility.Debug.LogInfo($"{go.name} EnterRangeState OnExit", go);
    }
    public override void OnInitialization(IFSM<FSMTester> fsm)
    {
    }
    public override void OnTermination(IFSM<FSMTester> fsm)
    {
    }
}
