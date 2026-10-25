namespace Cosmos
{
    /// <summary>
    /// 模块的抽象基类，外部可扩展
    /// <para>生命周期由 <see cref="GameManager"/> 驱动；这里提供 internal 的直调入口，
    /// 这样框架侧不需要用反射 <c>InvokeMethod</c> 去调用 protected 的虚方法（虚分派语义不变）。</para>
    /// </summary>
    public abstract class Module
    {
        public bool Pause { get; protected set; }
        #region Methods
        internal void Update()
        {
            if (Pause)
                return;
            OnUpdate();
        }
        internal void LateUpdate()
        {
            if (Pause)
                return;
            OnLateUpdate();
        }
        internal void FixedUpdate()
        {
            if (Pause)
                return;
            OnFixedUpdate();
        }
        internal void ElapseUpdate(float realDeltaTime)
        {
            if (Pause)
                return;
            OnElapseUpdate(realDeltaTime);
        }
        /// <summary>
        /// 生命周期：初始化（等价 Awake）
        /// </summary>
        internal void InvokeInitialization()
        {
            OnInitialization();
        }
        /// <summary>
        /// 生命周期：激活（等价 OnEnable）
        /// </summary>
        internal void InvokeActive()
        {
            OnActive();
        }
        /// <summary>
        /// 生命周期：准备（等价 Start）
        /// </summary>
        internal void InvokePreparatory()
        {
            OnPreparatory();
        }
        /// <summary>
        /// 生命周期：失活（等价 OnDisable）
        /// </summary>
        internal void InvokeDeactive()
        {
            OnDeactive();
        }
        /// <summary>
        /// 生命周期：终止（等价 OnDestroy）
        /// </summary>
        internal void InvokeTermination()
        {
            OnTermination();
        }
        protected virtual void OnInitialization() { }
        protected virtual void OnActive() { }
        protected virtual void OnPreparatory() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnElapseUpdate(float realDeltaTime) { }
        protected virtual void OnLateUpdate() { }
        protected virtual void OnDeactive() { }
        protected virtual void OnTermination() { }
        #endregion
    }
}
