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
            UpdateSlotItem();
        }
        public void SaveInventoryJson()
        {
            PRX_Inventory.SaveJson();
        }
        /// <summary>
        /// 更新inventory的slot与item中的信息；
        /// </summary>
        public void UpdateSlotItem()
        {
            InventoryPanel.UpdateSlotItem(PRX_Inventory.InventoryDataSet);
        }
        /// <summary>
        ///更新item描述
        /// </summary>
        /// <param name="desc">描述信息</param>
        public void DisplayItemDesc(string desc)
        {
            InventoryPanel.UpdataItemDescription(desc);
        }
        /// <summary>
        /// 刷新inventory的数据
        /// </summary>
        public void FlushDataSet()
        {
            InventoryPanel.SlotContext.UpdateDataSet(PRX_Inventory.InventoryDataSet);
        }
    }
}
