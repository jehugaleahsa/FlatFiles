using System;
using System.IO;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Builds textual representations of data by giving each field a fixed width.
    /// </summary>
    public sealed class FixedLengthWriter : IWriter, IWriterWithMetadata
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
        public FixedLengthWriter(TextWriter writer, FixedLengthSchema schema, FixedLengthOptions? options = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }
            recordWriter = new FixedLengthRecordWriter(writer, schema, options);
        }

        /// <summary>
        /// Initializes a new FixedLengthBuilder with the given schema.
        /// </summary>
        /// <param name="writer">A writer over the fixed-length document.</param>
        /// <param name="injector">The schema injector to use to determine the schema.</param>
        /// <param name="options">The options used to format the output.</param>
        /// <exception cref="ArgumentNullException">The writer is null.</exception>
        /// <exception cref="ArgumentNullException">The schema injector is null.</exception>
        public FixedLengthWriter(TextWriter writer, FixedLengthSchemaInjector injector, FixedLengthOptions? options = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            if (injector == null)
            {
                throw new ArgumentNullException(nameof(injector));
            }
            recordWriter = new FixedLengthRecordWriter(writer, injector, options);
        }

        /// <summary>
        /// Raised when an error occurs while processing a column.
        /// </summary>
        public event EventHandler<ColumnErrorEventArgs>? ColumnError
        {
            add => recordWriter.ColumnError += value;
            remove => recordWriter.ColumnError -= value;
        }

        /// <summary>
        /// Raised when an error occurs while processing a record.
        /// </summary>
        public event EventHandler<RecordErrorEventArgs>? RecordError;

        IOptions IWriter.Options => recordWriter.Options;

        /// <summary>
        /// Gets the schema used to build the output.
        /// </summary>
        /// <returns>The schema used to build the output.</returns>
        public FixedLengthSchema? GetSchema()
        {
            return recordWriter.ActualSchema;
        }

        ISchema? IWriter.GetSchema()
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
            ++recordWriter.PhysicalRecordNumber;
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
            await recordWriter.WriteSchemaAsync().ConfigureAwait(false);
            await recordWriter.WriteRecordSeparatorAsync().ConfigureAwait(false);
            ++recordWriter.PhysicalRecordNumber;
            isSchemaWritten = true;
        }

        /// <summary>
        /// Writes the textual representation of the given values to the writer.
        /// </summary>
        /// <param name="values">The values to write.</param>
        /// <exception cref="ArgumentNullException">The values array is null.</exception>
        public void Write(object?[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            if (!isSchemaWritten)
            {
                if (recordWriter.Options.IsFirstRecordHeader)
                {
                    recordWriter.WriteSchema();
                    recordWriter.WriteRecordSeparator();
                    ++recordWriter.PhysicalRecordNumber;
                }
                isSchemaWritten = true;
            }
            try
            {
                recordWriter.WriteRecord(values);
                recordWriter.WriteRecordSeparator();
                ++recordWriter.PhysicalRecordNumber;
                ++recordWriter.LogicalRecordNumber;
            }
            catch (RecordProcessingException exception)
            {
                ProcessError(exception);
            }
            catch (FlatFileException exception)
            {
                var recordContext = GetMetadata();
                ProcessError(new RecordProcessingException(recordContext, Resources.InvalidRecordConversion, exception));
            }
        }

        /// <summary>
        /// Writes the textual representation of the given values to the writer.
        /// </summary>
        /// <param name="values">The values to write.</param>
        /// <exception cref="ArgumentNullException">The values array is null.</exception>
        public async Task WriteAsync(object?[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            if (!isSchemaWritten)
            {
                if (recordWriter.Options.IsFirstRecordHeader)
                {
                    await recordWriter.WriteSchemaAsync().ConfigureAwait(false);
                    await recordWriter.WriteRecordSeparatorAsync().ConfigureAwait(false);
                    ++recordWriter.PhysicalRecordNumber;
                }
                isSchemaWritten = true;
            }
            try
            {
                await recordWriter.WriteRecordAsync(values).ConfigureAwait(false);
                await recordWriter.WriteRecordSeparatorAsync().ConfigureAwait(false);
                ++recordWriter.PhysicalRecordNumber;
                ++recordWriter.LogicalRecordNumber;
            }
            catch (RecordProcessingException exception)
            {
                ProcessError(exception);
            }
            catch (FlatFileException exception)
            {
                var recordContext = GetMetadata();
                ProcessError(new RecordProcessingException(recordContext, Resources.InvalidRecordConversion, exception));
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

        private IRecordContext GetMetadata()
        {
            if (recordWriter.Metadata != null)
            {
                return recordWriter.Metadata;
            }
            var schema = recordWriter.GetSchema(new object[0]);
            var executionContext = new GenericExecutionContext(schema, recordWriter.Options.Clone());
            var recordContext = new GenericRecordContext(executionContext)
            {
                PhysicalRecordNumber = recordWriter.PhysicalRecordNumber,
                LogicalRecordNumber = recordWriter.LogicalRecordNumber
            };
            return recordContext;
        }

        IRecordContext IWriterWithMetadata.GetMetadata()
        {
            return GetMetadata();
        }
    }
}
