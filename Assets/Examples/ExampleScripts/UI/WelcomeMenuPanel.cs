using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
using UnityEngine.UI;
namespace Cosmos.Test
{
    public class WelcomeMenuPanel : UILogicResident
    {
        Text info;
        private void Start()
        {
            GetUIPanel<Button>("Btn_ShowInfo").onClick.AddListener(ShowInfo);
            GetUIPanel<Button>("Btn_HideInfo").onClick.AddListener(HideInfo);
            GetUIPanel<Button>("Btn_Quit").onClick.AddListener(Quit);
            info = GetUIPanel<Image>("Text_Info").transform.Find("Info").GetComponent<Text>();
        }
        void ShowInfo()
        {
            info.enabled = true;
        }
        void HideInfo()
        {
            info.enabled =false;
        }
        void Quit()
        {
            this.gameObject.SetActive(false);
        }
    }
}