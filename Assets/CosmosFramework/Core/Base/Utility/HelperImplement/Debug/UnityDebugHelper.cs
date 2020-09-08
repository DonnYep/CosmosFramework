using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Cosmos
{
    public class UnityDebugHelper : IDebugHelper
    {
        public void LogInfo(object msg, object context)
        {
            if (context == null)
                Debug.Log("<b>-->><color=" + MessageColor.BLUE + ">" + msg + "</color></b>");
            else
                Debug.Log("<b>-->><color=" + MessageColor.BLUE + ">" + msg + "</color></b>", context as Object);
        }
        public void LogInfo(object msg, string msgColor, object context)
        {
            if (context == null)
                Debug.Log("<b>-->><color=" + msgColor + ">" + msg + "</color></b>");
            else
                Debug.Log("<b>-->><color=" + msgColor + ">" + msg + "</color></b>", context as Object);
        }
        public void LogError(object msg, object context)
        {
            if (context == null)
                Debug.LogError("<b>-->><color=#FF0000>" + msg + "</color></b>");
            else
                Debug.LogError("<b>-->><color=#FF0000>" + msg + "</color></b>", context as Object);
        }

        public void LogWarning(object msg, object context)
        {
            if (context == null)
                Debug.LogWarning("<b>-->><color=#FF5E00>" + msg + "</color></b>");
            else
                Debug.LogWarning("<b>-->><color=#FF5E00>" + msg + "</color></b>", context as Object);
        }
        public void LogFatal(object msg, object context)
        {
            if (context == null)
                Debug.LogError ("<b>-->><color=#FF5E00>" + msg + "</color></b>");
            else
                Debug.LogError("<b>-->><color=#FF5E00>" + msg + "</color></b>", context as Object);
        }
    }
}
