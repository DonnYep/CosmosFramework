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
using Cosmos.FSM;
using Cosmos.Data;
namespace Cosmos
{
    /// <summary>
    /// CosmosFramework外观类，封装模块的功能，进行解耦
    /// 所有调用功能都通过这个外观类与模块进行沟通
    /// </summary>
    public sealed class Facade : Singleton<Facade>
    {
        #region FacadeMethods
        public void InitAllModule()
        {
            Cosmos.Audio.AudioManager.Instance.DebugModule();
            Cosmos.Resource.ResourceManager.Instance.DebugModule();
            Cosmos.ObjectPool.ObjectPoolManager.Instance.DebugModule();
            Cosmos.Network.NetworkManager.Instance.DebugModule();
            Cosmos.Mono.MonoManager.Instance.DebugModule();
            Cosmos.Input.InputManager.Instance.DebugModule();
            Cosmos.UI.UIManager.Instance.DebugModule();
            Cosmos.Event.EventManager.Instance.DebugModule();
            Cosmos.Scene.SceneManager.Instance.DebugModule();
            Cosmos.FSM.FSMManager.Instance.DebugModule();
            Cosmos.Config.ConfigManager.Instance.DebugModule();
            Cosmos.Data.DataManager.Instance.DebugModule();
            Cosmos.Controller.ControllerManager.Instance.DebugModule();
            Utility.DebugLog("Module Count:\t" + GameManager.Instance.ModuleCount);
        }
        public void RegisterModule(string moduleName)
        {
            InitModule(moduleName);
        }
        IModule InitModule(string moduleName)
        {
            var result = GameManager.Instance.GetModule(moduleName);
            if (result == null)
            {
                var moduleResult = Utility.GetTypeInstance<IModule>(this.GetType().Assembly, Utility.Framework.GetModuleTypeFullName(moduleName));
                GameManager.Instance.RegisterModule(moduleName, moduleResult);
                return moduleResult;
            }
            else return result;
        }
        public IModule GetModule(string moduleName)
        {
            var moduleResult = GameManager.Instance.GetModule(moduleName);
            if (moduleResult == null)
            {
                moduleResult = InitModule(moduleName);
            }
            return moduleResult;
        }
        public bool HasModule(string moduleName)
        {
            return GameManager.Instance.HasModule(moduleName);
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
        public Coroutine DelayCoroutine(float delay, CFAction callBack)
        {
            return MonoManager.Instance.DelayCoroutine(delay, callBack);
        }
        #endregion
        #region AudioManager
        public void PlayBackgroundAudio(GameEventArgs arg)
        {
            AudioManager.Instance.PlayBackgroundAudio(arg);
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
        public void PlayWorldAudio(GameObject attachTarget, GameEventArgs arg)
        {
            AudioManager.Instance.PlayWorldAudio(attachTarget, arg);
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
        public void PlayMultipleAudio(GameObject attachTarget, GameEventArgs[] args)
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
        #region 基于Resources
        /// <summary>
        /// 同步加载资源，若可选参数为true，则返回实例化后的对象，否则只返回资源对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">相对Resource路径</param>
        /// <param name="instantiateGameObject">是否实例化GameObject类型</param>
        /// <returns></returns>
        public T LoadResAsset<T>(string path, bool instantiateGameObject = false)
            where T : UnityEngine.Object
        {
            return ResourceManager.Instance.LoadResAsset<T>(path, instantiateGameObject);
        }
        /// <summary>
        /// 异步加载资源,如果目标是Gameobject，则实例化
        /// </summary>
        public void LoadResAysnc<T>(string path, CFAction<T> callBack = null)
            where T : UnityEngine.Object
        {
            ResourceManager.Instance.LoadResAysnc<T>(path, callBack);
        }
        /// <summary>
        /// 异步加载资源,不实例化任何类型
        /// </summary>
        public void LoadResAssetAysnc<T>(string path, CFAction<T> callBack = null)
            where T : UnityEngine.Object
        {
            ResourceManager.Instance.LoadResAssetAysnc(path, callBack);
        }
        /// <summary>
        /// 使用unityEngine.Resources方法
        /// </summary>
        public List<T> LoadResFolderAssets<T>(string path)
            where T : UnityEngine.Object
        {
            return ResourceManager.Instance.LoadResFolderAssets<T>(path);
        }
        public T[] LoadResAll<T>(string path)
            where T : UnityEngine.Object
        {
            return ResourceManager.Instance.LoadResAll<T>(path);
        }
        #endregion
        #region 基于AssetBundle

        #endregion
        #endregion
        #region ScenesManager
        public void LoadScene(string sceneName, CFAction callBack = null)
        {
            SceneManager.Instance.LoadScene(sceneName, callBack);
        }
        public void LoadScene(int sceneIndex, CFAction callBack = null)
        {
            SceneManager.Instance.LoadScene(sceneIndex, callBack);
        }
        /// <summary>
        /// 回调函数只在完成后进行一次回调
        /// </summary>
        public void LoadSceneAsync(string sceneName, CFAction callBack = null)
        {
            SceneManager.Instance.LoadSceneAsync(sceneName, callBack);
        }
        /// <summary>
        /// 回调函数每次yield更新都会调用
        /// ，不会进行完成后的调用
        /// </summary>
        public void LoadSceneAsync(string sceneName, CFAction<float> callBack)
        {
            SceneManager.Instance.LoadSceneAsync(sceneName, callBack);
        }
        /// <summary>
        /// 回调函数每次yield更新都会调用
        /// ，不会进行完成后的调用
        /// </summary>
        public void LoadSceneAsync(string sceneName, CFAction<AsyncOperation> callBack)
        {
            SceneManager.Instance.LoadSceneAsync(sceneName, callBack);
        }
        /// <summary>
        /// 回调函数只在完成后进行一次回调
        /// </summary>
        public void LoadSceneAsync(int sceneIndex, CFAction callBack = null)
        {
            SceneManager.Instance.LoadSceneAsync(sceneIndex, callBack);
        }
        /// <summary>
        /// 回调函数每次yield更新都会调用
        /// ，不会进行完成后的调用
        /// </summary>
        public void LoadSceneAsync(int sceneIndex, CFAction<float> callBack = null)
        {
            SceneManager.Instance.LoadSceneAsync(sceneIndex, callBack);
        }
        /// <summary>
        /// 回调函数每次yield更新都会调用
        /// ，不会进行完成后的调用
        /// </summary>
        public void LoadSceneAsync(int sceneIndex, CFAction<AsyncOperation> callBack = null)
        {
            SceneManager.Instance.LoadSceneAsync(sceneIndex, callBack);
        }
        #endregion
        #region GameObjectPool
        public void RegisterObjcetSpawnPool(object objKey, GameObject spawnItem, CFAction<GameObject> onSpawn, CFAction<GameObject> onDespawn)
        {
            ObjectPoolManager.Instance.RegisterSpawnPool(objKey, spawnItem, onSpawn, onDespawn);
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
        public void DespawnObjects(object objKey, GameObject[] gos)
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
        public void SetObjectSpawnItem(object objKey, GameObject go)
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
        public GameObject SpawnObjectNotUsePool(GameObject go, Transform spawnTransform)
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
            where T : CFController
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
        public bool HasController<T>()
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
        /// <summary>
        /// 保存Json数据到本地的绝对路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="dataSet">装箱后的数据</param>
        /// <param name="callBack">回调函数，当写入成功后调用</param>
        public void SaveJsonDataToLocal<T>(string relativePath, string fileName, T dataSet, CFAction callBack = null)
        {
            DataManager.Instance.SaveJsonDataToLocal(relativePath, fileName, dataSet, callBack);
        }
        /// <summary>
        /// 从本地的绝对路径读取Json数据
        /// </summary>
        /// <typeparam name="T">反序列化的目标类型</typeparam>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="dataSet">装箱后的数据</param>
        /// <param name="callBack">回调函数，当读取成功后调用</param>
        public void LoadJsonDataFromLocal<T>(string relativePath, string fileName, ref T dataSet, CFAction<T> callBack = null)
        {
            DataManager.Instance.LoadJsonDataFromLocal(relativePath, fileName, ref dataSet, callBack);
        }
        public void LoadJsonDataFromLocal<T>(string fullRelativeFilePath, ref T dataSet, CFAction<T> callBack = null)
        {
            DataManager.Instance.LoadJsonDataFromLocal(fullRelativeFilePath, ref dataSet, callBack);
        }
        public void ParseDataFromResource<T>(string relativePath, string fileName, ref T dataSet, CFAction<T> callBack = null)
          where T : class, new()
        {
            DataManager.Instance.ParseDataFromResource(relativePath, fileName, ref dataSet, callBack);
        }
        #endregion
        #region ReferenceManager
        public int GetReferencePoolCount<T>()
            where T : class, IReference, new()
        {
            return ReferencePoolManager.Instance.GetPoolCount<T>();
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
        /// <summary>
        /// 载入面板，若字典中已存在，则返回且不使用回调。若不存在，则异步加载且使用回调。
        /// 基于Resources
        /// </summary>
        /// <typeparam name="T"> UILogicBase</typeparam>
        /// <param name="panelName">相对完整路径</param>
        /// <param name="callBack">仅在载入时回调</param>
        public void LoadPanel<T>(string panelName, CFAction<T> callBack = null)
                   where T : UILogicBase
        {
            UIManager.Instance.LoadPanel(panelName, callBack);
        }
        /// <summary>
        /// 载入面板，若字典中已存在，则使用回调，并返回。若不存在，则异步加载且使用回调。
        /// 基于Resources
        /// </summary>
        public void ShowPanel<T>(string panelName, CFAction<T> callBack = null)
            where T : UILogicBase
        {
            UIManager.Instance.ShowPanel(panelName, callBack);
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
        /// <summary>
        /// Resource文件夹相对路径
        /// 返回实例化的对象
        /// </summary>
        /// <param name="path">如UI\Canvas</param>
        public GameObject InitMainCanvas(string path)
        {
            return UIManager.Instance.InitMainCanvas(path);
        }
        #endregion
        #region FSMManager
        public FSMBase GetFSM<T>()
            where T : FSMBase
        {
            return FSMManager.Instance.GetFSM<T>();
        }
        public FSMBase GetFSM(Type type)
        {
            return GetFSM(type);
        }
        public FSMBase[] GetAllFSM<T>()
            where T : FSMBase
        {
            return FSMManager.Instance.GetAllFSM<T>();
        }
        public FSMBase[] GetAllFSM(Type type)
        {
            return FSMManager.Instance.GetAllFSM(type);
        }
        public bool HasFSM<T>()
            where T : FSMBase
        {
            return FSMManager.Instance.HasFSM<T>();
        }
        public bool HasFSM(Type type)
        {
            return FSMManager.Instance.HasFSM(type);
        }
        public IFSM<T> CreateFSM<T>(T owner, params FSMState<T>[] states)
            where T : class
        {
            return FSMManager.Instance.CreateFSM(owner, states);
        }
        public IFSM<T> CreateFSM<T>(string name, T owner, params FSMState<T>[] states)
            where T : class
        {
            return FSMManager.Instance.CreateFSM<T>(name, owner, states);
        }
        public IFSM<T> CreateFSM<T>(T owner, List<FSMState<T>> states)
            where T : FSMBase
        {
            return FSMManager.Instance.CreateFSM(owner, states);
        }
        public IFSM<T> CreateFSM<T>(string name, T owner, List<FSMState<T>> states)
         where T : class
        {
            return FSMManager.Instance.CreateFSM(name, owner, states);
        }
        public void DestoryFSM<T>()
         where T : class
        {
            FSMManager.Instance.DestoryFSM<T>();
        }
        public void DestoryFSM(Type type)
        {
            FSMManager.Instance.DestoryFSM(type);
        }
        public void ShutdownAllFSM()
        {
            FSMManager.Instance.ShutdownAllFSM();
        }
        #endregion
    }
}