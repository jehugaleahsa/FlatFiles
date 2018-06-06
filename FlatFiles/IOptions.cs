using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents reader/writer options that are common among file types.
    /// </summary>
    public interface IOptions
    {
        /// <summary>
        /// Gets whether the first record defines the schema of the file.
        /// </summary>
        bool IsFirstRecordSchema { get; }
    }
}
