using System;
using System.Globalization;

namespace Cosmos
{
    public static class DateTimeExts
    {
        /// <summary>
        /// Unix 纪元（1970-01-01T00:00:00Z）的 ticks
        /// </summary>
        static readonly long EpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        /// <summary>
        /// 返回年度第几个星期   默认星期日是第一天
        /// </summary>
        public static int WeekOfYear(this in DateTime @this)
        {
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(@this, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
        /// <summary>
        /// 返回年度第几个星期
        /// </summary>
        public static int WeekOfYear(this in DateTime @this, DayOfWeek week)
        {
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(@this, CalendarWeekRule.FirstDay, week);
        }
        /// <summary>
        /// 返回相对于当前时间的相对天数
        /// </summary>
        public static string GetDateTime(this in DateTime @this, int relativeday)
        {
            return @this.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTimeF(this in DateTime @this)
        {
            return @this.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }
        /// <summary>
        /// 获取该时间相对于纪元时间的分钟数
        /// </summary>
        public static double GetTotalMinutes(this in DateTime @this) 
        {
            return (@this.ToUniversalTime().Ticks - EpochTicks) / (double)TimeSpan.TicksPerMinute;
        }
        /// <summary>
        /// 获取该时间相对于纪元时间的小时数
        /// </summary>
        public static double GetTotalHours(this in DateTime @this)
        {
            return (@this.ToUniversalTime().Ticks - EpochTicks) / (double)TimeSpan.TicksPerHour;
        }
        /// <summary>
        /// 获取该时间相对于纪元时间的天数
        /// </summary>
        public static double GetTotalDays(this in DateTime @this) 
        {
            return (@this.ToUniversalTime().Ticks - EpochTicks) / (double)TimeSpan.TicksPerDay;
        }
    }
}
