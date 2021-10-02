using System;
using System.Threading.Tasks;
namespace Cosmos.Network
{
    public static class WaitForChannelExtensions
    {
        /// <summary>
        /// 异步连接，注意区分ClientChannel与ServerChannel；
        /// ClientChannel会阻塞，直到与服务器连接成功；
        /// ServerChannel会阻塞，直到任意客户端建立连接；
        /// </summary>
        public async static Task ConnectAsync(this INetworkChannel networkChannel)
        {
            await new WaitConnectedAwaiter(networkChannel);
        }
        public async static Task<byte[]> ReceiveDataAsync(this INetworkChannel networkChannel)
        {
            return await new WaitReceiveDataAwaiter(networkChannel);
        }
        /// <summary>
        /// 异步断开连接，注意区分ClientChannel与ServerChannel；
        /// ClientChannel会阻塞，直到与服务器断开连接；
        /// ServerChannel会阻塞，直到任意客户端断开连接；
        /// </summary>
        public async static Task DisconnectAsync(this INetworkChannel networkChannel, int conv = 0)
        {
            await new WaitDisconnectedAwaiter(networkChannel, conv);
        }
        public async static Task AbortAsync(this INetworkChannel networkChannel)
        {
            await new WaitAbortAwaiter(networkChannel);
        }
    }
}
