using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Mvvm
{
    [ImplementProvider]
    public abstract class Model
    {
        //public abstract string ViewModelKey { get; }
        protected void Fire(string cmdKey, object data = null)
        {
            MVVM.Fire(cmdKey, data);
        }
    }
}