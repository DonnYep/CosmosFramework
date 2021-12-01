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
    IFSM<FSMTester> fsm;
    IFSMManager fsmManager;
    private void OnValidate()
    {
        if (range <= 0)
            range = 0;
        if (pauseDelay < 0)
            pauseDelay = 0;
    }
    private async void Start()
    {
        fsmManager = CosmosEntry.FSMManager;

        var enterState = new EnterRangeState();
        var exitState = new ExitRangeState();

        fsm = fsmManager.CreateFSM("FSMTester", this, false, exitState, enterState);
        fsmManager.SetFSMGroupRefreshInterval<FSMTester>((int)(refreshInterval*1000));
        fsm.DefaultState = exitState;
        fsm.StartDefault();
        await new WaitForSeconds(pauseDelay);
        fsmManager.PauseFSMGroup<FSMTester>();
    }
}
