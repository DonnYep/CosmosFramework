using UnityEngine;
using Cosmos.UI;
using UnityEngine.UI;

public class EntityGameLoadingSlider : UGUIUIForm
{
    Slider sldProgress;
    CanvasGroup canvasGroup;
    Text txtProgress;

    public override void OnInit()
    {
        sldProgress = GetUILabel<Slider>("SldProgress");
        txtProgress = GetUILabel<Text>("TxtProgress");
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }
    public override void OnClose()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        base.OnClose();
    }
    public override void OnOpen()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        base.OnOpen();
    }
    public void UpdateProgress(float progress)
    {
        var percent = progress * 100;
        sldProgress.value = Mathf.Lerp(sldProgress.value, percent, Time.deltaTime * 10);
        txtProgress.text = Mathf.RoundToInt(sldProgress.value) + "%";
    }
}
