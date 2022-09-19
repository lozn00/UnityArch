using System;

namespace XFramework
{
    public class XObject : IDisposable
    {
        /// <summary>
        /// ���Id���ͷź��0��ÿ�γ�ʼ����Ҳ��ı�
        /// <para>�첽����ʱ�������</para>
        /// </summary>
        public long TagId { get; private set; }

        /// <summary>
        /// �Ƿ��Ѿ������ˣ�������ߵĶ��������Ͳ���׼ȷ
        /// </summary>
        public bool IsDisposed => TagId == 0;

        /// <summary>
        /// �Ƿ��Ѿ���ʼ������
        /// </summary>
        private bool isAwake = false;

        /// <summary>
        /// �Ƿ����Զ����
        /// </summary>
        private bool isFromPool = false;

        /// <summary>
        /// �Ƿ����ù�����
        /// </summary>
        private bool setFromPool = false;

        /// <summary>
        /// ֮ǰ�ı��Id
        /// </summary>
        private long beforeTagId = 0;

        protected XObject()
        {

        }

        protected virtual void OnStart()
        {
            AddTarget();    //��仰д�����ԭ���ǲ�ǿ��Ԥ��ע���¼�
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
        /// �����Ƿ����Գ���
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
        /// �����¼�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void Publish<T>(T args) where T : struct
        {
            EventManager.Instance?.Publish(args);
        }

        /// <summary>
        /// �Ƴ���������м���
        /// </summary>
        private void RemoveTarget()
        {
            if (this is IEvent)
                EventManager.Instance?.RemoveTarget(this);
        }

        /// <summary>
        /// ��ӱ�������м���
        /// </summary>
        protected void AddTarget()
        {
            if (this is IEvent)
                EventManager.Instance?.AddTarget(this);
        }
    }
}
