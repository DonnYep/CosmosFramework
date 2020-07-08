using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Cosmos.UI;
using Cosmos.FSM;
using Cosmos.Input;
using Cosmos.Entity;

namespace Cosmos
{
    /// <summary>
    /// 临时类门面类API对象
    /// </summary>
    public class FacadeObject
    {
        #region FacadeMethods
        public  void InitAllModule()
        {
            GameManager.AudioManager.DebugModule();
            GameManager.ResourceManager.DebugModule();
            GameManager.ObjectPoolManager.DebugModule();
            GameManager.NetworkManager.DebugModule();
            GameManager.MonoManager.DebugModule();
            GameManager.InputManager.DebugModule();
            GameManager.UIManager.DebugModule();
            GameManager.EventManager.DebugModule();
            GameManager.SceneManager.DebugModule();
            GameManager.FSMManager.DebugModule();
            GameManager.ConfigManager.DebugModule();
            GameManager.DataManager.DebugModule();
            GameManager.ControllerManager.DebugModule();
            GameManager.EntityManager.DebugModule();
            GameManager.HotfixManager.DebugModule();
            Utility.DebugLog("Module Count:\t" + GameManager.Instance.ModuleCount);
        }
        public void RegisterModule(ModuleEnum module)
        {
            InitModule(module);
        }
        //TODO 反射初始化需要为AOT做出预留，专门设计为IOS系统的symbol
        IModule InitModule(ModuleEnum module)
        {
#if UNITY_EDITOR||UNITY_EDITOR_WIN||UNITY_STANDALONE_WIN||UNITY_ANDROID
            var result = GameManager.Instance.GetModule(module);
            if (result == null)
            {
                var moduleResult = Utility.Assembly.GetTypeInstance<IModule>(Assembly.GetAssembly(typeof(Facade)), Utility.Framework.GetModuleTypeFullName(module.ToString()));
                GameManager.Instance.ModuleInitialization(moduleResult);
                return moduleResult;
            }
            else return result;
#elif UNITY_IOS
            return InitModuleForIOS(moduleName);
#endif
        }
        /// <summary>
        ///这个是为了避免IOS环境下的AOT编译无法通过反射初始化模块的方法
        /// </summary>
        IModule InitModuleForIOS(ModuleEnum module)
        {
            switch (module)
            {
                case ModuleEnum.Audio:
                    GameManager.AudioManager.DebugModule();
                    break;
                case ModuleEnum.Config:
                    GameManager.ConfigManager.DebugModule();
                    break;
                case ModuleEnum.Controller:
                    GameManager.ControllerManager.DebugModule();
                    break;
                case ModuleEnum.Data:
                    GameManager.DataManager.DebugModule();
                    break;
                case ModuleEnum.Entity:
                    GameManager.EntityManager.DebugModule();
                    break;
                case ModuleEnum.Event:
                    GameManager.EventManager.DebugModule();
                    break;
                case ModuleEnum.FSM:
                    GameManager.FSMManager.DebugModule();
                    break;
                case ModuleEnum.Input:
                    GameManager.InputManager.DebugModule();
                    break;
                case ModuleEnum.Mono:
                    GameManager.MonoManager.DebugModule();
                    break;
                case ModuleEnum.Network:
                    GameManager.NetworkManager.DebugModule();
                    break;
                case ModuleEnum.ObjectPool:
                    GameManager.ObjectPoolManager.DebugModule();
                    break;
                case ModuleEnum.ReferencePool:
                    GameManager.ReferencePoolManager.DebugModule();
                    break;
                case ModuleEnum.Resource:
                    GameManager.ResourceManager.DebugModule();
                    break;
                case ModuleEnum.Scene:
                    GameManager.SceneManager.DebugModule();
                    break;
                case ModuleEnum.UI:
                    GameManager.UIManager.DebugModule();
                    break;
            }
            var result = GameManager.Instance.GetModule(module);
            return result;
        }
        public  IModule GetModule(ModuleEnum module)
        {
            //var fullModuleName = Utility.Framework.GetModuleTypeFullName(moduleName);
            var moduleResult = GameManager.Instance.GetModule(module);
            if (moduleResult == null)
            {
                moduleResult = InitModule(module);
            }
            return moduleResult;
        }
        public bool HasModule(ModuleEnum module)
        {
            return GameManager.Instance.HasModule(module);
        }
        #endregion
        #region InputManager
        /// <summary>
        /// 设置输入解决方案
        /// </summary>
        /// <param name="inputDevice">InputDevice的具体平台实现方法</param>
        public void SetInputDevice(InputDevice inputDevice) { Input.InputManager.SetInputDevice(inputDevice); }
        /// <summary>
        /// 虚拟轴线是否存在
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualAxis(string name) { return GameManager.InputManager.IsExistVirtualAxis(name); }
        /// <summary>
        /// 虚拟按键是否存在
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualButton(string name) { return GameManager.InputManager.IsExistVirtualButton(name); }
        /// <summary>
        /// 注册虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public void RegisterVirtualButton(string name) { GameManager.InputManager.RegisterVirtualButton(name); }
        /// <summary>
        /// 注销虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public void DeregisterVirtualButton(string name) { GameManager.InputManager.DeregisterVirtualButton(name); }
        /// <summary>
        /// 注册虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public void RegisterVirtualAxis(string name) { GameManager.InputManager.RegisterVirtualAxis(name); }
        /// <summary>
        /// 注销虚拟轴线
        /// </summary>
        /// <param name="name">按键名称</param>
        public void DeregisterVirtualAxis(string name) { GameManager.InputManager.DeregisterVirtualAxis(name); }
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector3 MousePosition() { return GameManager.InputManager.MousePosition; }
        /// <summary>
        /// 获得轴线
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns></returns>
        public float GetAxis(string name) { return GameManager.InputManager.GetAxis(name); }
        /// <summary>
        /// 未插值的输入 -1，0 ，1
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns></returns>
        public float GetAxisRaw(string name) { return GameManager.InputManager.GetAxisRaw(name); }
        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonDown(string name) { return GameManager.InputManager.GetButtonDown(name); }
        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public bool GetButton(string name) { return GameManager.InputManager.GetButton(name); }
        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonUp(string name) { return GameManager.InputManager.GetButtonUp(name); }
        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonDown(string name) { GameManager.InputManager.SetButtonDown(name); }
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonUp(string name) { GameManager.InputManager.SetButtonUp(name); }
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        public void SetVirtualMousePosition(Vector3 value) { GameManager.InputManager.SetVirtualMousePosition(value); }
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        public void SetVirtualMousePosition(float x, float y, float z) { GameManager.InputManager.SetVirtualMousePosition(x, y, z); }
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisPositive(string name) { GameManager.InputManager.SetAxisPositive(name); }
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisNegative(string name) { GameManager.InputManager.SetAxisNegative(name); }
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisZero(string name) { GameManager.InputManager.SetAxisZero(name); }
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">值</param>
        public void SetAxis(string name, float value) { GameManager.InputManager.SetAxis(name, value); }

