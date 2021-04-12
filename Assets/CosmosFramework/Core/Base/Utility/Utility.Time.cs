using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    public static partial class Utility
    {
        /// <summary>
        /// 时间相关工具，提供了不同精度的时间戳等函数
        /// </summary>
        public static class Time
        {
            //1s=1000ms
            //1ms=1000us
            readonly static long epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
            /// <summary>
            /// 获取秒级别时间戳
            /// </summary>
            /// <returns>秒级别时间戳</returns>
            public static long SecondTimeStamp()
            {
                return (DateTime.UtcNow.Ticks - epoch) / 10000000;
            }
            /// <summary>
            /// 获取毫秒级别的时间戳
            /// </summary>
            /// <returns>毫秒级别时间戳</returns>
            public static long MillisecondTimeStamp()
            {
                return (DateTime.UtcNow.Ticks - epoch) / 10000;
            }
            /// <summary>
            /// 获取微秒级别时间戳
            /// </summary>
            /// <returns>微秒级别时间戳</returns>
            public static long MicrosecondTimeStamp()
            {
                return DateTime.UtcNow.Ticks - epoch;
            }
            /// <summary>
            ///秒级别UTC时间
            /// </summary>
            /// <returns>long时间</returns>
            public static long SecondNow()
            {
                return DateTime.UtcNow.Ticks / 10000000;
            }
            /// <summary>
            ///毫秒别UTC时间
            /// </summary>
            /// <returns>long时间</returns>
            public static long MillisecondNow()
            {
                return DateTime.UtcNow.Ticks / 10000;
            }
            /// <summary>
            ///微秒级别UTC时间
            /// </summary>
            /// <returns>long时间</returns>
            public static long MicrosecondNow()
            {
                return DateTime.UtcNow.Ticks;
            }
        }
    }
}
