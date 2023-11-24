using System.Collections.Generic;
using System;
namespace Cosmos.FSM
{
    //type.ToString()输出一个完全限定名，尝试使用反射机制获得对象
    internal sealed class FSM<T> : FSMBase, IFSM<T>, IReference
         where T : class
    {
        #region Properties
        Type stateBaseType = typeof(FSMState<T>);
        T owner;
        FSMState<T> currentState;
        bool isDestoryed;
        FSMState<T> defaultState;
        Action<FSMState<T>, FSMState<T>> onStateChange;
        public T Owner
        {
            get { return owner; }
            private set { owner = value; }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public FSMState<T> CurrentState { get { return currentState; } }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsDestoryed
        {
            get
            {
                return isDestoryed;
            }
            private set
            {
                isDestoryed = value;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public FSMState<T> DefaultState
        {
            get { return defaultState; }
            set { defaultState = value; }
        }
        /// <summary>
        /// state存储的类型为派生类
        /// </summary>
        readonly Dictionary<Type, FSMState<T>> fsmStateDict = new Dictionary<Type, FSMState<T>>();
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override int FSMStateCount
        {
            get { return fsmStateDict.Count; }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool IsRunning
        {
            get { return currentState != null; }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override Type OwnerType
        {
            get { return typeof(T); }
        }
        public override string CurrentStateName
        {
            get
            {
                return currentState != null ? currentState.GetType().FullName : Constants.NULL;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<FSMState<T>, FSMState<T>> OnStateChange
        {
            add { onStateChange += value; }
            remove { onStateChange -= value; }
        }
        #endregion

        #region Lifecycle
        public void Release()
        {
            if (currentState != null)
            {
                currentState.OnStateExit(this);
            }
            foreach (var state in fsmStateDict)
            {
                state.Value.OnTermination(this);
            }
            Name = null;
            IsDestoryed = true;
            fsmStateDict.Clear();
            currentState = null;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ChangeToDefaultState()
        {
            if (defaultState == null)
                return;
            Type type = defaultState.GetType();
            if (!stateBaseType.IsAssignableFrom(type))
                throw new ArgumentException($"State type {type.FullName} is invalid");
            var hasState = PeekState(type, out var state);
            if (!hasState)
                return;
            currentState = state;
            currentState.OnStateEnter(this);
            ChangeStateHandler(null, currentState);
        }
        /// <summary>
        /// FSM轮询，由拥有者轮询调用
        /// </summary>
        public override void OnRefresh()
        {
            if (Pause)
                return;
            currentState?.RefreshTransition(this);
            currentState?.OnStateStay(this);
        }
        public override void Shutdown()
        {
            ReferencePool.Release(this);
        }
        #endregion

        #region State
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool HasState(Type stateType)
        {
            if (stateType == null)
                throw new ArgumentNullException("State type is invalid !");
            if (!stateBaseType.IsAssignableFrom(stateType))
                throw new ArgumentException($"State type {stateType.FullName} is invalid !");
            return fsmStateDict.ContainsKey(stateType);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool HasState<TState>()
            where TState : FSMState<T>
        {
            return HasState(typeof(TState));
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ChangeState<TState>()
            where TState : FSMState<T>
        {
            ChangeState(typeof(TState));
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ChangeState(Type stateType)
        {
            FSMState<T> state = null;
            if (!stateBaseType.IsAssignableFrom(stateType))
                throw new ArgumentException($"State type {stateType.FullName} is invalid");
            var hasState = PeekState(stateType, out state);
            if (!hasState)
                return;
            var previouseState = currentState;
            var nextState = state;

            currentState?.OnStateExit(this);
            currentState = state;
            currentState?.OnStateEnter(this);

            ChangeStateHandler(previouseState, nextState);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void GetAllState(out List<FSMState<T>> result)
        {
            result = new List<FSMState<T>>();
            foreach (var state in fsmStateDict)
            {
                result.Add(state.Value);
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool PeekState(Type stateType, out FSMState<T> state)
        {
            if (stateType == null)
                throw new ArgumentNullException("State type is invaild !");
            if (!stateBaseType.IsAssignableFrom(stateType))
                throw new ArgumentNullException($"State type {stateType.FullName} is invaild !");
            state = null;
            if (fsmStateDict.TryGetValue(stateType, out state))
                return true;
            return false;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool PeekState<TState>(out TState state) where TState : FSMState<T>
        {
            state = null;
            var type = typeof(TState);
            if (fsmStateDict.TryGetValue(type, out var srcSate))
            {
                state = (TState)srcSate;
                return true;
            }
            return false;
        }
        public FSMState<T>[] GetAllState()
        {
            List<FSMState<T>> states = new List<FSMState<T>>();
            foreach (var state in fsmStateDict)
            {
                states.Add(state.Value);
            }
            return states.ToArray();
        }
        #endregion

        internal static FSM<T> Create(string name, T owner, params FSMState<T>[] states)
        {
            if (states == null || states.Length < 1)
                throw new ArgumentNullException("FSM owner is invalid");
            //从引用池获得同类
            FSM<T> fsm = ReferencePool.Acquire<FSM<T>>();
            fsm.Name = name;
            fsm.Owner = owner;
            fsm.IsDestoryed = false;
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i] == null)
                    throw new ArgumentNullException("FSM owner is invalid");
                Type type = states[i].GetType();
                if (fsm.HasState(type))
                    throw new ArgumentException($"FSM state {states[i].GetType().FullName} is is already exist");
                else
                {
                    states[i].OnInitialization(fsm);
                    fsm.fsmStateDict.Add(type, states[i]);
                }
            }
            return fsm;
        }
        internal static FSM<T> Create(string name, T owner, IList<FSMState<T>> states)
        {
            if (states == null || states.Count < 1)
                throw new ArgumentNullException("FSM owner is invalid");
            //从引用池获得同类
            FSM<T> fsm = ReferencePool.Acquire<FSM<T>>();
            fsm.Name = name;
            fsm.Owner = owner;
            fsm.IsDestoryed = false;
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i] == null)
                    throw new ArgumentNullException("FSM owner is invalid");
                Type type = states[i].GetType();
                if (fsm.HasState(type))
                    throw new ArgumentException($"FSM state {states[i].GetType()} is is already exist");
                else
                {
                    states[i].OnInitialization(fsm);
                    fsm.fsmStateDict.Add(type, states[i]);
                }
            }
            return fsm;
        }

        void ChangeStateHandler(FSMState<T> previouseState, FSMState<T> currentState)
        {
            onStateChange?.Invoke(previouseState, currentState);
        }
    }
}