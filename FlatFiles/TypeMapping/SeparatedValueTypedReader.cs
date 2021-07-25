using System;

namespace FlatFiles.TypeMapping
{
    internal sealed class SeparatedValueTypedReader<TEntity> : TypedReader<TEntity>, ISeparatedValueTypedReader<TEntity>
    {
        private readonly SeparatedValueReader reader;

        public SeparatedValueTypedReader(SeparatedValueReader reader, IMapper<TEntity> mapper)
            : base(mapper)
        {
            this.reader = reader;
        }

        public event EventHandler<SeparatedValueRecordReadEventArgs>? RecordRead
        {
            add => reader.RecordRead += value;
            remove => reader.RecordRead -= value;
        }

        public event EventHandler<SeparatedValueRecordParsedEventArgs>? RecordParsed
        {
            add => reader.RecordParsed += value;
            remove => reader.RecordParsed -= value;
        }

        public override IReader Reader => reader;

        SeparatedValueReader ISeparatedValueTypedReader<TEntity>.Reader => reader;

        SeparatedValueSchema? ISeparatedValueTypedReader<TEntity>.GetSchema()
        {
            return reader.GetSchema();
        }
    }
}
