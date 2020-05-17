using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cosmos.UI;
using Cosmos.FSM;
using System.Reflection;
namespace Cosmos
{
    /// <summary>
    /// CosmosFramework外观类，封装模块的功能，进行解耦
    /// 所有调用功能都通过这个外观类与模块进行沟通
    /// </summary>
    public sealed partial class Facade
    {
        #region FacadeMethods
        public static void InitAllModule()
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
        public static void RegisterModule(string moduleName)
        {
            InitModule(moduleName);
        }
        //TODO 反射初始化需要为AOT做出预留，专门设计为IOS系统的symbol
        static IModule InitModule(string moduleName)
        {
#if UNITY_EDITOR||UNITY_EDITOR_WIN||UNITY_STANDALONE_WIN||UNITY_ANDROID
            var result = GameManager.Instance.GetModule(moduleName);
            if (result == null)
            {
                var moduleResult = Utility.Assembly.GetTypeInstance<IModule>(Assembly.GetAssembly(typeof(Facade)), Utility.Framework.GetModuleTypeFullName(moduleName));
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
        static IModule InitModuleForIOS(string moduleName)
        {
            switch (moduleName)
            {
                case CFModules.AUDIO:
                    GameManager.AudioManager.DebugModule();
                    break;
                case CFModules.CONFIG:
                    GameManager.ConfigManager.DebugModule();
                    break;
                case CFModules.CONTROLLER:
                    GameManager.ControllerManager.DebugModule();
                    break;
                case CFModules.DATA:
                    GameManager.DataManager.DebugModule();
                    break;
                case CFModules.ENTITY:
                    GameManager.EntityManager.DebugModule();
                    break;
                case CFModules.EVENT:
                    GameManager.EventManager.DebugModule();
                    break;
                case CFModules.FSM:
                    GameManager.FSMManager.DebugModule();
                    break;
                case CFModules.INPUT:
                    GameManager.InputManager.DebugModule();
                    break;
                case CFModules.MONO:
                    GameManager.MonoManager.DebugModule();
                    break;
                case CFModules.NETWORK:
                    GameManager.NetworkManager.DebugModule();
                    break;
                case CFModules.OBJECTPOOL:
                    GameManager.ObjectPoolManager.DebugModule();
                    break;
                case CFModules.REFERENCE:
                    GameManager.ReferencePoolManager.DebugModule();
                    break;
                case CFModules.RESOURCE:
                    GameManager.ResourceManager.DebugModule();
                    break;
                case CFModules.SCENE:
                    GameManager.SceneManager.DebugModule();
                    break;
                case CFModules.UI:
                    GameManager.UIManager.DebugModule();
                    break;
            }
            var result = GameManager.Instance.GetModule(moduleName);
            return result;
        }
        public static IModule GetModule(string moduleName)
        {
            var moduleResult = GameManager.Instance.GetModule(moduleName);
            if (moduleResult == null)
            {
                moduleResult = InitModule(moduleName);
            }
            return moduleResult;
        }
        public static bool HasModule(string moduleName)
        {
            return GameManager.Instance.HasModule(moduleName);
        }
        #endregion
        #region InputManager
        /// <summary>
        /// 虚拟轴线是否存在
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualAxis(string name){return GameManager.InputManager.IsExistVirtualAxis(name);}
        /// <summary>
        /// 虚拟按键是否存在
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualButton(string name){return GameManager.InputManager.IsExistVirtualButton(name);}
        /// <summary>
        /// 注册虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public void RegisterVirtualButton(string name){GameManager.InputManager.RegisterVirtualButton(name);}
        /// <summary>
        /// 注销虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public void DeregisterVirtualButton(string name){GameManager.InputManager.DeregisterVirtualButton(name);}
        /// <summary>
        /// 注册虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public void RegisterVirtualAxis(string name){GameManager.InputManager.RegisterVirtualAxis(name);}
        /// <summary>
        /// 注销虚拟轴线
        /// </summary>
        /// <param name="name">按键名称</param>
        public void DeregisterVirtualAxis(string name){GameManager.InputManager.DeregisterVirtualAxis(name);}
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector3 MousePosition() { return GameManager.InputManager.MousePosition; }
        /// <summary>
        /// 获得轴线
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns></returns>
        public float GetAxis(string name){ return GameManager.InputManager.GetAxis(name);}
        /// <summary>
        /// 未插值的输入 -1，0 ，1
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns></returns>
        public float GetAxisRaw(string name){return GameManager.InputManager.GetAxisRaw(name); }
        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonDown(string name){ return GameManager.InputManager.GetButtonDown(name); }
        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public bool GetButton(string name){return GameManager.InputManager.GetButton(name);}
        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonUp(string name){return GameManager.InputManager.GetButtonUp(name);}
        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonDown(string name){GameManager.InputManager.SetButtonDown(name);}
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonUp(string name){GameManager.InputManager.SetButtonUp(name);}
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        public void SetVirtualMousePosition(Vector3 value){GameManager.InputManager.SetVirtualMousePosition(value);}
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        public void SetVirtualMousePosition(float x, float y, float z){GameManager.InputManager.SetVirtualMousePosition(x, y, z);}
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisPositive(string name){GameManager.InputManager.SetAxisPositive(name);}
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisNegative(string name){GameManager.InputManager.SetAxisNegative(name);}
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisZero(string name){GameManager.InputManager.SetAxisZero(name);}
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">值</param>
        public void SetAxis(string name, float value){GameManager.InputManager.SetAxis(name, value);}

        #endregion
        #region EventManager
        public static void AddEventListener(string eventKey, CFAction<object, GameEventArgs> handler)
        {
            GameManager.EventManager.AddListener(eventKey, handler);
        }
        public static void RemoveEventListener(string eventKey, CFAction<object, GameEventArgs> hander)
        {
            GameManager.EventManager.RemoveListener(eventKey, hander);
        }
        public static void DispatchEvent(string eventKey, object sender, GameEventArgs arg)
        {
            GameManager.EventManager.DispatchEvent(eventKey, sender, arg);
        }
        public static void RegisterEvent(string eventKey)
        {
            GameManager.EventManager.RegisterEvent(eventKey);
        }
        public static void DeregisterEvent(string eventKey)
        {
            GameManager.EventManager.DeregisterEvent(eventKey);
        }
        public static void ClearEvent(string eventKey)
        {
            GameManager.EventManager.ClearEvent(eventKey);
        }
        public static void ClearAllEvent()
        {
            GameManager.EventManager.ClearAllEvent();
        }
        public static bool HasEvent(string eventKey)
        {
            return GameManager.EventManager.HasEvent(eventKey);
        }
        #endregion
        #region MonoManager
        public static void AddMonoListener(CFAction act, UpdateType type, CFAction<short> callBack = null)
        {
            GameManager.MonoManager.AddListener(act, type, callBack);
        }
        public static void RemoveMonoListener(CFAction act, UpdateType type, short monoID)
        {
            GameManager.MonoManager.RemoveListener(act, type, monoID);
        }
        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return GameManager.MonoManager.StartCoroutine(routine);
        }
        public static void StopCoroutine(IEnumerator routine)
        {
            GameManager.MonoManager.StopCoroutine(routine);
        }
        public static void StopCoroutine(Coroutine routine)
        {
            GameManager.MonoManager.StopCoroutine(routine);
        }
        /// <summary>
        /// 嵌套协程
        /// </summary>
        /// <param name="routine">执行条件</param>
        /// <param name="callBack">执行条件结束后自动执行回调函数</param>
        /// <returns>Coroutine</returns>
        public static Coroutine StartCoroutine(Coroutine routine, CFAction callBack)
        {
            return GameManager.MonoManager.StartCoroutine(routine, callBack);
        }
        public static Coroutine DelayCoroutine(float delay)
        {
            return GameManager.MonoManager.DelayCoroutine(delay);
        }
        public static Coroutine DelayCoroutine(float delay, CFAction callBack)
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
        public static Coroutine PredicateCoroutine(Func<bool> handler, CFAction callBack)
        {
            return GameManager.MonoManager.PredicateCoroutine(handler, callBack);
        }
        #endregion
        #region AudioManager
        public static void PlayBackgroundAudio(GameEventArgs arg)
        {
            GameManager.AudioManager.PlayBackgroundAudio(arg);
        }
        public static void PauseBackgroundAudio()
        {
            GameManager.AudioManager.PauseBackgroundAudio();
        }
        public static void UnpauseBackgroundAudio()
        {
            GameManager.AudioManager.UnpauseBackgroundAudio();
        }
        public static void StopBackgroundAudio()
        {
            GameManager.AudioManager.StopBackgroundAudio();
        }
        public static void PlayWorldAudio(GameObject attachTarget, GameEventArgs arg)
        {
            GameManager.AudioManager.PlayWorldAudio(attachTarget, arg);
        }
        public static void PauseWorldAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.PauseWorldAudio(attachTarget);
        }
        public static void UnpauseWorldAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.UnpauseWorldAudio(attachTarget);
        }
        public static void StopWorldAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.StopWorldAudio(attachTarget);
        }
        public static void PlayMultipleAudio(GameObject attachTarget, GameEventArgs[] args)
        {
            GameManager.AudioManager.PlayMultipleAudio(attachTarget, args);
        }
        public static void PauseMultipleAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.PauseMultipleAudio(attachTarget);
        }
        public static void UnpauseMultipleAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.UnpauseMultipleAudio(attachTarget);
        }
        public static void StopMultipleAudio(GameObject attachTarget)
        {
            GameManager.AudioManager.StopMultipleAudio(attachTarget);
        }
        public static void StopAllWorldAudio()
        {
            GameManager.AudioManager.StopAllWorldAudio();
        }
        public static void SetAudioMuteState(bool state)
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
        public static T LoadResAsset<T>(string path, bool instantiateGameObject = false)
            where T : UnityEngine.Object
        {
            return GameManager.ResourceManager.LoadResAsset<T>(path, instantiateGameObject);
        }
        /// <summary>
        /// 异步加载资源,如果目标是Gameobject，则实例化
        /// </summary>
        public static void LoadResAysnc<T>(string path, CFAction<T> callBack = null)
            where T : UnityEngine.Object
        {
            GameManager.ResourceManager.LoadResAysnc<T>(path, callBack);
        }
        /// <summary>
        /// 异步加载资源,不实例化任何类型
        /// </summary>
        public static void LoadResAssetAysnc<T>(string path, CFAction<T> callBack = null)
            where T : UnityEngine.Object
        {
            GameManager.ResourceManager.LoadResAssetAysnc(path, callBack);
        }
        /// <summary>
        /// 使用unityEngine.Resources方法
        /// 载入resources文件夹下的指定文件夹下某一类型的所有资源
        /// </summary>
        public static List<T> LoadResFolderAssets<T>(string path)
            where T : UnityEngine.Object
        {
            return GameManager.ResourceManager.LoadResFolderAssets<T>(path);
        }
        /// <summary>
        /// 使用unityEngine.Resources方法
        /// 载入resources文件夹下的指定文件夹下某一类型的所有资源
        /// </summary>
        public static T[] LoadResAll<T>(string path)
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
        public static void LoadDependenciesABAsync(string abName)
        {
            GameManager.ResourceManager.LoadDependenciesABAsync(abName);
        }
        /// <summary>
        /// 异步加载AB包，若不存在，则从web端加载
        /// </summary>
        /// <param name="abName">AssetBundle Name</param>
        /// <param name="isManifest">是否为AB清单</param>
        public static void LoadABAsync(string abName, bool isManifest = false)
        {
            GameManager.ResourceManager.LoadABAsync(abName, isManifest);
        }
        /// <summary>
        /// 异步加载AB包清单
        /// </summary>
        public static void LoadABManifestAsync()
        {
            GameManager.ResourceManager.LoadABManifestAsync();
        }
        public static void UnloadAsset(string abName, bool unloadAllAssets = false)
        {
            GameManager.ResourceManager.UnloadAsset(abName, unloadAllAssets);
        }
        /// <summary>
        /// 卸载所有资源
        /// </summary>
        /// <param name="unloadAllAssets">是否卸所有实体对象</param>
        public static void UnloadAllAsset(bool unloadAllAssets = false)
        {
            GameManager.ResourceManager.UnloadAllAsset(unloadAllAssets);
        }
        #endregion
        #endregion
        #region ScenesManager
        public static void LoadScene(string sceneName, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadScene(sceneName, callBack);
        }
        public static void LoadScene(int sceneIndex, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadScene(sceneIndex, callBack);
        }
        public static void LoadSceneAsync(string sceneName)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName);
        }
        /// <summary>
        /// 回调函数只在完成后进行一次回调
        /// </summary>
        public static void LoadSceneAsync(string sceneName, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, callBack);
        }
        /// <summary>
        /// 回调函数每次yield更新都会调用
        /// ，不会进行完成后的调用
        /// </summary>
        public static void LoadSceneAsync(string sceneName, CFAction<float> callBack)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, callBack);
        }
        /// <summary>
        /// 回调函数每次yield更新都会调用
        /// ，不会进行完成后的调用
        /// </summary>
        public static void LoadSceneAsync(string sceneName, CFAction<AsyncOperation> callBack)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, callBack);
        }
        public static void LoadSceneAsync(int sceneIndex)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex);
        }
        /// <summary>
        /// 回调函数只在完成后进行一次回调
        /// </summary>
        public static void LoadSceneAsync(int sceneIndex, CFAction callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, callBack);
        }
        /// <summary>
        /// 回调函数每次yield更新都会调用
        /// ，不会进行完成后的调用
        /// </summary>
        public static void LoadSceneAsync(int sceneIndex, CFAction<float> callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, callBack);
        }
        /// <summary>
        /// 回调函数每次yield更新都会调用
        /// ，不会进行完成后的调用
        /// </summary>
        public static void LoadSceneAsync(int sceneIndex, CFAction<AsyncOperation> callBack = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, callBack);
        }
        #endregion
        #region GameObjectPool
        public static void RegisterObjcetSpawnPool(object objKey, GameObject spawnItem, CFAction<GameObject> onSpawn, CFAction<GameObject> onDespawn)
        {
            GameManager.ObjectPoolManager.RegisterSpawnPool(objKey, spawnItem, onSpawn, onDespawn);
        }
        public static void DeregisterObjectSapwnPool(object objKey)
        {
            GameManager.ObjectPoolManager.DeregisterSpawnPool(objKey);
        }
        public static int GetObjectSpawnPoolItemCount(object objKey)
        {
            return GameManager.ObjectPoolManager.GetPoolCount(objKey);
        }
        public static GameObject SpawnObject(object objKey)
        {
            return GameManager.ObjectPoolManager.Spawn(objKey);
        }
        public static void DespawnObject(object objKey, GameObject go)
        {
            GameManager.ObjectPoolManager.Despawn(objKey, go);
        }
        public static void DespawnObjects(object objKey, GameObject[] gos)
        {
            GameManager.ObjectPoolManager.Despawns(objKey, gos);
        }
        public static void ClearObjectSpawnPool(object objKey)
        {
            GameManager.ObjectPoolManager.Clear(objKey);
        }
        public static void ClearAllObjectSpawnPool()
        {
            GameManager.ObjectPoolManager.ClearAll();
        }
        public static void SetObjectSpawnItem(object objKey, GameObject go)
        {
            GameManager.ObjectPoolManager.SetSpawnItem(objKey, go);
        }
        /// <summary>
        /// 对象池生成对象在激活状态时所在的容器，场景中唯一，被销毁后依旧会创建
        /// </summary>
        /// <returns></returns>
        public static GameObject GetObjectSpawnPoolActiveMount()
        {
            return GameManager.ObjectPoolManager.ActiveObjectMount;
        }
        /// <summary>
        /// 生成对象但不经过池，通常用在一次性对象的产生上
        /// </summary>
        public static GameObject SpawnObjectNotUsePool(GameObject go, Transform spawnTransform)
        {
            return GameManager.ObjectPoolManager.SpawnNotUsePool(go, spawnTransform);
        }
        #endregion
        #region ControllerManager
        public static void RegisterController<T>(T controller)
            where T : CFController
        {
            GameManager.ControllerManager.RegisterController<T>(controller);
        }
        /// <summary>
        /// 注销控制器，如果当前控制器是最后一个，则注销后，这个类别也会自动注销
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        public static void DeregisterController<T>(T controller)
            where T : CFController
        {
            GameManager.ControllerManager.DeregisterController<T>(controller);
        }
        public static T GetController<T>(CFPredicateAction<T> predicate)
            where T : CFController
        {
            return GameManager.ControllerManager.GetController<T>(predicate);
        }
        public static T[] GetControllers<T>(CFPredicateAction<T> predicate)
            where T : CFController
        {
            return GameManager.ControllerManager.GetControllers(predicate);
        }
        /// <summary>
        /// 获取当前注册的T类型重，controller所包含的数量
        /// </summary>
        public static short GetControllerItemCount<T>()
        {
            return GameManager.ControllerManager.GetControllerItemCount<T>();
        }
        /// <summary>
        /// 获取当前注册的所有类型controller总数
        /// </summary>
        public static short GetControllerTypeCount()
        {
            return GameManager.ControllerManager.GetControllerTypeCount();
        }
        public static bool HasController<T>()
            where T : CFController
        {
            return GameManager.ControllerManager.HasController<T>();
        }
        public static bool HasControllerItem<T>(T controller)
            where T : CFController
        {
            return GameManager.ControllerManager.HasControllerItem(controller);
        }
        public static void ClearAllController()
        {
            GameManager.ControllerManager.ClearAllController();
        }
        public static void ClearControllerItem<T>()
            where T : CFController
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
        public static void SaveJsonDataToLocal<T>(string relativePath, string fileName, T dataSet, bool binary = false, CFAction callBack = null)
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
        public static string LoadJsonDataFromLocal(string relativePath, string fileName, bool binary = false, CFAction callBack = null)
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
        public static string LoadJsonDataFromLocal(string fullRelativeFilePath, bool binary = false, CFAction callBack = null)
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
        public static void ParseDataFromResource<T>(string relativePath, string fileName, ref T dataSet, CFAction<T> callBack = null)
          where T : class, new()
        {
            GameManager.DataManager.ParseDataFromResource(relativePath, fileName, ref dataSet, callBack);
        }
        #endregion
        #region ReferenceManager
        public static int GetReferencePoolCount<T>()
            where T : class, IReference, new()
        {
            return GameManager.ReferencePoolManager.GetPoolCount<T>();
        }
        public static T SpawnReference<T>()
            where T : class, IReference, new()
        {
            return GameManager.ReferencePoolManager.Spawn<T>();
        }
        public static IReference SpawnReference(Type type)
        {
            return GameManager.ReferencePoolManager.Spawn(type);
        }
        /// <summary>
        /// 生成引用接口
        /// </summary>
        /// <typeparam name="T">生成类型</typeparam>
        /// <returns> 返回生成后的接口类型对象</returns>
        public static IReference SpawnReferenceInterface<T>()
            where T : class, IReference, new()
        {
            return GameManager.ReferencePoolManager.SpawnInterface<T>();
        }
        public static void DespawnReference(IReference refer)
        {
            GameManager.ReferencePoolManager.Despawn(refer);
        }
        public static void DespawnsReference(params IReference[] refer)
        {
            GameManager.ReferencePoolManager.Despawns(refer);
        }
        public static void DespawnsReference<T>(List<T> refers)
            where T : class, IReference, new()
        {
            GameManager.ReferencePoolManager.Despawns<T>(refers);
        }
        public static void DespawnsReference<T>(T[] refers)
            where T : class, IReference, new()
        {
            GameManager.ReferencePoolManager.Despawns<T>(refers);
        }
        public static void ClearReferencePool(Type type)
        {
            GameManager.ReferencePoolManager.Clear(type);
        }
        public static void ClearReferencePool<T>()
            where T : class, IReference, new()
        {
            GameManager.ReferencePoolManager.Clear<T>();
        }
        public static void ClearAllReferencePool()
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
        public static void LoadPanel<T>(string panelName, CFAction<T> callBack = null)
                   where T : UILogicBase
        {
            GameManager.UIManager.LoadPanel(panelName, callBack);
        }
        /// <summary>
        /// 载入面板，若字典中已存在，则使用回调，并返回。若不存在，则异步加载且使用回调。
        /// 基于Resources
        /// </summary>
        public static void ShowPanel<T>(string panelName, CFAction<T> callBack = null)
            where T : UILogicBase
        {
            GameManager.UIManager.ShowPanel(panelName, callBack);
        }
        public static void HidePanel(string panelName)
        {
            GameManager.UIManager.HidePanel(panelName);
        }
        public static void RemovePanel(string panelName)
        {
            GameManager.UIManager.RemovePanel(panelName);
        }
        public static void HasPanel(string panelName)
        {
            GameManager.UIManager.HasPanel(panelName);
        }
        /// <summary>
        /// Resource文件夹相对路径
        /// 返回实例化的对象
        /// </summary>
        /// <param name="path">如UI\Canvas</param>
        public static GameObject InitMainCanvas(string path)
        {
            return GameManager.UIManager.InitMainCanvas(path);
        }
        /// <summary>
        /// Resource文件夹相对路径
        /// 返回实例化的对象
        /// </summary>
        /// <param name="path">如UI\Canvas</param>
        /// <param name="name">生成后重命名的名称</param>
        public static GameObject InitMainCanvas(string path, string name)
        {
            return GameManager.UIManager.InitMainCanvas(path, name);
        }
        #endregion
        #region FSMManager
        public static FSMBase GetFSM<T>()
            where T : FSMBase
        {
            return GameManager.FSMManager.GetFSM<T>();
        }
        public static FSMBase GetFSM(Type type)
        {
            return GameManager.FSMManager.GetFSM(type);
        }
        public static FSMBase[] GetAllFSM<T>()
            where T : FSMBase
        {
            return GameManager.FSMManager.GetAllFSM<T>();
        }
        public static FSMBase[] GetAllFSM(Type type)
        {
            return GameManager.FSMManager.GetAllFSM(type);
        }
        public static bool HasFSM<T>()
            where T : FSMBase
        {
            return GameManager.FSMManager.HasFSM<T>();
        }
        public static bool HasFSM(Type type)
        {
            return GameManager.FSMManager.HasFSM(type);
        }
        public static IFSM<T> CreateFSM<T>(T owner, params FSMState<T>[] states)
            where T : class
        {
            return GameManager.FSMManager.CreateFSM(owner, states);
        }
        public static IFSM<T> CreateFSM<T>(string name, T owner, params FSMState<T>[] states)
            where T : class
        {
            return GameManager.FSMManager.CreateFSM<T>(name, owner, states);
        }
        public static IFSM<T> CreateFSM<T>(T owner, List<FSMState<T>> states)
            where T : FSMBase
        {
            return GameManager.FSMManager.CreateFSM(owner, states);
        }
        public static IFSM<T> CreateFSM<T>(string name, T owner, List<FSMState<T>> states)
         where T : class
        {
            return GameManager.FSMManager.CreateFSM(name, owner, states);
        }
        public static void DestoryFSM<T>()
         where T : class
        {
            GameManager.FSMManager.DestoryFSM<T>();
        }
        public static void DestoryFSM(Type type)
        {
            GameManager.FSMManager.DestoryFSM(type);
        }
        public static void ShutdownAllFSM()
        {
            GameManager.FSMManager.ShutdownAllFSM();
        }
        #endregion
    }
}