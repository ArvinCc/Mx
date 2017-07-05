using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mx
{
    namespace Json
    {
        public class Map<TKey, TValue> : Dictionary<TKey, TValue>
        {
            public void put(TKey key, TValue value)
            {
                base.Add(key, value);
            }

            public TValue get(TKey key)
            {
                if (base.ContainsKey(key))
                {
                    return base[key];
                }
                return default(TValue);
            }

            public void remove(TKey key)
            {
                base.Remove(key);
            }

            public int getSize()
            {
                return base.Count;
            }

            public List<TKey> getKeys()
            {
                return new List<TKey>(base.Keys);
            }

            public List<TValue> getValues()
            {
                return new List<TValue>(base.Values);
            }
        }
    }
}
