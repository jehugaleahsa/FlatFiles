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
        private readonly Action<TEntity, object[]> serializer;
        private readonly int workCount;

        public TypedWriter(IWriter writer, IMapper<TEntity> mapper)
        {
            this.writer = writer;
            this.serializer = mapper.GetWriter();
            this.workCount = mapper.WorkCount;
        }

        public ISchema GetSchema()
        {
            return writer.GetSchema();
        }

        public void Write(TEntity entity)
        {
            object[] values = new object[workCount];
            serializer(entity, values);
            writer.Write(values);
        }

        public async Task WriteAsync(TEntity entity)
        {
            object[] values = new object[workCount];
            serializer(entity, values);
            await writer.WriteAsync(values);
        }
    }

    internal interface ITypeMapperInjector
    {
        ValueTuple<int, Action<object, object[]>> SetMatcher(object entity);
    }

    internal sealed class MultiplexingTypedWriter : ITypedWriter<object>
    {
        private readonly IWriter writer;
        private readonly ITypeMapperInjector injector;

        public MultiplexingTypedWriter(IWriter writer, ITypeMapperInjector injector)
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
            (int workCount, Action<object, object[]> serializer) = injector.SetMatcher(entity);
            object[] values = new object[workCount];
            serializer(entity, values);
            writer.Write(values);
        }

        public async Task WriteAsync(object entity)
        {
            (int workCount, Action<object, object[]> serializer) = injector.SetMatcher(entity);
            object[] values = new object[workCount];
            serializer(entity, values);
            await writer.WriteAsync(values);
        }
    }
}
