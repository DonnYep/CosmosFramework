using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class TCPClientPeer : IRemotePeer,IRefreshable
    {
        public long Conv { get; set; }

        public bool Available { get; private set; }

        public void Clear()
        {
        }
        public void OnActive()
        {
        }
        public void OnDeactive()
        {
        }
        public void OnRefresh()
        {
        }
        public void SendMessage(INetworkMessage netMsg)
        {
            var buffer= netMsg.EncodeMessage();
        }
        public virtual void MessageHandler(INetworkMessage msg)
        {

        }
    }
}
