using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Mvvm;
namespace Cosmos.Test
{
    public class MED_Inventory : Mediator
    {
        public InventoryPanel InventoryPanel { get { return CosmosEntry.UIManager.PeekUIForm<InventoryPanel>("InventoryPanel"); } }
        PRX_Inventory PRX_Inventory { get { return MVVM.PeekProxy<PRX_Inventory>(MVVMDefine.PRX_Inventory); } }

        public override string MediatorName { get; protected set; }= MVVMDefine.MED_Inventory;
        public override void HandleEvent(object sender, NotifyArgs notifyArgs){}
        public void LoadInventoryJson()
        {
            PRX_Inventory.LoadJson();
            UpdateInventorySlotItem();
        }
        public void SaveInventoryJson()
        {
            PRX_Inventory.SaveJson();
        }
        public void UpdateInventorySlotItem()
        {
            InventoryPanel.SlotContext.UpdateSlot(PRX_Inventory.InventoryDataSet);
            InventoryPanel.SlotContext.UpdateItem(PRX_Inventory.InventoryDataSet);
        }
        public void UpdateItemDescription(string desc)
        {
            InventoryPanel.UpdataItemDescription(desc);
        }
        public void UpdateDataSet()
        {
            InventoryPanel.SlotContext.UpdateDataSet(PRX_Inventory.InventoryDataSet);
        }
    }
}
