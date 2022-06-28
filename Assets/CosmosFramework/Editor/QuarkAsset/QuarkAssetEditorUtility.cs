using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Quark.Editor
{
    public static partial class QuarkAssetEditorUtility
    {
        static Dictionary<string, List<string>> dependenciesMap = new Dictionary<string, List<string>>();
        public static void BuildAssetBundle(BuildTarget target, string outPath, BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }
            BuildPipeline.BuildAssetBundles(outPath, options | BuildAssetBundleOptions.DeterministicAssetBundle, target);
        }
        public static void BuildSceneBundle(string[] sceneList, string outPath)
        {
            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }
            var buildOptions = new BuildPlayerOptions()
            {
                scenes = sceneList,
                locationPathName = outPath,
                target = BuildTarget.StandaloneWindows,
                options = BuildOptions.BuildAdditionalStreamedScenes
            };
            BuildPipeline.BuildPlayer(buildOptions);
            AssetDatabase.Refresh();
        }
        /// <summary>
        /// 获取原生Folder资源icon
        /// </summary>
        /// <returns>icon</returns>
        public static Texture2D GetFolderIcon()
        {
            return EditorGUIUtility.FindTexture("Folder Icon");
        }
        public static Texture2D ToTexture2D(Texture texture)
        {
            return Texture2D.CreateExternalTexture(
                texture.width,
                texture.height,
                TextureFormat.RGB24,
                false, false,
                texture.GetNativeTexturePtr());
        }
        /// <summary>
        /// 获取除自生以外的依赖资源的所有路径；
        /// </summary>
        /// <param name="path">目标资源地址</param>
        /// <returns>依赖的资源路径</returns>
        public static string[] GetDependencises(string path)
        {
            dependenciesMap.Clear();
            //全部小写
            List<string> list = null;
            if (!dependenciesMap.TryGetValue(path, out list))
            {
                list = AssetDatabase.GetDependencies(path).Select((s) => s.ToLower()).ToList();
                list.Remove(path.ToLower());
                //检测依赖路径
                CheckAssetsPath(list);
                dependenciesMap[path] = list;
            }
            return list.ToArray();
        }
        /// <summary>
        /// 获取可以打包的资源
        /// </summary>
        static void CheckAssetsPath(List<string> list)
        {
            if (list.Count == 0)
                return;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var path = list[i];
                //文件不存在,或者是个文件夹移除
                if (!File.Exists(path) || Directory.Exists(path))
                {
                    list.RemoveAt(i);
                    continue;
                }
                //判断路径是否为editor依赖
                if (path.Contains("/editor/"))
                {
                    list.RemoveAt(i);
                    continue;
                }
                //特殊后缀
                var ext = Path.GetExtension(path).ToLower();
                if (ext == ".cs" || ext == ".js" || ext == ".dll")
                {
                    list.RemoveAt(i);
                    continue;
                }
            }
        }
    }
}
