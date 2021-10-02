using Cosmos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CosmosModuleExtensions
{
    public static GameObject Instance(this IAudioManager audioManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IAudioManager>();
    }
    public static GameObject Instance(this IControllerManager controllerManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IControllerManager>();
    }
    public static GameObject Instance(this IFSMManager fSMManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IFSMManager>();
    }
    public static GameObject Instance(this IObjectPoolManager objectPoolManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IObjectPoolManager>();
    }
    public static GameObject Instance(this IConfigManager configManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IConfigManager>();
    }
    public static GameObject Instance(this IInputManager inputManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IInputManager>();
    }
    public static GameObject Instance(this INetworkManager networkManager)
    {
        return MonoGameManager.Instance.GetModuleMount<INetworkManager>();
    }
    public static GameObject Instance(this IResourceManager resourceManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IResourceManager>();
    }
    public static GameObject Instance(this IUIManager uiManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IUIManager>();
    }
    public static GameObject Instance(this IHotfixManager hotfixManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IHotfixManager>();
    }
    public static GameObject Instance(this IDataNodeManager dataNodeManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IDataNodeManager>();
    }
    public static GameObject Instance(this IEntityManager entityManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IEntityManager>();
    }
    public static GameObject Instance(this IEventManager eventManager)
    {
        return MonoGameManager.Instance.GetModuleMount<IEventManager>();
    }
    public static GameObject Instance(this ISceneManager sceneManager)
    {
        return MonoGameManager.Instance.GetModuleMount<ISceneManager>();
    }
}
