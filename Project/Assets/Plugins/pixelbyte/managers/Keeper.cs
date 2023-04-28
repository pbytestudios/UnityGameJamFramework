using UnityEngine;
using System.Collections.Generic;
using System;

namespace Pixelbyte.IO
{
    public class Keeper : Singleton<Keeper>
    {
        Dictionary<string, object> pack = new Dictionary<string, object>();

        public static void Set(string key, object value)
        {
            I.pack[key] = value;
        }

        /// <summary>
        /// Gets the object stored with the specified key. 
        /// Returns the default value if the key is not found.
        /// </summary>
        public static object Get(string key, object defValue = null)
        {
            if (!I.pack.ContainsKey(key))
            {
                return defValue;
            }

            return I.pack[key];
        }

        /// <summary>
        /// Gets the instance of type T stored with the specified key.
        /// Returns the default value if the key is not found.
        /// </summary>
        public static T Get<T>(string key, T defValue)
        {
            if (!I.pack.ContainsKey(key))
            {
                return defValue;
            }

            return (T)I.pack[key];
        }

        /// <summary>
        /// Gets the instance of type T stored with the specified key.
        /// Calls the given CreateDefault() method if the key is not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="CreateDefault"></param>
        /// <returns></returns>
        public static T GetOrCreate<T>(string key, Func<T> CreateDefault)
        {
            if (!I.pack.ContainsKey(key))
            {
                return CreateDefault();
            }

            return (T)I.pack[key];
        }

        /// <summary>
        /// Gets the object stored with the specified key removing it from the "pack" if it exists
        /// Returns the default value if the key is not found.
        /// if one does not exist.
        /// </summary>
        public static object Pop(string key, object defValue = null)
        {
            object result = Get(key, defValue);

            Delete(key);
            return result;
        }

        /// <summary>
        /// Gets the instance of type T stored with the specified key removing it from the "pack" if it exists
        /// Returns the default value if the key is not found.
        /// if one does not exist.
        /// </summary>
        public static T Pop<T>(string key, T defValue)
        {
            T result = Get<T>(key, defValue);
            Delete(key);
            return result;
        }

        /// <summary>
        /// Deletes the object referenced by the given key
        /// </summary>
        /// <returns>True if an object was removed, false otherwise</returns>
        public static bool Delete(string key) { return I.pack.Remove(key); }
    }

}