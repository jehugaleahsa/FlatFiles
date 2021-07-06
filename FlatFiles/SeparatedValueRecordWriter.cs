using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordWriter
    {
        private readonly TextWriter writer;
        private readonly SeparatedValueSchemaInjector injector;
        private readonly string quoteString;
        private readonly string doubleQuoteString;

        public SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchema schema, SeparatedValueOptions options)
        {
            this.writer = writer;
            var executionContext = new SeparatedValueExecutionContext()
            {
                Schema = schema,
                Options = options.Clone()
            };
            Metadata = new SeparatedValueRecordContext()
            {
                ExecutionContext = executionContext
            };
            quoteString = options.Quote.ToString();
            doubleQuoteString = options.Quote.ToString() + options.Quote;
        }

        public SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchemaInjector injector, SeparatedValueOptions options)
            : this(writer, (SeparatedValueSchema)null, options)
        {
            this.injector = injector;
        }

        public SeparatedValueRecordContext Metadata { get; }

        public void WriteRecord(object[] values)
        {
            string joined = FormatAndJoinValues(values);
            writer.Write(joined);
        }

        public async Task WriteRecordAsync(object[] values)
        {
            var joined = FormatAndJoinValues(values);
            await writer.WriteAsync(joined).ConfigureAwait(false);
        }

        private string FormatAndJoinValues(object[] values)
        {
            var executionContext = Metadata.ExecutionContext;
            var schema = GetSchema(values);
            executionContext.Schema = schema;
            if (schema != null && values.Length != schema.ColumnDefinitions.PhysicalCount)
            {
                throw new RecordProcessingException(Metadata, Resources.WrongNumberOfValues);
            }
            var formattedValues = FormatValues(values);
            EscapeValues(formattedValues);
            string joined = String.Join(executionContext.Options.Separator, formattedValues);
            return joined;
        }

        private SeparatedValueSchema GetSchema(object[] values)
        {
            return injector == null ? Metadata.ExecutionContext.Schema : injector.GetSchema(values);
        }

        private string[] FormatValues(object[] values)
        {
            var schema = Metadata.ExecutionContext.Schema;
            if (schema == null)
            {
                string[] results = new string[values.Length];
                for (int index = 0; index != results.Length; ++index)
                {
                    results[index] = ToString(values[index]);
                }
                return results;
            }
            return schema.FormatValues(Metadata, values);
        }

        private static string ToString(object value)
        {
            return value == null ? String.Empty : value.ToString();
        }

        private void EscapeValues(string[] values)
        {
            for (int index = 0; index != values.Length; ++index)
            {
                values[index] = Escape(values[index]);
            }
        }

        private string Escape(string value)
        {
            if (NeedsEscaped(value))
            {
                return quoteString + value.Replace(quoteString, doubleQuoteString) + quoteString;
            }
            return value;
        }

        private bool NeedsEscaped(string value)
        {
            // Never escape null.
            if (value == null)
            {
                return false;
            }
            var options = Metadata.ExecutionContext.Options;
            if (options.QuoteBehavior == QuoteBehavior.AlwaysQuote)
            {
                return true;
            }
            if (options.QuoteBehavior == QuoteBehavior.Never)
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
            if (value.Contains(options.Separator))
            {
                return true;
            }
            // Escape strings containing the record separator.
            if (options.RecordSeparator != null && value.Contains(options.RecordSeparator))
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
            if (Metadata.ExecutionContext.Schema == null)
            {
                return;
            }
            string joined = JoinSchema();
            writer.Write(joined);
        }

        public async Task WriteSchemaAsync()
        {
            if (injector != null)
            {
                return;
            }
            if (Metadata.ExecutionContext.Schema == null)
            {
                return;
            }
            string joined = JoinSchema();
            await writer.WriteAsync(joined).ConfigureAwait(false);
        }

        private String JoinSchema()
        {
            var names = GetColumnNames();
            string joined = String.Join(Metadata.ExecutionContext.Options.Separator, names);
            return joined;
        }

        private string[] GetColumnNames()
        {
            var schema = Metadata.ExecutionContext.Schema;
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
            var separator = Metadata.ExecutionContext.Options.RecordSeparator ?? Environment.NewLine;
            writer.Write(separator);
        }

        public async Task WriteRecordSeparatorAsync()
        {
            var separator = Metadata.ExecutionContext.Options.RecordSeparator ?? Environment.NewLine;
            await writer.WriteAsync(separator).ConfigureAwait(false);
        }

        public void WriteRaw(String data)
        {
            writer.Write(data);
        }

        public Task WriteRawAsync(String data)
        {
            return writer.WriteAsync(data);
        }
    }
}
