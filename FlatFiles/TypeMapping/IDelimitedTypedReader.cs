using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    ///  Represents a delimited reader that will generate entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being read.</typeparam>
    public interface IDelimitedTypedReader<out TEntity> : ITypedReader<TEntity>
    {
        /// <summary>
        /// Raised when a record is read, before it is parsed.
        /// </summary>
        event EventHandler<DelimitedRecordReadEventArgs>? RecordRead;

        /// <summary>
        /// Raised after a record is parsed.
        /// </summary>
        new event EventHandler<DelimitedRecordParsedEventArgs>? RecordParsed;

        /// <summary>
        /// Gets the underlying reader.
        /// </summary>
        new DelimitedReader Reader { get; }

        /// <summary>
        /// Gets the schema being used by the parser to parse record values.
        /// </summary>
        /// <returns>The schema being used by the parser.</returns>
        new DelimitedSchema? GetSchema();
    }
}
