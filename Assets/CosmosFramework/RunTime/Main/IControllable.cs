using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 可控生命周期接口，
    /// 能够暂停、恢复
    public interface IControllable
    {
        /// <summary>
        /// 暂停
        /// </summary>
        void OnPause();
        /// <summary>
        /// 恢复暂停
        /// </summary>
        void OnUnPause();
    }
}
