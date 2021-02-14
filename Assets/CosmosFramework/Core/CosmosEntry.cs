using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Cosmos
{
    public class CosmosEntry 
    {
        public static IAudioManager AudioManager { get { return GameManager.GetModule<IAudioManager>(); } }
        public static IControllerManager ControllerManager { get { return GameManager.GetModule<IControllerManager>(); } }
        public static IFSMManager FSMManager { get { return GameManager.GetModule<IFSMManager>(); } }
        public static IObjectPoolManager ObjectPoolManager { get { return GameManager.GetModule<IObjectPoolManager>(); } }
        public static IMonoManager MonoManager { get { return GameManager.GetModule<IMonoManager>(); } }
        public static IConfigManager ConfigManager { get { return GameManager.GetModule<IConfigManager>(); } }
        public static IInputManager InputManager { get { return GameManager.GetModule<IInputManager>(); } }
        public static INetworkManager NetworkManager { get { return GameManager.GetModule<INetworkManager>(); } }
        public static IReferencePoolManager ReferencePoolManager { get { return GameManager.GetModule<IReferencePoolManager>(); } }
        public static IResourceManager ResourceManager { get { return GameManager.GetModule<IResourceManager>(); } }
        public static IUIManager UIManager { get { return GameManager.GetModule<IUIManager>(); } }
        public static IHotfixManager HotfixManager { get { return GameManager.GetModule<IHotfixManager>(); } }
        public static IDataManager DataManager { get { return GameManager.GetModule<IDataManager>(); } }
        public static IEntityManager EntityManager { get { return GameManager.GetModule<IEntityManager>(); } }
        public static IEventManager EventManager { get { return GameManager.GetModule<IEventManager>(); } }
        public static ISceneManager SceneManager { get { return GameManager.GetModule<ISceneManager>(); } }

        public static GameObject AudioMount { get { return GameManager.GetModuleMount<IAudioManager>(); } }
        public static GameObject ControllerMount { get { return GameManager.GetModuleMount<IControllerManager>(); } }
        public static GameObject FSMMount { get { return GameManager.GetModuleMount<IFSMManager>(); } }
        public static GameObject ObjectPoolMount { get { return GameManager.GetModuleMount<IObjectPoolManager>(); } }
        public static GameObject MonoMount { get { return GameManager.GetModuleMount<IMonoManager>(); } }
        public static GameObject ConfigMount { get { return GameManager.GetModuleMount<IConfigManager>(); } }
        public static GameObject InputMount { get { return GameManager.GetModuleMount<IInputManager>(); } }
        public static GameObject NetworkMount { get { return GameManager.GetModuleMount<INetworkManager>(); } }
        public static GameObject ReferencePoolMount { get { return GameManager.GetModuleMount<IReferencePoolManager>(); } }
        public static GameObject ResourceMount { get { return GameManager.GetModuleMount<IResourceManager>(); } }
        public static GameObject UIMount { get { return GameManager.GetModuleMount<IUIManager>(); } }
        public static GameObject HotfixMount { get { return GameManager.GetModuleMount<IHotfixManager>(); } }
        public static GameObject DataMount { get { return GameManager.GetModuleMount<IDataManager>(); } }
        public static GameObject EntityMount { get { return GameManager.GetModuleMount<IEntityManager>(); } }
        public static GameObject EventMount { get { return GameManager.GetModuleMount<IEventManager>(); } }
        public static GameObject SceneMount { get { return GameManager.GetModuleMount<ISceneManager>(); } }
        public static void LaunchHelpers()
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
        }
        public static void LaunchModules()
        {
            GameManager.PreparatoryModule();
        }
    }
}