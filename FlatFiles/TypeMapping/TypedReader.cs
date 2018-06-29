using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    ///  Represents a reader that will generate entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being read.</typeparam>
    public interface ITypedReader<out TEntity>
    {
        /// <summary>
        /// Raised when an error occurs while processing a record.
        /// </summary>
        event EventHandler<ProcessingErrorEventArgs> Error;

        /// <summary>
        /// Raised when a record is parsed.
        /// </summary>
        event EventHandler<IRecordParsedEventArgs> RecordParsed;

        /// <summary>
        /// Gets the schema being used by the parser to parse record values.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        ISchema GetSchema();

        /// <summary>
        /// Reads the next record from the file.
        /// </summary>
        /// <returns>True if the next record was read; otherwise, false if the end of file was reached.</returns>
        bool Read();

        /// <summary>
        /// Reads the next record from the file.
        /// </summary>
        /// <returns>True if the next record was read; otherwise, false if the end of file was reached.</returns>
        ValueTask<bool> ReadAsync();

        /// <summary>
        /// Skips the next record from the file.
        /// </summary>
        /// <returns>True if the next record was skipped; otherwise, false if the end of the file was reached.</returns>
        bool Skip();

        /// <summary>
        /// Skips the next record from the file.
        /// </summary>
        /// <returns>True if the next record was skipped; otherwise, false if the end of the file was reached.</returns>
        ValueTask<bool> SkipAsync();

        /// <summary>
        /// Gets the last read entity.
        /// </summary>
        TEntity Current { get; }
    }

    /// <summary>
    ///  Represents a separated value reader that will generate entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being read.</typeparam>
    public interface ISeparatedValueTypedReader<out TEntity> : ITypedReader<TEntity>
    {
        /// <summary>
        /// Raised when a record is read, before it is parsed.
        /// </summary>
        event EventHandler<SeparatedValueRecordReadEventArgs> RecordRead;

        /// <summary>
        /// Raised after a record is parsed.
        /// </summary>
        new event EventHandler<SeparatedValueRecordParsedEventArgs> RecordParsed;
    }

    /// <summary>
    ///  Represents a fixed length reader that will generate entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being read.</typeparam>
    public interface IFixedLengthTypedReader<out TEntity> : ITypedReader<TEntity>
    {
        /// <summary>
        /// Raised when a record is read from the source file, before it is partitioned.
        /// </summary>
        event EventHandler<FixedLengthRecordReadEventArgs> RecordRead;

        /// <summary>
        /// Raised after a record is partitioned, before it is parsed.
        /// </summary>
        event EventHandler<FixedLengthRecordPartitionedEventArgs> RecordPartitioned;

        /// <summary>
        /// Raised after a record is parsed.
        /// </summary>
        new event EventHandler<FixedLengthRecordParsedEventArgs> RecordParsed;
    }

    internal abstract class TypedReader<TEntity> : ITypedReader<TEntity>
    {
        private readonly IReaderWithMetadata reader;
        private readonly Func<IRecordContext, object[], TEntity> deserializer;

        protected TypedReader(IReaderWithMetadata reader, IMapper<TEntity> mapper)
        {
            this.reader = reader;
            deserializer = mapper.GetReader();
        }

        event EventHandler<IRecordParsedEventArgs> ITypedReader<TEntity>.RecordParsed
        {
            add => reader.RecordParsed += value;
            remove => reader.RecordParsed -= value;
        }

        public event EventHandler<ProcessingErrorEventArgs> Error
        {
            add => reader.Error += value;
            remove => reader.Error -= value;
        }

        public ISchema GetSchema()
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
            var recordContext = reader.GetMetadata();
            Current = deserializer(recordContext, values);
        }

        public bool Skip()
        {
            return reader.Skip();
        }

        public async ValueTask<bool> SkipAsync()
        {
            return await reader.SkipAsync().ConfigureAwait(false);
        }

        public TEntity Current { get; private set; }
    }

    internal sealed class SeparatedValueTypedReader<TEntity> : TypedReader<TEntity>, ISeparatedValueTypedReader<TEntity>
    {
        private readonly SeparatedValueReader reader;

        public SeparatedValueTypedReader(SeparatedValueReader reader, IMapper<TEntity> mapper)
            : base(reader, mapper)
        {
            this.reader = reader;
        }

        public event EventHandler<SeparatedValueRecordReadEventArgs> RecordRead
        {
            add => reader.RecordRead += value;
            remove => reader.RecordRead -= value;
        }

        public event EventHandler<SeparatedValueRecordParsedEventArgs> RecordParsed
        {
            add => reader.RecordParsed += value;
            remove => reader.RecordParsed -= value;
        }
    }

    internal sealed class MultiplexingSeparatedValueTypedReader : ISeparatedValueTypedReader<object>
    {
        private readonly SeparatedValueReader reader;

        public MultiplexingSeparatedValueTypedReader(SeparatedValueReader reader)
        {
            this.reader = reader;
        }

        public object Current { get; private set; }

        public Func<IRecordContext, object[], object> Deserializer { get; set; }

        public event EventHandler<SeparatedValueRecordReadEventArgs> RecordRead
        {
            add => reader.RecordRead += value;
            remove => reader.RecordRead -= value;
        }

        event EventHandler<IRecordParsedEventArgs> ITypedReader<object>.RecordParsed
        {
            add => ((IReader)reader).RecordParsed += value;
            remove => ((IReader)reader).RecordParsed -= value;
        }

        public event EventHandler<SeparatedValueRecordParsedEventArgs> RecordParsed
        {
            add => reader.RecordParsed += value;
            remove => reader.RecordParsed -= value;
        }

        public event EventHandler<ProcessingErrorEventArgs> Error
        {
            add => reader.Error += value;
            remove => reader.Error -= value;
        }

        public ISchema GetSchema()
        {
            return null;
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
            Current = Deserializer(recordContext, values);
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

    internal sealed class FixedLengthTypedReader<TEntity> : TypedReader<TEntity>, IFixedLengthTypedReader<TEntity>
    {
        private readonly FixedLengthReader reader;

        public FixedLengthTypedReader(FixedLengthReader reader, IMapper<TEntity> mapper)
            : base(reader, mapper)
        {
            this.reader = reader;
        }

        public event EventHandler<FixedLengthRecordReadEventArgs> RecordRead
        {
            add => reader.RecordRead += value;
            remove => reader.RecordRead -= value;
        }

        public event EventHandler<FixedLengthRecordPartitionedEventArgs> RecordPartitioned
        {
            add => reader.RecordPartitioned += value;
            remove => reader.RecordPartitioned -= value;
        }

        public event EventHandler<FixedLengthRecordParsedEventArgs> RecordParsed
        {
            add => reader.RecordParsed += value;
            remove => reader.RecordParsed -= value;
        }
    }

    internal sealed class MultiplexingFixedLengthTypedReader : IFixedLengthTypedReader<object>
    {
        private readonly FixedLengthReader reader;

        public MultiplexingFixedLengthTypedReader(FixedLengthReader reader)
        {
            this.reader = reader;
        }

        public object Current { get; private set; }

        public Func<IRecordContext, object[], object> Deserializer { get; set; }

        public event EventHandler<FixedLengthRecordReadEventArgs> RecordRead
        {
            add => reader.RecordRead += value;
            remove => reader.RecordRead -= value;
        }

        public event EventHandler<FixedLengthRecordPartitionedEventArgs> RecordPartitioned
        {
            add => reader.RecordPartitioned += value;
            remove => reader.RecordPartitioned -= value;
        }

        public event EventHandler<FixedLengthRecordParsedEventArgs> RecordParsed
        {
            add => reader.RecordParsed += value;
            remove => reader.RecordParsed -= value;
        }

        event EventHandler<IRecordParsedEventArgs> ITypedReader<object>.RecordParsed
        {
            add => ((IReader)reader).RecordParsed += value;
            remove => ((IReader)reader).RecordParsed -= value;
        }

        public event EventHandler<ProcessingErrorEventArgs> Error
        {
            add => reader.Error += value;
            remove => reader.Error -= value;
        }

        public ISchema GetSchema()
        {
            return null;
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
            Current = Deserializer(recordContext, values);
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

    /// <summary>
    /// Provides extension methods for working with typed readers.
    /// </summary>
    public static class TypedReaderExtensions
    {
        /// <summary>
        /// Reads all of the entities from the typed reader.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity the reader is configured to read.</typeparam>
        /// <param name="reader">The reader to read the entities from.</param>
        /// <returns>The entities read by the reader.</returns>
        /// <remarks>This method only consumes records from the reader on-demand.</remarks>
        public static IEnumerable<TEntity> ReadAll<TEntity>(this ITypedReader<TEntity> reader)
        {
            while (reader.Read())
            {
                var entity = reader.Current;
                yield return entity;
            }
        }
    }
}
