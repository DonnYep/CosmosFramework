namespace Cosmos.UI
{
    public interface IUIForm
    {
        object Handle { get; }
        int Priority { get; }
        string UIFormName { get; }
        string UIGroupName { get; }
    }
}
