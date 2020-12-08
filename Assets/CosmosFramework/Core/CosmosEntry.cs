using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    [DefaultExecutionOrder(2000)]
    public class CosmosEntry : MonoBehaviour
    {
        public static IAudioManager AudioManager { get; private set; }
        public static IControllerManager ControllerManager { get; private set; }
        public static IFSMManager FSMManager { get; private set; }
        public static IObjectPoolManager ObjectPoolManager { get; private set; }
        public static IMonoManager MonoManager { get; private set; }
        public static IConfigManager ConfigManager { get; private set; }
        public static IInputManager InputManager { get; private set; }
        public static INetworkManager NetworkManager { get; private set; }
        public static IReferencePoolManager ReferencePoolManager { get; private set; }
        public static IResourceManager ResourceManager { get; private set; }
        public static IUIManager UIManager { get; private set; }
        public static IHotfixManager HotfixManager { get; private set; }
        public static IDataManager DataManager { get; private set; }
        public static IEntityManager EntityManager { get; private set; }
        public static IEventManager EventManager { get; private set; }
        public static ISceneManager SceneManager { get; private set; }
        private void Awake()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var length = assemblies.Length;
            for (int i = 0; i < length; i++)
            {
                var helper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IDebugHelper>(assemblies[i]);
                if (helper != null)
                {
                    Utility.Debug.SetHelper(helper);
                    break;
                }
            }
            for (int i = 0; i < length; i++)
            {
                var helper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IJsonHelper>(assemblies[i]);
                if (helper != null)
                {
                    Utility.Json.SetHelper(helper);
                    break;
                }
            }
            for (int i = 0; i < length; i++)
            {
                var helper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IMessagePackHelper>(assemblies[i]);
                if (helper != null)
                {
                    Utility.MessagePack.SetHelper(helper);
                    break;
                }
            }
            GameManager.PreparatoryModule();
            AssignManager();
        }
        void AssignManager()
        {
            try{AudioManager = GameManager.GetModule<IAudioManager>();}
            catch (Exception e){Utility.Debug.LogError(e);}
            try{ControllerManager = GameManager.GetModule<IControllerManager>();}
            catch (Exception e){Utility.Debug.LogError(e);}
            try { ObjectPoolManager= GameManager.GetModule<IObjectPoolManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { FSMManager = GameManager.GetModule<IFSMManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { MonoManager = GameManager.GetModule<IMonoManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { ConfigManager = GameManager.GetModule<IConfigManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { InputManager = GameManager.GetModule<IInputManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { NetworkManager = GameManager.GetModule<INetworkManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { ReferencePoolManager = GameManager.GetModule<IReferencePoolManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { ResourceManager = GameManager.GetModule<IResourceManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { UIManager = GameManager.GetModule<IUIManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { HotfixManager = GameManager.GetModule<IHotfixManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { DataManager = GameManager.GetModule<IDataManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { EntityManager = GameManager.GetModule<IEntityManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { EventManager = GameManager.GetModule<IEventManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
            try { SceneManager = GameManager.GetModule<ISceneManager>(); }
            catch (Exception e) { Utility.Debug.LogError(e); }
        }
    }
}