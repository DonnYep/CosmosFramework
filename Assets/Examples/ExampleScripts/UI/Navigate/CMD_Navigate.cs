using Cosmos.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.Test
{
    public class CMD_Navigate : Command
    {
        public override void ExecuteCommand(object sender, NotifyArgs notifyArgs)
        {
            Utility.Debug.LogInfo($"Run {MVVMDefine.CMD_Navigate}", MessageColor.RED);

            MVVM.RegisterProxy(new PRX_Inventory());
            MVVM.RegisterMediator(new MED_Inventory());
            MVVM.RegisterCommand<CMD_Inventory>(MVVMDefine.CMD_Inventory);
        }
    }
}