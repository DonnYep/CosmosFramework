using UnityEditor;
namespace Cosmos.Editor.Resource
{
    public class ResourceSettingLoader
    {
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <typeparam name="T">配置文件类型</typeparam>
        /// <returns>配置文件</returns>
        public static T LoadSettingData<T>()
            where T : UnityEngine.ScriptableObject
        {
            var settingType = typeof(T);
            var guids = AssetDatabase.FindAssets($"t:{settingType.Name}");
            if (guids.Length == 0)
            {
                var settingData = EditorUtil.CreateScriptableObject<T>(settingType.Name);
                return settingData;
            }
            else
            {
                if (guids.Length >= 1)
                {
                    //这里只提示，不抛出异常
                    EditorUtil.Debug.LogError($"Multiple {settingType.Name} files detected, defaulting to the first one !");
                }
                string filePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                var setting = AssetDatabase.LoadAssetAtPath<T>(filePath);
                return setting;
            }
        }
    }
}
