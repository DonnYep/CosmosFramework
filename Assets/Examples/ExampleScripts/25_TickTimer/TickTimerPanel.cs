using UnityEngine;
using UnityEngine.UI;
using Cosmos;
public class TickTimerPanel : MonoBehaviour
{
    [SerializeField] int interval = 1000;
    TickTimer tickTimer = new TickTimer(true);
    Button btnRun;
    Button btnPause;
    Button btnUnpause;
    Button btnStop;
    Text txtCountdown;
    int tickTaskId;
    int callCount = 0;
    bool hasOneTaskRun = false;
    void Awake()
    {
        btnRun = gameObject.GetComponentInChildren<Button>("BtnRun");
        btnPause = gameObject.GetComponentInChildren<Button>("BtnPause");
        btnUnpause = gameObject.GetComponentInChildren<Button>("BtnUnpause");
        btnStop = gameObject.GetComponentInChildren<Button>("BtnStop");
        txtCountdown = gameObject.GetComponentInChildren<Text>("TxtCountdown");
        btnRun.onClick.AddListener(OnRunClick);
        btnPause.onClick.AddListener(OnPauseClick);
        btnUnpause.onClick.AddListener(OnUnPauseClick);
        btnStop.onClick.AddListener(OnStopClick);
    }
    void Update()
    {
        tickTimer.TickRefresh();
    }
    void OnRunClick()
    {
        if (hasOneTaskRun)
            return;
        tickTaskId = tickTimer.AddTask(interval, OnTickTaskCallback, OnTickCancelCallback, int.MaxValue);
        txtCountdown.text = $"Task call count: {callCount}";
        hasOneTaskRun = true;
    }
    void OnPauseClick()
    {
        if (!hasOneTaskRun)
            return;
        tickTimer.PauseTask(tickTaskId);
        txtCountdown.text = $"Task call count: {callCount} PAUSE";
    }
    void OnUnPauseClick()
    {
        if (!hasOneTaskRun)
            return;
        tickTimer.UnPauseTask(tickTaskId);
        txtCountdown.text = $"Task call count: {callCount}";
    }
    void OnStopClick()
    {
        if (!hasOneTaskRun)
            return;
        tickTimer.RemoveTask(tickTaskId);
        txtCountdown.text = $"No task run";
    }
    void OnTickTaskCallback(int id)
    {
        txtCountdown.text = $"Task call count: {callCount++}";
    }
    void OnTickCancelCallback(int id)
    {
        Utility.Debug.LogInfo($"tick task {id} cancel");
        hasOneTaskRun = false;
        callCount = 0;
    }
}
