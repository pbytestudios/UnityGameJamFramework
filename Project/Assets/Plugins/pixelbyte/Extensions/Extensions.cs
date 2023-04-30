using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pixelbyte.Extensions
{
    /// <summary>
    ///  This class contains various extension methods which I have found useful in some way
    ///  Copyright 2018 Pixelbyte Studios LLC All rights reserved
    /// </summary>
    public static class Extensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Returns all interfaces that are implemented on 'type' that derive from the 'baseType'
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static Type[] GetInterfacesDerivedFrom(this Type type, Type baseType)
        {
            var dtypes =  type.GetInterfaces().Where((i) => baseType.IsAssignableFrom(i) && i != baseType);
            if (dtypes == null) return new Type[0];
            else
                return dtypes.ToArray();
        }

        public static string GetFriendlyName(this Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetFriendlyName(typeParameters[i]);
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }
            return friendlyName;
        }

        public static Color Alpha(this Color c, float alpha) => new Color(c.r, c.g, c.b, Mathf.Clamp(alpha, 0f, 1f));

        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = UnityEngine.Random.Range(0, n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static void Shuffle<T>(this List<T> array)
        {
            int n = array.Count;
            while (n > 1)
            {
                int k = UnityEngine.Random.Range(0, n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
