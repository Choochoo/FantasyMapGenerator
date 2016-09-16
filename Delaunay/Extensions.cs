using System;
using System.Collections.Generic;
using System.Linq;

namespace Delaunay
{
    public static class Extensions
    {
        public static T Pop<T>(this List<T> list)
        {
            T item = list.Last();
            list.RemoveAt(list.Count - 1);
            return item;
        }

        public static int Push<T>(this List<T> list, T item)
        {
            list.Add(item);
            return list.Count;
        }

        public static List<T> Filter<T>(this List<T> list, Func<T, int, List<T>, bool> check)
        {
            List<T> approved = new List<T>();
            int i = 0;
            foreach (T item in list)
            {
                if (check.Invoke(item, i, list))
                {
                    approved.Add(item);
                }
                i++;
            }
            return approved;
        }

        public static void SortFunc<T>(this List<T> list, Func<T, T, float> check)
        {
            int n = list.Count - 1;
            for (int i = 0; i < n; i++)
            {

                for (int j = n; j > i; j--)
                {
                    if (check.Invoke(list[j - 1], list[j]) > 0)
                    {
                        T temp = list[j - 1];
                        list[j - 1] = list[j];
                        list[j] = temp;
                    }
                }
            }
        }
    }
}
