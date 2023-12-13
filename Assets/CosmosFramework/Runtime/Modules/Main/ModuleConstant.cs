namespace Cosmos
{
    internal class ModuleConstant
    {
        /// <summary>
        /// 框架生命周期，等价于Awake
        /// </summary>
        public const string INITIALIZATION = "OnInitialization";
        /// <summary>
        /// 框架生命周期，等价于OnEnable
        /// </summary>
        public const string ACTIVE = "OnActive";
        /// <summary>
        /// 框架生命周期，等价于Start
        /// </summary>
        public const string PREPARATORY = "OnPreparatory";
        /// <summary>
        /// 框架生命周期，等价OnDisable
        /// </summary>
        public const string DEACTIVE = "OnDeactive";
        /// <summary>
        /// 框架生命周期，等价于OnDestroy
        /// </summary>
        public const string TERMINATION = "OnTermination";
    }
}
