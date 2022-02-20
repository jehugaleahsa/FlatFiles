using System;

namespace FlatFiles.TypeMapping
{
    internal sealed class DelimitedTypedReader<TEntity> : TypedReader<TEntity>, IDelimitedTypedReader<TEntity>
    {
        private readonly DelimitedReader reader;

        public DelimitedTypedReader(DelimitedReader reader, IMapper<TEntity> mapper)
            : base(mapper)
        {
            this.reader = reader;
        }

        public event EventHandler<DelimitedRecordReadEventArgs>? RecordRead
        {
            add => reader.RecordRead += value;
            remove => reader.RecordRead -= value;
        }

        public event EventHandler<DelimitedRecordParsedEventArgs>? RecordParsed
        {
            add => reader.RecordParsed += value;
            remove => reader.RecordParsed -= value;
        }

        public override IReader Reader => reader;

        DelimitedReader IDelimitedTypedReader<TEntity>.Reader => reader;

        DelimitedSchema? IDelimitedTypedReader<TEntity>.GetSchema()
        {
            return reader.GetSchema();
        }
    }
}
