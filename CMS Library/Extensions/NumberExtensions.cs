namespace CMS.Library.Extensions
{
    using System;

    public static class NumberExtensions
    {
        public static int GetInt(this Random random, int min = 0, int max = int.MaxValue)
        {
            return random.Next(min, max);
        }
    }
}