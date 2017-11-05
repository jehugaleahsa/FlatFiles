using System;
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
        private readonly IWriter writer;
        private readonly TypedRecordWriter<TEntity> serializer;

        public TypedWriter(IWriter writer, TypedRecordWriter<TEntity> serializer)
        {
            this.writer = writer;
            this.serializer = serializer;
        }

        public ISchema GetSchema()
        {
            return writer.GetSchema();
        }

        public void Write(TEntity entity)
        {
            object[] values = new object[serializer.MemberCount];
            serializer.Write(entity, values);
            writer.Write(values);
        }

        public async Task WriteAsync(TEntity entity)
        {
            object[] values = new object[serializer.MemberCount];
            serializer.Write(entity, values);
            await writer.WriteAsync(values);
        }
    }
}
