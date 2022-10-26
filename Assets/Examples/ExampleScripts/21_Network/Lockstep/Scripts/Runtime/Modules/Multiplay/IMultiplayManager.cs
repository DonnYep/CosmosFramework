namespace Cosmos.Lockstep
{
    public interface IMultiplayManager:IModuleManager, IModuleInstance
    {
        int AuthorityConv { get;  }
        bool IsConnected { get; }
        /// <summary>
        /// 向服务器发送输入数据；
        /// </summary>
        /// <param name="inputData">按键操作数据</param>
        void SendInputData(byte[] inputData);
    }
}
