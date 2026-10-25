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

        //================================================
        // 内置模块的初始化优先级：数值越小越先初始化（也越先挂载生命周期与轮询回调）。
        // 以 100 为步长留出空隙，方便用户模块用中间值插队；
        // 用户模块未声明优先级时取 ModuleAttribute.DefaultPriority（10000），排在全部内置模块之后。
        //================================================
        public const int PRIORITY_EVENT = 100;
        public const int PRIORITY_RESOURCE = 200;
        public const int PRIORITY_WEB_REQUEST = 300;
        public const int PRIORITY_DATA_TABLE = 400;
        public const int PRIORITY_OBJECT_POOL = 500;
        public const int PRIORITY_ENTITY = 600;
        public const int PRIORITY_UI = 700;
        public const int PRIORITY_AUDIO = 800;
        public const int PRIORITY_INPUT = 900;
        public const int PRIORITY_FSM = 1000;
        public const int PRIORITY_PROCEDURE = 1100;
        public const int PRIORITY_NETWORK = 1200;
        public const int PRIORITY_DOWNLOAD = 1300;
    }
}
