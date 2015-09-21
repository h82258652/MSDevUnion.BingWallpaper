using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.RealtimeMessage
{
    public static class AVRMProtocolUtils
    {
        internal static T CaptureValueFromDictionary<T>(this IDictionary<string, object> data, string key)
        {
            T obj = default(T);
            try
            {
                if (data.ContainsKey(key))
                    obj = (T)data[key];
            }
            catch
            {
            }
            return obj;
        }

        internal static int CaptureInteger(this IDictionary<string, object> data, string key)
        {
            int num = 0;
            if (data.ContainsKey(key))
                num = Convert.ToInt32(data[key]);
            return num;
        }

        internal static long CaptureLong(this IDictionary<string, object> data, string key)
        {
            long num = 0L;
            if (data.ContainsKey(key))
                num = Convert.ToInt64(data[key]);
            return num;
        }

        internal static IList<T> CaptureListFromDictionary<T>(this IDictionary<string, object> data, string key)
        {
            IList<T> list = (IList<T>)new List<T>();
            foreach (object obj in AVRMProtocolUtils.CaptureValueFromDictionary<List<object>>(data, key))
                list.Add((T)obj);
            return list;
        }

        internal static void Write(this IDictionary<string, object> data, string key, object value)
        {
            if (data.ContainsKey(key))
                data[key] = value;
            else
                data.Add(key, value);
        }

        internal static IDictionary<string, object> Merge(this IDictionary<string, object> dataLeft, IDictionary<string, object> dataRight)
        {
            if (dataRight == null)
                return dataLeft;
            foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>)dataRight)
            {
                if (dataLeft.ContainsKey(keyValuePair.Key))
                    dataLeft[keyValuePair.Key] = keyValuePair.Value;
                else
                    dataLeft.Add(keyValuePair);
            }
            return dataLeft;
        }
    }
}