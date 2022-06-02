using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.FSM;
using Cosmos;
public class EnterRangeState : FSMState<Transform>
{
    public override void OnStateStay(IFSM<Transform> fsm)
    {
    }
    public override void OnStateEnter(IFSM<Transform> fsm)
    {
        var go = fsm.Owner.gameObject;
        Utility.Debug.LogInfo($"Enter {go.name} detection range", DebugColor.green,go);
    }
    public override void OnStateExit(IFSM<Transform> fsm)
    {
    }
    public override void OnInitialization(IFSM<Transform> fsm)
    {
    }
    public override void OnTermination(IFSM<Transform> fsm)
    {
    }
}
