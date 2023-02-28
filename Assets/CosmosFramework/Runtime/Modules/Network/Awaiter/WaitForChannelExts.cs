using System.Threading.Tasks;
namespace Cosmos.Network
{
    public static class WaitForChannelExts
    {
        public async static Task ConnectAsync(this INetworkClientChannel @this, string host, int port)
        {
            await new WaitConnectedAwaiter(@this,host,port);
        }
        public async static Task<byte[]> ReceiveDataAsync(this INetworkClientChannel @this)
        {
            return await new WaitReceiveDataAwaiter(@this);
        }
        public async static Task DisconnectAsync(this INetworkClientChannel @this)
        {
            await new WaitDisconnectedAwaiter(@this);
        }
    }
}
