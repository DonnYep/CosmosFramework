using System.Collections.Generic;
/// <summary>
/// View代理类
/// </summary>
namespace PureMVC
{
    public abstract class Mediator
    {
        public const string NAME = "Mediator";
        protected string mediatorName;
        public object ViewEntity { get; set; }
        public virtual string MediatorName { get { return mediatorName; } }
        public List<string> EventKeys { get; protected set; }
        public Mediator(string mediatorName):this(mediatorName,null) { }
        public Mediator(string mediatorName,object viewEntity) 
        {
            this.mediatorName = string.IsNullOrEmpty(mediatorName) ? NAME : mediatorName;
            ViewEntity = viewEntity;
        }
        public virtual void OnRegister() { }
        public virtual void OnRemove() { }
        public virtual void HandleEvent(INotifyArgs notifyArgs) { }
    }
}
