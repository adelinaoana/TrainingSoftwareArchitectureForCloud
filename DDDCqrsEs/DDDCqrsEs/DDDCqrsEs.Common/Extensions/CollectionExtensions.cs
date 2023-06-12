using System.Collections.Generic;

namespace DDDCqrsEs.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this IList<T> source, IEnumerable<T> newValues)
        {
            foreach (var value in newValues)
            {
                source.Add(value);
            }
        }
    }
}
