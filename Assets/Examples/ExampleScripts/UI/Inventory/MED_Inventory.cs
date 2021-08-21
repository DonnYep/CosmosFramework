using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PureMVC;

namespace Cosmos.Test
{
    /// <summary>
    /// 一个Mediator持有一个panel 的ViewEntity；
    /// </summary>
    public class MED_Inventory : Mediator
    {
        public new const string NAME = "MED_Inventory";

        InventoryPanel InventoryPanel;

        public MED_Inventory():base(NAME)
        {
            EventKeys = new List<string>();
            EventKeys.Add(MVCEventDefine.CMD_Inventory);
        }
        PRX_Inventory PRX_Inventory { get { return MVC.PeekProxy<PRX_Inventory>(PRX_Inventory.NAME); } }
        public override void HandleEvent(INotifyArgs notifyArgs)
        {
            var args= notifyArgs.CastTo<GameNotifyArgs>();
            var cmd = (InvCmd)args.OpCode;
            switch (cmd)
            {
                case InvCmd.Flush:
                    {
                        //刷新inventory的数据
                        InventoryPanel.SlotContext.UpdateDataSet(PRX_Inventory.InventoryDataSet);
                    }
                    break;
                case InvCmd.ShowDescription:
                    {
                        InventoryPanel.TxtDescription.text =Convert.ToString( args.NotifyData);
                    }
                    break;
            }
        }
        public void Init (object viewEntity)
        {
            InventoryPanel = viewEntity.CastTo<InventoryPanel>();
            InventoryPanel.BtnLoad.onClick.AddListener(LoadClick);
            InventoryPanel.BtnQuit.onClick.AddListener(QuitClick);
            InventoryPanel.BtnSave.onClick.AddListener(SaveClick);
            InventoryPanel.BtnUpdate.onClick.AddListener(UpdateClick);
        }
        /// <summary>
        /// 更新inventory的slot与item中的信息；
        /// </summary>
        public void UpdateSlotItem()
        {
            InventoryPanel.UpdateSlotItem(PRX_Inventory.InventoryDataSet);
        }
        public void UpdateSlots(InventoryDataset dataset)
        {
            InventoryPanel.UpdateSlotItem(dataset);
        }
        void QuitClick()
        {
            CosmosEntry.UIManager.DeactiveUIForm(InventoryPanel.UIFormName);
        }
        void LoadClick()
        {
            PRX_Inventory.LoadJson();
            InventoryPanel.UpdateSlotItem(PRX_Inventory.InventoryDataSet);
        }
        void UpdateClick()
        {
            InventoryPanel.UpdateSlotItem(PRX_Inventory.InventoryDataSet);
        }
        void SaveClick()
        {
            PRX_Inventory.SaveJson();
        }
    }
}
