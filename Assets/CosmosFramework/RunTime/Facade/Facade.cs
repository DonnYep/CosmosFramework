using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cosmos.Event;
using Cosmos.UI;
using Cosmos.Mono;
using Cosmos.Input;
using Cosmos.Scene;
using Cosmos.ObjectPool;
using Cosmos.Audio;
using Cosmos.Resource;
using Cosmos.Reference;
using Cosmos.Controller;
namespace Cosmos{
    /// <summary>
    /// CosmosFramework外观类，封装模块的功能，进行解耦
    /// 所有调用功能都通过这个外观类与模块进行沟通
    /// </summary>
    public sealed partial  class Facade : Singleton<Facade>
    {
        #region FacadeMethods
        public void InitAllModule()
        {
            Cosmos.Audio.AudioManager.Instance.OnInitialization();
            Cosmos.Resource.ResourceManager.Instance.OnInitialization();
            Cosmos.ObjectPool.ObjectPoolManager.Instance.OnInitialization();
            Cosmos.Network.NetworkManager.Instance.OnInitialization();
            Cosmos.Mono.MonoManager.Instance.OnInitialization();
            Cosmos.Input.InputManager.Instance.OnInitialization();
            Cosmos.UI.UIManager.Instance.OnInitialization();
            Cosmos.Event.EventManager.Instance.OnInitialization();
            Cosmos.Scene.SceneManager.Instance.OnInitialization();
            Cosmos.FSM.FSMManager.Instance.OnInitialization();
            Cosmos.Config.ConfigManager.Instance.OnInitialization();
            Cosmos.Data.DataManager.Instance.OnInitialization();
            Cosmos.Controller.ControllerManager.Instance.OnInitialization();
            Utility.DebugLog("Module Count:\t" + GameManager.Instance.ModuleCount);
        }
        public void RegisterModule(string  moduleName)
        {
            InitModule(moduleName);
        }
        IModule InitModule(string moduleName)
        {
            var result = GameManager.Instance.GetModule(moduleName);
            if (result == null)
            {
                string moduleFullName = "Cosmos." + moduleName + "." + moduleName + "Manager";
                return Utility.GetTypeInstance<IModule>(this.GetType().Assembly, moduleFullName);
            }
            else return result;
        }
        public IModule GetModule(string  moduleName)
        {
            var moduleResult = GameManager.Instance.GetModule(moduleName);
            if (moduleResult == null)
            {
               moduleResult= InitModule(moduleName);
            }
            return moduleResult;
        }
        #endregion
        #region InputManager
        #endregion
        #region EventManager
        public void AddEventListener(string eventKey, CFAction<object, GameEventArgs> handler)
        {
            EventManager.Instance.AddListener(eventKey, handler);
        }
        public void RemoveEventListener(string eventKey, CFAction<object, GameEventArgs> hander)
        {
            EventManager.Instance.RemoveListener(eventKey, hander);
        }
        public void DispatchEvent(string eventKey, object sender, GameEventArgs arg)
        {
            EventManager.Instance.DispatchEvent(eventKey, sender, arg);
        }
        public void RegisterEvent(string eventKey)
        {
            EventManager.Instance.RegisterEvent(eventKey);
        }
        public void DeregisterEvent(string eventKey)
        {
            EventManager.Instance.DeregisterEvent(eventKey);
        }
        public void ClearEvent(string eventKey)
        {
            EventManager.Instance.ClearEvent(eventKey);
        }
        public void ClearAllEvent()
        {
            EventManager.Instance.ClearAllEvent();
        }
        #endregion
        #region MonoManager
        public void AddMonoListener(CFAction act, UpdateType type, CFAction<short> callBack = null)
        {
            MonoManager.Instance.AddListener(act, type, callBack);
        }
        public void RemoveMonoListener(CFAction act, UpdateType type, short monoID)
        {
            MonoManager.Instance.RemoveListener(act, type, monoID);
        }
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return MonoManager.Instance.StartCoroutine(routine);
        }
        public void StopCoroutine(IEnumerator routine)
        {
            MonoManager.Instance.StopCoroutine(routine);
        }
        public void StopCoroutine(Coroutine routine)
        {
            MonoManager.Instance.StopCoroutine(routine);
        }
        /// <summary>
        /// 嵌套协程
        /// </summary>
        /// <param name="routine">执行条件</param>
        /// <param name="callBack">执行条件结束后自动执行回调函数</param>
        /// <returns>Coroutine</returns>
        public Coroutine StartCoroutine(Coroutine routine, CFAction callBack)
        {
            return MonoManager.Instance.StartCoroutine(routine, callBack);
        }
        public Coroutine DelayCoroutine(float delay)
        {
            return MonoManager.Instance.DelayCoroutine(delay);
        }
        public Coroutine DelayCoroutine(float delay,CFAction callBack)
        {
            return MonoManager.Instance.DelayCoroutine(delay,callBack);
        }
        #endregion
        #region AudioManager
        public void PlayBackgroundAudio( GameEventArgs arg)
        {
            AudioManager.Instance.PlayBackgroundAudio(arg as AudioEventArgs);
        }
        public void PauseBackgroundAudio()
        {
            AudioManager.Instance.PauseBackgroundAudio();
        }
        public void UnpauseBackgroundAudio()
        {
            AudioManager.Instance.UnpauseBackgroundAudio();
        }
        public void StopBackgroundAudio()
        {
            AudioManager.Instance.StopBackgroundAudio();
        }
        public void PlayWorldAudio(GameObject attachTarget,GameEventArgs arg)
        {
            AudioManager.Instance.PlayWorldAudio(attachTarget,  arg as AudioEventArgs);
        }
        public void PauseWorldAudio(GameObject attachTarget)
        {
            AudioManager.Instance.PauseWorldAudio(attachTarget);
        }
        public void UnpauseWorldAudio(GameObject attachTarget)
        {
            AudioManager.Instance.UnpauseWorldAudio(attachTarget);
        }
        public void StopWorldAudio(GameObject attachTarget)
        {
            AudioManager.Instance.StopWorldAudio(attachTarget);
        }
        public void PlayMultipleAudio(GameObject attachTarget, AudioEventArgs[] args)
        {
            AudioManager.Instance.PlayMultipleAudio(attachTarget, args);
        }
        public void PauseMultipleAudio(GameObject attachTarget)
        {
            AudioManager.Instance.PauseMultipleAudio(attachTarget);
        }
        public void UnpauseMultipleAudio(GameObject attachTarget)
        {
            AudioManager.Instance.UnpauseMultipleAudio(attachTarget);
        }
        public void StopMultipleAudio(GameObject attachTarget)
        {
            AudioManager.Instance.StopMultipleAudio(attachTarget);
        }
        public void StopAllWorldAudio()
        {
            AudioManager.Instance.StopAllWorldAudio();
        }
        public void SetAudioMuteState(bool state)
        {
            AudioManager.Instance.Mute = state;
        }
        #endregion
        #region ResourceManager
        public  T Load<T>(string path)
            where T : UnityEngine.Object
        {
            return ResourceManager.Instance.Load<T>(path);
        }
        public void LoadAysnc<T>(string path, CFAction<T> callBack = null)
            where T:UnityEngine.Object
        {
            ResourceManager.Instance.LoadAysnc<T>(path, callBack);
        }
        /// <summary>
        /// 使用unityEngine.Resources方法
        /// </summary>
        public List<T> LoadFolderAssets<T>(string path)
            where T:UnityEngine.Object
        {
            return ResourceManager.Instance.LoadFolderAssets<T>(path);
        }
        #endregion
        #region ScenesManager
        public void LoadScene(string sceneName, CFAction action=null)
        {
            SceneManager.Instance.LoadScene(sceneName, action);
        }
        public void LoadScene(int sceneIndex, CFAction action=null)
        {
            SceneManager.Instance.LoadScene(sceneIndex, action);
        }
        public void LoadSceneAsync(string sceneName, CFAction action=null)
        {
            SceneManager.Instance.LoadSceneAsync(sceneName, action);
        }
        public void LoadSceneAsync(int sceneIndex, CFAction action = null)
        {
            SceneManager.Instance.LoadSceneAsync(sceneIndex, action);
        }
        #endregion
        #region GameObjectPool
        public void RegisterObjcetSpawnPool(object objKey, GameObject spawnItem, CFAction<GameObject> onSpawn, CFAction<GameObject> onDespawn)
        {
            ObjectPoolManager.Instance.RegisterSpawnPool(objKey, spawnItem, onSpawn,onDespawn);
        }
        public void DeregisterObjectSapwnPool(object objKey)
        {
            ObjectPoolManager.Instance.DeregisterSpawnPool(objKey);
        }
        public int GetObjectSpawnPoolItemCount(object objKey)
        {
            return ObjectPoolManager.Instance.GetPoolCount(objKey);
        }
        public GameObject SpawnObject(object objKey)
        {
            return ObjectPoolManager.Instance.Spawn(objKey);
        }
        public void DespawnObject(object objKey, GameObject go)
        {
            ObjectPoolManager.Instance.Despawn(objKey, go);
        }
        public void DespawnObjects(object objKey,GameObject[] gos)
        {
            ObjectPoolManager.Instance.Despawns(objKey, gos);
        }
        public void ClearObjectSpawnPool(object objKey)
        {
            ObjectPoolManager.Instance.Clear(objKey);
        }
        public void ClearAllObjectSpawnPool()
        {
            ObjectPoolManager.Instance.ClearAll();
        }
        public void SetObjectSpawnItem(object objKey,GameObject go)
        {
            ObjectPoolManager.Instance.SetSpawnItem(objKey, go);
        }
        /// <summary>
        /// 对象池生成对象在激活状态时所在的容器，场景中唯一，被销毁后依旧会创建
        /// </summary>
        /// <returns></returns>
        public GameObject GetObjectSpawnPoolActiveMount()
        {
            return ObjectPoolManager.Instance.ActiveObjectMount;
        }
        /// <summary>
        /// 生成对象但不经过池，通常用在一次性对象的产生上
        /// </summary>
        public GameObject SpawnObjectNotUsePool(GameObject go,Transform spawnTransform)
        {
            return ObjectPoolManager.Instance.SpawnNotUsePool(go, spawnTransform);
        }
        #endregion
        #region ControllerManager
        public void RegisterController<T>(T controller)
            where T : CFController
        {
            ControllerManager.Instance.RegisterController<T>(controller);
        }
        /// <summary>
        /// 注销控制器，如果当前控制器是最后一个，则注销后，这个类别也会自动注销
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        public void DeregisterController<T>(T controller)
            where T : CFController
        {
            ControllerManager.Instance.DeregisterController<T>(controller);
        }
        public T GetController<T>(CFPredicateAction<T> predicate)
            where T:CFController
        {
            return ControllerManager.Instance.GetController<T>(predicate);
        }
        public T[] GetControllers<T>(CFPredicateAction<T> predicate)
            where T : CFController
        {
            return ControllerManager.Instance.GetControllers(predicate);
        }
        /// <summary>
        /// 获取当前注册的T类型重，controller所包含的数量
        /// </summary>
        public short GetControllerItemCount<T>()
        {
            return ControllerManager.Instance.GetControllerItemCount<T>();
        }
        /// <summary>
        /// 获取当前注册的所有类型controller总数
        /// </summary>
        public short GetControllerTypeCount()
        {
            return ControllerManager.Instance.GetControllerTypeCount();
        }
        public bool  HasController<T>()
            where T : CFController
        {
            return ControllerManager.Instance.HasController<T>();
        }
        public bool HasControllerItem<T>(T controller)
            where T : CFController
        {
            return ControllerManager.Instance.HasControllerItem(controller);
        }
        public void ClearAllController()
        {
            ControllerManager.Instance.ClearAllController();
        }
        public void ClearControllerItem<T>()
            where T : CFController
        {
            ControllerManager.Instance.ClearControllerItem<T>();
        }
        #endregion
        #region DataManager

