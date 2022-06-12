using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源模块文件清单提供者
    /// </summary>
    public interface IResourceDataProvider
    {
        Dictionary<string, ResourceBundle> GetResourceObject();
    }
}
