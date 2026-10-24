namespace Cosmos.Resource
{
    /// <summary>
    /// Provider 初始化操作。
    /// <para>异步初始化时由 Provider 调用 OperationSystem.StartOperation 启动；同步初始化可直接 Complete。</para>
    /// </summary>
    public class InitializationOperation : OperationBase
    {
        /// <summary>
        /// Provider 名称
        /// </summary>
        public string ProviderName { get; private set; }
        /// <summary>
        /// 底层方案版本号（可选）
        /// </summary>
        public string Version { get; private set; }

        public InitializationOperation(string providerName)
        {
            ProviderName = providerName;
        }
        /// <summary>
        /// 初始化完成。
        /// </summary>
        /// <param name="version">底层方案版本号，可为空</param>
        public void Complete(string version = null)
        {
            if (IsDone)
                return;
            Version = version;
            Error = null;
            Status = OperationStatus.Succeeded;
            Progress = 1f;
        }
        /// <summary>
        /// 初始化失败。
        /// </summary>
        public void Fail(string error)
        {
            if (IsDone)
                return;
            Error = error;
            Status = OperationStatus.Failed;
            Progress = 1f;
        }
    }
}
