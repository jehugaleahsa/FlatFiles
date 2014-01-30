using System;
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
        private readonly TextWriter writer;
        private readonly bool ownsStream;
        private readonly SeparatedValueSchema schema;
        private readonly bool isFirstRecordSchema;
        private readonly string separator;
        private readonly string recordSeparator;
        private bool isFirstLine;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of a SeparatedValueBuilder.
        /// </summary>
        /// <param name="fileName">The name of the file to write to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public SeparatedValueWriter(string fileName, SeparatedValueSchema schema)
            : this (File.OpenWrite(fileName), schema, new SeparatedValueOptions(), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueBuilder.
        /// </summary>
        /// <param name="fileName">The name of the file to write to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        /// <param name="options">The options to use to format the output.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public SeparatedValueWriter(string fileName, SeparatedValueSchema schema, SeparatedValueOptions options)
            : this(File.OpenWrite(fileName), schema, options, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueBuilder.
        /// </summary>
        /// <param name="stream">The stream to write the output to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options is null.</exception>
        public SeparatedValueWriter(Stream stream, SeparatedValueSchema schema)
            : this(stream, schema, new SeparatedValueOptions(), false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueBuilder.
        /// </summary>
        /// <param name="stream">The stream to write the output to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        /// <param name="options">The options used to format the output.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options is null.</exception>
        public SeparatedValueWriter(Stream stream, SeparatedValueSchema schema, SeparatedValueOptions options)
            : this(stream, schema, options, false)
        {
        }

        private SeparatedValueWriter(Stream stream, SeparatedValueSchema schema, SeparatedValueOptions options, bool ownsStream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            this.writer = new StreamWriter(stream, options.Encoding ?? Encoding.Default);
            this.ownsStream = ownsStream;
            this.schema = schema;
            this.isFirstRecordSchema = options.IsFirstRecordSchema;
            this.separator = options.Separator;
            this.recordSeparator = options.RecordSeparator;
            this.isFirstLine = true;
        }

        /// <summary>
        /// Finalizes the SeparatedValueBuilder.
        /// </summary>
        ~SeparatedValueWriter()
        {
            dispose(false);
        }

        /// <summary>
        /// Releases any resources being held by the parser.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        private void dispose(bool disposing)
        {
            if (disposing)
            {
                if (ownsStream)
                {
                    writer.Dispose();
                }
                else
                {
                    writer.Flush();
                }
            }
            isDisposed = true;
        }

        /// <summary>
        /// Gets the schema used to build the output.
        /// </summary>
        /// <returns>The schema used to build the output.</returns>
        public ISchema GetSchema()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("SeparatedValueWriter");
            }
            return schema;
        }

        /// <summary>
        /// Writes the textual representation of the given values to the writer.
        /// </summary>
        /// <param name="values">The values to write.</param>
        /// <exception cref="System.ArgumentNullException">The values array is null.</exception>
        public void Write(object[] values)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("SeparatedValueWriter");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length != schema.ColumnDefinitions.Count)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, "values");
            }
            if (isFirstLine)
            {
                if (isFirstRecordSchema)
                {
                    buildSchema(writer);
                }
                isFirstLine = false;
            }
            string[] formattedValues = schema.FormatValues(values).Select(v => escape(v)).ToArray();
            string joined = String.Join(separator, formattedValues);
            writer.Write(joined);
            writer.Write(recordSeparator);
        }

        private void buildSchema(TextWriter writer)
        {
            string[] names = schema.ColumnDefinitions.Select(d => escape(d.ColumnName)).ToArray();
            string joined = String.Join(separator, names);
            writer.Write(joined);
            writer.Write(recordSeparator);
        }

        private string escape(string value)
        {
            string escaped = value;
            if (value != null && value.Contains(separator))
            {
                escaped = "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return escaped;
        }
    }
}
