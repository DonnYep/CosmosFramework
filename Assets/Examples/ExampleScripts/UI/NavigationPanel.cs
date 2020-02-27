using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos.UI;
namespace Cosmos.Test
{
    public class NavigationPanel :UILogicTemporary
    {
        protected override void OnInitialization()
        {
            GetUIPanel<Button>("Btn_ShowWelcomeMenu").onClick.AddListener(ShowWelcomeMenu);
            GetUIPanel<Button>("Btn_RemoveWelcomeMenu").onClick.AddListener(RemoveWelcomeMenu);
        }
        protected override void OnTermination()
        {
            base.OnTermination();
            GetUIPanel<Button>("Btn_ShowWelcomeMenu").onClick.RemoveAllListeners();
            GetUIPanel<Button>("Btn_RemoveWelcomeMenu").onClick.RemoveAllListeners();
        }
        void ShowWelcomeMenu()
        {
            Facade.Instance.ShowPanel<WelcomeMenuPanel>("WelcomeMenu",panel=>panel.gameObject.SetActive(true));
        }
        void RemoveWelcomeMenu()
        {
            Facade.Instance.RemovePanel("WelcomeMenu");
        }
    }
}