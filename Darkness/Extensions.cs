namespace Darkness;

public static class Extensions
{
    public static void Shuffle<T>(this IList<T> list, Random random)
    {
        int n = list.Count;
        while (n-- > 1)
        {
            int k = random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
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

    public static void RemoveIfNotNull<T>(this ICollection<T> collection, T? item)
        where T : class
    {
        if (item is not null)
        {
            collection.Remove(item);
        }
    }
}
