/// <summary>
/// ViewModel代理类
/// </summary>
namespace Cosmos.Mvvm
{
    public abstract class Command
    {
        public abstract void ExecuteCommand (object sender,NotifyArgs notifyArgs);
    }
}
