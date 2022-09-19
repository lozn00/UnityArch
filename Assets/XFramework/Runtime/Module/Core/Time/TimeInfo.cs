using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public class TimeInfo : Singleton<TimeInfo>, IDisposable
    {
        protected readonly DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        protected DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private int timeZone;

        /// <summary>
        /// ʱ������Сʱ����
        /// </summary>
        public int TimeZone
        {
            get
            {
                return this.timeZone;
            }
            set
            {
                this.timeZone = value;
                dt = dt1970.AddHours(value);
            }
        }

        public void Init()
        {

        }

        /// <summary>
        /// �ͻ��˵�ʱ��������룩
        /// </summary>
        /// <returns></returns>
        public long ClientNow()
        {
            return (this.DateTimeNow().Ticks - this.dt1970.Ticks) / 10000;
        }

        /// <summary>
        /// �ͻ��˵�ʱ������룩
        /// </summary>
        /// <returns></returns>
        public long ClientNowSeconds()
        {
            return this.ClientNow() / 1000;
        }

        /// <summary>
        /// ����ʱ��������룩��ȡʱ��
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public DateTime ToDateTime(long timeStamp)
        {
            return this.dt.AddTicks(timeStamp * 10000);
        }

        /// <summary>
        /// ʱ�����룩
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public long Transition(DateTime dateTime)
        {
            return (dateTime.Ticks - this.dt.Ticks) / 10000;
        }

        /// <summary>
        /// ��ǰʱ��
        /// </summary>
        /// <returns></returns>
        public virtual DateTime DateTimeNow()
        {
            return DateTime.Now;
        }

        public void Dispose()
        {
            
        }
    }
}
