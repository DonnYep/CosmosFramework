using Cosmos.Operation;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    /// <summary>
    /// 网络请求句柄基类。
    /// <para>由 <see cref="WebRequestScheduler"/> 调度并发执行，支持：</para>
    /// <para>1、并发控制与优先级排队；</para>
    /// <para>2、超时控制与失败重试；</para>
    /// <para>3、协程等待（yield return）、事件回调、async/await、同步等待；</para>
    /// <para>4、手动取消（Cancel）。</para>
    /// </summary>
    public abstract class WebRequestHandleBase : OperationBase
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public long TaskId { get; internal set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string URL { get; protected set; }
        /// <summary>
        /// 超时时间（秒），小于等于0表示不限制
        /// </summary>
        public float TimeoutSeconds { get; set; } = 30f;
        /// <summary>
        /// 失败重试次数
        /// </summary>
        public int RetryCount { get; set; } = 0;
        /// <summary>
        /// 是否超时
        /// </summary>
        public bool IsTimeout { get; protected set; }
        /// <summary>
        /// 当前是否已成功
        /// </summary>
        public bool IsSucceeded
        {
            get { return Status == OperationStatus.Succeeded; }
        }
        /// <summary>
        /// 当前是否已失败
        /// </summary>
        public bool IsFailed
        {
            get { return Status == OperationStatus.Failed; }
        }
        /// <summary>
        /// 请求开始的UTC时间
        /// </summary>
        public DateTime StartTime { get; protected set; }
        /// <summary>
        /// 当前请求对象（完成回调期间仍有效，用于读取结果）
        /// </summary>
        public UnityWebRequest UnityWebRequest { get; protected set; }
        /// <summary>
        /// 当前实际尝试次数（含重试）
        /// </summary>
        public int AttemptCount { get; protected set; }

        internal WebRequestScheduler Scheduler { get; set; }

        bool requestStarted;
        bool cleanupDone;
        bool requestDisposed;

        /// <summary>
        /// 请求真正开始执行（获得并发槽）事件
        /// </summary>
        internal event Action<WebRequestHandleBase> OnStartedInternal;
        /// <summary>
        /// 请求进度事件
        /// </summary>
        internal event Action<WebRequestHandleBase, float> OnProgressInternal;
        /// <summary>
        /// 请求完成事件（成功、失败或取消均触发）
        /// </summary>
        internal event Action<WebRequestHandleBase> OnCompletedInternal;

        protected WebRequestHandleBase(string url)
        {
            URL = url;
        }

        /// <summary>
        /// 取消请求
        /// </summary>
        public void Cancel()
        {
            Abort();
        }

        /// <summary>
        /// 创建并发送请求（调度器授予并发槽时调用）
        /// </summary>
        internal void DispatchRequest()
        {
            if (requestStarted)
                return;
            requestStarted = true;
            StartRequest();
            OnStartedInternal?.Invoke(this);
        }

        /// <summary>
        /// 子类创建请求并发送
        /// </summary>
        protected abstract void StartRequest();

        /// <summary>
        /// 请求成功时转换结果
        /// </summary>
        protected abstract void OnRequestSucceeded(UnityWebRequest request);

        /// <summary>
        /// 请求失败处理：优先重试，重试耗尽则判定失败
        /// </summary>
        protected void OnRequestFailed(string error)
        {
            if (AttemptCount < RetryCount)
            {
                AttemptCount++;
                DisposeRequest();
                requestDisposed = false;
                requestStarted = false;
                StartTime = DateTime.Now;
                // 重新排队等待并发槽
                if (Scheduler == null || !Scheduler.TryAcquireSlot(this))
                    return;
                DispatchRequest();
                return;
            }
            Finish(false, error);
        }

        protected override void OnStart()
        {
            StartTime = DateTime.Now;
        }

        protected override void OnUpdate()
        {
            if (IsDone)
                return;
            if (!requestStarted)
            {
                // 等待调度器分配并发槽
                if (Scheduler == null || !Scheduler.TryAcquireSlot(this))
                    return;
                DispatchRequest();
                return;
            }
            var request = UnityWebRequest;
            if (request == null)
            {
                OnRequestFailed("UnityWebRequest is null !");
                return;
            }
            // 超时检测
            if (TimeoutSeconds > 0 && (DateTime.Now - StartTime).TotalSeconds > TimeoutSeconds)
            {
                IsTimeout = true;
                request.Abort();
                OnRequestFailed($"Request timeout : {URL} , elapsed {TimeoutSeconds}s");
                return;
            }
            Progress = Mathf.Max(request.downloadProgress, request.uploadProgress);
            OnProgressInternal?.Invoke(this, Progress);
            if (!request.isDone)
                return;
#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
#elif UNITY_2018_1_OR_NEWER
            if (!request.isNetworkError && !request.isHttpError)
#endif
            {
                OnRequestSucceeded(request);
                Finish(true, null);
            }
            else
            {
                OnRequestFailed($"Network error : {request.error}");
            }
        }

        protected override void OnAbort()
        {
            FinishCleanup();
        }

        protected override void OnFinish()
        {
            FinishCleanup();
        }

        void Finish(bool succeeded, string error)
        {
            if (IsDone)
                return;
            Error = error;
            Status = succeeded ? OperationStatus.Succeeded : OperationStatus.Failed;
            Progress = succeeded ? 1 : Progress;
        }

        /// <summary>
        /// 收尾：触发完成事件 -> 释放请求 -> 释放并发槽（仅执行一次）
        /// </summary>
        void FinishCleanup()
        {
            if (cleanupDone)
                return;
            cleanupDone = true;
            OnCompletedInternal?.Invoke(this);
            DisposeRequest();
            if (Scheduler != null)
            {
                Scheduler.RemoveWaiting(this);
                Scheduler.ReleaseSlot();
            }
        }

        /// <summary>
        /// 释放请求对象（完成后由框架调用）
        /// </summary>
        internal void DisposeRequest()
        {
            if (requestDisposed)
                return;
            requestDisposed = true;
            if (UnityWebRequest != null)
            {
                UnityWebRequest.Dispose();
                UnityWebRequest = null;
            }
        }
    }

    /// <summary>
    /// 泛型网络请求句柄。
    /// <para>用法：</para>
    /// <code>
    /// var handle = WebRequestManager.AddDownloadTextTaskAsync("http://.../config.json");
    /// handle.Completed += h => { var text = h.Result; }; // 或者 await handle.Task
    /// </code>
    /// </summary>
    public class WebRequestHandle<T> : WebRequestHandleBase
    {
        readonly Func<string, UnityWebRequest> requestFactory;
        readonly Func<UnityWebRequest, T> resultConverter;
        readonly TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
        Action<WebRequestHandle<T>> completed;
        bool callbackInvoked;
        T result;

        /// <summary>
        /// 请求结果（成功后有效）
        /// </summary>
        public T Result
        {
            get { return result; }
        }
        /// <summary>
        /// 完成回调事件
        /// </summary>
        public new event Action<WebRequestHandle<T>> Completed
        {
            add
            {
                if (IsDone)
                    value?.Invoke(this);
                else
                    completed += value;
            }
            remove { completed -= value; }
        }
        /// <summary>
        /// 异步任务，支持await
        /// </summary>
        public new Task<T> Task
        {
            get { return taskCompletionSource.Task; }
        }
        /// <summary>
        /// 同步等待请求完成（注意会阻塞主线程）
        /// </summary>
        public T WaitForCompletion()
        {
            int frame = 60000;
            while (!IsDone && frame-- > 0)
            {
                OperationSystem.Update();
            }
            return result;
        }

        internal WebRequestHandle(string url, Func<string, UnityWebRequest> requestFactory, Func<UnityWebRequest, T> resultConverter)
            : base(url)
        {
            this.requestFactory = requestFactory;
            this.resultConverter = resultConverter;
        }

        protected override void StartRequest()
        {
            var request = requestFactory?.Invoke(URL);
            UnityWebRequest = request;
            if (request == null)
            {
                OnRequestFailed($"Failed to create UnityWebRequest : {URL}");
                return;
            }
            request.SendWebRequest();
        }

        protected override void OnRequestSucceeded(UnityWebRequest request)
        {
            result = resultConverter != null ? resultConverter.Invoke(request) : default(T);
        }

        protected override void OnFinish()
        {
            FireCallbacks();
            base.OnFinish();
        }

        protected override void OnAbort()
        {
            FireCallbacks();
            base.OnAbort();
        }

        void FireCallbacks()
        {
            if (callbackInvoked)
                return;
            callbackInvoked = true;
            if (IsSucceeded)
                taskCompletionSource.TrySetResult(result);
            else
                taskCompletionSource.TrySetException(new InvalidOperationException(Error ?? "WebRequest failed !"));
            completed?.Invoke(this);
        }
    }
}
