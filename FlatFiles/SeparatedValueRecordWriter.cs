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
            Metadata = new SeparatedValueRecordContext()
            {
                ExecutionContext = new SeparatedValueExecutionContext()
                {
                    Schema = schema,
                    Options = options.Clone()
                }
            };
            quoteString = String.Empty + options.Quote;
            doubleQuoteString = String.Empty + options.Quote + options.Quote;
        }

        public SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchemaInjector injector, SeparatedValueOptions options)
            : this(writer, (SeparatedValueSchema)null, options)
        {
            this.injector = injector;
        }

        public SeparatedValueRecordContext Metadata { get; }

        public void WriteRecord(object[] values)
        {
            Metadata.ExecutionContext.Schema = GetSchema(values);
            if (Metadata.ExecutionContext.Schema != null && values.Length != Metadata.ExecutionContext.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new RecordProcessingException(Metadata, Resources.WrongNumberOfValues);
            }
            var formattedValues = FormatValues(values);
            EscapeValues(formattedValues);
            string joined = String.Join(Metadata.ExecutionContext.Options.Separator, formattedValues);
            writer.Write(joined);
        }

        public async Task WriteRecordAsync(object[] values)
        {
            Metadata.ExecutionContext.Schema = GetSchema(values);
            if (Metadata.ExecutionContext.Schema != null && values.Length != Metadata.ExecutionContext.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new RecordProcessingException(Metadata, Resources.WrongNumberOfValues);
            }
            var formattedValues = FormatValues(values);
            EscapeValues(formattedValues);
            string joined = String.Join(Metadata.ExecutionContext.Options.Separator, formattedValues);
            await writer.WriteAsync(joined).ConfigureAwait(false);
        }

        private SeparatedValueSchema GetSchema(object[] values)
        {
            return injector == null ? Metadata.ExecutionContext.Schema : injector.GetSchema(values);
        }

        private string[] FormatValues(object[] values)
        {
            if (Metadata.ExecutionContext.Schema == null)
            {
                string[] results = new string[values.Length];
                for (int index = 0; index != results.Length; ++index)
                {
                    results[index] = ToString(values[index]);
                }
                return results;
            }
            return Metadata.ExecutionContext.Schema.FormatValues(Metadata, values);
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
            if (Metadata.ExecutionContext.Options.QuoteBehavior == QuoteBehavior.AlwaysQuote)
            {
                return true;
            }
            if (Metadata.ExecutionContext.Options.QuoteBehavior == QuoteBehavior.Never)
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
            if (value.Contains(Metadata.ExecutionContext.Options.Separator))
            {
                return true;
            }
            // Escape strings containing the record separator.
            if (Metadata.ExecutionContext.Options.RecordSeparator != null && value.Contains(Metadata.ExecutionContext.Options.RecordSeparator))
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
            var names = getColumnNames();
            string joined = String.Join(Metadata.ExecutionContext.Options.Separator, names);
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
            var names = getColumnNames();
            string joined = String.Join(Metadata.ExecutionContext.Options.Separator, names);
            await writer.WriteAsync(joined).ConfigureAwait(false);
        }

        private string[] getColumnNames()
        {
            var definitions = Metadata.ExecutionContext.Schema.ColumnDefinitions;
            string[] columnNames = new string[Metadata.ExecutionContext.Schema.ColumnDefinitions.Count];
            for (int columnIndex = 0; columnIndex != columnNames.Length; ++columnIndex)
            {
                var columnName = definitions[columnIndex].ColumnName;
                columnNames[columnIndex] = Escape(columnName);
            }
            return columnNames;
        }

        public void WriteRecordSeparator()
        {
            writer.Write(Metadata.ExecutionContext.Options.RecordSeparator ?? Environment.NewLine);
        }

        public async Task WriteRecordSeparatorAsync()
        {
            await writer.WriteAsync(Metadata.ExecutionContext.Options.RecordSeparator ?? Environment.NewLine).ConfigureAwait(false);
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
