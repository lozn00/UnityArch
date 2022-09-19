using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFramework
{
    public class OrderMapList<TKey, TValue> : SortedDictionary<TKey, List<TValue>>
    {
        /// <summary>
        /// ������ӣ��������������
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryAdd(TKey key, TValue value)
        {
            if (!base.TryGetValue(key, out var list))
            {
                list = new List<TValue>();
                base.Add(key, list);
            }

            if (!list.Contains(value))
            {
                list.Add(value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// ֱ�����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            if (!base.TryGetValue(key, out var list))
            {
                list = new List<TValue>();
                base.Add(key, list);
            }
            list.Add(value);
        }

        /// <summary>
        /// ���ص�һ��value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue GetOne(TKey key)
        {
            if (base.TryGetValue(key, out var list))
            {
                return list.Count > 0 ? list[0] : default;
            }

            return default;
        }

        public bool Remove(TKey key, TValue value)
        {
            if (base.TryGetValue(key, out var list))
            {
                if (list.Remove(value))
                {
                    if (list.Count == 0)
                        base.Remove(key);

                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(TKey key, int index)
        {
            if (base.TryGetValue(key, out var list))
            {
                list.RemoveAt(index);
                if (list.Count == 0)
                    base.Remove(key);
            }
        }
    }
}
