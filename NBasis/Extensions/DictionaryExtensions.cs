using System.Collections.Generic;

namespace NBasis
{
    public static class DictionaryExtensions
    {
        public static void CopyTo<TKey, TValue>(this IDictionary<TKey, TValue> from, IDictionary<TKey, TValue> to)
        {
            if ((from == null) || (from.Count == 0) || (to == null)) return;
            from.Keys.ForEach((key) =>
            {
                to[key] = from[key];
            });
        }
    }
}
