using System;

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
    }

    internal sealed class TypedWriter<TEntity> : ITypedWriter<TEntity>
    {
        private readonly IWriter writer;
        private readonly IRecordWriter<TEntity> serializer;

        public TypedWriter(IWriter writer, IRecordWriter<TEntity> serializer)
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
            object[] values = serializer.Write(entity);
            writer.Write(values);
        }
    }
}
