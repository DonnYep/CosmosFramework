using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Data
{
    internal sealed partial class DataManager
    {
        /// <summary>
        /// 数据结点。
        /// </summary>
        private class DataNode : IDataNode, IReference
        {
            private static readonly DataNode[] EmptyDataNodeArray = new DataNode[] { };
            private string nodeName;
            private Variable nodeData;
            private DataNode nodeParent;
            private List<DataNode> nodeChilds;

            public DataNode()
            {
                nodeName = null;
                nodeData = null;
                nodeParent = null;
                nodeChilds = null;
            }

            /// <summary>
            /// 创建数据结点。
            /// </summary>
            /// <param name="name">数据结点名称。</param>
            /// <param name="parent">父数据结点。</param>
            /// <returns>创建的数据结点。</returns>
            public static DataNode Create(string name, DataNode parent)
            {
                if (!IsValidName(name))
                {
                    throw new ArgumentException("Name of data node is invalid.");
                }
                DataNode node = ReferencePool.Accquire<DataNode>();
                node.nodeName = name;
                node.nodeParent = parent;
                return node;
            }
            /// <summary>
            /// 获取数据结点的名称。
            /// </summary>
            public string Name { get { return nodeName; } }
            /// <summary>
            /// 获取数据结点的完整名称。
            /// </summary>
            public string FullName
            {
                get
                {
                    return nodeParent == null ? nodeName : $"{nodeParent.FullName}{PathSplitSeparator[0]}{nodeName}";
                }
            }
            /// <summary>
            /// 获取父数据结点。
            /// </summary>
            public IDataNode Parent
            {
                get
                {
                    return nodeParent;
                }
            }
            /// <summary>
            /// 获取子数据结点的数量。
            /// </summary>
            public int ChildCount
            {
                get
                {
                    return nodeChilds != null ? nodeChilds.Count : 0;
                }
            }
            /// <summary>
            /// 根据类型获取数据结点的数据。
            /// </summary>
            /// <typeparam name="T">要获取的数据类型。</typeparam>
            /// <returns>指定类型的数据。</returns>
            public T GetData<T>() where T : Variable
            {
                return (T)nodeData;
            }
            /// <summary>
            /// 获取数据结点的数据。
            /// </summary>
            /// <returns>数据结点数据。</returns>
            public Variable GetData()
            {
                return nodeData;
            }
            /// <summary>
            /// 设置数据结点的数据。
            /// </summary>
            /// <typeparam name="T">要设置的数据类型。</typeparam>
            /// <param name="data">要设置的数据。</param>
            public void SetData<T>(T data) where T : Variable
            {
                SetData((Variable)data);
            }
            /// <summary>
            /// 设置数据结点的数据。
            /// </summary>
            /// <param name="data">要设置的数据。</param>
            public void SetData(Variable data)
            {
                if (nodeData != null)
                {
                    ReferencePool.Release(nodeData);
                }
                nodeData = data;
            }
            /// <summary>
            /// 根据索引检查是否存在子数据结点。
            /// </summary>
            /// <param name="index">子数据结点的索引。</param>
            /// <returns>是否存在子数据结点。</returns>
            public bool HasChild(int index)
            {
                return index >= 0 && index < ChildCount;
            }

            /// <summary>
            /// 根据名称检查是否存在子数据结点。
            /// </summary>
            /// <param name="name">子数据结点名称。</param>
            /// <returns>是否存在子数据结点。</returns>
            public bool HasChild(string name)
            {
                if (!IsValidName(name))
                {
                    throw new ArgumentException("Name is invalid.");
                }

                if (nodeChilds == null)
                {
                    return false;
                }

                foreach (DataNode child in nodeChilds)
                {
                    if (child.Name == name)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// 根据索引获取子数据结点。
            /// </summary>
            /// <param name="index">子数据结点的索引。</param>
            /// <returns>指定索引的子数据结点，如果索引越界，则返回空。</returns>
            public IDataNode GetChild(int index)
            {
                return index >= 0 && index < ChildCount ? nodeChilds[index] : null;
            }

            /// <summary>
            /// 根据名称获取子数据结点。
            /// </summary>
            /// <param name="name">子数据结点名称。</param>
            /// <returns>指定名称的子数据结点，如果没有找到，则返回空。</returns>
            public IDataNode GetChild(string name)
            {
                if (!IsValidName(name))
                {
                    throw new ArgumentException("Name is invalid.");
                }

                if (nodeChilds == null)
                {
                    return null;
                }

                foreach (DataNode child in nodeChilds)
                {
                    if (child.Name == name)
                    {
                        return child;
                    }
                }
                return null;
            }
            /// <summary>
            /// 根据名称获取或增加子数据结点。
            /// </summary>
            /// <param name="name">子数据结点名称。</param>
            /// <returns>指定名称的子数据结点，如果对应名称的子数据结点已存在，则返回已存在的子数据结点，否则增加子数据结点。</returns>
            public IDataNode GetOrAddChild(string name)
            {
                DataNode node = (DataNode)GetChild(name);
                if (node != null)
                {
                    return node;
                }
                node = Create(name, this);
                if (nodeChilds == null)
                {
                    nodeChilds = new List<DataNode>();
                }
                nodeChilds.Add(node);
                return node;
            }
            /// <summary>
            /// 获取所有子数据结点。
            /// </summary>
            /// <returns>所有子数据结点。</returns>
            public IDataNode[] GetAllChild()
            {
                if (nodeChilds == null)
                {
                    return EmptyDataNodeArray;
                }

                return nodeChilds.ToArray();
            }

            /// <summary>
            /// 获取所有子数据结点。
            /// </summary>
            /// <param name="results">所有子数据结点。</param>
            public void GetAllChild(List<IDataNode> results)
            {
                if (results == null)
                {
                    throw new ArgumentException("Results is invalid.");
                }

                results.Clear();
                if (nodeChilds == null)
                {
                    return;
                }

                foreach (DataNode child in nodeChilds)
                {
                    results.Add(child);
                }
            }

            /// <summary>
            /// 根据索引移除子数据结点。
            /// </summary>
            /// <param name="index">子数据结点的索引位置。</param>
            public void RemoveChild(int index)
            {
                DataNode node = (DataNode)GetChild(index);
                if (node == null)
                {
                    return;
                }
                nodeChilds.Remove(node);
                ReferencePool.Release(node);
            }
            /// <summary>
            /// 根据名称移除子数据结点。
            /// </summary>
            /// <param name="name">子数据结点名称。</param>
            public void RemoveChild(string name)
            {
                DataNode node = (DataNode)GetChild(name);
                if (node == null)
                {
                    return;
                }
                nodeChilds.Remove(node);
                ReferencePool.Release(node);
            }
            public void ClearNode()
            {
                if (nodeData != null)
                {
                    ReferencePool.Release(nodeData);
                    nodeData = null;
                }

                if (nodeChilds != null)
                {
                    foreach (DataNode child in nodeChilds)
                    {
                        ReferencePool.Release(child);
                    }

                    nodeChilds.Clear();
                }
            }
            /// <summary>
            /// 获取数据结点字符串。
            /// </summary>
            /// <returns>数据结点字符串。</returns>
            public override string ToString()
            {
                return Utility.Text.Format("{0}: {1}", FullName, ToDataString());
            }
            /// <summary>
            /// 获取数据字符串。
            /// </summary>
            /// <returns>数据字符串。</returns>
            public string ToDataString()
            {
                if (nodeData == null)
                {
                    return "<Null>";
                }

                return Utility.Text.Format("[{0}] {1}", nodeData.Type.Name, nodeData.ToString());
            }
            public void Clear()
            {
                nodeName = null;
                nodeParent = null;
                ClearNode();
            }
            /// <summary>
            /// 检测数据结点名称是否合法。
            /// </summary>
            /// <param name="name">要检测的数据结点名称。</param>
            /// <returns>是否是合法的数据结点名称。</returns>
            static bool IsValidName(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return false;
                }

                foreach (string pathSplitSeparator in PathSplitSeparator)
                {
                    if (name.Contains(pathSplitSeparator))
                    {
                        return false;
                    }
                }

                return true;
            }


        }
    }
}