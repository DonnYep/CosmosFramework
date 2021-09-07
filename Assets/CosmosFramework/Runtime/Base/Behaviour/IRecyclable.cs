using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 是否可回收的；
    /// </summary>
    public interface IRecyclable
    {
        void OnRecycle();
    }
}
