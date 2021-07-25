using System;
using System.Threading.Tasks;

namespace FlatFiles.TypeMapping
{
    internal sealed class MultiplexingSeparatedValueTypedReader : ISeparatedValueTypedReader<object>
    {
        private readonly SeparatedValueReader reader;
        private object? current;

        public MultiplexingSeparatedValueTypedReader(SeparatedValueReader reader)
        {
            this.reader = reader;
        }

        public IReader Reader => reader;

        SeparatedValueReader ISeparatedValueTypedReader<object>.Reader => reader;

        // FIXME: We should throw an exception if no or all records read
        public object Current => current!;

        public Func<IRecordContext, object?[], object?>? Deserializer { get; set; }

        public event EventHandler<SeparatedValueRecordReadEventArgs>? RecordRead
        {
            add => reader.RecordRead += value;
            remove => reader.RecordRead -= value;
        }

        event EventHandler<IRecordParsedEventArgs>? ITypedReader<object>.RecordParsed
        {
            add => ((IReader)reader).RecordParsed += value;
            remove => ((IReader)reader).RecordParsed -= value;
        }

        public event EventHandler<SeparatedValueRecordParsedEventArgs>? RecordParsed
        {
            add => reader.RecordParsed += value;
            remove => reader.RecordParsed -= value;
        }

        public event EventHandler<RecordErrorEventArgs>? RecordError
        {
            add => reader.RecordError += value;
            remove => reader.RecordError -= value;
        }

        public event EventHandler<ColumnErrorEventArgs>? ColumnError
        {
            add => reader.ColumnError += value;
            remove => reader.ColumnError -= value;
        }

        public ISchema? GetSchema()
        {
            return reader.GetSchema();
        }

        SeparatedValueSchema? ISeparatedValueTypedReader<object>.GetSchema()
        {
            return reader.GetSchema();
        }

        public bool Read()
        {
            if (!reader.Read())
            {
                return false;
            }
            SetCurrent();
            return true;
        }

        public async ValueTask<bool> ReadAsync()
        {
            if (!await reader.ReadAsync().ConfigureAwait(false))
            {
                return false;
            }
            SetCurrent();
            return true;
        }

        private void SetCurrent()
        {
            var values = reader.GetValues();
            IReaderWithMetadata metadataReader = reader;
            var recordContext = metadataReader.GetMetadata();
            current = Deserializer!(recordContext, values);
        }

        public bool Skip()
        {
            return reader.Skip();
        }

        public ValueTask<bool> SkipAsync()
        {
            return reader.SkipAsync();
        }
    }
}
