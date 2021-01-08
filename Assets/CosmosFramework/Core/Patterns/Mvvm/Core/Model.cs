using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Mvvm
{
    [ImplementProvider]
    public abstract class Model
    {
        public abstract string CommandKey { get; }
        protected void Fire(string cmdKey, object data = null)
        {
            MVC.Fire(cmdKey, data);
        }
    }
}