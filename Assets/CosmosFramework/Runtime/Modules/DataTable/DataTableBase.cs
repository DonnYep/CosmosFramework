using System;
using System.Collections.Generic;

namespace Cosmos.DataTable
{
    /// <summary>
    /// 数据表基类（非泛型视图）。
    /// <para>相对旧版（享元 + 行对象池）的改动：</para>
    /// <para>1、行数据加载后**只读共享**：不再有行对象池，行不会被回收/复用，
    /// 因此业务持有的行引用永远指向同一份数据（旧版 RemoveRow 会把行还池并复用，导致引用“串行”）；</para>
    /// <para>2、表自己持有加载状态（<see cref="State"/>）与失败原因（<see cref="Error"/>），
    /// 失败原因不再丢失；</para>
    /// <para>3、事件不再“触发后置空导致晚订阅收不到”：已加载/已失败时订阅会**立即回调**；</para>
    /// <para>4、加载是幂等的（Loading 中重复请求直接忽略，已加载直接回调），并支持
    /// <see cref="IDataTableManager.ReloadDataTableAsync"/>。</para>
    /// </summary>
    public abstract class DataTableBase : IDisposable
    {
        /// <summary>
        /// 表名
        /// </summary>
        protected readonly string name;
        Action<DataTableBase> onReadSuccess;
        Action<DataTableBase> onReadFailure;
        readonly List<Action<DataTableBase>> completions = new List<Action<DataTableBase>>();
        public string Name { get { return name; } }
        /// <summary>
        /// 加载状态
        /// </summary>
        public DataTableState State { get; internal set; }
        /// <summary>
        /// 最近一次失败原因（State == Failed 时有效）
        /// </summary>
        public string Error { get; internal set; }
        /// <summary>
        /// 是否已加载
        /// </summary>
        public bool IsLoaded { get { return State == DataTableState.Loaded; } }
        /// <summary>
        /// 行数
        /// </summary>
        public abstract int RowCount { get; }
        /// <summary>
        /// 行类型
        /// </summary>
        public abstract Type RowType { get; }
        /// <summary>
        /// 表资源信息
        /// </summary>
        public DataTableAssetInfo AssetInfo { get; internal set; }
        /// <summary>
        /// 加载成功事件
        /// <para>已加载时订阅会立即回调（无需担心订阅晚于加载）</para>
        /// </summary>
        public event Action<DataTableBase> OnReadSuccess
        {
            add
            {
                if (value == null)
                    return;
                if (State == DataTableState.Loaded)
                {
                    value(this);
                    return;
                }
                onReadSuccess += value;
            }
            remove { onReadSuccess -= value; }
        }
        /// <summary>
        /// 加载失败事件
        /// <para>已失败时订阅会立即回调</para>
        /// </summary>
        public event Action<DataTableBase> OnReadFailure
        {
            add
            {
                if (value == null)
                    return;
                if (State == DataTableState.Failed)
                {
                    value(this);
                    return;
                }
                onReadFailure += value;
            }
            remove { onReadFailure -= value; }
        }
        protected DataTableBase(string name)
        {
            this.name = name;
            State = DataTableState.None;
        }
        /// <summary>
        /// 解析并写入行数据（由模块在资源加载完成后调用）
        /// </summary>
        /// <param name="dataTableBytes">表资源字节</param>
        internal abstract void ParseAndSet(byte[] dataTableBytes);
        /// <summary>
        /// 清空行数据（不改变状态）
        /// </summary>
        internal abstract void ClearRows();
        /// <summary>
        /// 追加一个一次性完成回调（无论成功或失败都会回调）
        /// </summary>
        /// <param name="onCompleted">回调</param>
        internal void AddCompletion(Action<DataTableBase> onCompleted)
        {
            if (onCompleted != null)
                completions.Add(onCompleted);
        }
        /// <summary>
        /// 若已处于终态，立即回调并清空一次性回调
        /// </summary>
        internal void FlushCompletionsIfSettled()
        {
            if (State != DataTableState.Loaded && State != DataTableState.Failed)
                return;
            FlushCompletions();
        }
        internal void NotifyLoaded()
        {
            State = DataTableState.Loaded;
            Error = null;
            var successHandler = onReadSuccess;
            onReadSuccess = null;
            onReadFailure = null;
            FlushCompletions();
            successHandler?.Invoke(this);
        }
        internal void NotifyFailed(string error)
        {
            State = DataTableState.Failed;
            Error = string.IsNullOrEmpty(error) ? "unknown error" : error;
            var failureHandler = onReadFailure;
            onReadSuccess = null;
            onReadFailure = null;
            FlushCompletions();
            failureHandler?.Invoke(this);
        }
        void FlushCompletions()
        {
            if (completions.Count == 0)
                return;
            var callbacks = completions.ToArray();
            completions.Clear();
            for (int i = 0; i < callbacks.Length; i++)
                callbacks[i]?.Invoke(this);
        }
        /// <summary>
        /// 释放表数据（资源由模块负责卸载）
        /// </summary>
        public void Dispose()
        {
            ClearRows();
            completions.Clear();
            onReadSuccess = null;
            onReadFailure = null;
            State = DataTableState.None;
            Error = null;
            AssetInfo = default;
        }
    }
}
