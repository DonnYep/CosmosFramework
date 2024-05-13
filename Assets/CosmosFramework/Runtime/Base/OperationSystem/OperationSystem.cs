using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    internal class OperationSystem
    {
        private static readonly List<AsyncOperationBase> operationList = new List<AsyncOperationBase>(1000);
        private static readonly List<AsyncOperationBase> newList = new List<AsyncOperationBase>(1000);
        static AsyncOperationDriver asyncOperationDriver;
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
                    operation.OnUpdate();

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
        public static void StartOperation(AsyncOperationBase operation)
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
                asyncOperationDriver = go.AddComponent<AsyncOperationDriver>();
            }
        }
    }
}