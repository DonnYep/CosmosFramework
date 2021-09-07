using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace kcp
{
    public abstract class KcpServiceBase
    {
        public ushort Port =8521;
        public bool NoDelay = true;
        public uint Interval = 10;
        public int FastResend  = 2;
        public bool CongestionWindow  = false; // KCP 'NoCongestionWindow' is false by default. here we negate it for ease of use.
        public uint SendWindowSize = 4096; //Kcp.WND_SND; 32 by default. Mirror sends a lot, so we need a lot more.
        public uint ReceiveWindowSize = 4096; //Kcp.WND_RCV; 128 by default. Mirror sends a lot, so we need a lot more.
        public abstract void ServiceSetup();
        public abstract void ServiceUnpause();
        public abstract void ServicePause();
        public abstract void ServiceTick();
        public virtual void ServiceSend(KcpChannel channelId, ArraySegment<byte> segment, int connectionId) { }
        public abstract void ServiceDisconnect(int connectionId);
        public abstract void ServiceConnect(string address);
        public Uri ServiceUri()
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "Kcp";
            builder.Host = Dns.GetHostName();
            builder.Port = Port;
            return builder.Uri;
        }
        // PrettyBytes function from DOTSNET
        // pretty prints bytes as KB/MB/GB/etc.
        // long to support > 2GB
        // divides by floats to return "2.5MB" etc.
        public static string PrettyBytes(long bytes)
        {
            // bytes
            if (bytes < 1024)
                return $"{bytes} B";
            // kilobytes
            else if (bytes < 1024L * 1024L)
                return $"{(bytes / 1024f):F2} KB";
            // megabytes
            else if (bytes < 1024 * 1024L * 1024L)
                return $"{(bytes / (1024f * 1024f)):F2} MB";
            // gigabytes
            return $"{(bytes / (1024f * 1024f * 1024f)):F2} GB";
        }

    }
}