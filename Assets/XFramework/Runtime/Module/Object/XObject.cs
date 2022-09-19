using System;

namespace XFramework
{
    public class XObject : IDisposable
    {
        /// <summary>
        /// 标记Id，释放后归0，每次初始化后也会改变
        /// <para>异步操作时这很有用</para>
        /// </summary>
        public long TagId { get; private set; }

        /// <summary>
        /// 是否已经销毁了，如果是走的对象池这个就不会准确
        /// </summary>
        public bool IsDisposed => TagId == 0;

        /// <summary>
        /// 是否已经初始化过了
        /// </summary>
        private bool isAwake = false;

        /// <summary>
        /// 是否来自对象池
        /// </summary>
        private bool isFromPool = false;

        /// <summary>
        /// 是否设置过池子
        /// </summary>
        private bool setFromPool = false;

        /// <summary>
        /// 之前的标记Id
        /// </summary>
        private long beforeTagId = 0;

        protected XObject()
        {

        }

        protected virtual void OnStart()
        {
            AddTarget();    //这句话写在这的原因是不强求预先注册事件
        }

        protected virtual void OnDestroy()
        {
            
        }

        internal void Awake()
        {
            if (isAwake)
                return;

            isAwake = true;
            TagId = beforeTagId + 1;
            if (TagId == 0)
                ++TagId;

            OnStart();
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            isAwake = false;
            beforeTagId = TagId;
            TagId = 0;

            RemoveTarget();
            OnDestroy();

            if (isFromPool)
            {
                ObjectPool.Instance.Recycle(this);
            }

            setFromPool = false;
        }

        /// <summary>
        /// 设置是否来自池子
        /// </summary>
        /// <param name="fromPool"></param>
        internal void SetFromPool(bool fromPool)
        {
            if (setFromPool)
                return;

            setFromPool = true;
            isFromPool = fromPool;
        }

        /// <summary>
        /// 推送事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void Publish<T>(T args) where T : struct
        {
            EventManager.Instance?.Publish(args);
        }

        /// <summary>
        /// 移除本类的所有监听
        /// </summary>
        private void RemoveTarget()
        {
            if (this is IEvent)
                EventManager.Instance?.RemoveTarget(this);
        }

        /// <summary>
        /// 添加本类的所有监听
        /// </summary>
        protected void AddTarget()
        {
            if (this is IEvent)
                EventManager.Instance?.AddTarget(this);
        }
    }
}
