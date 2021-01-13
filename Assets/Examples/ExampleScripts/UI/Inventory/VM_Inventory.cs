using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Mvvm;
using UnityEngine;

namespace Cosmos.Test
{
    public class VM_Inventory : ViewModel
    {
        M_Inventory inventoryModel;
        public VM_Inventory()
        {
            inventoryModel = GetModel<M_Inventory>();
        }
        public override void Execute(object data)
        {

        }
        void LoadJson()
        {
            string json = null;
            JsonUtility.FromJsonOverwrite(json, inventoryModel.InventoryDataSet);
            Utility.Debug.LogInfo("LoadJsonDataFromLocal");
        }
    }
}
