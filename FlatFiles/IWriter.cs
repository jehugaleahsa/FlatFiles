using System;
using System.Threading.Tasks;

namespace FlatFiles
{
    /// <summary>
    /// Defines the operations that a writer must support.
    /// </summary>
    public interface IWriter
    {
        /// <summary>
        /// Raised when an error occurs while processing a column.
        /// </summary>
        event EventHandler<ColumnErrorEventArgs> ColumnError;

        /// <summary>
        /// Raised when an error occurs while processing a record.
        /// </summary>
        event EventHandler<RecordErrorEventArgs> RecordError;

        /// <summary>
        /// Gets the schema being used by the builder to create the textual representation.
        /// </summary>
        /// <returns>The schema being used by the builder to create the textual representation.</returns>
        ISchema GetSchema();

        /// <summary>
        /// Writes the textual representation of the given values to the writer.
        /// </summary>
        /// <param name="values">The values to write.</param>
        /// <returns>The textual representation of the given values.</returns>
        void Write(object[] values);

        /// <summary>
        /// Writes the textual representation of the given values to the writer.
        /// </summary>
        /// <param name="values">The values to write.</param>
        /// <returns>The textual representation of the given values.</returns>
        Task WriteAsync(object[] values);

        /// <summary>
        /// Write the given data directly to the output. By default, this will
        /// not include a newline.
        /// </summary>
        /// <param name="data">The data to write to the output.</param>
        /// <param name="writeRecordSeparator">Indicates whether a record separator should be written after the data.</param>
        void WriteRaw(String data, bool writeRecordSeparator = false);

        /// <summary>
        /// Write the given data directly to the output. By default, this will
        /// not include a newline.
        /// </summary>
        /// <param name="data">The data to write to the output.</param>
        /// <param name="writeRecordSeparator">Indicates whether a record separator should be written after the data.</param>
        Task WriteRawAsync(String data, bool writeRecordSeparator = false);
    }

    internal interface IWriterWithMetadata : IWriter
    {
        IRecordContext GetMetadata();
    }
}
