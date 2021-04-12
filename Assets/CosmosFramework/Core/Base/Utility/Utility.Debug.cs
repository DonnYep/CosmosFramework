using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public static partial class Utility
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
            public static void LogInfo(object msg, object context = null)
            {
                debugHelper?.LogInfo(msg, context);
            }
            public static void LogInfo(object msg, string msgColor, object context = null)
            {
                debugHelper?.LogInfo(msg, msgColor, context);
            }
            public static void LogWarning(object msg, object context = null)
            {
                debugHelper?.LogWarning(msg, context);
            }
            public static void LogError(object o, object context = null)
            {
                debugHelper?.LogError(o, context);
            }
            public static void LogFatal(object o, object context = null)
            {
                debugHelper?.LogFatal(o, context);
            }
        }
    }
}