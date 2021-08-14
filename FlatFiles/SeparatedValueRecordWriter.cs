using System;
using System.IO;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordWriter
    {
        private readonly TextWriter writer;
        private readonly SeparatedValueSchemaInjector? injector;
        private readonly string quoteString;
        private readonly string doubleQuoteString;
        private SeparatedValueRecordContext? recordContext;

        public SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchema? schema, SeparatedValueOptions? options)
        {
            this.writer = writer;
            this.Schema = schema;
            this.Options = options == null ? new SeparatedValueOptions() : options.Clone();
            this.quoteString = Options.Quote.ToString();
            this.doubleQuoteString = Options.Quote.ToString() + Options.Quote;
        }

        public SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchemaInjector injector, SeparatedValueOptions? options)
            : this(writer, (SeparatedValueSchema?)null, options)
        {
            this.injector = injector;
        }

        public SeparatedValueSchema? Schema { get; }

        public SeparatedValueOptions Options { get; }

        public SeparatedValueRecordContext? Metadata => recordContext;

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
                schema = SeparatedValueSchema.BuildDynamicSchema(Options, values.Length);
            }
            var recordContext = NewRecordContext(schema);
            this.recordContext = recordContext;
            if (values.Length != schema.ColumnDefinitions.PhysicalCount)
            {
                throw new RecordProcessingException(recordContext, Resources.WrongNumberOfValues);
            }
            var formattedValues = FormatValues(recordContext, values);
            EscapeValues(formattedValues);
            var joined = string.Join(Options.Separator, formattedValues);
            return joined;
        }

        private SeparatedValueRecordContext NewRecordContext(SeparatedValueSchema schema)
        {
            var executionContext = new SeparatedValueExecutionContext(schema, Options.Clone());
            return new SeparatedValueRecordContext(executionContext)
            {
                PhysicalRecordNumber = PhysicalRecordNumber,
                LogicalRecordNumber = LogicalRecordNumber
            };
        }

        private SeparatedValueSchema? GetSchema(object?[] values)
        {
            if (injector == null)
            {
                return Schema;
            }
            return injector.GetSchema(values);
        }

        private string[] FormatValues(SeparatedValueRecordContext recordContext, object?[] values)
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
            return value?.ToString() ?? string.Empty;
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
                return string.Empty;
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
            if (value == string.Empty)
            {
                return false;
            }
            // Escape strings beginning or ending in whitespace.
            if (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[value.Length - 1]))
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
            if (injector != null)
            {
                return;
            }
            if (Schema == null)
            {
                return;
            }
            var joined = JoinSchema(Schema);
            writer.Write(joined);
        }

        public async Task WriteSchemaAsync()
        {
            if (injector != null)
            {
                return;
            }
            if (Schema == null)
            {
                return;
            }
            var joined = JoinSchema(Schema);
            await writer.WriteAsync(joined).ConfigureAwait(false);
        }

        private string JoinSchema(SeparatedValueSchema schema)
        {
            var names = GetColumnNames(schema);
            var joined = string.Join(Options.Separator, names);
            return joined;
        }

        private string[] GetColumnNames(SeparatedValueSchema schema)
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
