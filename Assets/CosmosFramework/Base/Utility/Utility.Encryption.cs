using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    public static partial class Utility
    {
        /// <summary>
        /// 加密工具
        /// </summary>
        public static class Encryption
        {
            public  enum GUIDFormat
            {
                N,D,B,P,X
            }
            public static string GUID(GUIDFormat format)
            {
                return Guid.NewGuid().ToString(format.ToString());
            }
        }
    }
}