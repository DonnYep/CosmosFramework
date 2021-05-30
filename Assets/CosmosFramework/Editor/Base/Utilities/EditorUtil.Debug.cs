using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Cosmos.CosmosEditor
{
    public static partial class EditorUtil
    {
        public static class Debug
        {
            public static void LogInfo(object msg, object context = null)
            {
                if (context == null)
                   UnityEngine. Debug.Log($"<b><color={MessageColor.CYAN}>{"[EDITOR-INFO]-->>"} </color></b>{msg}");
                else
                    UnityEngine.Debug.Log($"<b><color={MessageColor.CYAN}>{"[EDITOR-INFO]-->>"}</color></b>{msg}", context as Object);
            }
            public static void LogInfo(object msg, string msgColor, object context = null)
            {
                if (context == null)
                    UnityEngine.Debug.Log($"<b><color={msgColor }>{"[EDITOR-INFO]-->>"}</color></b>{msg}");
                else
                    UnityEngine.Debug.Log($"<b><color={msgColor }>{"[EDITOR-INFO]-->>"}</color></b>{msg}", context as Object);
            }
            public static void LogWarning(object msg, object context = null)
            {
                if (context == null)
                    UnityEngine.Debug.LogWarning($"<b><color={MessageColor.ORANGE}>{"[EDITOR-WARNING]-->>" }</color></b>{msg}");
                else
                    UnityEngine.Debug.LogWarning($"<b><color={MessageColor.ORANGE}>{"[EDITOR-WARNING]-->>" }</color></b>{msg}", context as Object);
            }
            public static void LogError(object msg, object context = null)
            {
                if (context == null)
                    UnityEngine.Debug.LogError($"<b><color={MessageColor.RED}>{"[EDITOR-ERROR]-->>"} </color></b>{msg}");
                else
                    UnityEngine.Debug.LogError($"<b><color={MessageColor.RED}>{"[EDITOR-ERROR]-->>"}</color></b>{msg}", context as Object);
            }
            public static void LogFatal(object msg, object context = null)
            {
                if (context == null)
                    UnityEngine.Debug.LogError($"<b><color={MessageColor.RED}>{ "[EDITOR-FATAL]-->>" }</color></b>{msg}");
                else
                    UnityEngine.Debug.LogError($"<b><color={MessageColor.RED}>{ "[EDITOR-FATAL]-->>" }</color></b>{msg}", context as Object);
            }
        }
    }
}