using System.Collections.Generic;

/// <summary>
/// View代理类
/// </summary>
namespace Cosmos.Mvvm
{
    public abstract class Mediator
    {
        public object ViewEntity { get; set; }
        public abstract string MediatorName { get; protected set; }
        public List<string> EventKeys { get; protected set; }
        public virtual void OnRegister() { }
        public virtual void OnRemove() { }
        public virtual void HandleEvent(object sender,NotifyArgs notifyArgs) { }
    }
}
