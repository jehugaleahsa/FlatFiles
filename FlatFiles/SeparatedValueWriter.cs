using System;
using System.IO;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Builds textual representations of data by separating fields with a delimiter.
    /// </summary>
    public sealed class SeparatedValueWriter : IWriter, IWriterWithMetadata
    {
        private readonly SeparatedValueRecordWriter recordWriter;
        private bool isSchemaWritten;

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
                throw new ArgumentNullException(nameof(writer));
            }
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }
            if (options == null)
            {
                options = new SeparatedValueOptions();
            }
            recordWriter = new SeparatedValueRecordWriter(writer, schema, options);
        }

        /// <summary>
        /// Initializes a new SeparatedValueWriter with the given schema.
        /// </summary>
        /// <param name="writer">A writer over the separated value document.</param>
        /// <param name="injector">The schema injector to use to determine the schema.</param>
        /// <param name="options">The options used to format the output.</param>
        /// <exception cref="ArgumentNullException">The writer is null.</exception>
        /// <exception cref="ArgumentNullException">The schema injector is null.</exception>
        public SeparatedValueWriter(TextWriter writer, SeparatedValueSchemaInjector injector, SeparatedValueOptions options = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (injector == null)
            {
                throw new ArgumentNullException(nameof(injector));
            }
            if (options == null)
            {
                options = new SeparatedValueOptions();
            }
            recordWriter = new SeparatedValueRecordWriter(writer, injector, options);
        }

        /// <summary>
        /// Raised when an error occurs while processing a column.
        /// </summary>
        public event EventHandler<ColumnErrorEventArgs> ColumnError
        {
            add => recordWriter.Metadata.ColumnError += value;
            remove => recordWriter.Metadata.ColumnError -= value;
        }

        /// <summary>
        /// Raised when an error occurs while processing a record.
        /// </summary>
        public event EventHandler<RecordErrorEventArgs> RecordError;

        /// <summary>
        /// Gets the schema used to build the output.
        /// </summary>
        /// <returns>The schema used to build the output.</returns>
        public SeparatedValueSchema GetSchema()
        {
            return recordWriter.Metadata.ExecutionContext.Schema;
        }

        ISchema IWriter.GetSchema()
        {
            return GetSchema();
        }

        /// <summary>
        /// Write the textual representation of the record schema.
        /// </summary>
        /// <remarks>If the header or records have already been written, this call is ignored.</remarks>
        public void WriteSchema()
        {
            if (isSchemaWritten)
            {
                return;
            }
            if (recordWriter.Metadata.ExecutionContext.Schema != null)
            {
                recordWriter.WriteSchema();
                recordWriter.WriteRecordSeparator();
                ++recordWriter.Metadata.PhysicalRecordNumber;
            }
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
            if (recordWriter.Metadata.ExecutionContext.Schema != null)
            {
                await recordWriter.WriteSchemaAsync().ConfigureAwait(false);
                await recordWriter.WriteRecordSeparatorAsync().ConfigureAwait(false);
                ++recordWriter.Metadata.PhysicalRecordNumber;
            }
            isSchemaWritten = true;
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
                throw new ArgumentNullException(nameof(values));
            }
            if (!isSchemaWritten)
            {
                if (recordWriter.Metadata.ExecutionContext.Options.IsFirstRecordSchema && recordWriter.Metadata.ExecutionContext.Schema != null)
                {
                    recordWriter.WriteSchema();
                    recordWriter.WriteRecordSeparator();
                    ++recordWriter.Metadata.PhysicalRecordNumber;
                }
                isSchemaWritten = true;
            }
            try
            {
                recordWriter.WriteRecord(values);
                recordWriter.WriteRecordSeparator();
                ++recordWriter.Metadata.PhysicalRecordNumber;
                ++recordWriter.Metadata.LogicalRecordNumber;
            }
            catch (RecordProcessingException exception)
            {
                ProcessError(exception);
            }
            catch (FlatFileException exception)
            {
                ProcessError(new RecordProcessingException(recordWriter.Metadata, Resources.InvalidRecordConversion, exception));
            }
        }

        /// <summary>
        /// Writes the textual representation of the given values to the writer.
        /// </summary>
        /// <param name="values">The values to write.</param>
        /// <exception cref="System.ArgumentNullException">The values array is null.</exception>
        public async Task WriteAsync(object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            if (!isSchemaWritten)
            {
                if (recordWriter.Metadata.ExecutionContext.Options.IsFirstRecordSchema && recordWriter.Metadata.ExecutionContext.Schema != null)
                {
                    await recordWriter.WriteSchemaAsync().ConfigureAwait(false);
                    await recordWriter.WriteRecordSeparatorAsync().ConfigureAwait(false);
                    ++recordWriter.Metadata.PhysicalRecordNumber;
                }
                isSchemaWritten = true;
            }
            try
            {
                await recordWriter.WriteRecordAsync(values).ConfigureAwait(false);
                await recordWriter.WriteRecordSeparatorAsync().ConfigureAwait(false);
                ++recordWriter.Metadata.PhysicalRecordNumber;
                ++recordWriter.Metadata.LogicalRecordNumber;
            }
            catch (RecordProcessingException exception)
            {
                ProcessError(exception);
            }
            catch (FlatFileException exception)
            {
                ProcessError(new RecordProcessingException(recordWriter.Metadata, Resources.InvalidRecordConversion, exception));
            }
        }

        /// <summary>
        /// Write the given data directly to the output. By default, this will
        /// not include a newline.
        /// </summary>
        /// <param name="data">The data to write to the output.</param>
        /// <param name="writeRecordSeparator">Indicates whether a newline should be written after the data.</param>
        public void WriteRaw(String data, bool writeRecordSeparator = false)
        {
            recordWriter.WriteRaw(data);
            if (writeRecordSeparator)
            {
                recordWriter.WriteRecordSeparator();
            }
        }

        /// <summary>
        /// Write the given data directly to the output. By default, this will
        /// not include a newline.
        /// </summary>
        /// <param name="data">The data to write to the output.</param>
        /// <param name="writeRecordSeparator">Indicates whether a record separator should be written after the data.</param>
        public async Task WriteRawAsync(String data, bool writeRecordSeparator = false)
        {
            await recordWriter.WriteRawAsync(data);
            if (writeRecordSeparator)
            {
                await recordWriter.WriteRecordSeparatorAsync();
            }
        }

        private void ProcessError(RecordProcessingException exception)
        {
            if (RecordError != null)
            {
                var args = new RecordErrorEventArgs(exception);
                RecordError(this, args);
                if (args.IsHandled)
                {
                    return;
                }
            }
            throw exception;
        }

        IRecordContext IWriterWithMetadata.GetMetadata()
        {
            return recordWriter.Metadata;
        }
    }
}
