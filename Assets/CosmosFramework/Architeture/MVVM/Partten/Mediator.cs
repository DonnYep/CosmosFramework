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
        public abstract void OnBind();
        public abstract void OnUnbind();

    }
}
