using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.ComponentModel;
namespace Cosmos
{
    public enum UpdateType : int
    {
        Update = 0,
        FixedUpdate = 1,
        LateUpdate = 2
    }
}
namespace Cosmos.Mono
{
    /// <summary>
    /// 不继承自mono的对象通过这个管理器来实现update等需要mono才能做到的功能
    /// 当前只生成一个mc
    /// </summary>
    internal sealed class MonoManager : Module<MonoManager>
    {
        #region Properties
        Dictionary<short, IMonoPool> monoDict = new Dictionary<short, IMonoPool>();
        // 单个monoController update委托的容量
        public static readonly short _UpdateCapacity = 100;
        // 单个monoController fixedUpdate委托的容量
        public static readonly short _FixedUpdateCapacity = 100;
        // 单个monoController lateUpdate委托的容量
        public static readonly short _LateUpdateCapacity = 100;
        // monoController的总数量
        short monoControllerCount = 0;
        #endregion

        #region Methods
        /// <summary>
        /// 嵌套协程
        /// </summary>
        /// <param name="routine">执行条件</param>
        /// <param name="callBack">执行条件结束后自动执行回调函数</param>
        /// <returns>Coroutine</returns>
        internal Coroutine StartCoroutine(Coroutine routine, Action callBack)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            return (monoDict[monoControllerCount] as MonoPool).StartCoroutine(routine, callBack);
        }
        internal Coroutine DelayCoroutine(float delay, Action callBack)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            return (monoDict[monoControllerCount] as MonoPool).DelayCoroutine(delay, callBack);
        }
        internal Coroutine PredicateCoroutine(Func<bool> handler, Action callBack)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            return (monoDict[monoControllerCount] as MonoPool).PredicateCoroutine(handler, callBack);
        }
        /// <summary>
        /// 添加事件监听者
        /// 有一个回调函数，默认为空
        /// 使用回调函数操作对应命令
        /// 尽量不要使用匿名函数，否则释放会出现问题。匿名函数本质实现都是创建对象添加到委托，因此释放需要注意
        /// </summary>
        internal void AddListener(Action act, UpdateType type, out short monoPoolID)
        {
            short poolID = -1;
            poolID = FindUseable(type);
            if (poolID == -1)
            {
                poolID = CreateMonoController().MonoID;
                monoDict[monoControllerCount].AddListener(act, type);
            }
            monoDict[poolID].AddListener(act, type);
            monoPoolID = poolID;
        }
        /// <summary>
        /// 移除监听者
        /// </summary>
        /// <param name="act"></param>
        /// <param name="type"></param>
        /// <param name="key'">委托所在的序号</param>
        internal void RemoveListener(Action act, UpdateType type, short monoPoolID)
        {
            if (!monoDict.ContainsKey(monoPoolID))
            {
                throw new ArgumentException("MonoManager : \n" + "MonoPool ID" + monoPoolID + " does not exist!");
            }
            else
                monoDict[monoPoolID].RemoveListener(act, type);
        }
        /// <summary>
        /// 开启协程
        /// 默认使用当前基数最高的对象
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        internal Coroutine StartCoroutine(IEnumerator routine)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            return (monoDict[monoControllerCount] as MonoPool).StartCoroutine(routine);
        }
        /// <summary>
        /// 关闭一个monoController上的所有迭代器
        /// 慎用
        /// </summary>
        internal void StopAllCoroutines()
        {
            if (monoControllerCount == 0)
                return;
            foreach (var mc in monoDict.Values)
            {
                (mc as MonoPool).StopAllCoroutines();
            }
        }
        /// <summary>
        /// 关闭协程
        /// </summary>
        /// <param name="methodName"></param>
        internal void StopCoroutine(IEnumerator routine)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            (monoDict[monoControllerCount] as MonoPool).StopCoroutine(routine);
        }
        internal void StopCoroutine(Coroutine routine)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            (monoDict[monoControllerCount] as MonoPool).StopCoroutine(routine);
        }
        /// <summary>
        /// 根据输入的key判断是否全空，是则注销
        /// </summary>
        /// <param name="index"></param>
        void DestoryMonoController(short index)
        {
            if (!monoDict.ContainsKey(index))
                return;
            if (monoDict[index].IsAllActionEmpty)
                Deregister(index);
        }
        /// <summary>
        /// 注册到monoDict
        /// </summary>
        /// <param name="index"></param>
        /// <param name="mc"></param>
        void Register(short index, IMonoPool mc)
        {
            if (!monoDict.ContainsKey(index))
                monoDict.Add(monoControllerCount, mc);
        }
        /// <summary>
        /// 注销到monoDict
        /// </summary>
        /// <param name="index"></param>
        void Deregister(short index)
        {
            if (monoDict.ContainsKey(index))
            {
                GameManager.KillObject((monoDict[index] as MonoPool).gameObject);
                monoDict.Remove(index);
            }
        }
        /// <summary>
        /// 返回第一个符合条件的对象的ID，否则返回-1
        /// </summary>
        /// <param name="type"></param>
        /// <param name="currentIndex"></param>
        short FindUseable(UpdateType type)
        {
            short id = -1;
            foreach (var mc in monoDict.Values)
            {
                MonoPool monoc = mc as MonoPool;
                id = monoc.UseableMono(type);
                if (id != -1)
                    return id;
            }
            return id;
        }
        /// <summary>
        /// 创建新的monoController
        /// </summary>
        MonoPool CreateMonoController()
        {
            ++monoControllerCount;
            GameObject go = new GameObject("MonoPool" + "_Num_" + monoControllerCount);
            go.transform.SetParent(ModuleMountObject.transform);
            MonoPool mc =go.AddComponent<MonoPool>();
            mc.MonoID = monoControllerCount;
            Register(monoControllerCount, mc);
            return mc;
        }
        #endregion
    }
}