using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Operation
{
    internal class OperationSystem
    {
        private static readonly List<OperationBase> operationList = new List<OperationBase>(1000);
        private static readonly List<OperationBase> newList = new List<OperationBase>(1000);
        /// <summary>
        /// 异步刷新驱动
        /// </summary>
        static OperationDriver asyncOperationDriver;
        public static void Update()
        {
            // 添加新增的异步操作
            if (newList.Count > 0)
            {
                bool sorting = false;
                foreach (var operation in newList)
                {
                    if (operation.Priority > 0)
                    {
                        sorting = true;
                        break;
                    }
                }

                operationList.AddRange(newList);
                newList.Clear();

                // 重新排序优先级
                if (sorting)
                    operationList.Sort();
            }
            // 更新进行中的异步操作
            for (int i = 0; i < operationList.Count; i++)
            {

                var operation = operationList[i];
                if (operation.IsFinish)
                    continue;

                if (operation.IsDone == false)
                    operation.InternalOnUpdate();

                if (operation.IsDone)
                    operation.SetFinish();
            }
            // 移除已经完成的异步操作
            for (int i = operationList.Count - 1; i >= 0; i--)
            {
                var operation = operationList[i];
                if (operation.IsFinish)
                    operationList.RemoveAt(i);
            }
        }
        /// <summary>
        /// 销毁异步操作系统
        /// </summary>
        public static void DestroyAll()
        {
            operationList.Clear();
            newList.Clear();
        }
        /// <summary>
        /// 开始处理异步操作类
        /// </summary>
        public static void StartOperation(OperationBase operation)
        {
            CheckAsyncOperationDriver();
            newList.Add(operation);
            operation.SetStart();

        }
        static void CheckAsyncOperationDriver()
        {
            if (asyncOperationDriver == null)
            {
                var go = new GameObject("AsyncOperationDriver ");
                go.hideFlags = HideFlags.HideInHierarchy;
                GameObject.DontDestroyOnLoad(go);
                asyncOperationDriver = go.AddComponent<OperationDriver>();
            }
        }
    }
}