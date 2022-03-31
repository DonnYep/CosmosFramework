using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Cosmos.FSM;
public class ExitRangeState : FSMState<Transform>
{
    public override void Action(IFSM<Transform> fsm)
    {
    }
    public override void OnEnter(IFSM<Transform> fsm)
    {
        var go = fsm.Owner.gameObject;
        Utility.Debug.LogInfo($"Exit {go.name} detection range", go);
    }
    public override void OnExit(IFSM<Transform> fsm)
    {
    }
    public override void OnInitialization(IFSM<Transform> fsm)
    {
    }
    public override void OnTermination(IFSM<Transform> fsm)
    {
    }
}
