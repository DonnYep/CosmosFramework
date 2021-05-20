using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

public class QuarkAssetBuildLoader
{

    static Dictionary<string, AssetInfo> assetInfoDict = new Dictionary<string, AssetInfo>();

    private static string curRootAsset = string.Empty;
    private static float curProgress = 0f;


    [MenuItem("Cosmos/SetAssetbundleName")]
    static void SetABNames()
    {
        string path = GetSelectedAssetPath();
        if (path == null)
        {
            Debug.LogWarning("请先选择目标文件夹");
            return;
        }
        QuarkAssetBuildLoader.GetAllAssets(path);
    }
    [MenuItem("Cosmos/ClearAllAssetbundelname")]
    static void CleaarAllABNames()
    {
        string[] abnames = AssetDatabase.GetAllAssetBundleNames();
        foreach (var n in abnames)
        {
            AssetDatabase.RemoveAssetBundleName(n, true);
        }
    }

    public static void GetAllAssets(string rootDir)
    {
        assetInfoDict.Clear();

        DirectoryInfo dirinfo = new DirectoryInfo(rootDir);
        FileInfo[] fs = dirinfo.GetFiles("*.*", SearchOption.AllDirectories);
        int ind = 0;
        foreach (var f in fs)
        {
            curProgress = (float)ind / (float)fs.Length;
            curRootAsset = "正在分析依赖：" + f.Name;
            EditorUtility.DisplayProgressBar(curRootAsset, curRootAsset, curProgress);
            ind++;
            int index = f.FullName.IndexOf("Assets");
            if (index != -1)
            {
                string assetPath = f.FullName.Substring(index);
                Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                string upath = AssetDatabase.GetAssetPath(asset);
                if (assetInfoDict.ContainsKey(assetPath) == false
                    && assetPath.StartsWith("Assets")
                    && !(asset is MonoScript)
                    && !(asset is LightingDataAsset)
                    && asset != null
                    )
                {
                    AssetInfo info = new AssetInfo(upath, true);
                    //标记一下是文件夹下根资源
                    CreateDeps(info);
                }
                EditorUtility.UnloadUnusedAssetsImmediate();
            }
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
        EditorUtility.ClearProgressBar();

        int setIndex = 0;
        foreach (KeyValuePair<string, AssetInfo> kv in assetInfoDict)
        {
            EditorUtility.DisplayProgressBar("正在设置ABName", kv.Key, (float)setIndex / (float)assetInfoDict.Count);
            setIndex++;
            AssetInfo a = kv.Value;
            a.SetAssetBundleName(2);
        }
        EditorUtility.ClearProgressBar();
        EditorUtility.UnloadUnusedAssetsImmediate();
        AssetDatabase.SaveAssets();
    }
    /// <summary>
    /// 递归分析每个所被依赖到的资源
    /// </summary>
    /// <param name="self"></param>
    /// <param name="parent"></param>
    static void CreateDeps(AssetInfo self, AssetInfo parent = null)
    {
        if (self.HasParent(parent))
            return;
        if (assetInfoDict.ContainsKey(self.assetPath) == false)
        {
            assetInfoDict.Add(self.assetPath, self);
        }
        self.AddParent(parent);

        Object[] deps = EditorUtility.CollectDependencies(new Object[] { self.GetAsset() });
        for (int i = 0; i < deps.Length; i++)
        {
            Object o = deps[i];
            if (o is MonoScript || o is LightingDataAsset)
                continue;
            string path = AssetDatabase.GetAssetPath(o);
            if (path == self.assetPath)
                continue;
            if (path.StartsWith("Assets") == false)
                continue;
            AssetInfo info = null;
            if (assetInfoDict.ContainsKey(path))
            {
                info = assetInfoDict[path];
            }
            else
            {
                info = new AssetInfo(path);
                assetInfoDict.Add(path, info);
            }
            EditorUtility.DisplayProgressBar(curRootAsset, path, curProgress);
            CreateDeps(info, self);
        }
        EditorUtility.UnloadUnusedAssetsImmediate();
    }

    static string GetSelectedAssetPath()
    {
        var selected = Selection.activeObject;
        if (selected == null)
        {
            return null;
        }
        Debug.Log(selected.GetType());
        if (selected is DefaultAsset)
        {
            string path = AssetDatabase.GetAssetPath(selected);
            Debug.Log("选中路径： " + path);
            return path;
        }
        else
        {
            return null;
        }
    }
}

