using UnityEngine;
using System.Collections;
using Cosmos.FSM;
namespace Cosmos
{
    /// <summary>
    /// 有限状态机数据基类，三个生命周期，类似鱼ScriptableObject
    /// </summary>
    public abstract class FSMData
    {

        protected FSMBase stateMachine;
        public FSMBase StateMachine { get { return stateMachine; } }
        public abstract void OnInit();
        /// <summary>
        /// 状态机恢复
        /// </summary>
        public abstract void OnRenewal();
        /// <summary>
        /// 状态机终结
        /// </summary>
        public abstract void OnTermination();
    }
}