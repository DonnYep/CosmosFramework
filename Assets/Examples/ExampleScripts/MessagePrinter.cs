using UnityEngine;

namespace Cosmos.Test
{
    public class MessagePrinter: MonoBehaviour {
        public void PrintMessage(string message)
        {
            Utility.Debug.LogInfo(message, DebugColor.maroon,this);
        }
        public void PrintWarningMessage(string message)
        {
            Utility.Debug.LogWarning(message);
        }
        public void PrintErrorMessage(string message)
        {
            Utility.Debug.LogError(message);
        }
    }
}