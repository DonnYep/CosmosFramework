using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.GenericMvvm
{
    public interface IView
    {
        string Name { get; }
        void ExecuteEvent(string cmdName, object data);
        void BindEvent();
        void HandleEvent(string cmdName, object data = null);
        void OnUnbind();
    }
}
