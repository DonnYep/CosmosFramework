using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PureMVC;
namespace Cosmos.Test
{
    public class GameNotifyArgs : NotifyArgs
    {
        public byte OpCode { get; set; }
        public GameNotifyArgs(string notifyName) : base(notifyName){}
        public GameNotifyArgs(string notifyName, object notifyData) : base(notifyName, notifyData){}
    }
}
