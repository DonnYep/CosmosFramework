using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cosmos;
using Cosmos.FSM;

public class FSMTester : MonoBehaviour
{
    [SerializeField] Transform target;
    public Transform Target { get { return target; } }
    [SerializeField] float refreshInterval = 1;
    [SerializeField] int range = 10;
    [SerializeField] int pauseDelay=10;
    public int Range { get { return range; } }
    FSM<FSMTester> fsm;
    IFSMManager fsmManager;
    private void OnValidate()
    {
        if (range <= 0)
            range = 0;
        if (pauseDelay < 0)
            pauseDelay = 0;
    }
    private void Start()
    {
        fsmManager = CosmosEntry.FSMManager;
        var enterState = new EnterRangeState();
        var enterTrigger = new EnterTestTrigger();
        var exitState = new ExitRangeState();
        var exitTrigger = new ExitTestTrigger();
        exitState.AddTrigger(enterTrigger, enterState);
        enterState.AddTrigger(exitTrigger, exitState);
        fsm = fsmManager.CreateFSM("FSMTester",this,false, exitState, enterState) as FSM<FSMTester>;
        fsmManager.SetFSMSetRefreshInterval<FSMTester>(refreshInterval);
        fsm.DefaultState = exitState;
        fsm.StartDefault();
        Utility.Unity.DelayCoroutine(pauseDelay, () => { fsmManager.PauseFSMSet<FSMTester>(); });
    }
}
