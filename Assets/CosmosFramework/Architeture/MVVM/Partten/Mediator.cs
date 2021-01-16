using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public abstract void HandleEvent(object sender,NotifyArgs notifyArgs);
    }
}
