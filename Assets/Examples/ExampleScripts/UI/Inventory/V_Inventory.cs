using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Mvvm;
namespace Cosmos.Test
{
    public class V_Inventory : View
    {
        InventoryPanel inventoryPanel;
        public V_Inventory()
        {
            inventoryPanel = CosmosEntry.UIManager.PeekUIForm<InventoryPanel>("InventoryPanel");
        }
        //public override void BindVMKey()
        //{
        //    BindKey(UIEventDefine.VM_Inventory);
        //}
        //protected override void HandleEvent(string vmKey, object data = null)
        //{
        //    switch (vmKey)
        //    {
        //        case UIEventDefine.UI_SAVE_INV:
        //            break;
        //    }
        //}

    }
}
