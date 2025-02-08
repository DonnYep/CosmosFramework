using System.Collections.Generic;
using System;
using System.Text;
using MessagePack;
namespace Cosmos.Lockstep
{
    //================================================
    /*
    *1、多人同步模块负责向服务器发送byte[]消息，接收并广播服务器广播的
    *byte[] 数据；
    */
    //================================================
    [Module]
    public class MultiplayManager : Module, IMultiplayManager
    {
        public int AuthorityConv { get; private set; }
        MultiplayEntitiesAgent multiplayEntitiesAgent;
        public bool IsConnected { get; private set; }
        Action<byte[]> SendMessage;
        /// <summary>
        /// 向服务器发送输入数据；
        /// </summary>
        /// <param name="inputData">按键操作数据</param>
        public void SendInputData(byte[] inputData)
        {
            var authorityInputOpdata = new MultiplayData((byte)MultiplayOperationCode.PlayerInput, 0, 0, inputData);
            var data = MultiplayData.Serialize(authorityInputOpdata);
            SendMessage(data);
        }
        protected override void OnPreparatory()
        {
            GameEntry.ServiceManager.OnReceiveData += OnReceiveDataHandle;
            GameEntry.ServiceManager.OnDisconnected += OnDisconnectHandle;
            SendMessage = GameEntry.ServiceManager.SendMessage; ;
            multiplayEntitiesAgent = new MultiplayEntitiesAgent();
            GameEntry.InputManager.SetInputHelper(new StandardInputHelper());
        }
        void OnReceiveDataHandle(byte[] buffer)
        {
            var opData = MultiplayData.Deserialize(buffer);
            ProcessHandler(opData);
        }
        void OnDisconnectHandle()
        {
            AuthorityConv = 0;
            multiplayEntitiesAgent.OnMulitplayDisconnected();
            IsConnected = false;
        }
        void ProcessHandler(MultiplayData opData)
        {
            var opCode = (MultiplayOperationCode)opData.OperationCode;
            switch (opCode)
            {
                case MultiplayOperationCode.SYN:
                    {
                        var messageDict = MessagePackSerializer.Deserialize<Dictionary<byte, object>>(opData.DataContract);
                        var authorityConv = messageDict.GetValue((byte)MultiplayParameterCode.AuthorityConv);
                        var serverSyncInterval = messageDict.GetValue((byte)MultiplayParameterCode.ServerSyncInterval);
                        AuthorityConv = Convert.ToInt32(authorityConv);
                        MultiplayConstant.IntervalMS = Convert.ToInt32(serverSyncInterval);

                        multiplayEntitiesAgent.OnMulitplayConnected();

                        var remoteConvsJson = Convert.ToString(messageDict.GetValue((byte)MultiplayParameterCode.RemoteConvs));
                        var remoteConvs = Utility.Json.ToObject<List<int>>(remoteConvsJson);
                        if (remoteConvs != null)
                        {
                            var length = remoteConvs.Count;
                            for (int i = 0; i < length; i++)
                            {
                                multiplayEntitiesAgent.OnMulitplayPlayerEnter(remoteConvs[i]);
                            }
                        }
                        IsConnected = true;
                    }
                    break;
                case MultiplayOperationCode.PlayerEnter:
                    {
                        var enterNetId = BitConverter.ToInt32(opData.DataContract, 0);
                        multiplayEntitiesAgent.OnMulitplayPlayerEnter(enterNetId);
                    }
                    break;
                case MultiplayOperationCode.PlayerExit:
                    {
                        var exitNetId = BitConverter.ToInt32(opData.DataContract, 0);
                        multiplayEntitiesAgent.OnMulitplayPlayerExit(exitNetId);
                    }
                    break;
                case MultiplayOperationCode.PlayerInput:
                    {
                        var fixTransports = MessagePackSerializer.Deserialize<Dictionary<int, List<byte[]>>>(opData.DataContract);
                        if (fixTransports != null)
                        {
                            multiplayEntitiesAgent.OnMultiplayInput(fixTransports);
                        }
                    }
                    break;
                case MultiplayOperationCode.FIN:
                    {
                        Utility.Debug.LogError(Encoding.UTF8.GetString(opData.DataContract));
                        GameEntry.ServiceManager.Disconnect();
                    }
                    break;
            }
        }
    }
}