using System;
using System.IO;
using System.Linq;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Builds textual representations of data by giving each field a fixed width.
    /// </summary>
    public sealed class FixedLengthWriter : IWriter
    {
        private readonly FixedLengthRecordWriter recordWriter;
        private bool isFirstLine;

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
            this.isFirstLine = true;
        }

        /// <summary>
        /// Gets the schema used to build the output.
        /// </summary>
        /// <returns>The schema used to build the output.</returns>
        public FixedLengthSchema GetSchema()
        {
            return recordWriter.Schema;
        }

        ISchema IWriter.GetSchema()
        {
            return GetSchema();
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
            if (isFirstLine)
            {
                if (recordWriter.Options.IsFirstRecordHeader)
                {
                    recordWriter.WriteSchema();
                    recordWriter.WriteRecordSeparator();
                }
                isFirstLine = false;
            }
            recordWriter.WriteRecord(values);
            recordWriter.WriteRecordSeparator();
        }
    }
}
