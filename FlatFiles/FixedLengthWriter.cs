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
        private readonly StreamWriter writer;
        private readonly bool ownsStream;
        private readonly FixedLengthRecordWriter recordWriter;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of a FixedLengthBuilder.
        /// </summary>
        /// <param name="fileName">The name of the file to write the output to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        public FixedLengthWriter(string fileName, FixedLengthSchema schema)
            : this(File.OpenWrite(fileName), schema, new FixedLengthOptions(), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FixedLengthBuilder.
        /// </summary>
        /// <param name="fileName">The name of the file to write the output to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        /// <param name="options">The options used to format the output.</param>
        public FixedLengthWriter(string fileName, FixedLengthSchema schema, FixedLengthOptions options)
            : this(File.OpenWrite(fileName), schema, options, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FixedLengthBuilder.
        /// </summary>
        /// <param name="stream">The stream to write the output to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        public FixedLengthWriter(Stream stream, FixedLengthSchema schema)
            : this(stream, schema, new FixedLengthOptions(), false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FixedLengthBuilder.
        /// </summary>
        /// <param name="stream">The stream to write the output to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        /// <param name="options">The options used to format the output.</param>
        public FixedLengthWriter(Stream stream, FixedLengthSchema schema, FixedLengthOptions options)
            : this(stream, schema, options, false)
        {
        }

        private FixedLengthWriter(Stream stream, FixedLengthSchema schema, FixedLengthOptions options, bool ownsStream)
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
            this.writer = new StreamWriter(stream, options.Encoding ?? new UTF8Encoding(false));
            this.ownsStream = ownsStream;
            this.recordWriter = new FixedLengthRecordWriter(schema, options.Clone());
        }

        /// <summary>
        /// Finalizes the FixedLengthBuilder.
        /// </summary>
        ~FixedLengthWriter()
        {
            dispose(false);
        }

        /// <summary>
        /// Releases any resources currently held by the parser.
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
        public FixedLengthSchema GetSchema()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("FixedLengthWriter");
            }
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
            if (isDisposed)
            {
                throw new ObjectDisposedException("FixedLengthWriter");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            recordWriter.WriteRecord(writer, values);
            recordWriter.WriteRecordSeparator(writer);
        }
    }
}
