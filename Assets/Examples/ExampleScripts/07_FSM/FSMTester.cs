using UnityEngine;
using Cosmos;
using Cosmos.FSM;

public class FSMTester : MonoSingleton<FSMTester>
{
    [SerializeField] Transform target;
    [SerializeField] Transform objectA;
    [SerializeField] Transform objectB;
    public Transform Target { get { return target; } }
    public Transform ObjectA { get { return objectA; } }
    public Transform ObjectB { get { return objectB; } }
    [SerializeField] float refreshInterval = 1;
    [SerializeField] int objectDetectionRange = 10;
    [SerializeField] int pauseDelay = 10;
    public int ObjectDetectionRange { get { return objectDetectionRange; } }
    IFSM<Transform> fsmA;
    IFSM<Transform> fsmB;
    IFSMManager fsmManager;
    private void OnValidate()
    {
        if (objectDetectionRange <= 0)
            objectDetectionRange = 0;
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

        fsmA = fsmManager.CreateFSM("FSMTesterA", objectA, exitState, enterState);
        fsmA.DefaultState = exitState;
        fsmA.StartDefault();

        fsmB = fsmManager.CreateFSM("FSMTesterB", ObjectB, exitState, enterState);
        fsmB.DefaultState = enterState;
        fsmB.StartDefault();

        //fsmManager.SetFSMGroupRefreshInterval<Transform>((int)(refreshInterval * 1000));
    }
}
