using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace XFramework
{
    internal enum TimerType
    {
        None,
        Wait,
        Once,
        Repeat,
    }

    internal class TimerTask : XObject, IAwake<long, long, TimerType, object>
    {
        public long Id { get; private set; }

        public object Callback { get; private set; }

        public long Time { get; private set; }

        public TimerType TimerType { get; private set; }

        public void Initialize(long id, long time, TimerType type, object callback)
        {
            this.Id = id;
            this.Time = time;
            this.TimerType = type;
            this.Callback = callback;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.Id = default;
            this.Time = default;
            this.TimerType = default;
            this.Callback = default;
        }
    }

    public sealed class TimerManager : CommonObject, IUpdate
    {
        /// <summary>
        /// ����id - ��ʱ����
        /// </summary>
        private Dictionary<long, TimerTask> timerTasks = new Dictionary<long, TimerTask>();
        /// <summary>
        /// ����ʱ�� - ����id
        /// </summary>
        private SortedDictionary<long, List<long>> timerTaskIds = new SortedDictionary<long, List<long>>();
        /// <summary>
        /// �洢���й��ڵĵ���ʱ��
        /// </summary>
        private Queue<long> timerOutTimeIds = new Queue<long>();
        /// <summary>
        /// �洢���й��ڵ�����id
        /// </summary>
        private Queue<long> timerOutTimeTaskIds = new Queue<long>();
        /// <summary>
        /// ��СҪ�����ʱ��
        /// </summary>
        private long minTime = long.MaxValue;

        private long timerId = 0;

        public void Update()
        {
            if (timerTaskIds.Count == 0)
                return;

            long curTime = GetCurTime();

            if (curTime < minTime)
                return;

            foreach (var dict in timerTaskIds)
            {
                long time = dict.Key;
                if (time > curTime)
                {
                    minTime = time;
                    break;
                }
                timerOutTimeIds.Enqueue(time);
            }

            while (timerOutTimeIds.Count > 0)
            {
                long time = timerOutTimeIds.Dequeue();

                foreach (var taskId in timerTaskIds[time])
                {
                    timerOutTimeTaskIds.Enqueue(taskId);
                }

                timerTaskIds.Remove(time);
            }

            while (timerOutTimeTaskIds.Count > 0)
            {
                long id = timerOutTimeTaskIds.Dequeue();

                TimerTask timerTask = GetChild(id);
                Run(timerTask);
            }
        }

        private TimerTask GetChild(long id)
        {
            timerTasks.TryGetValue(id, out var task);
            return task;
        }

        private TimerTask AddChild(long time, TimerType timerType, object callback)
        {
            long id = ++timerId;
            if (id == 0)
                id = ++timerId;

            var task = ObjectFactory.Create(typeof(TimerTask), id, time, timerType, callback, true) as TimerTask;
            timerTasks.Add(id, task);
            return task;
        }

        private long GetCurTime()
        {
            return TimeHelper.ClientNow();
        }

        private void Run(TimerTask task)
        {
            if (task == null)
                return;

            switch (task.TimerType)
            {
                case TimerType.None:
                    break;

                case TimerType.Wait:
                    {
                        var tcs = task.Callback as TaskCompletionSource<bool>;
                        tcs?.SetResult(true);
                        Remove(task.Id);
                        break;
                    }

                case TimerType.Once:
                    {
                        Action action = task.Callback as Action;
                        action?.Invoke();
                        Remove(task.Id);
                        break;
                    }

                case TimerType.Repeat:
                    {
                        Action action = task.Callback as Action;
                        action?.Invoke();
                        long tillTime = task.Time + GetCurTime();
                        AddTimer(tillTime, task);
                        break;
                    }

                default:
                    break;
            }
        }

        private void AddTimer(long tillTime, TimerTask task)
        {
            List<long> list;
            if (!timerTaskIds.TryGetValue(tillTime, out list))
            {
                list = new List<long>();
                timerTaskIds.Add(tillTime, list);
            }
            list.Add(task.Id);

            if (tillTime < minTime)
                minTime = tillTime;
        }

        private bool Remove(long timerId)
        {
            if (timerId == 0)
                return false;

            TimerTask timerTask = GetChild(timerId);
            if (timerTask == null)
                return false;

            timerTask.Dispose();
            timerTasks.Remove(timerId);

            return true;
        }

        /// <summary>
        /// �Ƴ�һ����ʱ����
        /// </summary>
        /// <param name="timerId"></param>
        public void RemoveTimerId(ref long timerId)
        {
            Remove(timerId);
            timerId = 0;
        }

        /// <summary>
        /// �ȴ�һ��ʱ��
        /// </summary>
        /// <param name="waitTime">�ȴ�ʱ�䣨���룩</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> WaitAsync(long waitTime, XCancellationToken cancellationToken = null)
        {
            long tillTime = GetCurTime() + waitTime;
            return await WaitTillAsync(tillTime, cancellationToken);
        }

        /// <summary>
        /// �ȴ�һ֡
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> WaitFrameAsync(XCancellationToken cancellationToken = null)
        {
            return await WaitAsync(1, cancellationToken);
        }

        /// <summary>
        /// �ӳ�һ��ʱ���ִ�лص�
        /// </summary>
        /// <param name="delayTime">�ӳ�ʱ�䣨���룩</param>
        /// <param name="action">�ص�</param>
        /// <returns></returns>
        public long StartOnceTimer(long delayTime, Action action)
        {
            long tillTime = GetCurTime() + delayTime;
            TimerTask timerTask = AddChild(delayTime, TimerType.Once, action);
            AddTimer(tillTime, timerTask);

            return timerTask.Id;
        }

        /// <summary>
        /// ����һ���ظ�ִ�е�����
        /// </summary>
        /// <param name="repeatTime">�ظ�ִ�е�ʱ�䣨���룩</param>
        /// <param name="action">�ص�</param>
        /// <returns></returns>
        public long StartRepeatedTimer(long repeatTime, Action action)
        {
            long tillTime = GetCurTime() + repeatTime;
            TimerTask timerTask = AddChild(repeatTime, TimerType.Repeat, action);
            AddTimer(tillTime, timerTask);

            return timerTask.Id;
        }

        /// <summary>
        /// ÿִ֡�е�����
        /// </summary>
        /// <param name="repeatTime"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public long RepeatedFrameTimer(Action action)
        {
            return this.StartRepeatedTimer(1, action);
        }

        /// <summary>
        /// ֱ��ʱ��ﵽtillTimeʱִ��action��һ�����ڲ�Ҫ���߼�����ĵط�
        /// </summary>
        /// <param name="tillTime">ֱ��ʱ�䣬���뼶</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public long WaitTill(long tillTime, Action action)
        {
            if (tillTime <= GetCurTime())
            {
                Log.Warnning("WaitTillTimer tillTime less than current time");
            }

            TimerTask timerTask = AddChild(tillTime, TimerType.Once, action);
            AddTimer(tillTime, timerTask);

            return timerTask.Id;
        }

        /// <summary>
        /// ֱ��ʱ��ﵽtillTimeʱ����true
        /// </summary>
        /// <param name="tillTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> WaitTillAsync(long tillTime, XCancellationToken cancellationToken = null)
        {
            if (tillTime <= GetCurTime())
                return true;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            TimerTask timerTask = AddChild(tillTime, TimerType.Wait, tcs);
            AddTimer(tillTime, timerTask);

            long timerId = timerTask.Id;
            void Function()
            {
                if (Remove(timerId))
                    tcs.SetResult(false);
            }

            bool result = false;
            try
            {
                cancellationToken?.Register(Function);
                result = await tcs.Task;
            }
            finally
            {
                cancellationToken?.Remove(Function);
            }

            return result;
        }

        protected override void Destroy()
        {
            foreach (TimerTask task in timerTasks.Values)
            {
                task.Dispose();
            }

            timerTasks.Clear();
            timerTaskIds.Clear();
            timerOutTimeIds.Clear();
            timerOutTimeTaskIds.Clear();
        }
    }
}
