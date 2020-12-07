using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HotfixMethodAttribute:Attribute
    {
        public string TargetName { get; private set; }
        public HotfixMethodAttribute(string targetName)
        {
            TargetName = targetName;
        }
    }
}
