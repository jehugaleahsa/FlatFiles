using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    ///  Represents a fixed length reader that will generate entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being read.</typeparam>
    public interface IFixedLengthTypedReader<out TEntity> : ITypedReader<TEntity>
    {
        /// <summary>
        /// Raised when a record is read from the source file, before it is partitioned.
        /// </summary>
        event EventHandler<FixedLengthRecordReadEventArgs>? RecordRead;

        /// <summary>
        /// Raised after a record is partitioned, before it is parsed.
        /// </summary>
        event EventHandler<FixedLengthRecordPartitionedEventArgs>? RecordPartitioned;

        /// <summary>
        /// Raised after a record is parsed.
        /// </summary>
        new event EventHandler<FixedLengthRecordParsedEventArgs>? RecordParsed;

        /// <summary>
        /// Gets the underlying reader.
        /// </summary>
        new FixedLengthReader Reader { get; }

        /// <summary>
        /// Gets the schema being used by the parser to parse record values.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        new FixedLengthSchema? GetSchema();
    }
}
