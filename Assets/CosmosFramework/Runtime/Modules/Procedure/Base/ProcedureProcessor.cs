using System;
using System.Collections.Generic;

namespace Cosmos.Procedure
{
    public class ProcedureProcessor<T> where T : class
    {
        ProcedureNodeBase<T> currentNode;
        Dictionary<Type, ProcedureNodeBase<T>> typeNodeDict
            = new Dictionary<Type, ProcedureNodeBase<T>>();
        Action<Type, Type> procedureNodeChanged;
        /// <summary>
        /// ExitType===EnterType
        /// </summary>
        public event Action<Type, Type> ProcedureNodeChanged
        {
            add { procedureNodeChanged += value; }
            remove { procedureNodeChanged -= value; }
        }
        /// <summary>
        /// 当前状态；
        /// </summary>
        public ProcedureNodeBase<T> CurrentNode{ get { return currentNode; } }
        /// <summary>
        /// 状态机持有者；
        /// </summary>
        public T Handle { get; private set; }
        /// <summary>
        /// 状态数量；
        /// </summary>
        public int NodeCount { get { return typeNodeDict.Count; } }
        /// <summary>
        /// 构造函数；
        /// </summary>
        /// <param name="handle">状态机持有者对象</param>
        public ProcedureProcessor(T handle)
        {
            Handle = handle;
        }
        /// <summary>
        /// 添加一个状态；
        /// </summary>
        /// <param name="node">状态</param>
        /// <returns>添加结果</returns>
        public bool AddNode(ProcedureNodeBase<T> node)
        {
            var type = node.GetType();
            if (!typeNodeDict.ContainsKey(type))
            {
                typeNodeDict.Add(type, node);
                node?.OnInit(this);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 添加一组状态；
        /// </summary>
        /// <param name="nodes">状态集合</param>
        public void AddNodes(params ProcedureNodeBase<T>[] nodes)
        {
            var length = nodes.Length;
            for (int i = 0; i < length; i++)
            {
                AddNode(nodes[i]);
            }
        }
        /// <summary>
        /// 移除一个状态；
        /// </summary>
        /// <param name="nodeType">状态类型</param>
        /// <returns>移除结果</returns>
        public bool RemoveNode(Type nodeType)
        {
            if (typeNodeDict.ContainsKey(nodeType))
            {
                var state = typeNodeDict[nodeType];
                typeNodeDict.Remove(nodeType);
                state?.OnDestroy(this);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否存在状态；
        /// </summary>
        /// <param name="nodeType">状态类型</param>
        /// <returns>存在结果</returns>
        public bool HasNode(Type nodeType)
        {
            return typeNodeDict.ContainsKey(nodeType);
        }
        /// <summary>
        /// 获取状态；
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <param name="node">获取的状态</param>
        /// <returns>获取结果</returns>
        public bool PeekNode(Type stateType, out ProcedureNodeBase<T> node)
        {
            return typeNodeDict.TryGetValue(stateType, out node);
        }
        /// <summary>
        /// 轮询；
        /// </summary>
        public void Refresh()
        {
            currentNode?.OnUpdate(this);
        }
        /// <summary>
        /// 切换状态；
        /// </summary>
        /// <param name="nodeType">状态类型</param>
        public void ChangeNode(Type nodeType)
        {
            if (typeNodeDict.TryGetValue(nodeType, out var state))
            {
                if (state != null)
                {
                    currentNode?.OnExit(this);
                    var exitedNodeType = currentNode == null ? null : currentNode.GetType();
                    currentNode = state;
                    currentNode?.OnEnter(this);
                    var enteredNodeType = currentNode == null ? null : currentNode.GetType();
                    procedureNodeChanged?.Invoke(exitedNodeType, enteredNodeType);
                }
            }
        }
        /// <summary>
        /// 清理所有状态；
        /// </summary>
        public void ClearAllNode()
        {
            currentNode?.OnExit(this);
            foreach (var state in typeNodeDict.Values)
            {
                state?.OnDestroy(this);
            }
            typeNodeDict.Clear();
        }
    }
}
