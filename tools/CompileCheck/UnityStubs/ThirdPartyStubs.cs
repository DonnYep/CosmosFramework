// Third-party API stubs for compile-checking the optional resource adapters outside Unity.
// They mirror the API surface the adapters use (Addressables 1.x / YooAsset 2.x-style / QuarkAsset 0.2.x).
// These stubs are ONLY used by tools/RoslynCheck; they never enter the Unity project.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
    public enum AsyncOperationStatus
    {
        None,
        Succeeded,
        Failed,
    }

    public struct SceneInstance
    {
        public Scene Scene { get; set; }
    }

    public struct AsyncOperationHandle
    {
        public bool IsDone { get { return false; } }
        public float PercentComplete { get { return 0; } }
        public AsyncOperationStatus Status { get { return AsyncOperationStatus.None; } }
        public object Result { get { return null; } }
        public Exception OperationException { get { return null; } }
        public bool IsValid() { return false; }
    }

    public struct AsyncOperationHandle<TObject>
    {
        public bool IsDone { get { return false; } }
        public float PercentComplete { get { return 0; } }
        public AsyncOperationStatus Status { get { return AsyncOperationStatus.None; } }
        public TObject Result { get { return default(TObject); } }
        public Exception OperationException { get { return null; } }
        public bool IsValid() { return false; }
    }
}

namespace UnityEngine.AddressableAssets.ResourceLocators
{
    public interface IResourceLocator
    {
    }
}

namespace UnityEngine.AddressableAssets
{
    using UnityEngine.ResourceManagement.AsyncOperations;

    public static class Addressables
    {
        public static AsyncOperationHandle<ResourceLocators.IResourceLocator> InitializeAsync() { return default; }
        public static AsyncOperationHandle<List<ResourceLocators.IResourceLocator>> UpdateCatalogs() { return default; }
        public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(object key) { return default; }
        public static AsyncOperationHandle<IList<TObject>> LoadAssetsAsync<TObject>(object key, Action<TObject> callback) { return default; }
        public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(object key, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100) { return default; }
        public static AsyncOperationHandle<GameObject> InstantiateAsync(object key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) { return default; }
        public static void Release<TObject>(AsyncOperationHandle<TObject> handle) { }
        public static void ReleaseInstance(AsyncOperationHandle<GameObject> handle) { }
        public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(AsyncOperationHandle<SceneInstance> handle) { return default; }
    }
}

namespace YooAsset
{
    public enum EOperationStatus
    {
        None,
        Processing,
        Succeed,
        Failed,
    }

    public abstract class InitializeParameters
    {
    }

    public sealed class EditorSimulateModeParameters : InitializeParameters
    {
        public string SimulateManifestFilePath;
    }

    public sealed class OfflinePlayModeParameters : InitializeParameters
    {
    }

    public sealed class HostPlayModeParameters : InitializeParameters
    {
    }

    public abstract class AsyncOperationBase
    {
        public EOperationStatus Status { get; protected set; }
        public string Error { get; protected set; }
        public float Progress { get; protected set; }
        public bool IsDone { get { return Status == EOperationStatus.Succeed || Status == EOperationStatus.Failed; } }
    }

    public class InitializationOperation : AsyncOperationBase
    {
    }

    public abstract class OperationHandleBase
    {
        public EOperationStatus Status { get { return EOperationStatus.None; } }
        public string LastError { get { return null; } }
        public float Progress { get { return 0; } }
        public bool IsDone { get { return true; } }
        public bool IsValid { get { return false; } }
    }

    public sealed class AssetOperationHandle : OperationHandleBase
    {
        public UnityEngine.Object AssetObject { get { return null; } }
        public void Release() { }
    }

    public sealed class SubAssetsOperationHandle : OperationHandleBase
    {
        public UnityEngine.Object[] AllAssetObjects { get { return null; } }
        public void Release() { }
    }

    public sealed class AllAssetsOperationHandle : OperationHandleBase
    {
        public UnityEngine.Object[] AllAssetObjects { get { return null; } }
        public void Release() { }
    }

    public sealed class SceneOperationHandle : OperationHandleBase
    {
        public Scene SceneObject { get { return default; } }
        public bool ActivateScene() { return true; }
        public void UnloadAsync() { }
    }

    public sealed class ResourcePackage
    {
        public string PackageName { get { return null; } }
        public InitializationOperation InitializeAsync(InitializeParameters parameters) { return null; }
        public AssetOperationHandle LoadAssetAsync<TObject>(string location) where TObject : UnityEngine.Object { return null; }
        public SubAssetsOperationHandle LoadSubAssetsAsync<TObject>(string location) where TObject : UnityEngine.Object { return null; }
        public AllAssetsOperationHandle LoadAllAssetsAsync<TObject>(string location) where TObject : UnityEngine.Object { return null; }
        public SceneOperationHandle LoadSceneAsync(string location, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100) { return null; }
        public void UnloadUnusedAssets() { }
    }

    public static class YooAssets
    {
        public static void Initialize(ILogger logger = null) { }
        public static void Destroy() { }
        public static ResourcePackage CreatePackage(string packageName) { return null; }
        public static ResourcePackage GetPackage(string packageName) { return null; }
        public static bool HasPackage(string packageName) { return false; }
        public static void DestroyPackage(string packageName) { }
    }

    public interface ILogger
    {
    }
}

namespace Quark.Asset
{
    public class QuarkDataset : ScriptableObject
    {
    }
}

namespace Quark
{
    using Quark.Asset;

    public static class QuarkResources
    {
        public static void LaunchAssetBundleMode(string persistentPath, Action onSuccess, Action<string> onFailure, string manifestAesKey = "", ulong encryptionOffset = 0) { }
        public static void LaunchAssetDatabaseMode(QuarkDataset quarkDataset, Action onSuccess, Action<string> onFailure) { }
        public static string GetBuildVersion() { return null; }
        public static Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback) where T : UnityEngine.Object { return null; }
        public static Coroutine LoadMainAndSubAssetsAsync<T>(string assetName, Action<T[]> callback) where T : UnityEngine.Object { return null; }
        public static Coroutine LoadAllAssetAsync(string assetBundleName, Action<UnityEngine.Object[]> callback) { return null; }
        public static Coroutine LoadSceneAsync(string sceneName, Action<float> progress, Action callback, bool additive = false) { return null; }
        public static Coroutine UnloadSceneAsync(string sceneName, Action<float> progress, Action callback) { return null; }
        public static void UnloadAsset(string assetName) { }
        public static void UnloadAllAssetBundle(bool unloadAllLoadedObjects = true) { }
    }
}
