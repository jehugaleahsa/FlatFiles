using System;

namespace FlatFiles
{
    /// <inheritdoc />
    /// <summary>
    /// Holds the information related to a parsed separated value record.
    /// </summary>
    public class SeparatedValueRecordParsedEventArgs : EventArgs, IRecordParsedEventArgs
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of a SeparatedValueRecordParsedEventArgs.
        /// </summary>
        internal SeparatedValueRecordParsedEventArgs(IRecordContext metadata, object?[]? values)
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
