using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 一次资源加载记录（与具体 Provider 无关）。
    /// </summary>
    public sealed class ResourceLoadRecord
    {
        /// <summary>
        /// 资源标识
        /// </summary>
        public string Key;
        /// <summary>
        /// Provider 名称
        /// </summary>
        public string ProviderName;
        /// <summary>
        /// 目标类型
        /// </summary>
        public string AssetTypeName;
        /// <summary>
        /// 加载方式
        /// </summary>
        public string LoadMode;
        /// <summary>
        /// 加载耗时（秒）
        /// </summary>
        public float DurationSeconds;
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succeeded;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error;
        /// <summary>
        /// 调用栈（开启捕获时有效）
        /// </summary>
        public string StackTrace;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime;

        public override string ToString()
        {
            return $"[{(Succeeded ? "OK" : "FAIL")}] {Key} ({AssetTypeName}/{LoadMode}) {DurationSeconds:F3}s {Error}";
        }
    }
}
