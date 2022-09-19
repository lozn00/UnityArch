using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public interface IAwake
    {
        void Initialize();
    }

    public interface IAwake<T>
    {
        void Initialize(T args);
    }

    public interface IAwake<T1, T2>
    {
        void Initialize(T1 arg1, T2 arg2);
    }

    public interface IAwake<T1, T2, T3>
    {
        void Initialize(T1 arg1, T2 arg2, T3 arg3);
    }

    public interface IAwake<T1, T2, T3, T4>
    {
        void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }

    public interface IAwake<T1, T2, T3, T4, T5>
    {
        void Initialize(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }
}
