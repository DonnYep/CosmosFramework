using System;
using System.Threading.Tasks;
namespace Cosmos.Network
{
    public static class WaitForChannelExtensions
    {
        public async static Task ConnectAsync(this INetworkChannel networkChannel)
        {
            await new WaitConnectedAwaiter(networkChannel);
        }
        public async static Task<byte[]> ReceiveDataAsync(this INetworkChannel networkChannel)
        {
            return await new WaitReceiveDataAwaiter(networkChannel);
        }
        public async static Task DisconnectAsync(this INetworkChannel networkChannel,int conv=0)
        {
            await new WaitDisconnectedAwaiter(networkChannel,conv);
        }
        public async static Task AbortAsync(this INetworkChannel networkChannel)
        {
            await new WaitAbortAwaiter(networkChannel);
        }
    }
}
