using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace XFramework
{   
    public sealed class MiniTweenManager : CommonObject, IAwake, IUpdate
    {
        /// <summary>
        /// 所有正在执行的动画
        /// </summary>
        private HashSet<MiniTween> allTweens = new HashSet<MiniTween>();

        /// <summary>
        /// 所有失效的动画
        /// </summary>
        private Queue<MiniTween> invalidTweens = new Queue<MiniTween>();

        /// <summary>
        /// 所有支持的动画参数类型
        /// </summary>
        private Dictionary<string, Type> tweenTypes = new Dictionary<string, Type>();

        public void Initialize()
        {
            var types = TypesManager.Instance.GetTypes(typeof(MiniTweenTypeAttribute));
            foreach (var type in types)
            {
                var attris = type.GetCustomAttributes(typeof(MiniTweenTypeAttribute), false);
                if (attris.Length == 0)
                    continue;

                MiniTweenTypeAttribute attri = attris[0] as MiniTweenTypeAttribute;
                tweenTypes.Add(attri.TypeName, type);
            }
        }

        public void Update()
        {
            float deltaTime = Time.deltaTime;
            foreach (var tween in allTweens)
            {
                tween.AddElapsedTime(deltaTime);
            }

            while (invalidTweens.Count > 0)
            {
                var tween = invalidTweens.Dequeue();
                tween.Dispose();
                allTweens.Remove(tween);
            }
        }

        /// <summary>
        /// 执行动画
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public MiniTween<T> To<T>(XObject parent, T startValue, T endValue, float duration)
        {
            string typeName = typeof(T).Name;
            if (!tweenTypes.TryGetValue(typeName, out var tweenType))
            {
                Log.Error($"MiniTween.To This type({typeof(T)}) is not supported");
                return null;
            }

            if (duration <= 0)
            {
                Log.Error("MiniTween.To duration <= 0.");
                return null;
            }

            var taskManager = Common.Instance.Get<TaskManager>();
            if (taskManager == null || taskManager.IsDisposed)
            {
                Log.Error("MiniTween.To taskManager not exsit!");
                return null;
            }

            MiniTween<T> tween = ObjectFactory.Create(tweenType, parent, startValue, endValue, duration, true) as MiniTween<T>;
            var cancellationToken = new XCancellationToken();

            async Task<bool> Waiter()
            {
                return await taskManager.WaitForCompleted(() => tween.IsCompelted, cancellationToken);
            }

            tween.SetTask(Waiter(), cancellationToken);
            this.Add(tween);

            return tween;
        }

        private void Add(MiniTween tween)
        {
            this.allTweens.Add(tween);
        }

        internal void Remove(MiniTween tween)
        {
            this.invalidTweens.Enqueue(tween);
        }

        protected override void Destroy()
        {
            using var list = XList<MiniTween>.Create();
            list.AddRange(allTweens);
            foreach (var tween in list)
            {
                tween?.Dispose();
            }
            this.allTweens.Clear();
            this.invalidTweens.Clear();
            this.tweenTypes.Clear();
        }
    }
}
