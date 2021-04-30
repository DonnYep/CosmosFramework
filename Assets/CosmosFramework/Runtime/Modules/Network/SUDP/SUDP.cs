using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SUDP
{
    /********************************************************************************
    Secured UDP Transfer Class -- UDP安全传输类
    采用TCP滑动窗口原理在应用层解决UDP协议传输的丢包,乱序等问题,实现UDP可靠传输;
    要求传入的UdpClient实例为已连接的实例,即调用此类的方法前请先执行初始化实例并连接;
    发送接收的数据类采用Stream,可应用到其子类FileStream,MemoryStream等
    ********************************************************************************/
    public class SUdpTransfer
    {
        const int defaultPacketSize = 513;//默认UDP包数据内容大小
        const int defaultGroupSize = 7;//组(窗口)大小
        const int confirmTimeOut = 1000;//确认超时时间
        const int maxResendCount = 4;//最大重发次数(超出则认为连接断开)
        const int receiveTimeOut = 4000;//接收超时时间

        UdpClient client;//已连接的UdpClient实例
        AutoResetEvent confirmEvent = new AutoResetEvent(false);//等待确认回复用的事件
        private int groupSeq;//当前传送的组序号
        private Thread thListen;//发送端确认包接收线程
        private bool error;//出错标志

        public SUdpTransfer(UdpClient client)
        {
            this.client = client;
        }

        //数据发送函数,返回值0:发送成功,-1:发送失败
        public int Send(Stream stream)
        {
            return Send(stream, defaultPacketSize, defaultGroupSize);
        }

        public int Send(Stream stream, int packetSize, int groupSize)
        {
            error = false;

            int dataSize = packetSize - 1;

            int i, read, readSum;
            byte flag = 0;//UDP包中的标志字节,包含组序号,包序号(即组内序号),发送结束标志
            int resendCount = 0;//重发次数标记

            try
            {
                //启动确认包接收线程
                thListen = new Thread(new ThreadStart(Listen));
                thListen.IsBackground = true;
                thListen.Start();

                groupSeq = 0;
                stream.Seek(0, SeekOrigin.Begin);
                confirmEvent.Reset();
                while (true)
                {
                    if (error)
                        return -1;

                    readSum = 0;//记录读取字节数以便重发时Stream的回退

                    for (i = 0; i < groupSize; i++)
                    {
                        flag = (byte)(((byte)groupSeq) << 4 | (((byte)i) << 1));
                        byte[] buffer = new byte[packetSize];
                        read = stream.Read(buffer, 0, dataSize);

                        readSum += read;

                        if (read == dataSize)
                        {
                            if (stream.Position == stream.Length)//已经读完
                            {
                                flag |= 0x01;//结束标记位置1
                                buffer[read] = flag;//数据包标志字节放于每个包的最后
                                client.Send(buffer, read + 1);

                                break;
                            }

                            buffer[read] = flag;
                            client.Send(buffer, read + 1);
                        }
                        else if (read > 0)//已经读完
                        {
                            flag |= 0x01;//结束标记位置1
                            buffer[read] = flag;//数据包标志字节放于每个包的最后
                            client.Send(buffer, read + 1);

                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (error)
                        return -1;

                    if (confirmEvent.WaitOne(confirmTimeOut))//收到确认
                    {
                        if ((int)(flag & 0x01) == 1)//发送完毕
                            break;

                        groupSeq = (groupSeq + 1) % 16;
                        resendCount = 0;
                    }
                    else//未收到确认
                    {
                        if (resendCount >= maxResendCount)//超出最大重发次数
                        {
                            thListen.Abort();
                            return -1;
                        }

                        //重发
                        stream.Seek(-1 * readSum, SeekOrigin.Current);
                        resendCount++;
                    }
                }

                //发送完毕,关闭确认包接收线程
                thListen.Abort();
            }
            catch//异常
            {
                thListen.Abort();
                return -1;
            }

            return 0;
        }

        //确认包接收线程函数
        private void Listen()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                while (true)
                {
                    byte[] confirm = client.Receive(ref ipep);

                    if ((int)confirm[0] == groupSeq)//收到确认
                    {
                        confirmEvent.Set();//激活接收确认事件
                    }
                    else if (confirm[0] == 0xFF)//传输中断命令
                    {
                        error = true;
                        break;
                    }
                }
            }
            catch//异常
            {
                error = true;
            }
        }

        //数据接收函数,返回值0:接收成功,-1:接收失败
        public int Receive(ref Stream stream)
        {
            return Receive(ref stream, defaultPacketSize, defaultGroupSize);
        }

        public int Receive(ref Stream stream, int packetSize, int groupSize)
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);

            int[] groupFlag = new int[groupSize];
            byte[][] groupData = new byte[groupSize][];
            byte flag;//UDP包中的标志字节,包含组序号,包序号(即组内序号),发送结束标志
            int groupSeq, packetSeq, myGroupSeq;
            int dataSize = packetSize - 1;
            int i, endFlag, currentGroupSize;
            int socketRecieveTimeOut = client.Client.ReceiveTimeout;//保存原来的接收超时时间

            try
            {
                client.Client.ReceiveTimeout = receiveTimeOut;//设置接收超时时间
                currentGroupSize = groupSize;
                myGroupSeq = 0;

                while (true)
                {
                    byte[] data = client.Receive(ref ipep);
                    {
                        if (data.Length < 2)//最小数据长度为2字节
                        {
                            if (data.Length == 1 && data[0] == 0xFF)//传输中断命令
                            {
                                client.Client.ReceiveTimeout = socketRecieveTimeOut;//恢复原来的接收超时时间
                                return -1;
                            }
                            continue;
                        }

                        flag = data[data.Length - 1];//数据包标志字节在每个数据包的最后
                        groupSeq = flag >> 4;//前四位:组序号
                        packetSeq = (flag & 0x0F) >> 1;//中间三位:包序号
                        endFlag = flag & 0x01;//最后一位:发送结束标记

                        if (groupSeq != myGroupSeq)//不属于希望接受的数据包组
                        {
                            if ((groupSeq + 1) % 16 == myGroupSeq)//上一组回复的确认未收到
                            {
                                byte[] confirmData = new byte[] { (byte)groupSeq };
                                client.Send(confirmData, confirmData.Length);//回复确认
                            }
                            continue;
                        }

                        if (groupFlag[packetSeq] == 1)//已接收该包则丢弃
                        {
                            continue;
                        }

                        groupFlag[packetSeq] = 1;//记录
                        groupData[packetSeq] = data;//暂存数据
                        if (endFlag == 1)//收到含结束标记的包
                        {
                            currentGroupSize = packetSeq + 1;//改变当前组的窗口大小
                        }

                        //检测是否该组包已全部接收
                        for (i = 0; i < currentGroupSize; i++)
                        {
                            if (groupFlag[i] == 0)
                                break;
                        }
                        if (i == currentGroupSize)//已全部接收
                        {
                            //写入数据
                            for (i = 0; i < currentGroupSize; i++)
                            {
                                stream.Write(groupData[i], 0, groupData[i].Length - 1);
                            }
                            byte[] confirmData = new byte[] { (byte)groupSeq };

                            client.Send(confirmData, confirmData.Length);//回复确认
                            client.Send(confirmData, confirmData.Length);//回复两次,确保收到

                            if (endFlag == 1)//已收到结束包则退出
                            {
                                break;
                            }

                            myGroupSeq = (myGroupSeq + 1) % 16;
                            currentGroupSize = groupSize;
                            Array.Clear(groupFlag, 0, groupData.Length);
                        }
                    }
                }
            }
            catch//异常
            {
                client.Client.ReceiveTimeout = socketRecieveTimeOut;//恢复原来的接收超时时间
                return -1;
            }

            client.Client.ReceiveTimeout = socketRecieveTimeOut;
            return 0;
        }
    }
}