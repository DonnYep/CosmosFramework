using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// ViewModel代理类
/// </summary>
namespace Cosmos.Mvvm
{
    public abstract class Command
    {
        public abstract string CommandKey { get; set; }
        public abstract void ExecuteCommand(object sender, object data);
    }
}
