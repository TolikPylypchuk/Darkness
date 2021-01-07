using System;
using System.Collections.Generic;

namespace Darkness
{
    public static class Extensions
    {
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            int n = list.Count;
            while (n-- > 1)
            {
                int k = random.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IEnumerable<IEnumerable<T>> Rows<T>(this T[,] array)
        {
            int numRows = array.GetLength(0);
            int numCols = array.GetLength(1);

            IEnumerable<T> sliceRow(int row)
            {
                for (int col = 0; col < numCols; col++)
                {
                    yield return array[row, col];
                }
            }

            for (int row = 0; row < numRows; row++)
            {
                yield return sliceRow(row);
            }
        }

        public static void AddIfNotNull<T>(this ICollection<T> collection, T? item)
            where T : class
        {
            if (item is not null)
            {
                collection.Add(item);
            }
        }
    }
}
