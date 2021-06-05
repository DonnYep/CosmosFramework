using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Cosmos.Quark;
using UnityEditor;
using UnityEngine;

namespace Cosmos.CosmosEditor
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
        [RuntimeInitializeOnLoadMethod]
        static void InitQuarkAssetData()
        {
            if (QuarkUtility.QuarkAssetLoadMode == QuarkAssetLoadMode.AssetDatabase)
            {
                var quarkAssetData = QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance;
                QuarkUtility.SetAssetDatabaseModeData(quarkAssetData);
            }
        }
    }
}
