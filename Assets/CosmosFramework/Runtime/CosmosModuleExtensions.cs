using Cosmos.Audio;
using Cosmos.Config;
using Cosmos.Controller;
using Cosmos.DataNode;
using Cosmos.DataTable;
using Cosmos.Entity;
using Cosmos.Event;
using Cosmos.FSM;
using Cosmos.Hotfix;
using Cosmos.Input;
using Cosmos.Network;
using Cosmos.ObjectPool;
using Cosmos.Resource;
using Cosmos.Scene;
using Cosmos.UI;
using Cosmos.Procedure;
using UnityEngine;
namespace Cosmos
{
    public static class CosmosModuleExtensions
    {
        public static GameObject Instance(this IProcedureManager  procedureManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IProcedureManager>();
        }
        public static GameObject Instance(this IAudioManager audioManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IAudioManager>();
        }
        public static GameObject Instance(this IControllerManager controllerManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IControllerManager>();
        }
        public static GameObject Instance(this IFSMManager fSMManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IFSMManager>();
        }
        public static GameObject Instance(this IObjectPoolManager objectPoolManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IObjectPoolManager>();
        }
        public static GameObject Instance(this IConfigManager configManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IConfigManager>();
        }
        public static GameObject Instance(this IInputManager inputManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IInputManager>();
        }
        public static GameObject Instance(this INetworkManager networkManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<INetworkManager>();
        }
        public static GameObject Instance(this IResourceManager resourceManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IResourceManager>();
        }
        public static GameObject Instance(this IUIManager uiManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IUIManager>();
        }
        public static GameObject Instance(this IHotfixManager hotfixManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IHotfixManager>();
        }
        public static GameObject Instance(this IDataNodeManager dataNodeManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IDataNodeManager>();
        }
        public static GameObject Instance(this IEntityManager entityManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IEntityManager>();
        }
        public static GameObject Instance(this IEventManager eventManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IEventManager>();
        }
        public static GameObject Instance(this ISceneManager sceneManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<ISceneManager>();
        }
        public static GameObject Instance(this IDataTableManager  dataTableManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IDataTableManager>();
        }
    }
}