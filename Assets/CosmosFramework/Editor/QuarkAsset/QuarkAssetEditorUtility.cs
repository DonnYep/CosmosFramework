using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Cosmos.QuarkAsset;
namespace Cosmos.CosmosEditor
{
    public class QuarkAssetEditorUtility
    {
        public static void ClearQuarkAsset()
        {
            var filePath = Utility.IO.CombineRelativeFilePath(QuarkAssetConst.QuarkAssetFileName, ApplicationConst.LibraryPath);
            var lnkPath = Utility.IO.CombineRelativeFilePath(QuarkAssetConst.LinkedQuarkAssetFileName, ApplicationConst.LibraryPath);
            Utility.IO.DeleteFile(filePath);
            Utility.IO.DeleteFile(lnkPath);
        }
        public static QuarkAssetData LoadQuarkAssetData()
        {
            var filePath = Utility.IO.CombineRelativeFilePath(QuarkAssetConst.QuarkAssetFileName, ApplicationConst.LibraryPath);
            var lnkPath = Utility.IO.CombineRelativeFilePath(QuarkAssetConst.LinkedQuarkAssetFileName, ApplicationConst.LibraryPath);
            var json = Utility.IO.ReadTextFileContent(filePath);
            var lnkJson = Utility.IO.ReadTextFileContent(lnkPath);
            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(lnkJson))
                return null;
            return Utility.Json.ToObject<QuarkAssetData>(json);
        }
        public static void SetAndSaveQuarkAsset(QuarkAssetData assetData)
        {
            var json = Utility.Json.ToJson(assetData, true);
            var lnkDict = EncodeSchema(assetData);
            var linkedJson = Utility.Json.ToJson(lnkDict, true);
            Utility.IO.OverwriteTextFile(ApplicationConst.LibraryPath, QuarkAssetConst.QuarkAssetFileName, json);
            Utility.IO.OverwriteTextFile(ApplicationConst.LibraryPath, QuarkAssetConst.LinkedQuarkAssetFileName, linkedJson);
        }
        static Dictionary<string, LinkedList<QuarkAssetObject>> EncodeSchema(QuarkAssetData assetData)
        {
            var lnkDict = new Dictionary<string, LinkedList<QuarkAssetObject>>();
            var length = assetData.QuarkAssetObjectList.Count;
            for (int i = 0; i < length; i++)
            {
                var name = assetData.QuarkAssetObjectList[i].AssetName;
                if (!lnkDict.TryGetValue(name, out var lnkList))
                {
                    var lnk = new LinkedList<QuarkAssetObject>();
                    lnk.AddLast(assetData.QuarkAssetObjectList[i]);
                    lnkDict.Add(name, lnk);
                }
                else
                {
                    lnkList.AddLast(assetData.QuarkAssetObjectList[i]);
                }
            }
            return lnkDict;
        }
    }
}
