using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 生命周期接口
    /// </summary>
   public interface  IBehaviour
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 终止
        /// </summary>
        void OnTermination();
    }
}
