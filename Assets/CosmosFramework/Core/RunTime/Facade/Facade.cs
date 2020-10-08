using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cosmos.UI;
using Cosmos.FSM;
using Cosmos.Input;
using System.Reflection;
using Cosmos.Entity;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Net.Sockets;
using System.Net;
using Cosmos.Config;

namespace Cosmos
{
    /// <summary>
    /// CosmosFramework外观类，封装模块的功能，Action
    /// 所有调用功能都通过这个外观类与模块进行沟通
    /// </summary>
    public static partial class Facade
    {
        /// <summary>
        /// 检测框架提供的模块；
        /// </summary>
        public static void CheckCosmosModule()
        {
            GameManager.CheckModule();
        }
        #region CustomeModule
        /// <summary>
        /// 线程安全；
        /// 获取自定义模块；
        /// 需要从Module类派生;
        /// 此类模块不由CF框架生成，由用户自定义
        /// </summary>
        /// <typeparam name="TModule">实现模块功能的类对象</typeparam>
        /// <returns>获取的模块</returns>
        public static TModule CustomeModule<TModule>()
            where TModule : Module<TModule>, new()
        {
            return GameManager.CustomeModule<TModule>();
        }
        /// <summary>
        /// 初始化自定义模块
        /// </summary>
        /// <param name="assembly">模块所在程序集</param>
        public static void InitCustomeModule(Assembly assembly)
        {
            GameManager.InitCustomeModule(assembly);
        }
        #endregion
        #region GameManagerUpdate
        public static event Action FixedRefreshHandler
        {
            add { GameManager.FixedRefreshHandler+= value; }
            remove{GameManager.FixedRefreshHandler -= value;}
        }
        public static event Action LateRefreshHandler
        {
            add { GameManager.LateRefreshHandler+= value; }
            remove{GameManager.LateRefreshHandler -= value;}
        }
        public static event Action RefreshHandler
        {
            add { GameManager.RefreshHandler += value; }
            remove{GameManager.RefreshHandler -= value;}
        }
        #endregion
        #region InputManager
        /// <summary>
        /// 设置输入解决方案
        /// </summary>
        /// <param name="inputDevice">InputDevice的具体平台实现方法</param>
        public static void SetInputDevice(InputDevice inputDevice) { GameManager.InputManager.SetInputDevice(inputDevice); }
        /// <summary>
        /// 虚拟轴线是否存在
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        public static bool IsExistVirtualAxis(string name) { return GameManager.InputManager.IsExistVirtualAxis(name); }
        /// <summary>
        /// 虚拟按键是否存在
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        public static bool IsExistVirtualButton(string name) { return GameManager.InputManager.IsExistVirtualButton(name); }
        /// <summary>
        /// 注册虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public static void RegisterVirtualButton(string name) { GameManager.InputManager.RegisterVirtualButton(name); }
        /// <summary>
        /// 注销虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public static void DeregisterVirtualButton(string name) { GameManager.InputManager.DeregisterVirtualButton(name); }
        /// <summary>
        /// 注册虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public static void RegisterVirtualAxis(string name) { GameManager.InputManager.RegisterVirtualAxis(name); }
        /// <summary>
        /// 注销虚拟轴线
        /// </summary>
        /// <param name="name">按键名称</param>
        public static void DeregisterVirtualAxis(string name) { GameManager.InputManager.DeregisterVirtualAxis(name); }
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public static Vector3 MousePosition() { return GameManager.InputManager.MousePosition; }
        /// <summary>
        /// 获得轴线
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns></returns>
        public static float GetAxis(string name) { return GameManager.InputManager.GetAxis(name); }
        /// <summary>
        /// 未插值的输入 -1，0 ，1
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns></returns>
        public static float GetAxisRaw(string name) { return GameManager.InputManager.GetAxisRaw(name); }
        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public static bool GetButtonDown(string name) { return GameManager.InputManager.GetButtonDown(name); }
        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public static bool GetButton(string name) { return GameManager.InputManager.GetButton(name); }
        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public static bool GetButtonUp(string name) { return GameManager.InputManager.GetButtonUp(name); }
        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        public static void SetButtonDown(string name) { GameManager.InputManager.SetButtonDown(name); }
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        public static void SetButtonUp(string name) { GameManager.InputManager.SetButtonUp(name); }
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        public static void SetVirtualMousePosition(Vector3 value) { GameManager.InputManager.SetVirtualMousePosition(value); }
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        public static void SetVirtualMousePosition(float x, float y, float z) { GameManager.InputManager.SetVirtualMousePosition(x, y, z); }
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public static void SetAxisPositive(string name) { GameManager.InputManager.SetAxisPositive(name); }
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public static void SetAxisNegative(string name) { GameManager.InputManager.SetAxisNegative(name); }
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        public static void SetAxisZero(string name) { GameManager.InputManager.SetAxisZero(name); }
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">值</param>
        public static void SetAxis(string name, float value) { GameManager.InputManager.SetAxis(name, value); }

