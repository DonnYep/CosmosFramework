namespace Cosmos
{
    /// <summary>
    /// 模块的抽象基类，外部可扩展
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