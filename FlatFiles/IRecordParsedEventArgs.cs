using System;

namespace FlatFiles
{
    /// <summary>
    /// Holds the information related to a parsed record.
    /// </summary>
    public interface IRecordParsedEventArgs
    {
        /// <summary>
        /// Gets any metadata associated with the current read process.
        /// </summary>
        IProcessMetadata Metadata { get; }

        /// <summary>
        /// Gets the parsed record values read from the source file.
        /// </summary>
        object[] Values { get; }
    }
}
