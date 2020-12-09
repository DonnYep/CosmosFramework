using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public interface IElapesRefreshable
    {
        /// <summary>
        /// 时间流逝轮询;
        /// </summary>
        /// <param name="msNow">utc毫秒当前时间</param>
        void OnElapseRefresh(long msNow);
    }
}
