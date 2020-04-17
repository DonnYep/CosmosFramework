using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos { 
    public sealed partial class Utility
    {
        public sealed class Text
        {
            static StringBuilder stringBuilderCache;
           public static StringBuilder StringBuilderCache
            {
                get
                {
                    if (stringBuilderCache == null)
                        stringBuilderCache = new StringBuilder(1024);
                    return stringBuilderCache;
                }
                set { stringBuilderCache = value; }
            }
            public static string AppendFormat(string format,params object[] args)
            {
                StringBuilderCache.Length = 0;
                StringBuilderCache.AppendFormat(format, args);
                return StringBuilderCache.ToString();
            }
            public static void ClearStringBuilder()
            {
                StringBuilderCache.Clear();
            }
        }
    }
}
