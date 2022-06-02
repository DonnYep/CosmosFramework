using Cosmos;
using PureMVC;

public class CMD_Navigate : Command
{
    public override void ExecuteCommand(INotifyArgs notifyArgs)
    {
        Utility.Debug.LogInfo($"Run {MVCEventDefine.CMD_Navigate}", DebugColor.red);
        MVC.RegisterProxy(new PRX_Inventory());
        MVC.RegisterMediator(new MED_Inventory());
    }
}