        #endregion
        #region EventManager
        public void AddEventListener(string eventKey, CFAction<object, GameEventArgs> handler)
        {
            GameManager.EventManager.AddListener(eventKey, handler);
        }
        public void RemoveEventListener(string eventKey, CFAction<object, GameEventArgs> hander)
        {
            GameManager.EventManager.RemoveListener(eventKey, hander);
        }
        public void DispatchEvent(string eventKey, object sender, GameEventArgs arg)
        {
            GameManager.EventManager.DispatchEvent(eventKey, sender, arg);
        }
        public void RegisterEvent(string eventKey)
        {
            GameManager.EventManager.RegisterEvent(eventKey);
        }
        public void DeregisterEvent(string eventKey)
        {
            GameManager.EventManager.DeregisterEvent(eventKey);
        }
        public void ClearEvent(string eventKey)
        {
            GameManager.EventManager.ClearEvent(eventKey);
        }
        public void ClearAllEvent()
        {
            GameManager.EventManager.ClearAllEvent();
        }
        public bool HasEvent(string eventKey)
        {
            return GameManager.EventManager.HasEvent(eventKey);
        }
        #endregion
        #region MonoManager
        public void AddMonoListener(CFAction act, UpdateType type, out short monoPoolID)
        {
            GameManager.MonoManager.AddListener(act, type, out monoPoolID);
        }
        public void RemoveMonoListener(CFAction act, UpdateType type, short monoID)
        {
            GameManager.MonoManager.RemoveListener(act, type, monoID);
        }
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return GameManager.MonoManager.StartCoroutine(routine);
        }
        public void StopCoroutine(IEnumerator routine)
        {
            GameManager.MonoManager.StopCoroutine(routine);
        }
        public void StopCoroutine(Coroutine routine)
        {
            GameManager.MonoManager.StopCoroutine(routine);
        }
        /// <summary>
        /// 嵌套协程
        /// </summary>
        /// <param name="routine">执行条件</param>
        /// <param name="callBack">执行条件结束后自动执行回调函数</param>
        /// <returns>Coroutine</returns>
        public Coroutine StartCoroutine(Coroutine routine, CFAction callBack)
        {
            return GameManager.MonoManager.StartCoroutine(routine, callBack);
        }
        public Coroutine DelayCoroutine(float delay, CFAction callBack)
        {
            return GameManager.MonoManager.DelayCoroutine(delay, callBack);
        }
        /// <summary>
        /// 条件函数；
        /// 当handler为true时，才执行callBack函数，期间协程挂起。
        /// </summary>
        /// <param name="handler">条件处理者</param>
        /// <param name="callBack">回调函数</param>
        /// <returns></returns>
        public Coroutine PredicateCoroutine(Func<bool> handler, CFAction callBack)
        {
            return GameManager.MonoManager.PredicateCoroutine(handler, callBack);
        }
        #endregion
        #region AudioManager
        public void PlayBackgroundAudio(GameEventArgs arg)
        {
            GameManager.AudioManager.PlayBackgroundAudio(arg);
        }
        public void PauseBackgroundAudio()
        {
            GameManager.AudioManager.PauseBackgroundAudio();
        }
        public void UnpauseBackgroundAudio()
        {
            GameManager.AudioManager.UnpauseBackgroundAudio();
        }
        public void StopBackgroundAudio()
        {
            GameManager.AudioManager.StopBackgroundAudio();
        }
        public void PlayWorldAudio(GameObject attachTarget, GameEventArgs arg)
        {
            GameManager.AudioManager.PlayWorldAudio(attachTarget, arg);
        }
        public void PauseWorldAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.PauseWorldAudio(attachTarget);
        }
        public void UnpauseWorldAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.UnpauseWorldAudio(attachTarget);
        }
        public void StopWorldAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.StopWorldAudio(attachTarget);
        }
        public void PlayMultipleAudio(GameObject attachTarget, GameEventArgs[] args)
        {
            GameManager.AudioManager.PlayMultipleAudio(attachTarget, args);
        }
        public void PauseMultipleAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.PauseMultipleAudio(attachTarget);
        }
        public void UnpauseMultipleAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.UnpauseMultipleAudio(attachTarget);
        }
        public void StopMultipleAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.StopMultipleAudio(attachTarget);
        }
        public void StopAllWorldAudio()
        {
            GameManager.AudioManager.StopAllWorldAudio();
        }
        public void SetAudioMuteState(bool state)
        {
            GameManager.AudioManager.Mute = state;
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
        public  T LoadResAsset<T>(string path, bool instantiateGameObject = false)
            where T : UnityEngine.Object
        {
            return GameManager.ResourceManager.LoadResAsset<T>(path, instantiateGameObject);
        }
        /// <summary>
        /// 异步加载资源,如果目标是Gameobject，则实例化
        /// </summary>
        public void LoadResAysnc<T>(string path, CFAction<T> callBack = null)
            where T : UnityEngine.Object
        {
            GameManager.ResourceManager.LoadResAysnc<T>(path, callBack);
        }
        /// <summary>
        /// 异步加载资源,不实例化任何类型
        /// </summary>
        public void LoadResAssetAysnc<T>(string path, CFAction<T> callBack = null)
            where T : UnityEngine.Object
        {
            GameManager.ResourceManager.LoadResAssetAysnc(path, callBack);
        }
        /// <summary>
        /// 使用unityEngine.Resources方法
        /// 载入resources文件夹下的指定文件夹下某一类型的所有资源
        /// </summary>
        public List<T> LoadResFolderAssets<T>(string path)
            where T : UnityEngine.Object
        {
            return GameManager.ResourceManager.LoadResFolderAssets<T>(path);
        }
        /// <summary>
        /// 使用unityEngine.Resources方法
        /// 载入resources文件夹下的指定文件夹下某一类型的所有资源
        /// </summary>
        public T[] LoadResAll<T>(string path)
            where T : UnityEngine.Object
        {
            return GameManager.ResourceManager.LoadResAll<T>(path);
        }
        #endregion
        #region 基于AssetBundle
        /// <summary>
        /// 异步加载AB 依赖包
        /// </summary>
        /// <param name="abName">ab包名称</param>
        public  void LoadDependenciesABAsync(string abName)
        {
            GameManager.ResourceManager.LoadDependenciesABAsync(abName);
        }
        /// <summary>
        /// 异步加载AB包，若不存在，则从web端加载
        /// </summary>
        /// <param name="abName">AssetBundle Name</param>
        /// <param name="isManifest">是否为AB清单</param>
        public void LoadABAsync(string abName, bool isManifest = false)
        {
            GameManager.ResourceManager.LoadABAsync(abName, isManifest);
        }
        /// <summary>
        /// 异步加载AB包清单
        /// </summary>
        public void LoadABManifestAsync()
        {
            GameManager.ResourceManager.LoadABManifestAsync();
        }
        public void UnloadAsset(string abName, bool unloadAllAssets = false)
        {
            GameManager.ResourceManager.UnloadAsset(abName, unloadAllAssets);
        }
        /// <summary>
        /// 卸载所有资源
        /// </summary>
        /// <param name="unloadAllAssets">是否卸所有实体对象</param>
        public void UnloadAllAsset(bool unloadAllAssets = false)
        {
            GameManager.ResourceManager.UnloadAllAsset(unloadAllAssets);
        }
        #endregion
        #endregion
        #region ScenesManager
        /// <summary>
        /// 同步加载 name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="callBack"></param>
        public  void LoadScene(string sceneName, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadScene(sceneName, callBack);
        }
        /// <summary>
        /// 同步加载 name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="callBack"></param>
        public  void LoadScene(string sceneName, bool additive, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadScene(sceneName, additive, callBack);
        }
        /// <summary>
        /// 同步加载 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="callBack"></param>
        public  void LoadScene(int sceneIndex, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadScene(sceneIndex, callBack);
        }
        /// <summary>
        /// 同步加载 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="callBack"></param>
        public  void LoadScene(int sceneIndex, bool additive, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadScene(sceneIndex, additive, callBack);
        }
        /// <summary>
        /// 异步加载 name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="callBack"></param>
        public  void LoadSceneAsync(string sceneName)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName);
        }
        /// <summary>
        /// 异步加载 name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="callBack"></param>
        public  void LoadSceneAsync(string sceneName, bool additive)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, additive);
        }
        public  void LoadSceneAsync(string sceneName, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, callBack);
        }
        public  void LoadSceneAsync(string sceneName, bool additive, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, additive, callBack);
        }
        public  void LoadSceneAsync(string sceneName, CFAction<float> callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, callBack);
        }
        public  void LoadSceneAsync(string sceneName, bool additive, CFAction<float> callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, additive, callBack);
        }
        public  void LoadSceneAsync(string sceneName, CFAction<AsyncOperation> callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, callBack);
        }
        public  void LoadSceneAsync(string sceneName, bool additive, CFAction<AsyncOperation> callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, additive, callBack);
        }
        public  void LoadSceneAsync(int sceneIndex)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex);
        }
        public  void LoadSceneAsync(int sceneIndex, bool additive)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, additive);
        }
        /// <summary>
        /// 异步加载 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="callBack"></param>
        public  void LoadSceneAsync(int sceneIndex, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, callBack);
        }
        public  void LoadSceneAsync(int sceneIndex, bool additive, CFAction<float> callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, additive, callBack);
        }
        /// <summary>
        /// 异步加载 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="callBack"></param>
        public  void LoadSceneAsync(int sceneIndex, CFAction<float> callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, callBack);
        }
        public  void LoadSceneAsync(int sceneIndex, bool additive, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, additive, callBack);
        }
        public  void LoadSceneAsync(int sceneIndex, CFAction<AsyncOperation> callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, callBack);
        }
        public  void LoadSceneAsync(int sceneIndex, bool additive, CFAction<AsyncOperation> callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, additive, callBack);
        }
        #endregion
        #region GameObjectPool
        /// <summary>
        /// 注册对象池
        /// </summary>
        /// <param name="objKey">池中的key</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <param name="onSpawn">生成时触发的事件，默认会将对象激活</param>
        /// <param name="onDespawn">回收时触发的事件，默认会将对象失活</param>
        public void RegisterObjcetSpawnPool(object objKey, GameObject spawnItem, CFAction<GameObject> onSpawn, CFAction<GameObject> onDespawn)
        {
            GameManager.ObjectPoolManager.RegisterSpawnPool(objKey, spawnItem, onSpawn, onDespawn);
        }
        public  void DeregisterObjectSpawnPool(object objKey)
        {
            GameManager.ObjectPoolManager.DeregisterSpawnPool(objKey);
        }
        public int GetObjectSpawnPoolItemCount(object objKey)
        {
            return GameManager.ObjectPoolManager.GetPoolCount(objKey);
        }
        public GameObject SpawnObject(object objKey)
        {
            return GameManager.ObjectPoolManager.Spawn(objKey);
        }
        public void DespawnObject(object objKey, GameObject go)
        {
            GameManager.ObjectPoolManager.Despawn(objKey, go);
        }
        public void DespawnObjects(object objKey, GameObject[] gos)
        {
            GameManager.ObjectPoolManager.Despawns(objKey, gos);
        }
        public void ClearObjectSpawnPool(object objKey)
        {
            GameManager.ObjectPoolManager.Clear(objKey);
        }
        public void ClearAllObjectSpawnPool()
        {
            GameManager.ObjectPoolManager.ClearAll();
        }
        public  void SetObjectSpawnItem(object objKey, GameObject go)
        {
            GameManager.ObjectPoolManager.SetSpawnItem(objKey, go);
        }
        /// <summary>
        /// 对象池生成对象在激活状态时所在的容器，场景中唯一，被销毁后依旧会创建
        /// </summary>
        /// <returns></returns>
        public  GameObject GetObjectSpawnPoolActiveMount()
        {
            return GameManager.ObjectPoolManager.ActiveObjectMount;
        }
        /// <summary>
        /// 生成对象但不经过池，通常用在一次性对象的产生上
        /// </summary>
        public  GameObject SpawnObjectNotUsePool(GameObject go, Transform spawnTransform)
        {
            return GameManager.ObjectPoolManager.SpawnNotUsePool(go, spawnTransform);
        }
        #endregion
        #region ControllerManager
        public  void RegisterController<T>(T controller)
            where T : ControllerBase
        {
            GameManager.ControllerManager.RegisterController<T>(controller);
        }
        /// <summary>
        /// 注销控制器，如果当前控制器是最后一个，则注销后，这个类别也会自动注销
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        public  void DeregisterController<T>(T controller)
            where T : ControllerBase
        {
            GameManager.ControllerManager.DeregisterController<T>(controller);
        }
        public  T GetController<T>(CFPredicateAction<T> predicate)
            where T : ControllerBase
        {
            return GameManager.ControllerManager.GetController<T>(predicate);
        }
        public T[] GetControllers<T>(CFPredicateAction<T> predicate)
            where T : ControllerBase
        {
            return GameManager.ControllerManager.GetControllers(predicate);
        }
        /// <summary>
        /// 获取当前注册的T类型重，controller所包含的数量
        /// </summary>
        public short GetControllerItemCount<T>()
        {
            return GameManager.ControllerManager.GetControllerItemCount<T>();
        }
        /// <summary>
        /// 获取当前注册的所有类型controller总数
        /// </summary>
        public short GetControllerTypeCount()
        {
            return GameManager.ControllerManager.GetControllerTypeCount();
        }
        public  bool HasController<T>()
            where T : ControllerBase
        {
            return GameManager.ControllerManager.HasController<T>();
        }
        public bool HasControllerItem<T>(T controller)
            where T : ControllerBase
        {
            return GameManager.ControllerManager.HasControllerItem(controller);
        }
        public void ClearAllController()
        {
            GameManager.ControllerManager.ClearAllController();
        }
        public void ClearControllerItem<T>()
            where T : ControllerBase
        {
            GameManager.ControllerManager.ClearControllerItem<T>();
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
        public  void SaveJsonDataToLocal<T>(string relativePath, string fileName, T dataSet, bool binary = false, CFAction callBack = null)
            where T : class, new()
        {
            GameManager.DataManager.SaveJsonDataToLocal(relativePath, fileName, dataSet, binary, callBack);
        }
        /// <summary>
        /// 从本地的绝对路径读取Json数据
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="binary">是否为二进制文件</param>
        /// <param name="callBack">回调函数，当读取成功后调用</param>
        /// <returns>返回一个Json</returns>
        public  string LoadJsonDataFromLocal(string relativePath, string fileName, bool binary = false, CFAction callBack = null)
        {
            return GameManager.DataManager.LoadJsonDataFromLocal(relativePath, fileName, binary, callBack);
        }
        /// <summary>
        /// 从本地的绝对路径读取Json数据
        /// </summary>
        /// <param name="fullRelativeFilePath">相对路径完整路径，包含到文件名后缀</param>
        /// <param name="binary">是否为二进制文件</param>
        /// <param name="callBack">回调函数，当读取成功后调用</param>
        /// <returns>返回一个Json</returns>
        public string LoadJsonDataFromLocal(string fullRelativeFilePath, bool binary = false, CFAction callBack = null)
        {
            return GameManager.DataManager.LoadJsonDataFromLocal(fullRelativeFilePath, binary, callBack);
        }
        /// <summary>
        /// 从Resource文件夹下读取Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="dataSet">存储json的类模型</param>
        /// <param name="callBack">回调函数</param>
        public  void ParseDataFromResource<T>(string relativePath, string fileName, ref T dataSet, CFAction<T> callBack = null)
          where T : class, new()
        {
            GameManager.DataManager.ParseDataFromResource(relativePath, fileName, ref dataSet, callBack);
        }
        #endregion
        #region ReferenceManager
        public  int GetReferencePoolCount<T>()
            where T : class, IReference, new()
        {
            return GameManager.ReferencePoolManager.GetPoolCount<T>();
        }
        public  T SpawnReference<T>()
            where T : class, IReference, new()
        {
            return GameManager.ReferencePoolManager.Spawn<T>();
        }
        public  IReference SpawnReference(Type type)
        {
            return GameManager.ReferencePoolManager.Spawn(type);
        }
        /// <summary>
        /// 生成引用接口
        /// </summary>
        /// <typeparam name="T">生成类型</typeparam>
        /// <returns> 返回生成后的接口类型对象</returns>
        public  IReference SpawnReferenceInterface<T>()
            where T : class, IReference, new()
        {
            return GameManager.ReferencePoolManager.SpawnInterface<T>();
        }
        public  void DespawnReference(IReference refer)
        {
            GameManager.ReferencePoolManager.Despawn(refer);
        }
        public void DespawnsReference(params IReference[] refer)
        {
            GameManager.ReferencePoolManager.Despawns(refer);
        }
        public void DespawnsReference<T>(List<T> refers)
            where T : class, IReference, new()
        {
            GameManager.ReferencePoolManager.Despawns<T>(refers);
        }
        public void DespawnsReference<T>(T[] refers)
            where T : class, IReference, new()
        {
            GameManager.ReferencePoolManager.Despawns<T>(refers);
        }
        public void ClearReferencePool(Type type)
        {
            GameManager.ReferencePoolManager.Clear(type);
        }
        public void ClearReferencePool<T>()
            where T : class, IReference, new()
        {
            GameManager.ReferencePoolManager.Clear<T>();
        }
        public void ClearAllReferencePool()
        {
            GameManager.ReferencePoolManager.ClearAll();
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
            GameManager.UIManager.LoadPanel(panelName, callBack);
        }
        /// <summary>
        /// 载入面板，若字典中已存在，则使用回调，并返回。若不存在，则异步加载且使用回调。
        /// 基于Resources
        /// </summary>
        public void ShowPanel<T>(string panelName, CFAction<T> callBack = null)
            where T : UILogicBase
        {
            GameManager.UIManager.ShowPanel(panelName, callBack);
        }
        public void HidePanel(string panelName)
        {
            GameManager.UIManager.HidePanel(panelName);
        }
        public void RemovePanel(string panelName)
        {
            GameManager.UIManager.RemovePanel(panelName);
        }
        public void HasPanel(string panelName)
        {
            GameManager.UIManager.HasPanel(panelName);
        }
        /// <summary>
        /// Resource文件夹相对路径
        /// 返回实例化的对象
        /// </summary>
        /// <param name="path">如UI\Canvas</param>
        public GameObject InitMainCanvas(string path)
        {
            return GameManager.UIManager.InitMainCanvas(path);
        }
        /// <summary>
        /// Resource文件夹相对路径
        /// 返回实例化的对象
        /// </summary>
        /// <param name="path">如UI\Canvas</param>
        /// <param name="name">生成后重命名的名称</param>
        public GameObject InitMainCanvas(string path, string name)
        {
            return GameManager.UIManager.InitMainCanvas(path, name);
        }
        public GameObject GetMainCanvas()
        {
            return GameManager.UIManager.MainUICanvas;
        }
        #endregion
        #region FSMManager
        public  FSMBase GetIndividualFSM<T>()
        where T : class
        {
            return GameManager.FSMManager.GetIndividualFSM<T>();
        }
        public FSMBase GetIndividualFSM(Type type)
        {
            return GameManager.FSMManager.GetIndividualFSM(type);
        }
        /// <summary>
        /// 获取某类型状态机元素集合中元素的个数
        /// </summary>
        /// <typeparam name="T">拥有者</typeparam>
        /// <returns>元素数量</returns>
        public  int GetFSMSetElementCount<T>()
    where T : class
        {
            return GameManager.FSMManager.GetFSMSetElementCount<T>();
        }
        public int GetFSMSetElementCount(Type type)
        {
            return GameManager.FSMManager.GetFSMSetElementCount(type);
        }
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>状态机集合</returns>
        public List<FSMBase> GetFSMSet<T>()
            where T : class
        {
            return GameManager.FSMManager.GetFSMSet<T>();
        }
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <param name="type">类型对象</param>
        /// <returns>状态机集合</returns>
        public List<FSMBase> GetFSMSet(Type type)
        {
            return GameManager.FSMManager.GetFSMSet(type);
        }
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        public FSMBase GetSetElementFSM<T>(Predicate<FSMBase> predicate)
            where T : class
        {
            return GameManager.FSMManager.GetSetElementFSM<T>(predicate);
        }
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        public FSMBase GetSetElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            return GameManager.FSMManager.GetSetElementFSM(type, predicate);
        }
        public FSMBase[] GetAllIndividualFSM()
        {
            return GameManager.FSMManager.GetAllIndividualFSM();
        }
        public bool HasIndividualFSM<T>()
            where T : class
        {
            return GameManager.FSMManager.HasIndividualFSM<T>();
        }
        public bool HasIndividualFSM(Type type)
        {
            return GameManager.FSMManager.HasIndividualFSM(type);
        }
        /// <summary>
        /// 是否拥有指定类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>是否存在</returns>
        public bool HasFSMSet<T>()
            where T : class
        {
            return GameManager.FSMManager.HasFSMSet<T>();
        }
        public bool HasFSMSet(Type type)
        {
            return GameManager.FSMManager.HasFSMSet(type);
        }
        public bool HasSetElementFSM<T>(Predicate<FSMBase> predicate)
     where T : class
        {
            return GameManager.FSMManager.HasSetElementFSM<T>(predicate);
        }
        public bool HasSetElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            return GameManager.FSMManager.HasSetElementFSM(type, predicate);
        }
        public bool HasSetElementFSM<T>(FSMBase fsm)
             where T : class
        {
            return GameManager.FSMManager.HasSetElementFSM<T>(fsm);
        }
        public bool HasSetElementFSM(Type type, FSMBase fsm)
        {
            return GameManager.FSMManager.HasSetElementFSM(type, fsm);
        }
        /// <summary>
        /// 创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="Individual">是否为独立状态机</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(T owner, bool Individual, params FSMState<T>[] states)
            where T : class
        {
            return GameManager.FSMManager.CreateFSM<T>(owner, Individual, states);
        }
        public IFSM<T> CreateFSM<T>(string name, T owner, bool Individual, params FSMState<T>[] states)
             where T : class
        {
            return GameManager.FSMManager.CreateFSM<T>(name, owner, Individual, states);
        }
        public IFSM<T> CreateFSM<T>(T owner, bool Individual, List<FSMState<T>> states)
            where T : class
        {
            return GameManager.FSMManager.CreateFSM<T>(owner, Individual, states);
        }
        /// <summary>
        /// 创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="Individual">是否为独立状态机</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(string name, T owner, bool Individual, List<FSMState<T>> states)
             where T : class
        {
            return GameManager.FSMManager.CreateFSM<T>(name, owner, Individual, states);
        }
        /// <summary>
        /// 销毁独立的状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DestoryIndividualFSM<T>()
             where T : class
        {
            GameManager.FSMManager.DestoryIndividualFSM<T>();
        }
        public void DestoryIndividualFSM(Type type)
        {
            GameManager.FSMManager.DestoryIndividualFSM(type);
        }
        public  void DestoryFSMSet<T>()
