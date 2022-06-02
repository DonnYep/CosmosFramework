using Cosmos;
using PureMVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CMD_Navigate : Command
{
    public override void ExecuteCommand(INotifyArgs notifyArgs)
    {
        Utility.Debug.LogInfo($"Run {MVCEventDefine.CMD_Navigate}", DebugColor.red);
        MVC.RegisterProxy(new PRX_Inventory());
        MVC.RegisterMediator(new MED_Inventory());
    }
}