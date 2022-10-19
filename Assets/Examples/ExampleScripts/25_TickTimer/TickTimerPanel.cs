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
    Text txtCountdown;
    int tickTaskId;
    int callCount = 0;
    bool hasOneTaskRun = false;
    void Awake()
    {
        btnRun = gameObject.GetComponentInChildren<Button>("BtnRun");
        btnPause = gameObject.GetComponentInChildren<Button>("BtnPause");
        btnUnpause = gameObject.GetComponentInChildren<Button>("BtnUnpause");
        txtCountdown = gameObject.GetComponentInChildren<Text>("TxtCountdown");
        btnRun.onClick.AddListener(OnRunClick);
        btnPause.onClick.AddListener(OnPauseClick);
        btnUnpause.onClick.AddListener(OnUnPauseClick);
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
        tickTimer.PauseTask(tickTaskId);
        txtCountdown.text = $"Task call count: {callCount} PAUSE";
    }
    void OnUnPauseClick()
    {
        tickTimer.UnPauseTask(tickTaskId);
        txtCountdown.text = $"Task call count: {callCount}";
    }
    void OnTickTaskCallback(int id)
    {
        txtCountdown.text = $"Task call count: {callCount++}";
    }
    void OnTickCancelCallback(int id)
    {
        Utility.Debug.LogInfo($"tick task {id} cancel");
        hasOneTaskRun = false;
    }
}
