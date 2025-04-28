using System.Collections.Concurrent;

namespace App.Utils
{
    public static class CTaskGeneric
    {

        /// <summary>
        /// Paralle ForEach Asinc 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="dop"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(dop)
                select Task.Run(async delegate
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                            await body(partition.Current);
                    }
                }));
        }
    }
}