where T : class
        {
            GameManager.FSMManager.DestoryFSMSet<T>();
        }
        public void DestoryFSMSet(Type type)
        {
            GameManager.FSMManager.DestoryFSMSet(type);
        }
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <typeparam name="T">拥有者</typeparam>
        /// <param name="predicate">查找条件</param>
        public void DestorySetElementFSM<T>(Predicate<FSMBase> predicate)
            where T : class
        {
            GameManager.FSMManager.DestorySetElementFSM<T>(predicate);
        }
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找条件</param>
        public void DestorySetElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            GameManager.FSMManager.DestorySetElementFSM(type, predicate);
        }
        public void ShutdownFSMSet<T>()
            where T : class
        {
            GameManager.FSMManager.ShutdownFSMSet<T>();
        }
        /// <summary>
        /// 终止某一类型的状态机集合
        /// </summary>
        /// <param name="type">拥有者类型</param>
        public void ShutdownFSMSet(Type type)
        {
            GameManager.FSMManager.ShutdownFSMSet(type);
        }
        /// <summary>
        /// 终止独立的状态机群组
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        public void ShutdownIndividualFSM<T>()
            where T : class
        {
            GameManager.FSMManager.ShutdownIndividualFSM<T>();
        }
        public void ShutdownIndividualFSM(Type type)
        {
            GameManager.FSMManager.ShutdownIndividualFSM(type);
        }
        public void ShutdownAllFSM()
        {
            GameManager.FSMManager.ShutdownAllFSM();
        }
        #endregion
        #region EntityManager
        public int GetEntityCount<T>()
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.GetEntityCount<T>();
        }
        public  int GetEntityCount(Type type)
        {
            return GameManager.EntityManager.GetEntityCount(type);
        }
        public   void AttachEntity<T>(int entityID, Transform target)
            where T : EntityObject, new()
        {
            GameManager.EntityManager.AttachEntity<T>(entityID, target);
        }
        public   void AttachEntity(Type type, int entityID, Transform target)
        {
            GameManager.EntityManager.AttachEntity(type, entityID, target);
        }
        public   void DetachEntity<T>(int entityID)
            where T : EntityObject
        {
            GameManager.EntityManager.DetachEntity<T>(entityID);
        }
        public   void DetachEntity(Type type, int entityID)
        {
            GameManager.EntityManager.DetachEntity(type, entityID);
        }
        public   void AddEntity<T>(int entityID, IEntityObject entity)
            where T : EntityObject, new()
        {
            GameManager.EntityManager.AddEntity<T>(entityID, entity);
        }
        public   void AddEntity(Type type, int entityID, IEntityObject entity)
        {
            GameManager.EntityManager.AddEntity(type, entityID, entity);
        }
        /// <summary>
        /// 移除entity，并获得这个entity下的gameobject对象;
        /// 返回是否移除Entity对象成功，GameObject entity可能为空
        /// </summary>
        /// <typeparam name="T">entity类型</typeparam>
        /// <param name="entityID">实体ID</param>
        /// <param name="entity">实体的对象，可能为空</param>
        /// <returns>是否移除成功</returns>
        public  bool RemoveEntity<T>(int entityID, out GameObject entity)
      where T : EntityObject, new()
        {
            return GameManager.EntityManager.RemoveEntity<T>(entityID, out entity);
        }
        public bool RemoveEntity(Type type, int entityID, out GameObject entity)
        {
            return GameManager.EntityManager.RemoveEntity(type, entityID, out entity);
        }
        /// <summary>
        /// 移除实体对象
        /// 实体对象被移除后，自动被引用池回收
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entityID">实体ID</param>
        /// <returns>返回是否移除成功</returns>
        public   bool RemoveEntity<T>(int entityID)
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.RemoveEntity<T>(entityID);
        }
        public   bool RemoveEntity(Type type, int entityID)
        {
            return GameManager.EntityManager.RemoveEntity(type, entityID);
        }
        public  IEntityObject GetEntity<T>(int entityID)
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.GetEntity<T>(entityID);
        }
        public IEntityObject GetEntity(Type type, int entityID)
        {
            return GameManager.EntityManager.GetEntity(type, entityID);
        }
        public   ICollection<IEntityObject> GetEntityCollection<T>()
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.GetEntityCollection<T>();
        }
        public   ICollection<IEntityObject> GetEntityCollection(Type type)
        {
            return GameManager.EntityManager.GetEntityCollection(type);
        }
        public   IEntityObject[] GetEntities<T>()
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.GetEntities<T>();
        }
        public   IEntityObject[] GetEntities(Type type)
        {
            return GameManager.EntityManager.GetEntities(type);
        }
        public   List<IEntityObject> GetEntityList<T>()
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.GetEntityList<T>();
        }
        public   List<IEntityObject> GetEntityList(Type type)
        {
            return GameManager.EntityManager.GetEntityList(type);
        }
        public   bool SetEntity<T>(int entityID, GameObject entityObject)
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.SetEntity<T>(entityID, entityObject);
        }
        public   bool SetEntity(Type type, int entityID, GameObject entityObject)
        {
            return GameManager.EntityManager.SetEntity(type, entityID, entityObject);
        }
        public  void RegisterEntity<T>()
            where T : EntityObject, new()
        {
            GameManager.EntityManager.RegisterEntity<T>();
        }
        public   void RegisterEntity(Type type)
        {
            GameManager.EntityManager.RegisterEntity(type);
        }
        public  bool DeregisterEntity<T>()
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.DeregisterEntity<T>();
        }
        public   bool DeregisterEntity(Type type)
        {
            return GameManager.EntityManager.DeregisterEntity(type);
        }
        public  bool ActiveEntity<T>(int entityID)
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.ActiveEntity<T>(entityID);
        }
        public   bool ActiveEntity(Type type, int entityID)
        {
            return GameManager.EntityManager.ActiveEntity(type, entityID);
        }
        public   void ActiveEntities<T>()
            where T : EntityObject, new()
        {
            GameManager.EntityManager.ActiveEntities<T>();
        }
        public void ActiveEntities(Type type)
        {
            GameManager.EntityManager.ActiveEntities(type);
        }
        public   bool DeactiveEntity<T>(int entityID)
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.DeactiveEntity<T>(entityID);
        }
        public  bool DeactiveEntity(Type type, int entityID)
        {
            return GameManager.EntityManager.DeactiveEntity(type, entityID);
        }
        public void DeactiveEntities<T>()
            where T : EntityObject, new()
        {
            GameManager.EntityManager.DeactiveEntities<T>();
        }
        public void DeactiveEntities(Type type)
        {
            GameManager.EntityManager.DeactiveEntities(type);
        }
        public bool HasEntity<T>(int entityID)
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.HasEntity<T>(entityID);
        }
        public  bool HasEntity(Type type, int entityID)
        {
            return GameManager.EntityManager.HasEntity(type, entityID);
        }
        public bool HasEntityType<T>()
            where T : EntityObject, new()
        {
            return GameManager.EntityManager.HasEntityType<T>();
        }
        public  bool HasEntityType(Type type)
        {
            return GameManager.EntityManager.HasEntityType(type);
        }
        #endregion
    }
}