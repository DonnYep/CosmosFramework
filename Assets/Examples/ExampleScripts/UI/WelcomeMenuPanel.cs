using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
using UnityEngine.UI;
using Cosmos;
    public class WelcomeMenuPanel : UILogicResident
    {
        Text info;
        InputField inputMsg;
        protected override void OnInitialization()
        {
            GetUIPanel<Button>("Btn_ShowInfo").onClick.AddListener(ShowInfo);
            GetUIPanel<Button>("Btn_HideInfo").onClick.AddListener(HideInfo);
            GetUIPanel<Button>("Btn_Quit").onClick.AddListener(Quit);
            info = GetUIPanel<Image>("Txt_Info").transform.Find("Info").GetComponent<Text>();
            inputMsg = GetUIPanel<InputField>("Input_Msg");
        }
        protected override void OnTermination()
        {
            GetUIPanel<Button>("Btn_ShowInfo").onClick.RemoveAllListeners();
            GetUIPanel<Button>("Btn_HideInfo").onClick.RemoveAllListeners();
            GetUIPanel<Button>("Btn_Quit").onClick.RemoveAllListeners();
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
