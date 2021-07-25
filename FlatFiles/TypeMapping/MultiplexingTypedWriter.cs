using System;
using System.Threading.Tasks;

namespace FlatFiles.TypeMapping
{
    internal sealed class MultiplexingTypedWriter : ITypedWriter<object>
    {
        private readonly IWriterWithMetadata writer;
        private readonly ITypeMapperInjector injector;

        public MultiplexingTypedWriter(IWriterWithMetadata writer, ITypeMapperInjector injector)
        {
            this.writer = writer;
            this.injector = injector;
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
            return null;
        }

        public void WriteSchema()
        {
            writer.WriteSchema();
        }

        public async Task WriteSchemaAsync()
        {
            await writer.WriteSchemaAsync().ConfigureAwait(false);
        }

        public void Write(object entity)
        {
            var values = Serialize(entity);
            writer.Write(values);
        }

        public async Task WriteAsync(object entity)
        {
            var values = Serialize(entity);
            await writer.WriteAsync(values).ConfigureAwait(false);
        }

        private object?[] Serialize(object entity)
        {
            var context = injector.SetMatcher(entity);
            var values = new object?[context.LogicalCount];
            var metadataWriter = writer;
            var recordContext = metadataWriter.GetMetadata();
            context.Serialize(recordContext, entity, values);
            return values;
        }
    }
}
