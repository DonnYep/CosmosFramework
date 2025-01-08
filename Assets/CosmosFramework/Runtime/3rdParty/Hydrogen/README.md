# Hydrogen

Hydrogen是一个功能独立完善，支持IPv4与IPv6，易于集成的TCP库。Hydrogen支持UPM格式，可直接将库放置于Unity的Packages目录下。

## TCP消息结构

- 【消息头记录消息体的长度，长度2位】【消息体，最长长度为1 << 13】

## 实现自定义的消息头解码器

- TcpClient的构造参数中存在一个IHeaderEncoder 参数，需要自定义实现一个数据头解析对象。以protobuf为例，添加完毕pb库后，实现如下代码：
```csharp
using System.Buffers.Binary;
using Hydrogen;
public class HeaderEncoder: IHeaderEncoder
{
    public ushort DecodeHeader(byte[] header)
    {
        var len = BinaryPrimitives.ReadUInt16BigEndian(header);
        return len;
    }
    public byte[] EncodeHeader(ushort len)
    {
        var headerBuf = new byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(headerBuf, (ushort)len);
        return headerBuf;
    }
}
```
## TcpClient接口使用

- TcpClient接口使用
```csharp
//实例化client
client = new TcpClient(new HeaderEncoder());

//监听socket连接成功事件
client.OnConnected += OnConnectedHandler;
//监听socket接收事件
client.OnData += OnDataHandler;
//监听socket断开连接事件
client.OnDisconnected += OnDisconnectedHandler;


//连接你的socket地址
client.Connect(127.0.0.1, 8888);

//发送序列化成byte的数据
client.SendMessage(bytes);

```
## TcpClient的日志

- Hydrogen内存在一个Log的日志类。此类默认使用了unity的日志调用，如果需要自定义的日志类型，请直接赋值监听函数。日志类如下：
```csharp
using System;
using UnityEngine;
namespace Hydrogen
{
    public static class Log
    {
        public static Action<object> Info = (obj) => Debug.Log(obj);
        public static Action<object> Warning = (obj) => Debug.LogWarning(obj);
        public static Action<object> Error = (obj) => Debug.LogError(obj);
    }
}
```