using System;

namespace XFramework
{
    public static class TimeHelper
    {
        /// <summary>
        /// 一天的时间戳（毫秒）
        /// </summary>
        public const long OneDay = 86400000;
        /// <summary>
        /// 一个小时的时间戳（毫秒）
        /// </summary>
        public const long Hour = 3600000;
        /// <summary>
        /// 一分钟的时间戳（毫秒）
        /// </summary>
        public const long Minute = 60000;

        /// <summary>
        /// 客户端的时间戳（毫秒）
        /// </summary>
        /// <returns></returns>
        public static long ClientNow()
        {
            return TimeInfo.Instance.ClientNow();
        }

        /// <summary>
        /// 客户端的时间戳（秒）
        /// </summary>
        /// <returns></returns>
        public static long ClientNowSeconds()
        {
            return TimeInfo.Instance.ClientNowSeconds();
        }

        /// <summary>
        /// 根据时间戳（毫秒）获取时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(long timeStamp)
        {
            return TimeInfo.Instance.ToDateTime(timeStamp);
        }

        /// <summary>
        /// 时间差（毫秒）
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long Transition(DateTime dateTime)
        {
            return TimeInfo.Instance.Transition(dateTime);
        }

        /// <summary>
        /// 获取今天的时间戳（毫秒）
        /// </summary>
        /// <returns></returns>
        public static long GetTodayTime()
        {
            long now = ClientNow();
            long ratio = now / OneDay;
            //return OneDay * ratio + TimeInfo.Instance.TimeZone * Hour;
            return OneDay * ratio;
        }

        /// <summary>
        /// 是否处于今天
        /// </summary>
        /// <param name="timeStamp">毫秒级时间戳</param>
        /// <returns></returns>
        public static bool IsToday(long timeStamp)
        {
            long today = GetTodayTime();
            long tomorrow = today + OneDay;
            return timeStamp >= today && timeStamp < tomorrow;
        }
    }
}
