using Object = UnityEngine.Object;
namespace Cosmos.Editor
{
    public static partial class EditorUtil
    {
        public static class Debug
        {
            public static void LogInfo(object msg, object context = null)
            {
                if (context == null)
                    UnityEngine.Debug.Log($"<b><color={DebugColor.cyan}>{"[EDITOR-INFO]-->>"} </color></b>{msg}");
                else
                    UnityEngine.Debug.Log($"<b><color={DebugColor.cyan}>{"[EDITOR-INFO]-->>"}</color></b>{msg}", context as Object);
            }
            public static void LogInfo(object msg, DebugColor debugColor, object context = null)
            {
                if (context == null)
                    UnityEngine.Debug.Log($"<b><color={debugColor}>{"[EDITOR-INFO]-->>"}</color></b>{msg}");
                else
                    UnityEngine.Debug.Log($"<b><color={debugColor}>{"[EDITOR-INFO]-->>"}</color></b>{msg}", context as Object);
            }
            public static void LogWarning(object msg, object context = null)
            {
                if (context == null)
                    UnityEngine.Debug.LogWarning($"<b><color={DebugColor.orange}>{"[EDITOR-WARNING]-->>" }</color></b>{msg}");
                else
                    UnityEngine.Debug.LogWarning($"<b><color={DebugColor.orange}>{"[EDITOR-WARNING]-->>" }</color></b>{msg}", context as Object);
            }
            public static void LogError(object msg, object context = null)
            {
                if (context == null)
                    UnityEngine.Debug.LogError($"<b><color={DebugColor.red}>{"[EDITOR-ERROR]-->>"} </color></b>{msg}");
                else
                    UnityEngine.Debug.LogError($"<b><color={DebugColor.red}>{"[EDITOR-ERROR]-->>"}</color></b>{msg}", context as Object);
            }
        }
    }
}