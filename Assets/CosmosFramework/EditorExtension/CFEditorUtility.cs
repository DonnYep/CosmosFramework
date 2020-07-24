using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
# if UNITY_EDITOR
using UnityEditor;
#endif
# if UNITY_EDITOR
namespace Cosmos
{
    public sealed partial class CFEditorUtility
    {
        public static void SetEditorPrefsBool(string key, bool value)
        {
#if UNITY_EDITOR
            string fullKey = GetEditorKey(key);
            EditorPrefs.DeleteKey(fullKey);
            EditorPrefs.SetBool(fullKey, value);
#endif
        }
        public static void SetEditorPrefsFloat(string key, float value)
        {
#if UNITY_EDITOR
            string fullKey = GetEditorKey(key);
            EditorPrefs.DeleteKey(fullKey);
            EditorPrefs.SetFloat(fullKey, value);
#endif
        }
        public static void SetEditorPrefsInt(string key, int value)
        {
#if UNITY_EDITOR

            string fullKey = GetEditorKey(key);
            EditorPrefs.DeleteKey(fullKey);
            EditorPrefs.SetInt(fullKey, value);
#endif
        }
        public static void SetEditorPresString(string key, string value)
        {
#if UNITY_EDITOR
            string fullKey = GetEditorKey(key);
            EditorPrefs.DeleteKey(fullKey);
            EditorPrefs.SetString(fullKey, value);
#endif
        }
        public static int GetEditorPrefsInt(string key)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetInt(GetEditorKey(key));
#else
                return 0;
#endif
        }
        public static bool GetEditorPrefsBool(string key)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetBool(GetEditorKey(key));
#else
                return false;
#endif
        }
        public static float GetEditorPrefsFloat(string key)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetFloat(GetEditorKey(key));
#else
                return 0;
#endif
        }
        public static string GetEditorPrefsString(string key)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetString(GetEditorKey(key));
#else
                return "";
#endif
        }
        public static void DeleteEditorPrefs(string key)
        {
#if UNITY_EDITOR
            EditorPrefs.DeleteKey(GetEditorKey(key));
#endif
        }
        public static bool HasEditorPrefsKey(string key)
        {
#if UNITY_EDITOR
            return EditorPrefs.HasKey(GetEditorKey(key));
#else
                return false;
#endif
        }
        public static string GetEditorKey(string key)
        {
#if UNITY_EDITOR

            Utility.Text.ClearStringBuilder();
            return Utility.Text.Format(EditorConst.APPEDITORPERFIX + "_" + key);
#else
                return "";
#endif
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
#endif

