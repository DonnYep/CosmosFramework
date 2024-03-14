using System;
using System.Collections.Generic;
using UnityEditor;

namespace Cosmos.Editor
{
    public static partial class EditorUtil
    {
        public static class IO
        {
            /// <summary>
            /// 遍历文件夹下的文件；
            /// 传入的文件夹名参考：Assets/Game
            /// </summary>
            /// <param name="folder">文件夹名</param>
            /// <param name="handler">处理方法</param>
            public static void TraverseFolderFile(string folder, Action<UnityEngine.Object> handler)
            {
                if (string.IsNullOrEmpty(folder))
                    throw new ArgumentNullException("Folder Name is invalid !");
                if (handler == null)
                    throw new ArgumentNullException("Handler is invalid !");
                if (AssetDatabase.IsValidFolder(folder))
                {
                    var assets = GetAllAssets<UnityEngine.Object>(folder);
                    if (assets != null)
                    {
                        var length = assets.Length;
                        for (int i = 0; i < length; i++)
                        {
                            handler.Invoke(assets[i]);
                        }
                    }
                    var subFolders = AssetDatabase.GetSubFolders(folder);
                    if (subFolders != null)
                    {
                        foreach (var subF in subFolders)
                        {
                            TraverseFolderFile(subF, handler);
                        }
                    }
                }
            }
            /// <summary>
            /// 遍历assets目录下的所有文件
            /// </summary>
            /// <param name="handler">遍历回调</param>
            public static void TraverseAllFolderFile(Action<UnityEngine.Object> handler)
            {
                if (handler == null)
                    throw new ArgumentNullException("Handler is invalid !");
                var assets = GetAllAssets<UnityEngine.Object>("Assets");
                if (assets != null)
                {
                    var length = assets.Length;
                    for (int i = 0; i < length; i++)
                    {
                        handler.Invoke(assets[i]);
                    }
                }
                var subFolder = AssetDatabase.GetSubFolders("Assets");
                if (subFolder != null)
                {
                    foreach (var subF in subFolder)
                    {
                        TraverseFolderFile(subF, handler);
                    }
                }
            }
            /// <summary>
            /// 获取所有Asset目录下，除文件夹、CS脚本以外的资源路径。
            /// </summary>
            public static string[] GetAllBundleableFilePath()
            {
                var paths = AssetDatabase.GetAllAssetPaths();
                List<string> pathList = new List<string>();
                foreach (var path in paths)
                {
                    if (!AssetDatabase.IsValidFolder(path))
                    {
                        var file = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                        if (!(file is MonoScript))
                        {
                            pathList.Add(path);
                        }
                    }
                }
                return pathList.ToArray();
            }
            /// <summary>
            /// 获取目录下，除文件夹、CS脚本以外的资源路径。
            /// </summary>
            /// <param name="folder">目录地址</param>
            /// <returns>可以作为ab的文件地址</returns>
            public static string[] GetAllBundleableFilePath(string folder)
            {
                List<string> pathList = new List<string>();
                TraverseFolderFile(folder, (asset) =>
                {
                    if (!(asset is MonoScript))
                    {
                        pathList.Add(AssetDatabase.GetAssetPath(asset));
                    }
                });
                return pathList.ToArray();
            }
        }
    }
}
