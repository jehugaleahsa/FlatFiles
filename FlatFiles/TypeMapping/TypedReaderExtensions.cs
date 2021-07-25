using System.Collections.Generic;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Provides extension methods for working with typed readers.
    /// </summary>
    public static class TypedReaderExtensions
    {
        /// <summary>
        /// Reads all of the entities from the typed reader.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity the reader is configured to read.</typeparam>
        /// <param name="reader">The reader to read the entities from.</param>
        /// <returns>The entities read by the reader.</returns>
        /// <remarks>This method only consumes records from the reader on-demand.</remarks>
        public static IEnumerable<TEntity> ReadAll<TEntity>(this ITypedReader<TEntity> reader)
        {
            while (reader.Read())
            {
                var entity = reader.Current;
                yield return entity;
            }
        }

#if !NET451 && !NETSTANDARD1_6 && !NETSTANDARD2_0
        /// <summary>
        /// Reads each record from the given reader, such that each record is retrieved asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities returned by the reader.</typeparam>
        /// <param name="reader">The reader to retrieve the records from.</param>
        /// <returns>Each record from the given reader.</returns>
        public static async IAsyncEnumerable<TEntity> ReadAllAsync<TEntity>(this ITypedReader<TEntity> reader)
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                var entity = reader.Current;
                yield return entity;
            }
        }
#endif
    }
}
