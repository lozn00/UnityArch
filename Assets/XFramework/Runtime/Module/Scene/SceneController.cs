using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UScene = UnityEngine.SceneManagement.Scene;

namespace XFramework
{
    public sealed class SceneController : CommonObject, IUpdate, ILateUpdate, IFixedUpdate
    {
        public string CurSceneName { get; private set; }
        private Scene curScene;
        private LoadingScene loadingScene;

        /// <summary>
        /// 正在加载单个场景用于取消的令牌
        /// </summary>
        private XCancellationToken sceneCancellationToken;

        /// <summary>
        /// 正在加载的场景用于取消的令牌，不是真正意义的取消，只是取消了之后的操作
        /// </summary>
        private Dictionary<string, XCancellationToken> loadSceneCancellationToken = new Dictionary<string, XCancellationToken>();

        /// <summary>
        /// 正在卸载的场景用于取消的令牌，不是真正意义的取消，只是取消了之后的操作
        /// </summary>
        private Dictionary<string, XCancellationToken> unloadSceneCancellationToken = new Dictionary<string, XCancellationToken>();

        #region LoadScene
        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        public async Task<bool> LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
        {
            var taskManager = Common.Instance.Get<TaskManager>();

            if (this.loadSceneCancellationToken.TryGetValue(sceneName, out XCancellationToken token))
            {
                Log.Error($"{sceneName} scene is loading.");
                return false;
            }

            token = new XCancellationToken();
            this.loadSceneCancellationToken.Add(sceneName, token);

            // 开始异步加载场景
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            operation.allowSceneActivation = true;

            // 等待附加场景加载完成，如果因为某原因取消了加载，则返回false
            bool result = await taskManager.WaitForCompleted(operation, token);
            this.loadSceneCancellationToken?.Remove(sceneName);
            UIHelper.Clear();

            return result;
        }

        /// <summary>
        /// 异步加载场景，无参数初始化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sceneName"></param>
        /// <param name="isFromPool"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        public async Task<T> LoadSceneAsync<T>(string sceneName, bool isFromPool = false, LoadSceneMode loadSceneMode = LoadSceneMode.Single) where T : Scene, new()
        {
            try
            {
                T newScene = await this.LoadAsync<T>(sceneName, isFromPool, loadSceneMode);
                if (newScene is null)
                    return null;

                ObjectHelper.Awake(newScene);

                if (loadSceneMode == LoadSceneMode.Single)
                {
                    this.SetScene(newScene);
                }
                else if (loadSceneMode == LoadSceneMode.Additive)
                {
                    this.curScene.AddAdditiveScene(newScene);
                }

                return newScene;
            }
            catch (Exception e) 
            {
                Log.Error(e);
                return null;
            }
        }

        /// <summary>
        /// 异步加载场景，带1个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <param name="sceneName"></param>
        /// <param name="p1"></param>
        /// <param name="isFromPool"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        public async Task<T> LoadSceneAsync<T, P1>(string sceneName, P1 p1, bool isFromPool = false, LoadSceneMode loadSceneMode = LoadSceneMode.Single) where T : Scene, new()
        {
            try
            {
                T newScene = await this.LoadAsync<T>(sceneName, isFromPool, loadSceneMode);
                if (newScene is null)
                    return null;

                ObjectHelper.Awake(newScene, p1);

                if (loadSceneMode == LoadSceneMode.Single)
                {
                    this.SetScene(newScene);
                }
                else if (loadSceneMode == LoadSceneMode.Additive)
                {
                    this.curScene.AddAdditiveScene(newScene);
                }

                return newScene;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }            
        }

        /// <summary>
        /// 异步加载场景，带2个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <typeparam name="P2"></typeparam>
        /// <param name="sceneName"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="isFromPool"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        public async Task<T> LoadSceneAsync<T, P1, P2>(string sceneName, P1 p1, P2 p2, bool isFromPool = false, LoadSceneMode loadSceneMode = LoadSceneMode.Single) where T : Scene, new()
        {
            try
            {
                T newScene = await this.LoadAsync<T>(sceneName, isFromPool, loadSceneMode);
                if (newScene is null)
                    return null;

                ObjectHelper.Awake(newScene, p1, p2);

                if (loadSceneMode == LoadSceneMode.Single)
                {
                    this.SetScene(newScene);
                }
                else if (loadSceneMode == LoadSceneMode.Additive)
                {
                    this.curScene.AddAdditiveScene(newScene);
                }

                return newScene;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }            
        }

        /// <summary>
        /// 异步加载场景，带3个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <typeparam name="P2"></typeparam>
        /// <typeparam name="P3"></typeparam>
        /// <param name="sceneName"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="isFromPool"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        public async Task<T> LoadSceneAsync<T, P1, P2, P3>(string sceneName, P1 p1, P2 p2, P3 p3, bool isFromPool = false, LoadSceneMode loadSceneMode = LoadSceneMode.Single) where T : Scene, new()
        {
            try
            {
                T newScene = await this.LoadAsync<T>(sceneName, isFromPool, loadSceneMode);
                if (newScene is null)
                    return null;

                ObjectHelper.Awake(newScene, p1, p2, p3);

                if (loadSceneMode == LoadSceneMode.Single)
                {
                    this.SetScene(newScene);
                }
                else if (loadSceneMode == LoadSceneMode.Additive)
                {
                    this.curScene.AddAdditiveScene(newScene);
                }

                return newScene;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }            
        }