        #endregion
        #region EventManager
        public static void AddEventListener(string eventKey, Action<object, GameEventArgs> handler)
        {
            GameManager.EventManager.AddListener(eventKey, handler);
        }
        public static void RemoveEventListener(string eventKey, Action<object, GameEventArgs> hander)
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
        /// <param name="callback">执行条件结束后自动执行回调函数</param>
        /// <returns>Coroutine</returns>
        public static Coroutine StartCoroutine(Coroutine routine, Action callback)
        {
            return GameManager.MonoManager.StartCoroutine(routine, callback);
        }
        public static Coroutine DelayCoroutine(float delay, Action callback)
        {
            return GameManager.MonoManager.DelayCoroutine(delay, callback);
        }
        /// <summary>
        /// 条件函数；
        /// 当handler为true时，才执行callback函数，期间协程挂起。
        /// </summary>
        /// <param name="handler">条件处理者</param>
        /// <param name="callback">回调函数</param>
        /// <returns></returns>
        public static Coroutine PredicateCoroutine(Func<bool> handler, Action callback)
        {
            return GameManager.MonoManager.PredicateCoroutine(handler, callback);
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
        /// 利用挂载特性的泛型对象同步加载Prefab；
        /// 加载时会检测是否挂载T脚本，若挂载，则不做操作；若未挂载，则对T进行挂载
        /// </summary>
        /// <typeparam name="T">需要加载的类型</typeparam>
        /// <param name="instantiate">是否生实例化对象</param>
        /// <returns>返回实体化或未实例化的资源对象</returns>
        public static GameObject LoadResPrefab<T>(bool instantiate = false)
            where T : MonoBehaviour
        {
            return GameManager.ResourceManager.LoadResPrefab<T>(instantiate);
        }
        /// <summary>
        /// 利用挂载特性的泛型对象同步加载PrefabObject；
        /// </summary>
        /// <typeparam name="T">实现了引用池的非Mono对象</typeparam>
        /// <param name="instantiate">是否实例化</param>
        /// <returns>载入的对象</returns>
        public static GameObject LoadResPrefabInstance<T>(bool instantiate = false)
            where T : class, IReference, new()
        {
            return GameManager.ResourceManager.LoadResPrefabInstance<T>(instantiate);
        }
        /// <summary>
        /// 利用挂载特性的泛型对象异步加载Prefab；
        /// 泛型对象为Mono类型；
        /// 实例化对象；
        /// </summary>
        /// <typeparam name="T">非Mono对象</typeparam>
        /// <param name="callback">加载完毕后的回调</param>
        public static void LoadResPrefabAsync<T>(Action<T> callback = null)
            where T : MonoBehaviour
        {
            GameManager.ResourceManager.LoadResPrefabAsync<T>(callback);
        }
        /// <summary>
        /// 利用挂载特性的泛型对象异步加载Prefab；
        /// 实例化对象；
        /// 泛型对象为非Mono类型；
        /// 可用于T持有Gameobject类型引用的对象
        /// </summary>
        /// <typeparam name="T">非Mono类型</typeparam>
        /// <param name="callback">加载完毕后的回调</param>
        public static void LoadResPrefabInstanceAsync<T>(Action<T, GameObject> callback = null)
          where T : class, IReference, new()
        {
            GameManager.ResourceManager.LoadResPrefabInstanceAsync<T>(callback);
        }
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
        public static void LoadResAysnc<T>(string path, Action<T> callback = null)
            where T : UnityEngine.Object
        {
            GameManager.ResourceManager.LoadResAysnc<T>(path, callback);
        }
        /// <summary>
        /// 异步加载资源,不实例化任何类型
        /// </summary>
        public static void LoadResAssetAysnc<T>(string path, Action<T> callback = null)
            where T : UnityEngine.Object
        {
            GameManager.ResourceManager.LoadResAssetAysnc(path, callback);
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
        /// <summary>
        /// 同步加载 
        /// </summary>
        public static void LoadScene(string sceneName, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadScene(sceneName, loadedCallback);
        }
        /// <summary>
        /// 同步加载 
        /// </summary>
        public static void LoadScene(string sceneName, bool additive, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadScene(sceneName, additive, loadedCallback);
        }
        /// <summary>
        /// 同步加载 
        /// </summary>
        public static void LoadScene(int sceneIndex, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadScene(sceneIndex, loadedCallback);
        }
        /// <summary>
        /// 同步加载 
        /// </summary>
        public static void LoadScene(int sceneIndex, bool additive, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadScene(sceneIndex, additive, loadedCallback);
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        public static void LoadSceneAsync(string sceneName, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, loadedCallback);
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加场景</param>
        /// <param name="loadedCallback">加载完毕后的回调</param>
        public static void LoadSceneAsync(string sceneName, bool additive, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, additive, loadedCallback);
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        public static void LoadSceneAsync(string sceneName, Action<float> progressCallback, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, progressCallback, loadedCallback);
        }
        public static void LoadSceneAsync(string sceneName, bool additive, Action<float> progressCallback, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, additive, progressCallback, loadedCallback);
        }
        public static void LoadSceneAsync(string sceneName, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, progressCallback, loadedCallback);
        }
        public static void LoadSceneAsync(string sceneName, bool additive, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneName, additive, progressCallback, loadedCallback);
        }
        public static void LoadSceneAsync(int sceneIndex, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, loadedCallback);
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="additive">是否叠加场景</param>
        /// <param name="progressCallback">进度更新回调</param>
        /// <param name="loadedCallback">加载完毕回调</param>
        public static void LoadSceneAsync(int sceneIndex, bool additive, Action<float> progressCallback, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, additive, progressCallback, loadedCallback);
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="progressCallback">进度更新回调</param>
        /// <param name="loadDoneCallback">加载完毕回调</param>
        public static void LoadSceneAsync(int sceneIndex, Action<float> progressCallback, Action loadDoneCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, progressCallback, loadDoneCallback);
        }
        public static void LoadSceneAsync(int sceneIndex, bool additive, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, additive, loadedCallback);
        }
        public static void LoadSceneAsync(int sceneIndex, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, progressCallback, loadedCallback);
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="additive">是否叠加场景</param>
        /// <param name="progressCallback">进度更新回调</param>
        /// <param name="loadedCallback">加载完毕回调</param>
        public static void LoadSceneAsync(int sceneIndex, bool additive, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            GameManager.SceneManager.LoadSceneAsync(sceneIndex, additive, progressCallback, loadedCallback);
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
        public static void RegisterObjcetSpawnPool(object objKey, GameObject spawnItem, Action<GameObject> onSpawn, Action<GameObject> onDespawn)
        {
            GameManager.ObjectPoolManager.RegisterSpawnPool(objKey, spawnItem, onSpawn, onDespawn);
        }
        public static void DeregisterObjectSpawnPool(object objKey)
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
        public static bool RegisterController<T>(T controller)
            where T : class, IController
        {
            return GameManager.ControllerManager.RegisterController<T>(controller);
        }
        public static bool RegisterController(Type type, IController controller)
        {
            return GameManager.ControllerManager.RegisterController(type, controller);
        }
        /// <summary>
        /// 注销控制器，如果当前控制器是最后一个，则注销后，这个类别也会自动注销
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        public static bool DeregisterController<T>(T controller)
            where T : class, IController
        {
            return GameManager.ControllerManager.DeregisterController<T>(controller);
        }
        public static bool DeregisterController(Type type, IController controller)
        {
            return GameManager.ControllerManager.DeregisterController(type, controller);
        }
        public static T GetController<T>(Predicate<T> predicate)
            where T : class, IController
        {
            return GameManager.ControllerManager.GetController<T>(predicate);
        }
        public static T[] GetControllers<T>(Predicate<T> predicate)
            where T : class, IController
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
        public static short GetControllerItemCount(Type type)
        {
            return GameManager.ControllerManager.GetControllerItemCount(type);
        }
        /// <summary>
        /// 获取当前注册的所有类型controller总数
        /// </summary>
        public static short GetControllerTypeCount()
        {
            return GameManager.ControllerManager.GetControllerTypeCount();
        }
        public static bool HasController<T>()
            where T : class, IController
        {
            return GameManager.ControllerManager.HasController<T>();
        }
        public static bool HasController(Type type)
        {
            return GameManager.ControllerManager.HasController(type);
        }
        public static bool HasControllerItem<T>(T controller)
            where T : class, IController
        {
            return GameManager.ControllerManager.HasControllerItem(controller);
        }
        public static bool HasControllerItem(Type type, IController controller)
        {
            return GameManager.ControllerManager.HasControllerItem(type, controller);
        }
        public static void ClearAllController()
        {
            GameManager.ControllerManager.ClearAllController();
        }
        public static void ClearControllerItem<T>()
            where T : class, IController
        {
            GameManager.ControllerManager.ClearControllerItem<T>();
        }
        public static void ClearControllerItem(Type type)
        {
            GameManager.ControllerManager.ClearControllerItem(type);
        }
        #endregion
        #region DataManager
        /// <summary>
        /// 保存Json数据到本地的绝对路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="dataSet">装箱后的数据</param>
        /// <param name="callback">回调函数，当写入成功后调用</param>
        public static void SaveJsonDataToLocal<T>(string relativePath, string fileName, T dataSet, bool binary = false, Action callback = null)
            where T : class, new()
        {
            GameManager.DataManager.SaveJsonDataToLocal(relativePath, fileName, dataSet, binary, callback);
        }
        /// <summary>
        /// 从本地的绝对路径读取Json数据
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="binary">是否为二进制文件</param>
        /// <param name="callback">回调函数，当读取成功后调用</param>
        /// <returns>返回一个Json</returns>
        public static string LoadJsonDataFromLocal(string relativePath, string fileName, bool binary = false, Action callback = null)
        {
            return GameManager.DataManager.LoadJsonDataFromLocal(relativePath, fileName, binary, callback);
        }
        /// <summary>
        /// 从本地的绝对路径读取Json数据
        /// </summary>
        /// <param name="fullRelativeFilePath">相对路径完整路径，包含到文件名后缀</param>
        /// <param name="binary">是否为二进制文件</param>
        /// <param name="callback">回调函数，当读取成功后调用</param>
        /// <returns>返回一个Json</returns>
        public static string LoadJsonDataFromLocal(string fullRelativeFilePath, bool binary = false, Action callback = null)
        {
            return GameManager.DataManager.LoadJsonDataFromLocal(fullRelativeFilePath, binary, callback);
        }
        /// <summary>
        /// 从Resource文件夹下读取Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativePath">相对路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="dataSet">存储json的类模型</param>
        /// <param name="callback">回调函数</param>
        public static void ParseDataFromResource<T>(string relativePath, string fileName, ref T dataSet, Action<T> callback = null)
          where T : class, new()
        {
            GameManager.DataManager.ParseDataFromResource(relativePath, fileName, ref dataSet, callback);
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
        /// <param name="callback">仅在载入时回调</param>
        public static void LoadPanel<T>(string panelName, Action<T> callback = null)
                   where T : UILogicBase
        {
            GameManager.UIManager.LoadPanel(panelName, callback);
        }
        /// <summary>
        /// 通过特性UIResourceAttribute加载Panel
        /// </summary>
        /// <typeparam name="T">UILogicBase派生类</typeparam>
        /// <param name="callback">加载完毕后的回调</param>
        public static void LoadPanel<T>(Action<T> callback = null)
            where T : UILogicBase
        {
            GameManager.UIManager.LoadPanel<T>(callback);
        }
        /// <summary>
        /// 载入面板，若字典中已存在，则使用回调，并返回。若不存在，则异步加载且使用回调。
        /// 基于Resources
        /// </summary>
        public static void ShowPanel<T>(string panelName, Action<T> callback = null)
            where T : UILogicBase
        {
            GameManager.UIManager.ShowPanel(panelName, callback);
        }
        public static void ShowPanel<T>(Action<T> callback = null)
                where T : UILogicBase
        {
            GameManager.UIManager.ShowPanel(callback);
        }
        public static void HidePanel(string panelName)
        {
            GameManager.UIManager.HidePanel(panelName);
        }
        public static void RemovePanel(string panelName)
        {
            GameManager.UIManager.RemovePanel(panelName);
        }
        public static void RemovePanel<T>()
            where T : UILogicBase
        {
            GameManager.UIManager.RemovePanel<T>();
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
        public static GameObject GetMainCanvas()
        {
            return GameManager.UIManager.MainUICanvas;
        }
        #endregion
        #region FSMManager
        /// <summary>
        /// 为特定类型设置轮询间隔；
        /// 若设置时间为小于等于0，则默认使用0；
        /// </summary>
        /// <param name="type">类型目标</param>
        /// <param name="interval">轮询间隔</param>
        public static void SetFSMSetRefreshInterval(Type type, float interval)
        {
            GameManager.FSMManager.SetFSMSetRefreshInterval(type, interval);
        }
        /// <summary>
        /// 为特定类型设置轮询间隔
        /// 若设置时间为小于等于0，则默认使用0；
        /// </summary>
        /// <typeparam name="T">类型目标</typeparam>
        /// <param name="interval">轮询间隔</param>
        public static void SetFSMSetRefreshInterval<T>(float interval)
            where T : class
        {
            GameManager.FSMManager.SetFSMSetRefreshInterval<T>(interval);
        }
        /// <summary>
        /// 暂停指定类型fsm集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        public static void PauseFSMSet<T>()
            where T : class
        {
            GameManager.FSMManager.PauseFSMSet<T>();
        }
        /// <summary>
        /// 暂停指定类型fsm集合
        /// </summary>
        /// <param name="type">目标类型</param>
        public static void PauseFSMSet(Type type)
        {
            GameManager.FSMManager.PauseFSMSet(type);
        }
        /// <summary>
        /// 继续执行指定fsm集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        public static void UnPauseFSMSet<T>()
            where T : class
        {
            GameManager.FSMManager.UnPauseFSMSet<T>();
        }
        /// <summary>
        /// 继续执行指定fsm集合
        /// </summary>
        /// <param name="type">目标类型</param>
        public static void UnPauseFSMSet(Type type)
        {
            GameManager.FSMManager.UnPauseFSMSet(type);
        }
        public static FSMBase GetIndividualFSM<T>()
        where T : class
        {
            return GameManager.FSMManager.GetIndividualFSM<T>();
        }
        public static FSMBase GetIndividualFSM(Type type)
        {
            return GameManager.FSMManager.GetIndividualFSM(type);
        }
        /// <summary>
        /// 获取某类型状态机元素集合中元素的个数
        /// </summary>
        /// <typeparam name="T">拥有者</typeparam>
        /// <returns>元素数量</returns>
        public static int GetFSMSetElementCount<T>()
    where T : class
        {
            return GameManager.FSMManager.GetFSMSetElementCount<T>();
        }
        public static int GetFSMSetElementCount(Type type)
        {
            return GameManager.FSMManager.GetFSMSetElementCount(type);
        }
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>状态机集合</returns>
        public static List<FSMBase> GetFSMSet<T>()
            where T : class
        {
            return GameManager.FSMManager.GetFSMSet<T>();
        }
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <param name="type">类型对象</param>
        /// <returns>状态机集合</returns>
        public static List<FSMBase> GetFSMSet(Type type)
        {
            return GameManager.FSMManager.GetFSMSet(type);
        }
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        public static FSMBase GetSetElementFSM<T>(Predicate<FSMBase> predicate)
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
        public static FSMBase GetSetElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            return GameManager.FSMManager.GetSetElementFSM(type, predicate);
        }
        public static FSMBase[] GetAllIndividualFSM()
        {
            return GameManager.FSMManager.GetAllIndividualFSM();
        }
        public static bool HasIndividualFSM<T>()
            where T : class
        {
            return GameManager.FSMManager.HasIndividualFSM<T>();
        }
        public static bool HasIndividualFSM(Type type)
        {
            return GameManager.FSMManager.HasIndividualFSM(type);
        }
        /// <summary>
        /// 是否拥有指定类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>是否存在</returns>
        public static bool HasFSMSet<T>()
            where T : class
        {
            return GameManager.FSMManager.HasFSMSet<T>();
        }
        public static bool HasFSMSet(Type type)
        {
            return GameManager.FSMManager.HasFSMSet(type);
        }
        public static bool HasSetElementFSM<T>(Predicate<FSMBase> predicate)
     where T : class
        {
            return GameManager.FSMManager.HasSetElementFSM<T>(predicate);
        }
        public static bool HasSetElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            return GameManager.FSMManager.HasSetElementFSM(type, predicate);
        }
        public static bool HasSetElementFSM<T>(FSMBase fsm)
             where T : class
        {
            return GameManager.FSMManager.HasSetElementFSM<T>(fsm);
        }
        public static bool HasSetElementFSM(Type type, FSMBase fsm)
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
        public static IFSM<T> CreateFSM<T>(T owner, bool Individual, params FSMState<T>[] states)
            where T : class
        {
            return GameManager.FSMManager.CreateFSM<T>(owner, Individual, states);
        }
        /// <summary>
        ///  创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="Individual">是否为独立状态机</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public static IFSM<T> CreateFSM<T>(string name, T owner, bool Individual, params FSMState<T>[] states)
            where T : class
        {
            return GameManager.FSMManager.CreateFSM<T>(name, owner, Individual, states);
        }
        public static IFSM<T> CreateFSM<T>(T owner, bool Individual, List<FSMState<T>> states)
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
        public static IFSM<T> CreateFSM<T>(string name, T owner, bool Individual, List<FSMState<T>> states)
             where T : class
        {
            return GameManager.FSMManager.CreateFSM<T>(name, owner, Individual, states);
        }
        /// <summary>
        /// 销毁独立的状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void DestoryIndividualFSM<T>()
             where T : class
        {
            GameManager.FSMManager.DestoryIndividualFSM<T>();
        }
        public static void DestoryIndividualFSM(Type type)
        {
            GameManager.FSMManager.DestoryIndividualFSM(type);
        }
        public static void DestoryFSMSet<T>()
where T : class
        {
            GameManager.FSMManager.DestoryFSMSet<T>();
        }
        public static void DestoryFSMSet(Type type)
        {
            GameManager.FSMManager.DestoryFSMSet(type);
        }
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <typeparam name="T">拥有者</typeparam>
        /// <param name="predicate">查找条件</param>
        public static void DestorySetElementFSM<T>(Predicate<FSMBase> predicate)
            where T : class
        {
            GameManager.FSMManager.DestorySetElementFSM<T>(predicate);
        }
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找条件</param>
        public static void DestorySetElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            GameManager.FSMManager.DestorySetElementFSM(type, predicate);
        }
        public static void DestoryAllFSM()
        {
            GameManager.FSMManager.DestoryAllFSM();
        }
        #endregion
        #region EntityManager
        public static bool AddEntity<T>(T entity)
                   where T : class, IEntityObject, new()
        {
            return GameManager.EntityManager.AddEntity(entity);
        }
        public static bool AddEntity(Type type, IEntityObject entity)
        {
            return GameManager.EntityManager.AddEntity(type, entity);
        }
        /// <summary>
        /// 回收单个实体对象;
        /// 若实体对象存在于缓存中，则移除。若不存在，则不做操作；
        /// </summary>
        /// <param name="entity">实体对象</param>
        public static void RecoveryEntity<T>(T entity)
              where T : class, IEntityObject, new()
        {
            GameManager.EntityManager.RecoveryEntity(entity);
        }
        /// <summary>
        /// 回收某一类型的实体对象
        /// </summary>
        /// <param name="type">实体类型</param>
        public static void RecoveryEntities(Type type)
        {
            GameManager.EntityManager.RecoveryEntities(type);
        }
        /// <summary>
        /// 获得某一类型的实体数量
        /// </summary>
        public static int GetEntityCount<T>()
               where T : class, IEntityObject, new()
        {
            return GameManager.EntityManager.GetEntityCount<T>();
        }
        public static int GetEntityCount(Type type)
        {
            return GameManager.EntityManager.GetEntityCount(type);
        }
        public static T GetEntity<T>(Predicate<T> predicate)
             where T : class, IEntityObject, new()
        {
            return GameManager.EntityManager.GetEntity(predicate);
        }
        public static IEntityObject GetEntity(Type type, Predicate<IEntityObject> predicate)
        {
            return GameManager.EntityManager.GetEntity(type, predicate);
        }
        public static ICollection<IEntityObject> GetEntityCollection<T>()
             where T : IEntityObject, new()
        {
            return GameManager.EntityManager.GetEntityCollection<T>();
        }
        public static ICollection<IEntityObject> GetEntityCollection(Type type)
        {
            return GameManager.EntityManager.GetEntityCollection(type);
        }
        public static IEntityObject[] GetEntities<T>()
            where T : class, IEntityObject, new()
        {
            return GameManager.EntityManager.GetEntities<T>();
        }
        public static IEntityObject[] GetEntities(Type type)
        {
            return GameManager.EntityManager.GetEntities(type);
        }
        public static List<IEntityObject> GetEntityList<T>()
            where T : class, IEntityObject, new()
        {

            return GameManager.EntityManager.GetEntityList<T>();
        }
        public static List<IEntityObject> GetEntityList(Type type)
        {
            return GameManager.EntityManager.GetEntityList(type);
        }
        public static void RegisterEntityType<T>()
             where T : class, IEntityObject, new()
        {
            GameManager.EntityManager.RegisterEntityType<T>();
        }
        public static void RegisterEntityType(Type type)
        {
            GameManager.EntityManager.RegisterEntityType(type);
        }
        public static bool DeregisterEntityType<T>()
            where T : class, IEntityObject, new()
        {
            return GameManager.EntityManager.DeregisterEntityType<T>();
        }
        public static bool DeregisterEntityType(Type type)
        {
            return GameManager.EntityManager.DeregisterEntityType(type);
        }
        public static bool ActiveEntity<T>(T entity)
            where T : class, IEntityObject, new()
        {
            return GameManager.EntityManager.ActiveEntity(entity);
        }
        public static void ActiveEntity<T>(Predicate<T> predicate)
               where T : class, IEntityObject, new()
        {
            GameManager.EntityManager.ActiveEntity(predicate);
        }
        public static bool ActiveEntity(Type type, IEntityObject entity)
        {
            return GameManager.EntityManager.ActiveEntity(type, entity);
        }
        public static void ActiveEntity(Type type, Predicate<IEntityObject> predicate)
        {
            GameManager.EntityManager.ActiveEntity(type, predicate);
        }
        public static void ActiveEntities<T>()
            where T : class, IEntityObject, new()
        {
            GameManager.EntityManager.ActiveEntities<T>();
        }
        public static void ActiveEntities(Type type)
        {
            GameManager.EntityManager.ActiveEntities(type);
        }
        public static void DeactiveEntities<T>()
           where T : class, IEntityObject, new()
        {
            GameManager.EntityManager.DeactiveEntities<T>();
        }
        public static void DeactiveEntities(Type type)
        {
            GameManager.EntityManager.DeactiveEntities(type);
        }
        public static bool HasEntity<T>(Predicate<T> predicate)
             where T : class, IEntityObject, new()
        {
            return GameManager.EntityManager.HasEntity(predicate);
        }
        public static bool HasEntity(Type type, Predicate<IEntityObject> predicate)
        {
            return GameManager.EntityManager.HasEntity(type, predicate);
        }
        public static bool HasEntityType<T>()
             where T : class, IEntityObject, new()
        {
            return GameManager.EntityManager.HasEntityType<T>();
        }
        public static bool HasEntityType(Type type)
        {
            return GameManager.EntityManager.HasEntityType(type);
        }
        #endregion
        #region Network
        public static long NetworkConv { get { return GameManager.NetworkManager.Conv; } }
        /// <summary>
        /// 连接指定地址的网络
        /// 初始化建立连接;
        /// 当前只有UDP协议
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口</param>
        /// <param name="protocolType">协议类型</param>
        public static void NetworkConnect(string ip, int port, ProtocolType protocolType)
        {
            GameManager.NetworkManager.Connect(ip, port, protocolType);
        }
        /// <summary>
        /// 与远程建立连接；
        /// </summary>
        /// <param name="service">自定义实现的服务</param>
        public static void Connect(INetworkService service)
        {
            GameManager.NetworkManager.Connect(service);
        }
        /// <summary>
        /// 发送报文信息；
        /// 发送给特定的endpoint对象，可不局限于一个服务器点；
        /// </summary>
        /// <param name="netMsg">消息体</param>
        /// <param name="endPoint">远程对象</param>
        public static void SendNetworkMessage(INetworkMessage netMsg, IPEndPoint endPoint)
        {
            GameManager.NetworkManager.SendNetworkMessage(netMsg, endPoint);
        }
        /// <summary>
        /// 发送报文信息；
        /// 发送给默认的endpoint对象，目标为默认远程点；
        /// </summary>
        /// <param name="netMsg">消息体</param>
        public static void SendNetworkMessage(INetworkMessage netMsg)
        {
            GameManager.NetworkManager.SendNetworkMessage(netMsg);
        }
        public static void SendNetworkMessage(ushort opCode, byte[] message)
        {
            GameManager.NetworkManager.SendNetworkMessage(opCode, message);
        }
        public static void NetworkDisconnect(bool notifyRemote = true)
        {
            GameManager.NetworkManager.Disconnect(notifyRemote);
        }
        public static void RunHeartbeat(uint intervalSec, byte maxRecur)
        {
            GameManager.NetworkManager.RunHeartbeat(intervalSec, maxRecur);
        }
        public static event Action NetworkOnConnect
        {
            add { GameManager.NetworkManager.NetworkOnConnect += value; }
            remove { GameManager.NetworkManager.NetworkOnConnect -= value; }
        }
        public static event Action NetworkOnDisconnect
        {
            add { GameManager.NetworkManager.NetworkOnDisconnect += value; }
            remove { GameManager.NetworkManager.NetworkOnDisconnect -= value; }
        }
        #endregion
        #region ConfigManager
        public static bool AddConfigData(ushort cfgKey, bool boolValue, int intValue, float floatValue, string stringValue)
        {
            return GameManager.ConfigManager.AddConfigData(cfgKey, boolValue, intValue, floatValue, stringValue);
        }
        public static bool RemoveConfig(ushort cfgKey)
        {
            return GameManager.ConfigManager.RemoveConfig(cfgKey);
        }
        public static bool HasConfig(ushort cfgKey)
        {
            return GameManager.ConfigManager.HasConfig(cfgKey);
        }
        public static ConfigData? GetConfigData(ushort cfgKey)
        {
            return GameManager.ConfigManager.GetConfigData(cfgKey);
        }
        /// <summary>
        /// 从指定全局配置项中读取布尔值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的布尔值。</returns>
        public static bool GetConfigBool(ushort cfgKey)
        {
            return GameManager.ConfigManager.GetConfigBool(cfgKey);
        }
        /// <summary>
        /// 从指定全局配置项中读取布尔值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public static bool GetConfigBool(ushort cfgKey, bool defaultValue)
        {
            return GameManager.ConfigManager.GetConfigBool(cfgKey, defaultValue);
        }
        /// <summary>
        /// 从指定全局配置项中读取整数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的整数值。</returns>
        public static int GetConfigInt(ushort cfgKey)
        {
            return GameManager.ConfigManager.GetConfigInt(cfgKey);
        }
        /// <summary>
        /// 从指定全局配置项中读取整数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public static int GetConfigInt(ushort cfgKey, int defaultValue)
        {
            return GameManager.ConfigManager.GetConfigInt(cfgKey, defaultValue);
        }
        /// <summary>
        /// 从指定全局配置项中读取浮点数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的浮点数值。</returns>
        public static float GetConfigFloat(ushort cfgKey)
        {
            return GameManager.ConfigManager.GetConfigFloat(cfgKey);
        }
        /// <summary>
        /// 从指定全局配置项中读取浮点数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public static float GetConfigFloat(ushort cfgKey, float defaultValue)
        {
            return GameManager.ConfigManager.GetConfigFloat(cfgKey, defaultValue);
        }
        /// <summary>
        /// 从指定全局配置项中读取字符串值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的字符串值。</returns>
        public static string GetConfigString(ushort cfgKey)
        {
            return GameManager.ConfigManager.GetConfigString(cfgKey);
        }
        /// <summary>
        /// 从指定全局配置项中读取字符串值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public static string GetConfigString(ushort cfgKey, string defaultValue)
        {
            return GameManager.ConfigManager.GetConfigString(cfgKey, defaultValue);
        }
        public static void RemoveAllConfig()
        {
            GameManager.ConfigManager.RemoveAllConfig();
        }
        #endregion
    }
}