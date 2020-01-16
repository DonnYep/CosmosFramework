using UnityEngine;
using System.Collections;
using System;
using Cosmos.Event;
namespace Cosmos.Mono
{
    /// <summary>
    /// 继承自mono，运行update，用来更新
    /// </summary>
    [DisallowMultipleComponent]
    public class MonoController : MonoBehaviour,IMonoController
    {
        /// <summary>
        /// 当前mono的ID
        /// </summary>
        short monoID = -1;
        public short MonoID { get { return monoID; } set { monoID = value; } }
        /// <summary>
        /// update的委托数量
        /// </summary>
        short updateCount = 0;
        /// <summary>
        /// fixedupdate 的委托数量
        /// </summary>
        short fixedUpdateCount = 0;
        /// <summary>
        /// lateUpdate的委托数量
        /// </summary>
        short lateUpdateCount = 0;
        /// <summary>
        /// 是否当前对象的所有action都空了
        /// </summary>
        public bool IsAllActionEmpty {get{return (updateCount == 0 && lateUpdateCount == 0 && fixedUpdateCount == 0);}}
        event CFAction updateActions;
        event CFAction fixedUpdateActions;
        event CFAction lateUpdateActions;
        // Update is called once per frame
        void Update()
        {
            if(updateActions!=null)
                updateActions();
        }
        private void FixedUpdate()
        {
            if (fixedUpdateActions != null)
                fixedUpdateActions();
        }
        private void LateUpdate()
        {
            if (lateUpdateActions != null)
                lateUpdateActions();
        }
        ContainerState CheckActionsState(short currentSize,short capacity)
        {
            if (currentSize >= capacity)
            {
                return ContainerState.Full;
            }
            else
            {
                if (currentSize <= 0)
                {
                    //Utility.DebugLog( " MonoID     "+monoID+" action empty! ");
                    return ContainerState.Empty;
                }
                else
                {
                    return ContainerState.Hold;
                }
               
            }
        }
        /// <summary>
        /// 输入对应的type，检测自身对象是否符合，是则返回自身的ID，否则返回-1
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public short UseableMono(UpdateType type)
        {
            short id = -1;
            switch (type)
            {
                //当返回ID为0，即hold ——有空间，但是不满
                case UpdateType.FixedUpdate:
                if (((short)CheckActionsState(fixedUpdateCount, MonoManager._FixedUpdateCapacity)) != 1)
                    id = monoID;
                    return id;
                    break;
                case UpdateType.LateUpdate:
                    if (((short)CheckActionsState(lateUpdateCount, MonoManager._LateUpdateCapacity)) != 1)
                        id = monoID;
                    return id;
                    break;
                case UpdateType.Update:
                    if (((short)CheckActionsState(updateCount,MonoManager._UpdateCapacity)) != 1)
                        id = monoID;
                    return id;
                    break;
            }
            return id;
        }
        public void AddListener(CFAction act, UpdateType type)
        {
            switch (type)
            {
                case UpdateType.FixedUpdate:
                    if (((short)CheckActionsState(fixedUpdateCount, MonoManager._FixedUpdateCapacity))==1)
                        return;
                    fixedUpdateActions += act;
                    ++fixedUpdateCount;
                    break;
                case UpdateType.Update:
                    if (((short)CheckActionsState(updateCount, MonoManager._UpdateCapacity))== 1)
                        return;
                    updateActions += act;
                    ++updateCount;
                    break;
                case UpdateType.LateUpdate:
                    if (((short)CheckActionsState(lateUpdateCount, MonoManager._LateUpdateCapacity)) == 1)
                        return;
                    lateUpdateActions += act;
                   ++ lateUpdateCount;
                    break;
            }
        }
        public void RemoveListener(CFAction act, UpdateType type)
        {
            switch (type)
            {
                case UpdateType.FixedUpdate:
                    if (((short)CheckActionsState(fixedUpdateCount, MonoManager._FixedUpdateCapacity)) != -1)
                        return;
                    fixedUpdateActions -= act;
                    --fixedUpdateCount;
                    break;
                case UpdateType.Update:
                    if (((short)CheckActionsState(updateCount, MonoManager._UpdateCapacity)) != -1)
                        return;
                    updateActions -= act;
                    --updateCount;
                    break;
                case UpdateType.LateUpdate:
                    if (((short)CheckActionsState(lateUpdateCount, MonoManager._LateUpdateCapacity)) != -1)
                        return;
                    lateUpdateActions -= act;
                   -- lateUpdateCount;
                    break;
            }
        }
        /// <summary>
        /// 输入类型，返回对应的update的数量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public short MonoActionCount(UpdateType type)
        {
            switch (type)
            {
                case UpdateType.FixedUpdate:
                        return fixedUpdateCount ;
                    break;
                case UpdateType.Update:
                        return updateCount;
                    break;
                case UpdateType.LateUpdate:
                        return lateUpdateCount;
                    break;
            }
            return -1;
        }
        IEnumerator EnumDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
        }
        IEnumerator EnumCoroutine(Coroutine routine, CFAction callBack)
        {
            yield return routine;
            callBack?.Invoke();
        }
        public Coroutine DelayCoroutine(float delay)
        {
            return StartCoroutine(EnumDelay(delay));
        }
        /// <summary>
        /// 嵌套协程
        /// </summary>
        /// <param name="routine">执行条件</param>
        /// <param name="callBack">执行条件结束后自动执行回调函数</param>
        /// <returns>Coroutine</returns>
        public Coroutine StartCoroutine(Coroutine routine, CFAction callBack)
        {
            return StartCoroutine(EnumCoroutine(routine, callBack));
        }
    }
}