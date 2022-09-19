using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace XFramework
{
    public class Game : Singleton<Game>, IDisposable
    {
        public void Start()
        {
            Log.ILog = new UnityLogger();

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error($"{e}");
            };

            //SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance); 
            //ThreadSynchronizationContext.Instance.Update();

            ResourcesManager.Instance.SetLoader(new UnityResourcesManager());
            TimeInfo.Instance.Init();
            TypesManager.Instance.Init();
            ObjectPool.Instance.Init();
            GameObjectPool.Instance.Init();

            ObjectFactory.Create<ConfigManager>();
            ObjectFactory.Create<ResourcesRefDetection>();
            ObjectFactory.Create<TimerManager>();
            ObjectFactory.Create<TaskManager>();
            ObjectFactory.Create<MiniTweenManager>();
            ObjectFactory.Create<Global>();
            ObjectFactory.Create<UIEventManager>();
            ObjectFactory.Create<UIManager>();
            var sceneController = ObjectFactory.Create<SceneController>();

            sceneController.LoadSceneAsync<StartScene>(SceneName.Start).Coroutine();
            //sceneController.LoadSceneAsync<StartScene>(SceneName.Start).Coroutine();
        }

        public void Update()
        {
            try
            {
                Common.Instance.Update();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void LateUpdate()
        {
            try
            {
                Common.Instance.LateUpdate();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void FixedUpdate()
        {
            try
            {
                Common.Instance.FixedUpdate();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void Dispose()
        {
            EventManager.Instance.Dispose();
            TypesManager.Instance.Dispose();
            Common.Instance.Dispose();
            ObjectPool.Instance.Dispose();
            GameObjectPool.Instance.Dispose();
            TimeInfo.Instance.Dispose();
            ResourcesManager.Instance.Dispose();

            Instance = null;
        }
    }
}
