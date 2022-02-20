using System;

namespace FlatFiles
{
    /// <inheritdoc />
    /// <summary>
    /// Holds the information related to a parsed delimited record.
    /// </summary>
    public sealed class DelimitedRecordParsedEventArgs : EventArgs, IRecordParsedEventArgs
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of a DelimitedRecordParsedEventArgs.
        /// </summary>
        internal DelimitedRecordParsedEventArgs(IRecordContext metadata, object?[]? values)
        {
            RecordContext = metadata;
            Values = values;
        }

        /// <summary>
        /// Gets any metadata associated with the current read process.
        /// </summary>
        public IRecordContext RecordContext { get; }

        /// <summary>
        /// Gets the parsed record values read from the source file.
        /// </summary>
        public object?[]? Values { get; }
    }
}
