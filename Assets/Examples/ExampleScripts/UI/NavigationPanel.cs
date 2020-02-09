using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos.UI;
namespace Cosmos.Test
{
    public class NavigationPanel :UILogicTemporary
    {
        private void Start()
        {
            GetUIPanel<Button>("Btn_ShowWelcomeMenu").onClick.AddListener(ShowWelcomeMenu);
            GetUIPanel<Button>("Btn_RemoveWelcomeMenu").onClick.AddListener(RemoveWelcomeMenu);
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