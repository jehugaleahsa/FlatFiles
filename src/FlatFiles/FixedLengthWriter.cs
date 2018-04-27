using System;
using System.IO;
using System.Threading.Tasks;

namespace FlatFiles
{
    /// <summary>
    /// Builds textual representations of data by giving each field a fixed width.
    /// </summary>
    public sealed class FixedLengthWriter : IWriter
    {
        private readonly FixedLengthRecordWriter recordWriter;
        private bool isSchemaWritten;

        /// <summary>
        /// Initializes a new FixedLengthBuilder with the given schema.
        /// </summary>
        /// <param name="writer">A writer over the fixed-length document.</param>
        /// <param name="schema">The schema of the fixed-length document.</param>
        /// <param name="options">The options used to format the output.</param>
        /// <exception cref="ArgumentNullException">The writer is null.</exception>
        /// <exception cref="ArgumentNullException">The schema is null.</exception>
        public FixedLengthWriter(TextWriter writer, FixedLengthSchema schema, FixedLengthOptions options = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                options = new FixedLengthOptions();
            }
            this.recordWriter = new FixedLengthRecordWriter(writer, schema, options);
        }

        /// <summary>
        /// Gets the schema used to build the output.
        /// </summary>
        /// <returns>The schema used to build the output.</returns>
        public FixedLengthSchema GetSchema()
        {
            return recordWriter.Metadata.Schema;
        }

        ISchema IWriter.GetSchema()
        {
            return GetSchema();
        }

        /// <summary>
        /// Write the textual representation of the record schema to the writer.
        /// </summary>
        /// <remarks>If the header or records have already been written, this call is ignored.</remarks>
        public void WriteSchema()
        {
            if (isSchemaWritten)
            {
                return;
            }
            recordWriter.WriteSchema();
            recordWriter.WriteRecordSeparator();
            ++recordWriter.Metadata.RecordCount;
            isSchemaWritten = true;
        }

        /// <summary>
        /// Write the textual representation of the record schema to the writer.
        /// </summary>
        /// <remarks>If the header or records have already been written, this call is ignored.</remarks>
        public async Task WriteSchemaAsync()
        {
            if (isSchemaWritten)
            {
                return;
            }
            await recordWriter.WriteSchemaAsync();
            await recordWriter.WriteRecordSeparatorAsync();
            ++recordWriter.Metadata.RecordCount;
            isSchemaWritten = true;
        }

        /// <summary>
        /// Writes the textual representation of the given values to the writer.
        /// </summary>
        /// <param name="values">The values to write.</param>
        /// <exception cref="ArgumentNullException">The values array is null.</exception>
        public void Write(object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (!isSchemaWritten)
            {
                if (recordWriter.Metadata.Options.IsFirstRecordHeader)
                {
                    recordWriter.WriteSchema();
                    recordWriter.WriteRecordSeparator();
                    ++recordWriter.Metadata.RecordCount;
                }
                isSchemaWritten = true;
            }
            recordWriter.WriteRecord(values);
            recordWriter.WriteRecordSeparator();
            ++recordWriter.Metadata.RecordCount;
            ++recordWriter.Metadata.LogicalRecordCount;
        }

        /// <summary>
        /// Writes the textual representation of the given values to the writer.
        /// </summary>
        /// <param name="values">The values to write.</param>
        /// <exception cref="ArgumentNullException">The values array is null.</exception>
        public async Task WriteAsync(object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            if (!isSchemaWritten)
            {
                if (recordWriter.Metadata.Options.IsFirstRecordHeader)
                {
                    await recordWriter.WriteSchemaAsync();
                    await recordWriter.WriteRecordSeparatorAsync();
                    ++recordWriter.Metadata.RecordCount;
                }
                isSchemaWritten = true;
            }
            await recordWriter.WriteRecordAsync(values);
            await recordWriter.WriteRecordSeparatorAsync();
            ++recordWriter.Metadata.RecordCount;
            ++recordWriter.Metadata.LogicalRecordCount;
        }
    }
}
