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
        /// ���������ܽ���
        /// </summary>
        private const int SceneProgress = 10;

        /// <summary>
        /// �������ص�ǰ����
        /// </summary>
        private int curSceneProgress;

        /// <summary>
        /// Ԥ������Դ����
        /// </summary>
        private int totalCount;

        /// <summary>
        /// ��ǰԤ���ص���Դ��
        /// </summary>
        private int curCount;

        /// <summary>
        /// loading����
        /// </summary>
        private GameObject loading;

        /// <summary>
        /// �Ƿ�׼����
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

                // ��Դ����
                totalCount = assets.Count + objKeys.Count + configTasks.Count;

                if (totalCount > 0)
                {            
                    using (var tasks = XList<Task>.Create())
                    {
                        if (configTasks.Count > 0)
                        {
                            // ÿ�����л���һ�����þͼ�һ�ν���
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

                        // �ȴ�������Դ�������
                        await Task.WhenAll(tasks);
                    }
                }

                LoadSceneAsync(sceneName);
                var taskManager = Common.Instance.Get<TaskManager>();
                var timerManager = Common.Instance.Get<TimerManager>();

                // �ȴ����ȵ�0.9f
                if (!await taskManager.WaitForCompleted(this.IsReady, cancellationToken))
                    return false;

                // ��������������������������ȴ��������
                curSceneProgress = SceneProgress;
                await this.OnProgressChanged();

                // �ӳ�0.2��
                if (!await timerManager.WaitAsync(200, cancellationToken))
                    return false;

                // �������
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
        /// Ԥ�ȼ�����Դ
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
        /// Ԥ��ʵ�����������󲢷�������
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
        /// �첽���س���
        /// </summary>
        /// <param name="name"></param>
        private void LoadSceneAsync(string name)
        {
            sceneOperation = SceneManager.LoadSceneAsync(name);
            sceneOperation.allowSceneActivation = false;
        }

        /// <summary>
        /// ˢ�½���
        /// </summary>
        private async Task OnProgressChanged()
        {
            float scale = (curSceneProgress + curCount) * 1f / (totalCount + SceneProgress);
            await this.DoFill(scale);
        }

        /// <summary>
        /// ƽ������
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        private async Task DoFill(float scale)
        {
            tween?.Cancel();    //ȡ��֮ǰ�Ķ���
            tween = null;
            float duration = 0.3f;  // ������Լ����ʱ��
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
