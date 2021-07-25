using System;

namespace FlatFiles.TypeMapping
{
    internal sealed class FixedLengthTypedReader<TEntity> : TypedReader<TEntity>, IFixedLengthTypedReader<TEntity>
    {
        private readonly FixedLengthReader reader;

        public FixedLengthTypedReader(FixedLengthReader reader, IMapper<TEntity> mapper)
            : base(mapper)
        {
            this.reader = reader;
        }

        public event EventHandler<FixedLengthRecordReadEventArgs>? RecordRead
        {
            add => reader.RecordRead += value;
            remove => reader.RecordRead -= value;
        }

        public event EventHandler<FixedLengthRecordPartitionedEventArgs>? RecordPartitioned
        {
            add => reader.RecordPartitioned += value;
            remove => reader.RecordPartitioned -= value;
        }

        public event EventHandler<FixedLengthRecordParsedEventArgs>? RecordParsed
        {
            add => reader.RecordParsed += value;
            remove => reader.RecordParsed -= value;
        }

        public override IReader Reader => reader;

        FixedLengthReader IFixedLengthTypedReader<TEntity>.Reader => reader;

        FixedLengthSchema? IFixedLengthTypedReader<TEntity>.GetSchema()
        {
            return reader.GetSchema();
        }
    }
}
