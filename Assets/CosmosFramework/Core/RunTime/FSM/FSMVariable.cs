using System.Collections;
using Cosmos.FSM;
namespace Cosmos
{
    /// <summary>
    /// 有限状态机数据抽象基类；
    /// </summary>
    public abstract class FSMVariable : Variable
    {
        protected FSMBase stateMachine;
        public FSMBase StateMachine { get { return stateMachine; } protected set{ stateMachine = value; } }
    }
}