namespace Cosmos
{
    public interface  IController:IOperable,IControllable,IRefreshable
    {
        string ControllerName { get; set; }
    }
}