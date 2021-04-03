using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class KCPHeartbeat : IRefreshable,IOperable
    {
        long latestTime;
        bool canRun = false;
        public void OnActive()
        {
            canRun = true;
            latestTime = Utility.Time.MillisecondNow();
        }
        public void OnDeactive()
        {
            canRun = false;
        }
        public void OnRefresh()
        {
            if (!canRun)
                return;
            var hb = BitConverter.GetBytes(0);
            CosmosEntry.NetworkManager.SendNetworkMessage(hb); ;
        }
    }
}
