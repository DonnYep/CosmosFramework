using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 模块注入；
    /// 模块是internal的，建议使用模块的接口类型；
    /// 打上此标签后，传入模块接口类型，即可自动获得接口对应模块；
    /// </summary>
    [AttributeUsage( AttributeTargets.Field| AttributeTargets.Property,Inherited =false,AllowMultiple =false)]
    public class InjectModuleAttribute:Attribute
    {
    }
}
