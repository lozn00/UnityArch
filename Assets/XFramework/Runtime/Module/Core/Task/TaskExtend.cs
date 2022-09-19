using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace XFramework
{
    public static class TaskExtend
    {
        /// <summary>
        /// 当协程用，不会阻塞
        /// </summary>
        /// <param name="self"></param>
        public static void Coroutine(this Task self)
        {
            StartCoroutine(self.CoroutineInner());
        }

        private async static Task CoroutineInner(this Task self)
        {
            try
            {
                await self;
            }
            catch (System.Exception e)
            {
                Log.Error(e);
            }
        }

        private static void StartCoroutine(Task task)
        {

        }
    }
}
