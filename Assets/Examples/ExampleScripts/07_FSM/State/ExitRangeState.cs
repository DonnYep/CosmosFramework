using UnityEngine;
using Cosmos;
using Cosmos.FSM;
public class ExitRangeState : FSMState<Transform>
{
    public override void OnStateStay(IFSM<Transform> fsm)
    {
    }
    public override void OnStateEnter(IFSM<Transform> fsm)
    {
        var go = fsm.Owner.gameObject;
        Utility.Debug.LogInfo($"Exit {go.name} detection range", DebugColor.red, go);
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
