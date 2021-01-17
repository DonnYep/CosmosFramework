using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Test{
    public class MessagePrinter: MonoBehaviour {
        public void PrintMessage(string message)
        {
            Utility.DebugLog(message, MessageColor.MAROON,this);
        }
        public void PrintWarningMessage(string message)
        {
            Utility.DebugWarning(message);
        }
        public void PrintErrorMessage(string message)
        {
            Utility.DebugError(message);
        }
    }
}