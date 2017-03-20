namespace CMS.Library.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class GenericExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.SafeAny();
        }

        public static bool SafeAny<T>(this IEnumerable<T> source, Func<T, bool> predicate = null)
        {
            if (source == null)
            {
                return false;
            }

            return predicate == null
                ? source.Any()
                : source.Any(predicate);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }

        public static TRequested Retrieve<TRequested>(this object item, object requester)
            where TRequested : class
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Cant be null - casting would not be possible.");
            }

            TRequested requested = item as TRequested;

            if (requested == null)
            {
                throw new InvalidCastException("messagae"); // TODO: messages
            }

            return requested;
        }

        public static IEnumerable<T> AsEmpty<T>(this IEnumerable<T> source)
        {
            return Enumerable.Empty<T>();
        }

        public static IEnumerable<TReturn> AsEmpty<T, TReturn>(this IEnumerable<T> source)
        {
            return Enumerable.Empty<TReturn>();
        }
    }
}