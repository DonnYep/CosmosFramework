using UnityEngine;
using System.Collections;
using Cosmos;
using System.Net;
namespace Cosmos.Network
{
    //TODONetworkManager
    internal sealed class NetworkManager : Module<NetworkManager>
    {
        string serverIP;
        int serverPort;
        string clientIP;
        int clientPort;
        IPEndPoint serverEndPoint;
        public IPEndPoint ServerEndPoint
        {
            get
            {
                if (serverEndPoint == null)
                    serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
                return serverEndPoint;
            }
        }
        IPEndPoint clientEndPoint;
        public IPEndPoint ClientEndPoint
        {
            get
            {
                if (clientEndPoint == null)
                    clientEndPoint = new IPEndPoint(IPAddress.Parse(clientIP), clientPort);
                return clientEndPoint;
            }
        }
    }
}
