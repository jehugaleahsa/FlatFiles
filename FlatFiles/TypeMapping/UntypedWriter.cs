using System;
using System.Threading.Tasks;

namespace FlatFiles.TypeMapping
{
    internal class UntypedWriter<TEntity> : ITypedWriter<object>
    {
        private readonly ITypedWriter<TEntity> writer;

        public UntypedWriter(ITypedWriter<TEntity> writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Raised when an error occurs while processing a column.
        /// </summary>
        public event EventHandler<ColumnErrorEventArgs> ColumnError
        {
            add => writer.ColumnError += value;
            remove => writer.ColumnError -= value;
        }

        /// <summary>
        /// Raised when an error occurs while processing a record.
        /// </summary>
        public event EventHandler<RecordErrorEventArgs> RecordError
        {
            add => writer.RecordError += value;
            remove => writer.RecordError -= value;
        }

        public ISchema GetSchema()
        {
            return writer.GetSchema();
        }

        public void Write(object entity)
        {
            writer.Write((TEntity)entity);
        }

        public async Task WriteAsync(object entity)
        {
            await writer.WriteAsync((TEntity)entity).ConfigureAwait(false);
        }
    }
}
