using System;
using System.Collections;
using UnityEngine;

public class OpRunner : MonoBehaviour
{
    void Start()
    {
        //RunOp1();//方式1
        StartCoroutine(RunOp2());//方式2
    }
    void RunOp1()
    {
        var op = new WaitSecoundOp(3);
        op.Completed += Op_Completed;
        op.OnUpdate += Op_OnUpdate;
    }
    IEnumerator RunOp2()
    {
        var op = new WaitSecoundOp(3);
        while (!op.IsDone)
        {
            Debug.Log($"Progress {string.Format("{0:F}", op.Progress)}");
            yield return null;
        }
        Debug.Log("op done");
    }
    private void Op_OnUpdate(WaitSecoundOp op)
    {
        Debug.Log($"Progress {string.Format("{0:F}", op.Progress)}");
    }
    private void Op_Completed(Cosmos.OperationBase obj)
    {
        Debug.Log("op done");
    }
}
