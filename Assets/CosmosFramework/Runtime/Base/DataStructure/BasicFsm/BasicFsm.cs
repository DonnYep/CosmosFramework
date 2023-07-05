using System;
using System.Collections.Generic;
namespace Cosmos
{
    public class BasicFsm<T>
         where T : class
    {
        Type stateBaseType = typeof(BasicFsmState<T>);
        BasicFsmState<T> currentState;
        T owner;
        public T Owner
        {
            get { return owner; }
            private set { owner = value; }
        }
        string name;
        public string Name
        {
            get { return name; }
            set { name = value ?? string.Empty; }
        }
        Dictionary<Type, BasicFsmState<T>> fsmStateDict
            = new Dictionary<Type, BasicFsmState<T>>();
        public int FsmStateCount { get { return fsmStateDict.Count; } }
        public bool IsRunning { get { return currentState != null; } }
        public bool Pause { get; set; }
        public BasicFsm(T owner) : this(owner, string.Empty) { }
        public BasicFsm(T owner, string name)
        {
            this.owner = owner;
            this.name = name;
        }
        public void Update()
        {
            if (Pause)
                return;
            currentState?.UpdateTransition(this);
            currentState?.OnLogic(this);
        }
        public void Shutdown()
        {
            if (currentState != null)
            {
                currentState.OnExit(this);
            }
            foreach (var state in fsmStateDict)
            {
                state.Value.OnDestroy(this);
            }
            fsmStateDict.Clear();
            currentState = null;
        }
        public bool HasState(Type stateType)
        {
            if (stateType == null)
                throw new ArgumentNullException("State type is invalid !");
            if (!stateBaseType.IsAssignableFrom(stateType))
                throw new ArgumentException($"State type {stateType.FullName} is invalid !");
            return fsmStateDict.ContainsKey(stateType);
        }
        public bool HasState(BasicFsmState<T> state)
        {
            return HasState(state.GetType());
        }
        public void ChangeState<TState>()
    where TState : BasicFsmState<T>
        {
            ChangeState(typeof(TState));
        }
        public void ChangeState(Type stateType)
        {
            BasicFsmState<T> state = null;
            if (!stateBaseType.IsAssignableFrom(stateType))
                throw new ArgumentException("State type is invalid" + stateType.FullName);
            state = GetState(stateType);
            if (state == null)
                throw new ArgumentNullException($"FSM{currentState} can not change state to {state} which is not exist");
            currentState?.OnLogic(this);
            currentState = state;
            currentState?.OnLogic(this);
        }
        public TState GetState<TState>() where TState : BasicFsmState<T>
        {
            BasicFsmState<T> state = null;
            if (fsmStateDict.TryGetValue(typeof(TState), out state))
                return (TState)state;
            return null;
        }
        public BasicFsmState<T> GetState(Type stateType)
        {
            if (stateType == null)
                throw new ArgumentNullException("State type is invaild !");
            if (!stateBaseType.IsAssignableFrom(stateType))
                throw new ArgumentNullException($"State type {stateType.FullName} is invaild !");
            BasicFsmState<T> state = null;
            if (fsmStateDict.TryGetValue(stateType, out state))
                return state;
            return null;
        }
        public void AddStates(params BasicFsmState<T>[] states)
        {
            if (states == null || states.Length < 1)
                throw new ArgumentNullException("FSM owner is invalid");
            var stateLength = states.Length;
            for (int i = 0; i < stateLength; i++)
            {
                if (states[i] == null)
                    throw new ArgumentNullException("FSM owner is invalid");
                Type type = states[i].GetType();
                if (HasState(type))
                    throw new ArgumentException($"FSM state {states[i].GetType().FullName} is is already exist");
                else
                {
                    states[i].OnInit(this);
                    fsmStateDict.Add(type, states[i]);
                }
            }
        }
        public bool Removetate<TState>()
    where TState : BasicFsmState<T>
        {
            return Removetate(typeof(TState));
        }
        public bool Removetate(Type stateType)
        {
            if (stateType == null)
                throw new ArgumentNullException("State type is invaild !");
            if (!stateBaseType.IsAssignableFrom(stateType))
                throw new ArgumentNullException($"State type {stateType.FullName} is invaild !");
            if (currentState != null)
            {
                if (currentState.GetType() == stateType)
                {
                    currentState?.OnExit(this);
                    currentState = null;
                }
            }
            return fsmStateDict.Remove(stateType);
        }
    }
}
