using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace XFramework
{
    public class LoadingScene : Scene
    {
        /// <summary>
        /// 场景加载总进度
        /// </summary>
        private const int SceneProgress = 10;

        /// <summary>
        /// 场景加载当前进度
        /// </summary>
        private int curSceneProgress;

        /// <summary>
        /// 预加载资源总数
        /// </summary>
        private int totalCount;

        /// <summary>
        /// 当前预加载的资源数
        /// </summary>
        private int curCount;

        /// <summary>
        /// loading界面
        /// </summary>
        private GameObject loading;

        /// <summary>
        /// 是否准备好
        /// </summary>
        private bool isReady;

        private Scene scene;

        private AsyncOperation sceneOperation;

        private MiniTween<float> tween;

        protected override void OnStart()
        {
            base.OnStart();
            totalCount = 0;
            curCount = 0;
            curSceneProgress = 0;
            sceneOperation = null;
            isReady = false;
        }

        public override void Update()
        {
            if (sceneOperation == null || isReady)
                return;

            if (!sceneOperation.isDone)
            {
                float progress = sceneOperation.progress;
                int sceneProgress = (int)(progress * 10);

                if (curSceneProgress != sceneProgress)
                {
                    curSceneProgress = sceneProgress;
                    OnProgressChanged().Coroutine();
                }

                if (progress == 0.9f)
                    isReady = true;
            }
        }

        public async Task<bool> Load(Scene scene, XCancellationToken cancellationToken)
        {
            try
            {
                this.scene = scene;
                string sceneName = scene.Name;
                var global = Common.Instance.Get<Global>();
                loading = global.Loading;
                loading.SetViewActive(true);
                var reference = loading.Reference();
                var fill = reference.GetChild<Image>("Fill");
                var progress = reference.GetChild<XText>("Progress");
                fill.fillAmount = 0;
                progress.SetText("0%");

                using var assets = XDictionary<string, Type>.Create();
                scene.GetAssets(assets);

                using var objKeys = XList<string>.Create();
                scene.GetObjects(objKeys);

                using var configTasks = XList<Task>.Create();
                var configMgr = Common.Instance.Get<ConfigManager>();
                if (configMgr != null)
                    await configMgr.DeserializeConfigs(configTasks);

                // 资源总数
                totalCount = assets.Count + objKeys.Count + configTasks.Count;

                if (totalCount > 0)
                {            
                    using (var tasks = XList<Task>.Create())
                    {
                        if (configTasks.Count > 0)
                        {
                            // 每反序列化完一个配置就加一次进度
                            async Task LoadConfig(Task configTask)
                            {
                                await configTask;
                                ++curCount;
                                this.OnProgressChanged().Coroutine();
                            }

                            foreach (var configTask in configTasks)
                            {
                                tasks.Add(LoadConfig(configTask));
                            }
                        }

                        if (assets.Count > 0)
                        {
                            foreach (var asset in assets)
                            {
                                string key = asset.Key;
                                Type type = asset.Value;
                                tasks.Add(LoadAsset(key, type));
                                //await LoadAsset(key, type);
                            }
                        }

                        if (objKeys.Count > 0)
                        {
                            foreach (var key in objKeys)
                            {
                                tasks.Add(Instantiate(key, global.GameRoot));
                            }
                        }

                        // 等待所有资源加载完成
                        await Task.WhenAll(tasks);
                    }
                }

                LoadSceneAsync(sceneName);
                var taskManager = Common.Instance.Get<TaskManager>();
                var timerManager = Common.Instance.Get<TimerManager>();

                // 等待进度到0.9f
                if (!await taskManager.WaitForCompleted(this.IsReady, cancellationToken))
                    return false;

                // 表面进度设置满，进度拉满并等待动画完成
                curSceneProgress = SceneProgress;
                await this.OnProgressChanged();

                // 延迟0.2秒
                if (!await timerManager.WaitAsync(200, cancellationToken))
                    return false;

                // 允许激活场景
                sceneOperation.allowSceneActivation = true;
                return await taskManager.WaitForCompleted(sceneOperation, cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }            
        }

        private bool IsReady()
        {
            return this.isReady;
        }

        /// <summary>
        /// 预先加载资源
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task LoadAsset(string key, Type type)
        {
            long tagId = this.TagId;
            await ResourcesManager.LoadAssetAsync(scene, key, type);
            if (tagId != this.TagId)
                return;

            ++curCount;
            OnProgressChanged().Coroutine();
        }

        /// <summary>
        /// 预先实例化场景对象并放入对象池
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private async Task Instantiate(string key, Transform parent)
        {
            long tagId = this.TagId;
            GameObject obj = await ResourcesManager.InstantiateAsync(scene, key, parent, true);
            ResourcesManager.ReleaseInstance(obj);
            if (tagId != this.TagId)
                return;

            ++curCount;
            OnProgressChanged().Coroutine();
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="name"></param>
        private void LoadSceneAsync(string name)
        {
            sceneOperation = SceneManager.LoadSceneAsync(name);
            sceneOperation.allowSceneActivation = false;
        }

        /// <summary>
        /// 刷新进度
        /// </summary>
        private async Task OnProgressChanged()
        {
            float scale = (curSceneProgress + curCount) * 1f / (totalCount + SceneProgress);
            await this.DoFill(scale);
        }

        /// <summary>
        /// 平滑过渡
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        private async Task DoFill(float scale)
        {
            tween?.Cancel();    //取消之前的动画
            tween = null;
            float duration = 0.3f;  // 动画大约持续时间
            var reference = loading.Reference();
            var fill = reference.GetChild<Image>("Fill");
            var progress = reference.GetChild<XText>("Progress");
            var tweenManager = Common.Instance.Get<MiniTweenManager>();

            float start = fill.fillAmount;
            tween = tweenManager.To(this, start, scale, duration);
            tween.AddListener(n =>
            {
                fill.fillAmount = n;
                progress.SetText("{0:F0}%", n * 100);
            });
            await tween.Task;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            scene = null;
            tween?.Cancel();
            tween = null;
            loading.SetViewActive(false);
            loading = null;
            sceneOperation = null;
        }
    }
}
