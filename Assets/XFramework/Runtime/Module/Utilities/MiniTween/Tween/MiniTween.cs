using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public enum MiniLoopType
    {
        /// <summary>
        /// 不循环
        /// </summary>
        None,
        /// <summary>
        /// 重新开始
        /// </summary>
        Restart,
        /// <summary>
        /// A到B，B到A
        /// </summary>
        Yoyo,
    }

    public abstract class MiniTween : XObject
    {
        protected XObject parent;

        protected long tagId;

        /// <summary>
        /// 已经完成/取消
        /// </summary>
        public bool IsCancel { get; protected set; }

        public Task<bool> Task { get; protected set; }

        /// <summary>
        /// 已过时间
        /// </summary>
        protected float elapsedTime;

        /// <summary>
        /// 持续时间
        /// </summary>
        protected float duration;

        /// <summary>
        /// 执行次数
        /// </summary>
        protected int executeCount;

        /// <summary>
        /// 循环类型
        /// </summary>
        protected MiniLoopType loopType;

        /// <summary>
        /// 完成后的回调
        /// </summary>
        protected Action completed_Action { get; set; }

        protected XCancellationToken cancellationToken { get; set; }

        /// <summary>
        /// 当前进度
        /// </summary>
        public float Progress => this.duration > 0 ? Mathf.Clamp01(this.elapsedTime / this.duration) : -1f;

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsCompelted => (this.Progress >= 1f && this.executeCount == 0) || this.IsCancel;

        protected override void OnStart()
        {
            base.OnStart();
            this.IsCancel = false;
            this.executeCount = 1;
            this.loopType = MiniLoopType.None;
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="completed">立即完成</param>
        public abstract void Cancel(bool completed);

        /// <summary>
        /// 取消
        /// </summary>
        public virtual void Cancel()
        {
            if (this.IsCancel)
                return;

            this.IsCancel = true;
            if (this.Progress >= 0f && this.Progress < 1f)
                this.cancellationToken?.Cancel();
            this.cancellationToken = null;
            this.Task = null;
            this.executeCount = 0;
            Common.Instance.Get<MiniTweenManager>()?.Remove(this);
        }

        internal abstract Type GetArgType();

        /// <summary>
        /// 增加已过时间之后
        /// </summary>
        protected abstract void AddElapsedTimeAfter();

        /// <summary>
        /// 重置
        /// </summary>
        protected abstract void Reset();

        /// <summary>
        /// 增加已过时间
        /// </summary>
        /// <param name="deltaTime"></param>
        internal void AddElapsedTime(float deltaTime)
        {
            if (!this.CheckIsValid())
                return;

            this.elapsedTime += deltaTime;
            this.AddElapsedTimeAfter();
            this.CheckCompleted();
        }

        /// <summary>
        /// 检验是否有效
        /// </summary>
        /// <returns></returns>
        private bool CheckIsValid()
        {
            if (parent != null)
            {
                if (parent.IsDisposed || parent.TagId != tagId)
                {
                    this.Cancel();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 检验是否完成
        /// </summary>
        /// <returns>是否完成</returns>
        protected void CheckCompleted()
        {
            if (this.Progress >= 1f)
            {
                if (this.executeCount > 0) 
                    --this.executeCount;
                if (this.IsCompelted)
                {
                    this.completed_Action?.Invoke();
                    this.Cancel();
                }
                else
                {
                    this.Reset();
                }
            }
        }

        /// <summary>
        /// 添加完成后的监听
        /// </summary>
        /// <param name="action"></param>
        public void Add_OnCompleted(Action action)
        {
            this.completed_Action += action;
        }

        /// <summary>
        /// 移除完成后的监听
        /// </summary>
        /// <param name="action"></param>
        public void Remove_OnCompleted(Action action)
        {
            this.completed_Action -= action;
        }

        /// <summary>
        /// 移除所有完成后的监听
        /// </summary>
        public void RemoveAll_OnCompleted()
        {
            this.completed_Action = null;
        }

        /// <summary>
        /// 设置循环执行
        /// </summary>
        /// <param name="loopType"></param>
        /// <param name="count">执行次数，小于0时为无限循环，0无效</param>
        public void SetLoop(MiniLoopType loopType, int count = 1)
        {
            if (count == 0)
            {
                Log.Error("MiniTween SetLoop error, count == 0");
                return;
            }

            this.loopType = loopType;
            switch (loopType)
            {
                case MiniLoopType.None:
                    this.executeCount = this.executeCount != 0 ? 1 : this.executeCount;
                    break;
                case MiniLoopType.Restart:
                    this.executeCount = count;
                    break;
                case MiniLoopType.Yoyo:
                    this.executeCount = count > 0 ? count * 2 : -1;
                    break;
                default:
                    break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.Cancel();
            this.elapsedTime = 0;
            this.duration = 0;
            this.parent = null;
            this.tagId = 0;
        }
    }

    public class MiniTween<T> : MiniTween, IAwake<XObject, T, T, float>
    {
        /// <summary>
        /// 动态变化时的监听
        /// </summary>
        protected Action<T> setValue_Action;

        /// <summary>
        /// 起始值
        /// </summary>
        protected T startValue;

        /// <summary>
        /// 目标值
        /// </summary>
        protected T endValue;

        internal override Type GetArgType()
        {
            return typeof(T);
        }

        protected override void AddElapsedTimeAfter()
        {
            throw new NotImplementedException();
        }

        protected override void Reset()
        {
            base.elapsedTime = 0;
            switch (base.loopType)
            {
                case MiniLoopType.None:
                    break;
                case MiniLoopType.Restart:
                    break;
                case MiniLoopType.Yoyo:
                    (this.startValue, this.endValue) = (this.endValue, this.startValue);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        public override void Cancel()
        {
            this.Cancel(false);
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="completed">立即完成</param>
        public override void Cancel(bool completed)
        {
            if (base.IsCancel)
                return;

            if (completed)
            {
                base.IsCancel = true;
                base.elapsedTime = base.duration;
                this.setValue_Action?.Invoke(this.endValue);
                this.completed_Action?.Invoke();
            }

            base.IsCancel = false;
            base.Cancel();
        }

        /// <summary>
        /// 添加动态监听，每变化一次调用一次监听
        /// </summary>
        /// <param name="action"></param>
        public void AddListener(Action<T> action)
        {
            this.setValue_Action += action;
        }

        /// <summary>
        /// 移除动态监听
        /// </summary>
        /// <param name="action"></param>
        public void RemoveListener(Action<T> action)
        {
            this.setValue_Action -= action;
        }

        /// <summary>
        /// 移除所有的动态监听
        /// </summary>
        public void RemoveAllListeners()
        {
            this.setValue_Action = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.RemoveAllListeners();
            this.RemoveAll_OnCompleted();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        public virtual void Initialize(XObject parent, T startValue, T endValue, float duration)
        {
            base.parent = parent;
            base.tagId = parent.TagId;
            this.startValue = startValue;
            this.endValue = endValue;
            base.duration = duration;
        }

        internal virtual void SetTask(Task<bool> task, XCancellationToken cancellationToken)
        {
            base.Task = task;
            base.cancellationToken = cancellationToken;
        }
    }
}
