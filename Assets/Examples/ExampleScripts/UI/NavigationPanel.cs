using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
    public class NavigationPanel :UILogicTemporary
    {
        protected override void OnInitialization()
        {
            GetUIPanel<Button>("BtnShowWelcomeMenu").onClick.AddListener(ShowWelcomeMenu);
            GetUIPanel<Button>("BtnRemoveWelcomeMenu").onClick.AddListener(RemoveWelcomeMenu);
        }
        protected override void OnTermination()
        {
            base.OnTermination();
            GetUIPanel<Button>("BtnShowWelcomeMenu").onClick.RemoveAllListeners();
            GetUIPanel<Button>("BtnRemoveWelcomeMenu").onClick.RemoveAllListeners();
        }
        void ShowWelcomeMenu()
        {
            Facade.Instance.ShowPanel<WelcomeMenuPanel>("UI/WelcomeMenu"
                ,panel=> { panel.gameObject.name = "WelcomeMenu"; panel.gameObject.SetActive(true);});
        }
        void RemoveWelcomeMenu()
        {
            Facade.Instance.RemovePanel("UI/WelcomeMenu");
        }
    }