        #endregion
        #region ReferenceManager
        public int GetReferencePoolCount<T>()
            where T : class, IReference, new()
        {
            return  ReferencePoolManager.Instance.GetPoolCount<T>();
        }
        public T SpawnReference<T>()
            where T : class, IReference, new()
        {
            return ReferencePoolManager.Instance.Spawn<T>();
        }
        public IReference SpawnReference(Type type)
        {
            return ReferencePoolManager.Instance.Spawn(type);
        }
        public void DespawnReference(IReference refer)
        {
            ReferencePoolManager.Instance.Despawn(refer);
        }
        public void DespawnsReference<T>(List<T> refers)
            where T : class, IReference, new()
        {
            ReferencePoolManager.Instance.Despawns<T>(refers);
        }
        public void DespawnsReference<T>(T[] refers)
            where T : class, IReference, new()
        {
            ReferencePoolManager.Instance.Despawns<T>(refers);
        }
        public void ClearReferencePool(Type type)
        {
            ReferencePoolManager.Instance.Clear(type);
        }
        public void ClearAllReferencePool()
        {
            ReferencePoolManager.Instance.Clear();
        }
        #endregion
        #region UIManager
        public void ShowPanel<T>(string panelName,CFAction<T> callBack=null)
            where T:UILogicBase
        {
            UIManager.Instance.ShowPanel<T>(panelName, callBack);
        }
        public void HidePanel(string panelName)
        {
            UIManager.Instance.HidePanel(panelName);
        }
        public void RemovePanel(string panelName)
        {
            UIManager.Instance.RemovePanel(panelName);
        }
        public void HasPanel(string panelName)
        {
            UIManager.Instance.HasPanel(panelName);
        }
        #endregion
    }
}