        /// <summary>
        /// 异步加载场景，带4个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <typeparam name="P2"></typeparam>
        /// <typeparam name="P3"></typeparam>
        /// <typeparam name="P4"></typeparam>
        /// <param name="sceneName"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="isFromPool"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        public async Task<T> LoadSceneAsync<T, P1, P2, P3, P4>(string sceneName, P1 p1, P2 p2, P3 p3, P4 p4, bool isFromPool = false, LoadSceneMode loadSceneMode = LoadSceneMode.Single) where T : Scene, new()
        {
            try
            {
                T newScene = await this.LoadAsync<T>(sceneName, isFromPool, loadSceneMode);
                if (newScene is null)
                    return null;

                ObjectHelper.Awake(newScene, p1, p2, p3, p4);

                if (loadSceneMode == LoadSceneMode.Single)
                {
                    this.SetScene(newScene);
                }
                else if (loadSceneMode == LoadSceneMode.Additive)
                {
                    this.curScene.AddAdditiveScene(newScene);
                }

                return newScene;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }            
        }

        /// <summary>
        /// 异步加载场景，带5个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <typeparam name="P2"></typeparam>
        /// <typeparam name="P3"></typeparam>
        /// <typeparam name="P4"></typeparam>
        /// <typeparam name="P5"></typeparam>
        /// <param name="sceneName"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="p5"></param>
        /// <param name="isFromPool"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        public async Task<T> LoadSceneAsync<T, P1, P2, P3, P4, P5>(string sceneName, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, bool isFromPool = false, LoadSceneMode loadSceneMode = LoadSceneMode.Single) where T : Scene, new()
        {
            try
            {
                T newScene = await this.LoadAsync<T>(sceneName, isFromPool, loadSceneMode);
                if (newScene is null)
                    return null;

                ObjectHelper.Awake(newScene, p1, p2, p3, p4, p5);

                if (loadSceneMode == LoadSceneMode.Single)
                {
                    this.SetScene(newScene);
                }
                else if (loadSceneMode == LoadSceneMode.Additive)
                {
                    this.curScene.AddAdditiveScene(newScene);
                }

                return newScene;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }            
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sceneName"></param>
        /// <param name="isFromPool"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        private async Task<T> LoadAsync<T>(string sceneName, bool isFromPool, LoadSceneMode loadSceneMode = LoadSceneMode.Single) where T : Scene, new()
        {
            try
            {
                var taskManager = Common.Instance.Get<TaskManager>();
                if (loadSceneMode == LoadSceneMode.Single)
                {
                    if (loadingScene != null)
                    {
                        Log.Error($"LoadScene faild! A scene is currently loading.");
                        return null;
                    }

                    // 加载loading场景
                    AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName.Loading, LoadSceneMode.Additive);
                    operation.allowSceneActivation = true;

                    // 等待loading场景加载完成，如果因为某原因取消了加载，则返回null
                    this.sceneCancellationToken = new XCancellationToken();
                    if (!await taskManager.WaitForCompleted(operation, sceneCancellationToken))
                        return null;

                    // 创建Loading场景类和要加载的场景类
                    this.loadingScene = ObjectFactory.Create<LoadingScene>(true);
                    T newScene = ObjectFactory.CreateNoInit<T>(isFromPool);
                    this.ClearScene();
                    UIHelper.Clear();
                    newScene.Init(sceneName);

                    // 等待loading场景将资源和新场景加载完毕
                    this.sceneCancellationToken = new XCancellationToken();
                    bool result = await this.loadingScene.Load(newScene, sceneCancellationToken);

                    // loading场景履行了其职责后销毁
                    this.loadingScene.Dispose();
                    this.loadingScene = null;

                    // 如果取消了加载，则返回null
                    if (!result)
                        return null;

                    return newScene;
                }
                else if (loadSceneMode == LoadSceneMode.Additive)
                {
                    if (this.curScene is null)
                    {
                        Log.Error("There is currently no main scene.");
                        return null;
                    }

                    if (this.loadSceneCancellationToken.TryGetValue(sceneName, out XCancellationToken token))
                    {
                        Log.Error($"{sceneName} scene is loading.");
                        return null;
                    }

                    token = new XCancellationToken();
                    this.loadSceneCancellationToken.Add(sceneName, token);

                    // 加载附加场景 Start
                    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                    operation.allowSceneActivation = true;

                    // 等待附加场景加载完成，如果因为某原因取消了加载，则返回false
                    bool result = await taskManager.WaitForCompleted(operation, token);
                    this.loadSceneCancellationToken?.Remove(sceneName);

                    if (!result)
                        return null;

                    // 创建新场景类
                    T newScene = ObjectFactory.CreateNoInit<T>(isFromPool);
                    newScene.Init(sceneName);

                    return newScene;
                }

                return null;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }           
        }
        #endregion

        private void SetScene(Scene newScene)
        {
            this.curScene = newScene;
            this.CurSceneName = newScene.Name;
        }

        private void ClearScene()
        {
            this.curScene?.Dispose();
            this.curScene = null;
            this.CurSceneName = null;
        }

        protected override void Init()
        {
            
        }

        public void Update()
        {
            loadingScene?.Update();
            curScene?.Update();
        }

        public void LateUpdate()
        {
            loadingScene?.Update();
            curScene?.LateUpdate();
        }

        public void FixedUpdate()
        {
            loadingScene?.Update();
            curScene?.FixedUpdate();
        }

        protected override void Destroy()
        {
            sceneCancellationToken?.Cancel();
            sceneCancellationToken = null;

            foreach (var token in loadSceneCancellationToken.Values)
            {
                token.Cancel();
            }
            foreach (var token in unloadSceneCancellationToken.Values)
            {
                token.Cancel();
            }

            loadSceneCancellationToken.Clear();
            unloadSceneCancellationToken.Clear();
            curScene?.Dispose();
            curScene = null;
            CurSceneName = null;
            loadingScene?.Dispose();
            loadingScene = null;
        }
    }
}
