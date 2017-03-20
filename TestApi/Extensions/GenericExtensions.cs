namespace TestApi.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class GenericExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }

        public static bool SafeAny<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return false;
            }

            return source.Any();
        }
    }
}