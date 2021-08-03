using System.Collections.Generic;

namespace SkyAutoPro
{
    internal static class AutoProUtil
    {
        public static TValue GetValueOrDefault<Tkey, TValue>(this Dictionary<Tkey, TValue> keyValues, Tkey key)
        {
            TValue value = default(TValue);
            keyValues.TryGetValue(key, out value);
            return value;
        }
    }
}
