using System;
using System.Threading.Tasks;

namespace FlatFiles.TypeMapping
{
    internal sealed class TypedWriter<TEntity> : ITypedWriter<TEntity>
    {
        private readonly IWriterWithMetadata writer;
        private readonly Action<IRecordContext, TEntity, object[]> serializer;
        private readonly int logicalCount;

        public TypedWriter(IWriterWithMetadata writer, IMapper<TEntity> mapper)
        {
            this.writer = writer;
            serializer = mapper.GetWriter();
            logicalCount = mapper.LogicalCount;
        }

        /// <summary>
        /// Raised when an error occurs while processing a column.
        /// </summary>
        public event EventHandler<ColumnErrorEventArgs>? ColumnError
        {
            add => writer.ColumnError += value;
            remove => writer.ColumnError -= value;
        }

        /// <summary>
        /// Raised when an error occurs while processing a record.
        /// </summary>
        public event EventHandler<RecordErrorEventArgs>? RecordError
        {
            add => writer.RecordError += value;
            remove => writer.RecordError -= value;
        }

        public IWriter Writer => writer;


        public ISchema? GetSchema()
        {
            return writer.GetSchema();
        }

        public void WriteSchema()
        {
            writer.WriteSchema();
        }

        public async Task WriteSchemaAsync()
        {
            await writer.WriteSchemaAsync().ConfigureAwait(false);
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
            var values = new object[logicalCount];
            var recordContext = writer.GetMetadata();
            serializer(recordContext, entity, values);
            return values;
        }
    }
}
