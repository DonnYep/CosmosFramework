using System;
using System.Globalization;

namespace Cosmos
{
    public static partial class Utility
    {
        /// <summary>
        /// 时间相关工具，提供了不同精度的UTC时间戳等函数
        /// </summary>
        public static class Time
        {
            //1s=1000ms
            //1ms=1000us
            readonly static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            readonly static long epochTicks = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
            readonly static long secDivisor = (long)Math.Pow(10, 7);
            readonly static long msecDivisor = (long)Math.Pow(10, 4);
            /// <summary>
            /// 获取秒级别UTC时间戳
            /// </summary>
            /// <returns>秒级别时间戳</returns>
            public static long SecondTimeStamp()
            {
                return (DateTime.UtcNow.Ticks - epochTicks) / secDivisor;
            }
            /// <summary>
            /// 获取毫秒级别的UTC时间戳
            /// </summary>
            /// <returns>毫秒级别时间戳</returns>
            public static long MillisecondTimeStamp()
            {
                return (DateTime.UtcNow.Ticks - epochTicks) / msecDivisor;
            }
            /// <summary>
            /// 获取微秒级别UTC时间戳
            /// </summary>
            /// <returns>微秒级别UTC时间戳</returns>
            public static long MicrosecondTimeStamp()
            {
                return DateTime.UtcNow.Ticks - epochTicks;
            }
            /// <summary>
            ///秒级别UTC时间
            /// </summary>
            /// <returns>long时间</returns>
            public static long SecondNow()
            {
                return DateTime.UtcNow.Ticks / secDivisor;
            }
            /// <summary>
            ///毫秒别UTC时间
            /// </summary>
            /// <returns>long时间</returns>
            public static long MillisecondNow()
            {
                return DateTime.UtcNow.Ticks / msecDivisor;
            }
            /// <summary>
            ///微秒级别UTC时间
            /// </summary>
            /// <returns>long时间</returns>
            public static long MicrosecondNow()
            {
                return DateTime.UtcNow.Ticks;
            }
            /// <summary>
            /// 时间戳转UTC DateTime
            /// </summary>
            /// <returns>UTC DateTime</returns>
            public static DateTime TimestampToDateTime(long ts)
            {
                int length = (int)Math.Floor(Math.Log10(ts));
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                if (length == 9)
                    return dateTime.AddSeconds(ts);
                if (length == 12)
                    return dateTime.AddMilliseconds(ts);
                if (length == 16)
                    return dateTime.AddTicks(ts);
                return DateTime.MinValue;
            }
            /// <summary>
            /// 获取该时间相对于纪元时间的秒数；
            /// </summary>
            /// <returns>秒数</returns>
            public static long GetTotalSeconds()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalSeconds);
            }
            /// <summary>
            /// 获取该时间相对于纪元时间的毫秒数；
            /// </summary>
            /// <returns>毫秒数</returns>
            public static long GetTotalMilliseconds()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalMilliseconds);
            }
            /// <summary>
            /// 获取该时间相对于纪元时间的微秒数；
            /// </summary>
            /// <returns>微秒数</returns>
            public static long GetTotalMicroseconds()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalMilliseconds) / 10;
            }
            /// <summary>
            /// 获取该时间相对于纪元时间的分钟数
            /// </summary>
            public static double GetTotalMinutes()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalMinutes);
            }
            /// <summary>
            /// 获取该时间相对于纪元时间的小时数
            /// </summary>
            public static double GetTotalHours()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalHours);
            }
            /// <summary>
            /// 获取该时间相对于纪元时间的天数
            /// </summary>
            public static double GetTotalDays()
            {
                TimeSpan ts = DateTime.UtcNow - epoch;
                return Convert.ToInt64(ts.TotalHours);
            }
            /// <summary>
            /// 获取某一年有多少周；
            /// </summary>
            /// <param name="year">年份</param>
            /// <returns>该年周数</returns>
            public static int GetWeekAmount(int year)
            {
                var end = new DateTime(year, 12, 31); //该年最后一天
                var gc = new GregorianCalendar();
                return gc.GetWeekOfYear(end, CalendarWeekRule.FirstDay, DayOfWeek.Monday); //该年星期数
            }
            /// <summary>
            /// 得到一年中的某周的起始日和截止日；
            /// 年 nYear
            /// 周数 nNumWeek
            /// 周始 out dtWeekStart
            /// 周终 out dtWeekeEnd
            /// </summary>
            /// <param name="_"></param>
            /// <param name="year">年份</param>
            /// <param name="numWeek">第几周</param>
            /// <param name="dtWeekStart">开始日期</param>
            /// <param name="dtWeekeEnd">结束日期</param>
            public static void GetWeekTime(int year, int numWeek, out DateTime dtWeekStart, out DateTime dtWeekeEnd)
            {
                var dt = new DateTime(year, 1, 1);
                dt += new TimeSpan((numWeek - 1) * 7, 0, 0, 0);
                dtWeekStart = dt.AddDays(-(int)dt.DayOfWeek + (int)DayOfWeek.Monday);
                dtWeekeEnd = dt.AddDays((int)DayOfWeek.Saturday - (int)dt.DayOfWeek + 1);
            }
            /// <summary>
            /// 得到一年中的某周的起始日和截止日    周一到周五  工作日；
            /// </summary>
            /// <param name="_"></param>
            /// <param name="year">年份</param>
            /// <param name="numWeek">第几周</param>
            /// <param name="dtWeekStart">开始日期</param>
            /// <param name="dtWeekeEnd">结束日期</param>
            public static void GetWeekWorkTime(int year, int numWeek, out DateTime dtWeekStart, out DateTime dtWeekeEnd)
            {
                var dt = new DateTime(year, 1, 1);
                dt += new TimeSpan((numWeek - 1) * 7, 0, 0, 0);
                dtWeekStart = dt.AddDays(-(int)dt.DayOfWeek + (int)DayOfWeek.Monday);
                dtWeekeEnd = dt.AddDays((int)DayOfWeek.Saturday - (int)dt.DayOfWeek + 1).AddDays(-2);
            }
            /// <summary>
            /// 返回某年某月最后一天
            /// </summary>
            /// <param name="_"></param>
            /// <param name="year">年份</param>
            /// <param name="month">月份</param>
            /// <returns>日期</returns>
            public static int GetMonthLastDate(int year, int month)
            {
                DateTime lastDay = new DateTime(year, month, new GregorianCalendar().GetDaysInMonth(year, month));
                int day = lastDay.Day;
                return day;
            }
            /// <summary>
            /// 获得一段时间内有多少小时
            /// </summary>
            /// <param name="lhs">起始时间</param>
            /// <param name="rhs">终止时间</param>
            /// <returns>小时差</returns>
            public static string GetTimeDelay(DateTime lhs, DateTime rhs)
            {
                long lTicks = (rhs.Ticks - lhs.Ticks) / 10000000;
                string sTemp = (lTicks / 3600).ToString().PadLeft(2, '0') + ":";
                sTemp += (lTicks % 3600 / 60).ToString().PadLeft(2, '0') + ":";
                sTemp += (lTicks % 3600 % 60).ToString().PadLeft(2, '0');
                return sTemp;
            }
            /// <summary>
            /// 获取指定年份中指定月的天数
            /// </summary>
            public static int GetDaysOfMonth(int year, int month)
            {
                return month switch
                {
                    1 => 31,
                    2 => (IsLeapYear(year) ? 29 : 28),
                    3 => 31,
                    4 => 30,
                    5 => 31,
                    6 => 30,
                    7 => 31,
                    8 => 31,
                    9 => 30,
                    10 => 31,
                    11 => 30,
                    12 => 31,
                    _ => 0
                };
            }
            /// <summary>
            /// 是否为闰年
            /// </summary>
            public static bool IsLeapYear(int iYear)
            {
                //形式参数为年份
                //例如：2003
                var n = iYear;
                return n % 400 == 0 || n % 4 == 0 && n % 100 != 0;
            }
        }
    }
}
