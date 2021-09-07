using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Test
{
    public class InvMsg
    {
        public InvCmd InvCmd { get; set; }
        public object Msg { get; set; }
        public InvMsg(InvCmd invCmd):this(invCmd,null){}
        public InvMsg(InvCmd invCmd, object msg)
        {
            InvCmd = invCmd;
            Msg = msg;
        }
    }
}
