using System;

namespace FlatFiles
{
    /// <summary>
    /// Holds the information related to a parsed fixed length record.
    /// </summary>
    public class FixedLengthRecordParsedEventArgs : EventArgs, IRecordParsedEventArgs
    {
        /// <summary>
        /// Creates a new instance of a FixedLengthRecordParsedEventArgs.
        /// </summary>
        internal FixedLengthRecordParsedEventArgs(IRecordContext context, object[] values)
        {
            Context = context;
            Values = values;
        }

        /// <summary>
        /// Gets any metadata associated with the current read process.
        /// </summary>
        public IRecordContext Context { get; }

        /// <summary>
        /// Gets the parsed record values read from the source file.
        /// </summary>
        public object[] Values { get; }
    }
}
