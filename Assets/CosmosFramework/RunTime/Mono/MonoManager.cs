using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.ComponentModel;
namespace Cosmos.Mono
{

    public enum UpdateType:int
    {
        Update = 0,
        FixedUpdate = 1,
        LateUpdate=2
    }
    /// <summary>
    /// 不继承自mono的对象通过这个管理器来实现update等需要mono才能做到的功能
    /// 当前只生成一个mc
    /// </summary>
    public sealed class MonoManager:Module<MonoManager>
    {
        Dictionary<short, IMonoController> monoMap=new Dictionary<short, IMonoController>();
        // 单个monoController update委托的容量
       public  const short _UpdateCapacity= 100;
        // 单个monoController fixedUpdate委托的容量
        public const short _FixedUpdateCapacity = 100;
        // 单个monoController lateUpdate委托的容量
        public const short _LateUpdateCapacity = 100;
        // monoController的总数量
        short monoControllerCount = 0;
        /// 创建新的monoController
        MonoController CreateMonoController()
        {
            ++monoControllerCount;
            GameObject go = new GameObject("MonoController" + "_Num_" + monoControllerCount);
            go.transform.SetParent(ModuleMountObject.transform);
            //MonoController mc = go.AddComponent<MonoController>();
            MonoController mc = Utility.Add<MonoController>(go);
            mc.MonoID = monoControllerCount;
            Register(monoControllerCount, mc);
            return mc;
        }
        /// <summary>
        /// 添加事件监听者
        /// 有一个回调函数，默认为空
        /// 使用回调函数操作对应命令
        /// 尽量不要使用匿名函数，否则释放会出现问题。匿名函数本质实现都是创建对象添加到委托，因此释放需要注意
        /// </summary>
        public void AddListener(CFAction act, UpdateType type,CFAction<short>callBack=null)
        {
            short monoId = -1;
            monoId = FindUseable(type);
            if (monoId == -1)
            {
               monoId= CreateMonoController().MonoID;
                monoMap[monoControllerCount].AddListener(act, type);
                if (callBack != null)
                    callBack(monoId);
                return;
            }
            monoMap[monoId].AddListener(act, type);
            if (callBack != null)
                callBack(monoId);
        }
        /// <summary>
        /// 移除监听者
        /// </summary>
        /// <param name="act"></param>
        /// <param name="type"></param>
        /// <param name="key'">委托所在的序号</param>
        public void RemoveListener(CFAction act, UpdateType type,short monoID)
        {
            if(!monoMap.ContainsKey(monoID))
            {
                Utility.DebugError("MonoManager\n"+"MonoController ID"+monoID+" does not exist!");
                return;
            }
            monoMap[monoID].RemoveListener(act, type);
        }
        /// <summary>
        /// 开启协程
        /// 默认使用当前基数最高的对象
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            return (monoMap[monoControllerCount] as MonoController).StartCoroutine(routine);
        }
        /// <summary>
        /// 关闭一个monoController上的所有迭代器
        /// 慎用
        /// </summary>
        public void StopAllCoroutines()
        {
            if (monoControllerCount == 0)
                return;
                foreach (var mc in monoMap.Values)
            {
                (mc as MonoController).StopAllCoroutines();
            }
        }
        /// <summary>
        /// 关闭协程
        /// </summary>
        /// <param name="methodName"></param>
        public void StopCoroutine(IEnumerator routine)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            (monoMap[monoControllerCount] as MonoController).StopCoroutine(routine);
        }
        public void StopCoroutine(Coroutine routine)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            (monoMap[monoControllerCount] as MonoController).StopCoroutine(routine);
        }
        /// <summary>
        /// 根据输入的key判断是否全空，是则注销
        /// </summary>
        /// <param name="index"></param>
        void DestoryMonoController(short index)
        {
            if (!monoMap.ContainsKey(index))
                return;
            if (monoMap[index].IsAllActionEmpty)
                Deregister(index);
        }
        /// <summary>
        /// 注册到monoMap
        /// </summary>
        /// <param name="index"></param>
        /// <param name="mc"></param>
        void Register(short index,IMonoController mc)
        {
            if(!monoMap.ContainsKey(index))
                monoMap.Add(monoControllerCount, mc);
        }
        /// <summary>
        /// 注销到monoMap
        /// </summary>
        /// <param name="index"></param>
        void Deregister(short index)
        {
            if (monoMap.ContainsKey(index))
            {
                GameManager.KillObject((monoMap[index] as MonoController).gameObject);
                monoMap.Remove(index);
            }
        }
        /// <summary>
        /// 返回第一个符合条件的对象的ID，否则返回-1
        /// </summary>
        /// <param name="type"></param>
        /// <param name="currentIndex"></param>
        short  FindUseable(UpdateType type)
        {
            short id = -1;
            foreach (var mc in monoMap.Values)
            {
                MonoController monoc = mc as MonoController;
              id=monoc.UseableMono(type);
                if (id != -1)
                    return id;
            }
            return id;
        }
        /// <summary>
        /// 嵌套协程
        /// </summary>
        /// <param name="routine">执行条件</param>
        /// <param name="callBack">执行条件结束后自动执行回调函数</param>
        /// <returns>Coroutine</returns>
        public Coroutine StartCoroutine(Coroutine routine, CFAction callBack)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            return (monoMap[monoControllerCount] as MonoController).StartCoroutine(routine, callBack);
        }
        public Coroutine DelayCoroutine(float delay)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            return (monoMap[monoControllerCount] as MonoController).DelayCoroutine(delay);
        }
        public Coroutine DelayCoroutine(float delay,CFAction callBack)
        {
            if (monoControllerCount == 0)
                CreateMonoController();
            return (monoMap[monoControllerCount] as MonoController).DelayCoroutine(delay,callBack);
        }
    }
}