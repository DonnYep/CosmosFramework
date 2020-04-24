using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
# if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cosmos { 
   public static  partial class Utility
    {
        public static class Editor
        {
            public static void SetEditorPrefsBool(string key,bool value)
            {
                string fullKey = GetEditorKey(key);
                EditorPrefs.DeleteKey(fullKey);
                EditorPrefs.SetBool(fullKey, value);
            }
            public static void SetEditorPrefsFloat(string key, float value)
            {
                string fullKey = GetEditorKey(key);
                EditorPrefs.DeleteKey(fullKey);
                EditorPrefs.SetFloat(fullKey, value);
            }
            public static void SetEditorPrefsInt(string key, int value)
            {
                string fullKey = GetEditorKey(key);
                EditorPrefs.DeleteKey(fullKey);
                EditorPrefs.SetInt(fullKey, value);
            }
            public static void SetEditorPresString(string key, string value)
            {
                string fullKey = GetEditorKey(key);
                EditorPrefs.DeleteKey(fullKey);
                EditorPrefs.SetString(fullKey, value);
            }
            public static int GetEditorPrefsInt(string key)
            {
                return EditorPrefs.GetInt(GetEditorKey(key));
            }
            public static bool GetEditorPrefsBool(string key)
            {
                return EditorPrefs.GetBool(GetEditorKey(key));
            }
            public static float GetEditorPrefsFloat(string key)
            {
                return EditorPrefs.GetFloat(GetEditorKey(key));
            }
            public static string GetEditorPrefsString(string key)
            {
                return EditorPrefs.GetString(GetEditorKey(key));
            }
            public static void DeleteEditorPrefs(string key)
            {
                EditorPrefs.DeleteKey(GetEditorKey(key));
            }
            public static bool HasEditorPrefsKey(string key)
            {
                return EditorPrefs.HasKey(GetEditorKey(key));
            }
            public static string GetEditorKey(string key)
            {
                Utility.Text.ClearStringBuilder();
               return Utility.Text.Format(ApplicationConst.Editor.APPEDITORPERFIX + "_" + key);
            }
            /// <summary>
            /// 刷新unity编辑器，只在Editor环境下可用
            /// </summary>
            public static void RefreshEditor()
            {
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
        }
    }
}
