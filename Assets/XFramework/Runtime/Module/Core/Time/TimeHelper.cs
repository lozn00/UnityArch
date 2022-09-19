using System;

namespace XFramework
{
    public static class TimeHelper
    {
        /// <summary>
        /// һ���ʱ��������룩
        /// </summary>
        public const long OneDay = 86400000;
        /// <summary>
        /// һ��Сʱ��ʱ��������룩
        /// </summary>
        public const long Hour = 3600000;
        /// <summary>
        /// һ���ӵ�ʱ��������룩
        /// </summary>
        public const long Minute = 60000;

        /// <summary>
        /// �ͻ��˵�ʱ��������룩
        /// </summary>
        /// <returns></returns>
        public static long ClientNow()
        {
            return TimeInfo.Instance.ClientNow();
        }

        /// <summary>
        /// �ͻ��˵�ʱ������룩
        /// </summary>
        /// <returns></returns>
        public static long ClientNowSeconds()
        {
            return TimeInfo.Instance.ClientNowSeconds();
        }

        /// <summary>
        /// ����ʱ��������룩��ȡʱ��
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(long timeStamp)
        {
            return TimeInfo.Instance.ToDateTime(timeStamp);
        }

        /// <summary>
        /// ʱ�����룩
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long Transition(DateTime dateTime)
        {
            return TimeInfo.Instance.Transition(dateTime);
        }

        /// <summary>
        /// ��ȡ�����ʱ��������룩
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
        /// �Ƿ��ڽ���
        /// </summary>
        /// <param name="timeStamp">���뼶ʱ���</param>
        /// <returns></returns>
        public static bool IsToday(long timeStamp)
        {
            long today = GetTodayTime();
            long tomorrow = today + OneDay;
            return timeStamp >= today && timeStamp < tomorrow;
        }
    }
}
