using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace XFramework
{
    internal class TaskInfo : XObject, IAwake<long, object, TaskCompletionSource<bool>>
    {
        public long Id { get; set; }

        public object Callback { get; private set; }

        public TaskCompletionSource<bool> TCS { get; private set; }

        public void Initialize(long id, object cb, TaskCompletionSource<bool> tcs)
        {
            this.Id = id;
            this.Callback = cb;
            this.TCS = tcs;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.Id = 0;
            this.Callback = null;
            this.TCS = null;
        }
    }

    public sealed class TaskManager : CommonObject, ILateUpdate
    {
        /// <summary>
        /// 有效的任务
        /// </summary>
        private SortedDictionary<long, TaskInfo> validTasks = new SortedDictionary<long, TaskInfo>();

        /// <summary>
        /// 有效的任务Id
        /// </summary>
        private HashSet<long> validTaskIds = new HashSet<long>();

        /// <summary>
        /// 无效的任务Id
        /// </summary>
        private Queue<long> invalidTaskIds = new Queue<long>();

        /// <summary>
        /// 任务数
        /// </summary>
        private long taskCount = 0;

        public void LateUpdate()
        {
            //Log.Info(validTasks.Count.ToString());

            if (validTaskIds.Count == 0)
                return;

            using var taskIds = XList<long>.Create();
            taskIds.AddRange(validTaskIds);

            foreach (var taskId in taskIds)
            {
                TaskInfo taskInfo = GetTaskInfo(taskId);
                Run(taskInfo);
            }

            while (invalidTaskIds.Count > 0)
            {
                long id = invalidTaskIds.Dequeue();
                validTaskIds.Remove(id);
            }
        }

        private void Run(TaskInfo taskInfo)
        {
            if (taskInfo == null)
                return;

            long id = taskInfo.Id;
            object o = taskInfo.Callback;

            if (o is Func<bool> actionB)
            {
                if (actionB())
                {
                    taskInfo.TCS.SetResult(true);
                    this.Remove(id);
                }
            }
            else if (o is AsyncOperation rr)
            {
                //Log.Info($"id = {id}, isDone = {rr.isDone}, progress = {rr.progress}, type = {o.GetType()}");
                if (rr.isDone)
                {
                    taskInfo.TCS.SetResult(true);
                    this.Remove(id);
                }
            }
        }

        private TaskInfo GetTaskInfo(long id)
        {
            validTasks.TryGetValue(id, out TaskInfo taskInfo);
            return taskInfo;
        }

        private TaskInfo AddTaskInfo(object o, TaskCompletionSource<bool> tcs)
        {
            long id = ++taskCount;
            if (id == 0)
                id = ++taskCount;

            //Log.Info($"add taskInfo {id}");
            TaskInfo taskInfo = ObjectFactory.Create<TaskInfo, long, object, TaskCompletionSource<bool>>(id, o, tcs, true);
            validTasks.Add(id, taskInfo);
            validTaskIds.Add(id);

            return taskInfo;
        }

        private bool Remove(long id)
        {
            if (validTasks.TryRemove(id, out var taskInfo))
            {
                invalidTaskIds.Enqueue(id);
                taskInfo.Dispose();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 等待AsyncOperation的IsDone为true
        /// </summary>
        /// <param name="asyncOperation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> WaitForCompleted(AsyncOperation asyncOperation, XCancellationToken cancellationToken = null)
        {
            if (asyncOperation == null)
                return false;

            if (asyncOperation.isDone)
                return true;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            TaskInfo taskInfo = AddTaskInfo(asyncOperation, tcs);

            void Function()
            {
                if (this.Remove(taskInfo.Id))
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

        /// <summary>
        /// 等待tillFunc返回true
        /// </summary>
        /// <param name="tillFunc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> WaitForCompleted(Func<bool> tillFunc, XCancellationToken cancellationToken = null)
        {
            if (tillFunc == null)
                return false;

            if (tillFunc())
                return true;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            TaskInfo taskInfo = AddTaskInfo(tillFunc, tcs);

            void Function()
            {
                if (this.Remove(taskInfo.Id))
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
            foreach (var taskInfo in validTasks.Values)
            {
                taskInfo.Dispose();
            }
            validTasks.Clear();
            invalidTaskIds.Clear();
        }
    }
}
