using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Builds textual representations of data by separating fields with a delimiter.
    /// </summary>
    public sealed class SeparatedValueWriter : IWriter
    {
        private readonly SeparatedValueRecordWriter recordWriter;
        private bool isFirstLine;

        /// <summary>
        /// Initializes a new SeparatedValueWriter without a schema.
        /// </summary>
        /// <param name="writer">A writer over the separated value document.</param>
        /// <param name="options">The options used to format the output.</param>
        /// <exception cref="ArgumentNullException">The writer is null.</exception>
        public SeparatedValueWriter(TextWriter writer, SeparatedValueOptions options = null)
            : this(writer, null, options, false)
        {
        }

        /// <summary>
        /// Initializes a new SeparatedValueWriter with the given schema.
        /// </summary>
        /// <param name="writer">A writer over the separated value document.</param>
        /// <param name="schema">The schema of the separated value document.</param>
        /// <param name="options">The options used to format the output.</param>
        /// <exception cref="ArgumentNullException">The writer is null.</exception>
        /// <exception cref="ArgumentNullException">The schema is null.</exception>
        public SeparatedValueWriter(TextWriter writer, SeparatedValueSchema schema, SeparatedValueOptions options = null)
            : this(writer, schema, options, true)
        {
        }

        private SeparatedValueWriter(TextWriter writer, SeparatedValueSchema schema, SeparatedValueOptions options, bool hasSchema)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                options = new SeparatedValueOptions();
            }
            this.recordWriter = new SeparatedValueRecordWriter(writer, schema, options);
            this.isFirstLine = true;
        }

        /// <summary>
        /// Gets the schema used to build the output.
        /// </summary>
        /// <returns>The schema used to build the output.</returns>
        public SeparatedValueSchema GetSchema()
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
        /// <exception cref="System.ArgumentNullException">The values array is null.</exception>
        public void Write(object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (isFirstLine)
            {
                if (recordWriter.Options.IsFirstRecordSchema)
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
