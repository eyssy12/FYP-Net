namespace CMS.Dashboard.Test.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CMS.Dashboard.Test.Views.Models;

    public static class EnumerationsExtensions
    {
        public static IEnumerable<EnumId> CreateEnumerationCollection<TType>()
            where TType : struct, IConvertible
        {
            Type type = typeof(TType);

            IDictionary<int, string> dictionary = new Dictionary<int, string>();
            foreach (string name in Enum.GetNames(type))
            {
                dictionary.Add((int)Enum.Parse(type, name), name);
            }

            return dictionary.Select(d => new EnumId { Id = d.Key, Value = d.Value });
        }
    }
}