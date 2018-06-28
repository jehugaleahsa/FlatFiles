using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents a writer that will write entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being written.</typeparam>
    public interface ITypedWriter<TEntity>
    {
        /// <summary>
        /// Gets the schema being used by the writer to write record values.
        /// </summary>
        /// <returns>The schema being used by the writer.</returns>
        ISchema GetSchema();

        /// <summary>
        /// Writes the given entity to the underlying document.
        /// </summary>
        /// <param name="entity">The entity to write.</param>
        void Write(TEntity entity);

        /// <summary>
        /// Writes the given entity to the underlying document.
        /// </summary>
        /// <param name="entity">The entity to write.</param>
        Task WriteAsync(TEntity entity);
    }

    internal sealed class TypedWriter<TEntity> : ITypedWriter<TEntity>
    {
        private readonly IWriterWithMetadata writer;
        private readonly Action<IProcessMetadata, TEntity, object[]> serializer;
        private readonly int workCount;

        public TypedWriter(IWriterWithMetadata writer, IMapper<TEntity> mapper)
        {
            this.writer = writer;
            serializer = mapper.GetWriter();
            workCount = mapper.WorkCount;
        }

        public ISchema GetSchema()
        {
            return writer.GetSchema();
        }

        public void Write(TEntity entity)
        {
            var values = Serialize(entity);
            writer.Write(values);
        }

        public async Task WriteAsync(TEntity entity)
        {
            var values = Serialize(entity);
            await writer.WriteAsync(values).ConfigureAwait(false);
        }

        private object[] Serialize(TEntity entity)
        {
            var values = new object[workCount];
            var metadata = writer.GetMetadata();
            serializer(metadata, entity, values);
            return values;
        }
    }

    internal interface ITypeMapperInjector
    {
        (ISchema, int, Action<IProcessMetadata, object, object[]>) SetMatcher(object entity);
    }

    internal sealed class MultiplexingTypedWriter : ITypedWriter<object>
    {
        private readonly IWriterWithMetadata writer;
        private readonly ITypeMapperInjector injector;

        public MultiplexingTypedWriter(IWriterWithMetadata writer, ITypeMapperInjector injector)
        {
            this.writer = writer;
            this.injector = injector;
        }

        public ISchema GetSchema()
        {
            return null;
        }

        public void Write(object entity)
        {
            var values = Serialize(entity);
            writer.Write(values);
        }

        public async Task WriteAsync(object entity)
        {
            object[] values = Serialize(entity);
            await writer.WriteAsync(values).ConfigureAwait(false);
        }

        private object[] Serialize(object entity)
        {
            var (schema, workCount, serializer) = injector.SetMatcher(entity);
            var values = new object[workCount];
            IWriterWithMetadata metadataWriter = writer;
            var metadata = metadataWriter.GetMetadata();
            var copy = new ProcessMetadata()
            {
                Schema = schema,
                Options = metadata.Options,
                RecordCount = metadata.RecordCount,
                LogicalRecordCount = metadata.LogicalRecordCount
            };
            serializer(metadata, entity, values);
            return values;
        }
    }

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
            foreach (var entity in entities)
            {
                await writer.WriteAsync(entity).ConfigureAwait(false);
            }
        }
    }
}
