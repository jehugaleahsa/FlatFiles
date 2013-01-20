using System;

namespace FlatFileReaders
{
    /// <summary>
    /// Defines the operations that a parser must support.
    /// </summary>
    public interface IParser : IDisposable
    {
        /// <summary>
        /// Gets the schema being used by the parser to parse record values.
        /// </summary>
        /// <returns></returns>
        Schema GetSchema();

        /// <summary>
        /// Reads the next record from the file.
        /// </summary>
        /// <returns>True if the next record was read; otherwise, false if the end of file was reached.</returns>
        bool Read();

        /// <summary>
        /// Gets the values of the current record.
        /// </summary>
        /// <returns>The value of the current record.</returns>
        object[] GetValues();
    }
}
