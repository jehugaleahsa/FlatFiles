using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Provides extension methods for working with typed writers.
    /// </summary>
    public static class TypedWriterExtensions
    {
        /// <summary>
        /// Writes all of the entities to the typed writer.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity the writer is configured to write.</typeparam>
        /// <param name="writer">The reader to read the entities from.</param>
        /// <param name="entities">The entities to write to the file.</param>
        /// <returns>The entities written by the writer.</returns>
        public static void WriteAll<TEntity>(this ITypedWriter<TEntity> writer, IEnumerable<TEntity> entities)
        {
            if (writer.Writer.Options.IsFirstRecordSchema)
            {
                writer.WriteSchema();
            }
            foreach (var entity in entities)
            {
                writer.Write(entity);
            }
        }

        /// <summary>
        /// Writes all of the entities to the typed writer.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity the writer is configured to write.</typeparam>
        /// <param name="writer">The reader to read the entities from.</param>
        /// <param name="entities">The entities to write to the file.</param>
        /// <returns>The entities written by the writer.</returns>
        public static async Task WriteAllAsync<TEntity>(this ITypedWriter<TEntity> writer, IEnumerable<TEntity> entities)
        {
            if (writer.Writer.Options.IsFirstRecordSchema)
            {
                await writer.WriteSchemaAsync().ConfigureAwait(false);
            }
            foreach (var entity in entities)
            {
                await writer.WriteAsync(entity).ConfigureAwait(false);
            }
        }

#if !NET451 && !NETSTANDARD1_6 && !NETSTANDARD2_0
        /// <summary>
        /// Writes all of the entities to the typed writer.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity the writer is configured to write.</typeparam>
        /// <param name="writer">The reader to read the entities from.</param>
        /// <param name="entities">The entities to write to the file.</param>
        /// <returns>The entities written by the writer.</returns>
        public static async Task WriteAllAsync<TEntity>(this ITypedWriter<TEntity> writer, IAsyncEnumerable<TEntity> entities)
        {
            if (writer.Writer.Options.IsFirstRecordSchema)
            {
                await writer.WriteSchemaAsync().ConfigureAwait(false);
            }
            await foreach (var entity in entities.ConfigureAwait(false))
            {
                await writer.WriteAsync(entity).ConfigureAwait(false);
            }
        }
#endif
    }
}
