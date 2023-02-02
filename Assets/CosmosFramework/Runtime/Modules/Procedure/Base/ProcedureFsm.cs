using System;
using System.Collections.Generic;

namespace Cosmos.Procedure
{
    public class ProcedureFsm<T> where T : class
    {
        ProcedureFsmState<T> currentState;
        Dictionary<Type, ProcedureFsmState<T>> typeStateDict
            = new Dictionary<Type, ProcedureFsmState<T>>();
        Action<Type, Type> onProcedureChanged;
        /// <summary>
        /// ExitType===EnterType
        /// </summary>
        public event Action<Type, Type> OnProcedureChanged
        {
            add { onProcedureChanged += value; }
            remove { onProcedureChanged -= value; }
        }
        /// <summary>
        /// 状态机名；
        /// </summary>
        public string FsmName { get; private set; }
        /// <summary>
        /// 当前状态；
        /// </summary>
        public ProcedureFsmState<T> CurrentState { get { return currentState; } }
        /// <summary>
        /// 当前状态类型；
        /// </summary>
        public Type CurrentStateType { get { return currentState?.GetType(); } }
        /// <summary>
        /// 状态机持有者；
        /// </summary>
        public T Handle { get; private set; }
        /// <summary>
        /// 状态数量；
        /// </summary>
        public int StateCount { get { return typeStateDict.Count; } }
        /// <summary>
        /// 构造函数；
        /// </summary>
        /// <param name="handle">状态机持有者对象</param>
        /// <param name="fsmName">状态机名</param>
        public ProcedureFsm(T handle, string fsmName)
        {
            FsmName = fsmName;
            Handle = handle;
        }
        /// <summary>
        /// 构造函数；
        /// </summary>
        /// <param name="handle">状态机持有者对象</param>
        public ProcedureFsm(T handle) : this(handle, string.Empty) { }
        /// <summary>
        /// 设置默认状态；
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
        /// 添加一个状态；
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>添加结果</returns>
        public bool AddState(ProcedureFsmState<T> state)
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
        /// 添加一组状态；
        /// </summary>
        /// <param name="states">状态集合</param>
        public void AddStates(params ProcedureFsmState<T>[] states)
        {
            var length = states.Length;
            for (int i = 0; i < length; i++)
            {
                AddState(states[i]);
            }
        }
        /// <summary>
        /// 移除一个状态；
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
        /// 移除一组状态；
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
        /// 是否存在状态；
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <returns>存在结果</returns>
        public bool HasState(Type stateType)
        {
            return typeStateDict.ContainsKey(stateType);
        }
        /// <summary>
        /// 获取状态；
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <param name="state">获取的状态</param>
        /// <returns>获取结果</returns>
        public bool PeekState(Type stateType, out ProcedureFsmState<T> state)
        {
            return typeStateDict.TryGetValue(stateType, out state);
        }
        /// <summary>
        /// 轮询；
        /// </summary>
        public void Refresh()
        {
            currentState?.OnUpdate(this);
        }
        /// <summary>
        /// 切换状态；
        /// </summary>
        /// <param name="stateType">状态类型</param>
        public void ChangeState(Type stateType)
        {
            if (typeStateDict.TryGetValue(stateType, out var state))
            {
                if (state != null)
                {
                    currentState?.OnExit(this);
                    var exitedStateType = currentState == null ? null : currentState.GetType();
                    currentState = state;
                    currentState?.OnEnter(this);
                    var enteredStateType = currentState == null ? null : currentState.GetType();
                    onProcedureChanged?.Invoke(exitedStateType, enteredStateType);
                }
            }
        }
        /// <summary>
        /// 清理所有状态；
        /// </summary>
        public void ClearAllState()
        {
            foreach (var state in typeStateDict.Values)
            {
                state?.OnDestroy(this);
            }
            typeStateDict.Clear();
        }
    }
}
