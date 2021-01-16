using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.Mvvm
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple =false,Inherited =true)]
    public class NotifyHandlerAttribute:Attribute
    {

    }
}
