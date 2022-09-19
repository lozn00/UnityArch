using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace XFramework
{
    public sealed class TypesManager : Singleton<TypesManager>, IDisposable
    {
        /// <summary>
        /// 注册过的程序集
        /// </summary>
        private Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

        /// <summary>
        /// 存储所有的被标记了ClassBaseAttribute的类
        /// <para>Key : 标记的Attribute</para>
        /// <para>Value : 被标记的类的Type列表</para>
        /// </summary>
        private UnOrderMapSet<Type, Type> allTypes = new UnOrderMapSet<Type, Type>();

        /// <summary>
        /// 所有继承了IEvent(T)接口的类
        /// <para>Key : 类的Type</para>
        /// <para>Value : T类型列表</para>
        /// </summary>
        private UnOrderMapSet<Type, Type> allEvents = new UnOrderMapSet<Type, Type>();

        public void Init()
        {
            Add(this.GetType().Assembly);
        }

        public void Add(Assembly ass)
        {
            string assName = ass.FullName;
            assemblies[assName] = ass;

            allTypes.Clear();
            allEvents.Clear();
            foreach (Assembly assembly in assemblies.Values)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract)
                        continue;

                    //找到所有标记了BaseAttribute特性的类，包括其继承的特性
                    //然后存到allType里面去，方便后续获取标记特性的类型
                    var clasAttris = type.GetCustomAttributes(typeof(BaseAttribute), true);
                    if (clasAttris.Length > 0)
                    {
                        foreach (BaseAttribute attri in clasAttris)
                        {
                            allTypes.Add(attri.AttributeType, type);
                        }
                    }

                    var interfaces = type.GetInterfaces();
                    if (interfaces.Length > 0)
                    {
                        foreach (Type interfaceType in interfaces)
                        {
                            //Log.Info(interfaceType.Name);
                            //此处是将继承了IEvent<T>接口的类里面的T的类型存在，方便后续一次性注册事件
                            if (interfaceType.Name == "IEvent`1")
                            {
                                Type[] genericArguments = interfaceType.GetGenericArguments();
                                foreach (Type argumentType in genericArguments)
                                {
                                    allEvents.Add(type, argumentType);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取标记了指定特性的类
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public HashSet<Type> GetTypes(Type attributeType)
        {
            if (allTypes.TryGetValue(attributeType, out var set))
                return set;

            return new HashSet<Type>();
        }

        /// <summary>
        /// 获取所有继承了IEvent(T)其中T的类型
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public HashSet<Type> GetEvents(Type objectType)
        {
            if (allEvents.TryGetValue(objectType, out var set))
                return set;

            return new HashSet<Type>();
        }

        /// <summary>
        /// 尝试获取所有继承了IEvent(T)其中T的类型
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetEvents(Type objectType, out HashSet<Type> result)
        {
            result = null;

            if (allEvents.TryGetValue(objectType, out var set))
                result = set;

            return result != null;
        }

        public void Dispose()
        {
            assemblies.Clear();
            allTypes.Clear();
            Instance = null;
        }
    }
}
