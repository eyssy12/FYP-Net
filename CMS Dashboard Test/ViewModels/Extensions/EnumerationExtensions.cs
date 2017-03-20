namespace CMS.Dashboard.Test.ViewModels.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerationExtensions
    {
        public static IEnumerable<string> GetEnumValuesForType<TType>(this TType type)
            where TType : struct, IConvertible
        {
            return Enum.GetNames(typeof(TType)).ToArray();
        }
    }
}