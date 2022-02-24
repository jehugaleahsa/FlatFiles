using System;
using System.IO;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class DelimitedRecordWriter
    {
        private readonly TextWriter writer;
        private readonly DelimitedSchema? schema;
        private readonly DelimitedSchemaInjector? injector;
        private readonly string quoteString;
        private readonly string doubleQuoteString;
        private DelimitedRecordContext? recordContext;

        public DelimitedRecordWriter(TextWriter writer, DelimitedSchema? schema, DelimitedOptions? options)
        {
            this.writer = writer;
            this.schema = schema;
            this.Options = options == null ? new DelimitedOptions() : options.Clone();
            this.quoteString = Options.Quote.ToString();
            this.doubleQuoteString = Options.Quote.ToString() + Options.Quote;
        }

        public DelimitedRecordWriter(TextWriter writer, DelimitedSchemaInjector injector, DelimitedOptions? options)
            : this(writer, (DelimitedSchema?)null, options)
        {
            this.injector = injector;
        }

        public DelimitedSchema? ActualSchema => schema;

        public DelimitedSchema? Schema => GetSchema(new object[0]);

        public DelimitedOptions Options { get; }

        public DelimitedRecordContext? Metadata => recordContext;

        public event EventHandler<ColumnErrorEventArgs>? ColumnError;

        public int PhysicalRecordNumber { get; set; }

        public int LogicalRecordNumber { get; set; }

        public void WriteRecord(object?[] values)
        {
            string joined = FormatAndJoinValues(values);
            writer.Write(joined);
        }

        public async Task WriteRecordAsync(object?[] values)
        {
            var joined = FormatAndJoinValues(values);
            await writer.WriteAsync(joined).ConfigureAwait(false);
        }

        private string FormatAndJoinValues(object?[] values)
        {
            var schema = GetSchema(values);
            if (schema == null)
            {
                schema = DelimitedSchema.BuildDynamicSchema(Options, values.Length);
            }
            var recordContext = NewRecordContext(schema);
            this.recordContext = recordContext;
            if (values.Length != schema.ColumnDefinitions.PhysicalCount)
            {
                throw new RecordProcessingException(recordContext, Resources.WrongNumberOfValues);
            }
            var formattedValues = FormatValues(recordContext, values);
            EscapeValues(formattedValues);
            var joined = String.Join(Options.Separator, formattedValues);
            return joined;
        }

        private DelimitedRecordContext NewRecordContext(DelimitedSchema schema)
        {
            var executionContext = new DelimitedExecutionContext(schema, Options.Clone());
            return new DelimitedRecordContext(executionContext)
            {
                PhysicalRecordNumber = PhysicalRecordNumber,
                LogicalRecordNumber = LogicalRecordNumber
            };
        }

        private DelimitedSchema? GetSchema(object?[] values)
        {
            if (injector == null)
            {
                return schema;
            }
            return injector.GetSchema(values);
        }

        private string[] FormatValues(DelimitedRecordContext recordContext, object?[] values)
        {
            var schema = recordContext.ExecutionContext.Schema;
            if (schema == null)
            {
                string[] results = new string[values.Length];
                for (int index = 0; index != results.Length; ++index)
                {
                    results[index] = ToString(values[index]);
                }
                return results;
            }
            recordContext.ColumnError += ColumnError;
            return schema.FormatValues(recordContext, values);
        }

        private static string ToString(object? value)
        {
            return value?.ToString() ?? String.Empty;
        }

        private void EscapeValues(string[] values)
        {
            for (int index = 0; index != values.Length; ++index)
            {
                values[index] = Escape(values[index]);
            }
        }

        private string Escape(string? value)
        {
            if (value == null)
            {
                return String.Empty;
            }
            if (NeedsEscaped(value))
            {
                return quoteString + value.Replace(quoteString, doubleQuoteString) + quoteString;
            }
            return value;
        }

        private bool NeedsEscaped(string value)
        {
            if (Options.QuoteBehavior == QuoteBehavior.AlwaysQuote)
            {
                return true;
            }
            if (Options.QuoteBehavior == QuoteBehavior.Never)
            {
                return false;
            }
            // Don't escape empty strings.
            if (value == String.Empty)
            {
                return false;
            }
            // Escape strings beginning or ending in whitespace.
            if (Char.IsWhiteSpace(value[0]) || Char.IsWhiteSpace(value[value.Length - 1]))
            {
                return true;
            }
            // Escape strings containing the separator.
            if (value.Contains(Options.Separator))
            {
                return true;
            }
            // Escape strings containing the record separator.
            if (Options.RecordSeparator != null && value.Contains(Options.RecordSeparator))
            {
                return true;
            }
            // Escape strings containing quotes.
            if (value.Contains(quoteString))
            {
                return true;
            }
            return false;
        }

        public void WriteSchema()
        {
            if (schema == null)
            {
                return;
            }
            var joined = JoinSchema(schema);
            writer.Write(joined);
        }

        public async Task WriteSchemaAsync()
        {
            if (schema == null)
            {
                return;
            }
            var joined = JoinSchema(schema);
            await writer.WriteAsync(joined).ConfigureAwait(false);
        }

        private string JoinSchema(DelimitedSchema schema)
        {
            var names = GetColumnNames(schema);
            var joined = String.Join(Options.Separator, names);
            return joined;
        }

        private string[] GetColumnNames(DelimitedSchema schema)
        {
            var definitions = schema.ColumnDefinitions;
            string[] columnNames = new string[schema.ColumnDefinitions.Count];
            for (int columnIndex = 0; columnIndex != columnNames.Length; ++columnIndex)
            {
                var column = definitions[columnIndex];
                var columnName = column.ColumnName;
                columnNames[columnIndex] = Escape(columnName);
            }
            return columnNames;
        }

        public void WriteRecordSeparator()
        {
            var separator = Options.RecordSeparator ?? Environment.NewLine;
            writer.Write(separator);
        }

        public async Task WriteRecordSeparatorAsync()
        {
            var separator = Options.RecordSeparator ?? Environment.NewLine;
            await writer.WriteAsync(separator).ConfigureAwait(false);
        }

        public void WriteRaw(string data)
        {
            writer.Write(data);
        }

        public Task WriteRawAsync(string data)
        {
            return writer.WriteAsync(data);
        }
    }
}
