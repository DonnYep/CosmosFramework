using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Mvvm
{
    [ImplementProvider]
    public abstract class Model
    {
        public abstract string Name { get; }
        protected void SendEvent(string eventName, object data = null)
        {
            MVC.SendEvent(eventName, data);
        }
    }
}