using System;
using System.Collections.Generic;
namespace Cosmos
{
    /// <summary>
    /// 简易的抽象有限状态机。
    /// 此状态机是精简只含核心逻辑的状态机抽象。
    /// </summary>
    public class SimpleFsm<T> where T : class
    {
        SimpleFsmState<T> currentState;
        Dictionary<Type, SimpleFsmState<T>> typeStateDict
            = new Dictionary<Type, SimpleFsmState<T>>();
        Action<Type, Type> onStateChange;
        /// <summary>
        /// 状态切换事件，args=( previouseType , currentType )
        /// </summary>
        public event Action<Type, Type> OnStateChange
        {
            add { onStateChange += value; }
            remove { onStateChange -= value; }
        }
        /// <summary>
        /// 状态机名
        /// </summary>
        public string FsmName { get; private set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public SimpleFsmState<T> CurrentState { get { return currentState; } }
        /// <summary>
        /// 当前状态类型
        /// </summary>
        public Type CurrentStateType { get { return currentState?.GetType(); } }
        /// <summary>
        /// 状态机持有者
        /// </summary>
        public T Handle { get; private set; }
        /// <summary>
        /// 状态数量；
        /// </summary>
        public int StateCount { get { return typeStateDict.Count; } }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="handle">状态机持有者对象</param>
        /// <param name="fsmName">状态机名</param>
        public SimpleFsm(T handle, string fsmName)
        {
            FsmName = fsmName;
            Handle = handle;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="handle">状态机持有者对象</param>
        public SimpleFsm(T handle) : this(handle, string.Empty) { }
        /// <summary>
        /// 设置默认状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        public void SetDefaultState(Type stateType)
        {
            if (typeStateDict.TryGetValue(stateType, out var state))
            {
                currentState = state;
                currentState?.OnEnter(this);
            }
        }
        /// <summary>
        /// 添加一个状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>添加结果</returns>
        public bool AddState(SimpleFsmState<T> state)
        {
            var type = state.GetType();
            if (!typeStateDict.ContainsKey(type))
            {
                typeStateDict.Add(type, state);
                state?.OnInit(this);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 添加一组状态
        /// </summary>
        /// <param name="states">状态集合</param>
        public void AddStates(params SimpleFsmState<T>[] states)
        {
            var length = states.Length;
            for (int i = 0; i < length; i++)
            {
                AddState(states[i]);
            }
        }
        /// <summary>
        /// 移除一个状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <returns>移除结果</returns>
        public bool RemoveState(Type stateType)
        {
            if (typeStateDict.ContainsKey(stateType))
            {
                var state = typeStateDict[stateType];
                typeStateDict.Remove(stateType);
                state?.OnDestroy(this);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 移除一组状态
        /// </summary>
        /// <param name="stateTypes">状态类型集合</param>
        public void RemoveStates(params Type[] stateTypes)
        {
            var typeLength = stateTypes.Length;
            for (int i = 0; i < typeLength; i++)
            {
                RemoveState(stateTypes[i]);
            }
        }
        /// <summary>
        /// 是否存在状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <returns>存在结果</returns>
        public bool HasState(Type stateType)
        {
            return typeStateDict.ContainsKey(stateType);
        }
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="TState">状态类型</typeparam>
        /// <returns>获得的状态</returns>
        public TState GetState<TState>()
            where TState : SimpleFsmState<T>
        {
            var stateType = typeof(TState);
            var has = typeStateDict.TryGetValue(stateType, out var state);
            if (has)
                return (TState)state;
            return null;
        }
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <returns>获得的状态</returns>
        public SimpleFsmState<T> GetState(Type stateType)
        {
            var has = typeStateDict.TryGetValue(stateType, out var state);
            if (has)
                return state;
            return null;
        }
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <param name="state">获取的状态</param>
        /// <returns>获取结果</returns>
        public bool PeekState(Type stateType, out SimpleFsmState<T> state)
        {
            return typeStateDict.TryGetValue(stateType, out state);
        }
        /// <summary>
        /// 轮询
        /// </summary>
        public void Refresh()
        {
            currentState?.OnUpdate(this);
        }
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="TState">状态类型</typeparam>
        public void ChangeState<TState>()
        {
            ChangeState(typeof(TState));
        }
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        public void ChangeState(Type stateType)
        {
            if (typeStateDict.TryGetValue(stateType, out var state))
            {
                if (state != null)
                {
                    var previousType = currentState?.GetType();
                    var currentType = state?.GetType();
                    currentState?.OnExit(this);
                    currentState = state;
                    currentState?.OnEnter(this);
                    StateChangeHandler(previousType, currentType);
                }
            }
        }
        /// <summary>
        /// 清理所有状态
        /// </summary>
        public void ClearAllState()
        {
            currentState?.OnExit(this);
            foreach (var state in typeStateDict.Values)
            {
                state?.OnDestroy(this);
            }
            typeStateDict.Clear();
        }
        /// <summary>
        /// 状态切换处理函数
        /// </summary>
        /// <param name="previousType">先前的状态</param>
        /// <param name="currentType">现在的状态</param>
        void StateChangeHandler(Type previousType, Type currentType)
        {
            onStateChange?.Invoke(previousType, currentType);
        }
    }
}