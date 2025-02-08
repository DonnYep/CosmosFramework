using UnityEngine;
using Cosmos;
using System;
public class WaitSecoundOp : GameAsyncOperation
{
    public float waitTime;
    float curTime;
    public WaitSecoundOp(float waitTime)
    {
        if (waitTime < 0)
        {
            waitTime = 0.1f;
        }
        this.waitTime = waitTime;
        curTime = 0;
        StartOperation(this);
    }
    Action<WaitSecoundOp> onUpdate;
    public event Action<WaitSecoundOp> OnUpdate
    {
        add { onUpdate += value; }
        remove { onUpdate -= value; }
    }
    protected override void OnOpAbort()
    {
    }
    protected override void OnOpFinish()
    {
    }

    protected override void OnOpStart()
    {
    }

    protected override void OnOpUpdate()
    {
        curTime += Time.deltaTime;
        Progress =curTime / waitTime;
        onUpdate?.Invoke(this);
        if (curTime >= waitTime)
        {
            Status = OperationStatus.Succeeded;
        }
    }
}
