public class InventoryEventArgs
{
    public InvCmd InvCmd { get; set; }
    public object Msg { get; set; }
    public InventoryEventArgs(InvCmd invCmd) : this(invCmd, null) { }
    public InventoryEventArgs(InvCmd invCmd, object msg)
    {
        InvCmd = invCmd;
        Msg = msg;
    }
}
