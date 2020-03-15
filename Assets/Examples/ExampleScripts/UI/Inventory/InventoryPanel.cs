using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
namespace Cosmos
{
    public class InventoryPanel : UILogicResident
    {
        protected override void OnInitialization()
        {
            GetUIPanel<Button>("BtnLoad").onClick.AddListener(LoadClick);
            GetUIPanel<Button>("BtnQuit").onClick.AddListener(QuitClick);
            GetUIPanel<Button>("BtnSave").onClick.AddListener(SaveClick);
        }
        void LoadClick()
        {
            Utility.DebugLog("InventoryLoad>>" + this.GetType().Name);
        }
        void SaveClick()
        {
            Utility.DebugLog("InventorySave>>"+ this.GetType().FullName);
        }
        void QuitClick()
        {
            HidePanel();
        }
    }
}