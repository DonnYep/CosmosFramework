using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public sealed partial class Utility
    {
        public static class Debug
        {
            static IDebugHelper debugHelper = null;
            public static void SetHelper(IDebugHelper helper)
            {
                debugHelper = helper;
            }
            public static void ClearHelper()
            {
                debugHelper = null;
            }
            public static void LogInfo(object msg, Object context = null)
            {
                debugHelper?.LogInfo(msg, context);
            }
            public static void LogInfo(object msg, string msgColor, Object context = null)
            {
                debugHelper?.LogInfo(msg, msgColor, context);
            }
            public static void LogWarning(object msg, Object context = null)
            {
                debugHelper?.LogWarning(msg, context);
            }
            public static void LogError(object o, Object context = null)
            {
                debugHelper?.LogError(o, context);
            }
            public static void LogFatal(object o, Object context = null)
            {
                debugHelper?.LogFatal(o, context);
            }
        }
    }
}