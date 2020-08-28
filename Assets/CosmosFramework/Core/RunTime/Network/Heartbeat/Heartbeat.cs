using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class Heartbeat : IHeartbeat
    {
        public uint Conv { get; set; }
        /// <summary>
        /// 秒级别；
        /// 1代表1秒；
        /// </summary>
        public uint HeartbeatInterval { get; set; } = 5;
        /// <summary>
        /// 秒级别；
        /// 上一次心跳时间；
        /// </summary>
        public long LatestHeartbeatTime { get; private set; }
        /// <summary>
        /// 是否存活
        /// </summary>
        public bool Available { get; private set; }
        /// <summary>
        /// 最大失效次数
        /// </summary>
        public byte MaxRecurCount { get; set; } = 5;
        /// <summary>
        /// 失活时触发的委托；
        /// </summary>
        public Action UnavailableHandler { get; set; }
        /// <summary>
        /// 发送心跳的委托
        /// </summary>
        public Action<INetworkMessage> SendHeartbeatHandler { get; set; }
        /// <summary>
        /// 当前发送的心跳次数
        /// </summary>
        byte currentRecurCount;
        public void OnActive()
        {
            LatestHeartbeatTime = Utility.Time.SecondNow() + HeartbeatInterval;
            currentRecurCount = 0;
            Available = true;
        }
        public void OnRefresh()
        {
            if (!Available)
                return;
            long now = Utility.Time.SecondNow();
            if (now <= LatestHeartbeatTime)
                return;
            LatestHeartbeatTime = now + HeartbeatInterval;
            currentRecurCount += 1;
            if (currentRecurCount >= MaxRecurCount)
            {
                Available = false;
                UnavailableHandler?.Invoke();
                return;
            }
            SendHeartbeatHandler?.Invoke(UdpNetworkMessage.HeartbeatMessage(Conv));
            Utility.Debug.LogInfo($"客户端发起心跳：Conv : {Conv} ; currentRecurCount : {currentRecurCount}",MessageColor.ORANGE);
        }
        public void OnRenewal()
        {
            long now = Utility.Time.SecondNow();
            LatestHeartbeatTime = now + HeartbeatInterval;
            currentRecurCount = 0;
            Utility.Debug.LogInfo($"接收到心跳响应：Conv : {Conv} ");
        }
        public void OnDeactive()
        {
            currentRecurCount = 0;
            //HeartbeatInterval = 0;
            LatestHeartbeatTime = 0;
            Available = false;
        }
        public void Clear()
        {
            OnDeactive();
            UnavailableHandler = null;
        }
    }
}
