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
        private readonly TextWriter writer;
        private readonly bool ownsStream;
        private readonly SeparatedValueRecordWriter recordWriter;
        private bool isFirstLine;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of a SeparatedValueWriter.
        /// </summary>
        /// <param name="fileName">The name of the file to write to.</param>
        public SeparatedValueWriter(string fileName)
            : this(File.OpenWrite(fileName), null, new SeparatedValueOptions(), true, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueWriter.
        /// </summary>
        /// <param name="fileName">The name of the file to write to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public SeparatedValueWriter(string fileName, SeparatedValueSchema schema)
            : this(File.OpenWrite(fileName), schema, new SeparatedValueOptions(), true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueWriter.
        /// </summary>
        /// <param name="fileName">The name of the file to write to.</param>
        /// <param name="options">The options to use to format the output.</param>
        public SeparatedValueWriter(string fileName, SeparatedValueOptions options)
            : this(File.OpenWrite(fileName), null, options, true, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueWriter.
        /// </summary>
        /// <param name="fileName">The name of the file to write to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        /// <param name="options">The options to use to format the output.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public SeparatedValueWriter(string fileName, SeparatedValueSchema schema, SeparatedValueOptions options)
            : this(File.OpenWrite(fileName), schema, options, true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueWriter.
        /// </summary>
        /// <param name="stream">The stream to write the output to.</param>
        public SeparatedValueWriter(Stream stream)
            : this(stream, null, new SeparatedValueOptions(), false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueWriter.
        /// </summary>
        /// <param name="stream">The stream to write the output to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public SeparatedValueWriter(Stream stream, SeparatedValueSchema schema)
            : this(stream, schema, new SeparatedValueOptions(), false, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueWriter.
        /// </summary>
        /// <param name="stream">The stream to write the output to.</param>
        /// <param name="options">The options used to format the output.</param>
        /// <exception cref="System.ArgumentNullException">The options is null.</exception>
        public SeparatedValueWriter(Stream stream, SeparatedValueOptions options)
            : this(stream, null, options, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueWriter.
        /// </summary>
        /// <param name="stream">The stream to write the output to.</param>
        /// <param name="schema">The schema to use to build the output.</param>
        /// <param name="options">The options used to format the output.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options is null.</exception>
        public SeparatedValueWriter(Stream stream, SeparatedValueSchema schema, SeparatedValueOptions options)
            : this(stream, schema, options, false, true)
        {
        }

        private SeparatedValueWriter(Stream stream, SeparatedValueSchema schema, SeparatedValueOptions options, bool ownsStream, bool hasSchema)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            this.writer = new StreamWriter(stream, options.Encoding ?? new UTF8Encoding(false));
            this.ownsStream = ownsStream;
            this.recordWriter = new SeparatedValueRecordWriter(schema, options);
            this.isFirstLine = true;
        }

        /// <summary>
        /// Finalizes the SeparatedValueWriter.
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
        public SeparatedValueSchema GetSchema()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("SeparatedValueWriter");
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
            if (isFirstLine)
            {
                if (recordWriter.Options.IsFirstRecordSchema)
                {
                    writeSchema();
                }
                isFirstLine = false;
            }
            recordWriter.WriteRecord(writer, values);
            recordWriter.WriteRecordSeparator(writer);
        }

        private void writeSchema()
        {
            recordWriter.WriteSchema(writer);
            recordWriter.WriteRecordSeparator(writer);
        }
    }
}
