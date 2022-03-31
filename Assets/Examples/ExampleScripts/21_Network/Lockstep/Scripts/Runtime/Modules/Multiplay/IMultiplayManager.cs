using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.Lockstep
{
    public interface IMultiplayManager:IModuleManager
    {
        int AuthorityConv { get;  }
        bool IsConnected { get; }
        /// <summary>
        /// 向服务器发送输入数据；
        /// </summary>
        /// <param name="inputData">按键操作数据</param>
        void SendInputData(byte[] inputData);
    }
}
