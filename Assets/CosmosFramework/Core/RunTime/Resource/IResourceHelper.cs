﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos.Resource
{
    public interface IResourceHelper
    {
        /// <summary>
        /// 是否是编辑器模式
        /// </summary>
        bool IsEditorMode { get; }
        /// <summary>
        /// AssetBundle资源加载根路径
        /// </summary>
        string AssetBundleRootPath { get; }
        /// <summary>
        /// 所有AssetBundle资源包清单的名称
        /// </summary>
        string AssetBundleManifestName { get; }
        /// <summary>
        /// 缓存的所有AssetBundle包【AB包名称、AB包】
        /// </summary>
        Dictionary<string, AssetBundle> AssetBundles { get; }
        /// <summary>
        /// 所有AssetBundle资源包清单
        /// </summary>
        AssetBundleManifest AssetBundleManifest { get; }
        /// <summary>
        /// 所有AssetBundle的Hash128值【AB包名称、Hash128值】
        /// </summary>
        Dictionary<string, Hash128> AssetBundleHashs { get; }

        /// <summary>
        /// 设置加载器
        /// </summary>
        /// <param name="loadMode">加载模式</param>
        /// <param name="isEditorMode">是否是编辑器模式</param>
        /// <param name="manifestName">AB包清单名称</param>
        void SetLoader(ResourceLoadMode loadMode, bool isEditorMode, string manifestName);
        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        /// <param name="path">AssetBundle资源根路径</param>
        void SetAssetBundlePath(string path);
        /// <summary>
        /// 通过名称获取指定的AssetBundle
        /// </summary>
        /// <param name="assetBundleName">名称</param>
        /// <returns>AssetBundle</returns>
        AssetBundle GetAssetBundle(string assetBundleName);
        /// <summary>
        /// 加载资源（异步）
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingAction">加载中事件</param>
        /// <param name="loadDoneAction">加载完成事件</param>
        /// <param name="isPrefab">是否是加载预制体</param>
        /// <param name="parent">预制体加载完成后的父级</param>
        /// <param name="isUI">是否是加载UI</param>
        /// <returns>加载协程迭代器</returns>
        IEnumerator LoadAssetAsync<T>(ResourceAssetInfo info, Action<float> loadingAction, Action<T> loadDoneAction, bool isPrefab, Transform parent, bool isUI) where T :UnityEngine. Object;
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingAction">加载中事件</param>
        /// <param name="loadDoneAction">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        IEnumerator LoadSceneAsync(SceneInfo info, Action<float> loadingAction, Action loadDoneAction);
        /// <summary>
        /// 卸载资源（卸载AssetBundle）
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnLoadAsset(string assetBundleName, bool unloadAllLoadedObjects = false);
        /// <summary>
        /// 卸载所有资源（卸载AssetBundle）
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnLoadAllAsset(bool unloadAllLoadedObjects = false);
        /// <summary>
        /// 清理内存，释放空闲内存
        /// </summary>
        void ClearMemory();
    }
    }