using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace XFramework
{
    public class ConfigManager : CommonObject
    {
        /// <summary>
        /// �����ļ��� -> ����������
        /// </summary>
        protected Dictionary<string, byte[]> configBytes = new Dictionary<string, byte[]>();

        /// <summary>
        /// �������� -> �����л������Ķ���
        /// </summary>
        protected Dictionary<Type, object> configProtos = new Dictionary<Type, object>();

        /// <summary>
        /// �����Config���Ե�������
        /// <para>���� -> ����</para>
        /// </summary>
        protected Dictionary<string, Type> configTypes = new Dictionary<string, Type>();

        protected override void Init()
        {
            var types = TypesManager.Instance.GetTypes(typeof(ConfigAttribute));
            foreach (var type in types)
            {
                configTypes.Add(type.Name, type);
            }
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <returns></returns>
        public virtual async Task LoadAllConfigs()
        {
            var files = Directory.GetFiles("Assets/Resources/Config/Gen", "*.bytes");

            async Task Load(string fileName)
            {
                TextAsset asset = await ResourcesManager.LoadAssetAsync<TextAsset>(this, $"Config/Gen/{fileName}");
                if (asset is null)
                    return;

                configBytes[fileName] = asset.bytes;
            }

            using var tasks = XList<Task>.Create();
            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                tasks.Add(Load(fileName));
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// �����л����е�����
        /// </summary>
        /// <returns></returns>
        public virtual async Task DeserializeConfigs(List<Task> tasks)
        {
            if (configTypes.Count == configProtos.Count)
                return;

            if (configBytes.Count == 0)
                await this.LoadAllConfigs();

            if (configBytes.Count == 0)
                return;

            foreach (var configInfo in configBytes)
            {
                string name = configInfo.Key;
                byte[] bytes = configInfo.Value;
                if (configTypes.TryGetValue(name, out Type configType))
                {
                    if (configProtos.ContainsKey(configType))
                        continue;

                    tasks.Add(Deserialize(configType, bytes));
                }
                else
                {
                    configTypes.Remove(name);
                    Log.Error($"config name is {name} not exist.");
                }
            }
        }

        /// <summary>
        /// �����л�����
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected async Task Deserialize(Type configType, byte[] bytes)
        {
            var task = Task.Run(() =>
            {
                object obj = ProtobufHelper.FromBytes(bytes, configType);
                return obj;
            });

            object configObj = await task;
            configProtos.Add(configType, configObj);
        }

        protected override void Destroy()
        {
            configBytes.Clear();
            configProtos.Clear();
            configTypes.Clear();
        }
    }
}
