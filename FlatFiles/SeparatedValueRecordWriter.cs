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
                ProcessContext = new SeparatedValueProcessContext()
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
            Metadata.ProcessContext.Schema = GetSchema(values);
            if (Metadata.ProcessContext.Schema != null && values.Length != Metadata.ProcessContext.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, nameof(values));
            }
            var formattedValues = FormatValues(values);
            EscapeValues(formattedValues);
            string joined = String.Join(Metadata.ProcessContext.Options.Separator, formattedValues);
            writer.Write(joined);
        }

        public async Task WriteRecordAsync(object[] values)
        {
            Metadata.ProcessContext.Schema = GetSchema(values);
            if (Metadata.ProcessContext.Schema != null && values.Length != Metadata.ProcessContext.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, nameof(values));
            }
            var formattedValues = FormatValues(values);
            EscapeValues(formattedValues);
            string joined = String.Join(Metadata.ProcessContext.Options.Separator, formattedValues);
            await writer.WriteAsync(joined).ConfigureAwait(false);
        }

        private SeparatedValueSchema GetSchema(object[] values)
        {
            return injector == null ? Metadata.ProcessContext.Schema : injector.GetSchema(values);
        }

        private string[] FormatValues(object[] values)
        {
            if (Metadata.ProcessContext.Schema == null)
            {
                string[] results = new string[values.Length];
                for (int index = 0; index != results.Length; ++index)
                {
                    results[index] = ToString(values[index]);
                }
                return results;
            }
            return Metadata.ProcessContext.Schema.FormatValues(Metadata, values);
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
            if (Metadata.ProcessContext.Options.QuoteBehavior == QuoteBehavior.AlwaysQuote)
            {
                return true;
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
            if (value.Contains(Metadata.ProcessContext.Options.Separator))
            {
                return true;
            }
            // Escape strings containing the record separator.
            if (Metadata.ProcessContext.Options.RecordSeparator != null && value.Contains(Metadata.ProcessContext.Options.RecordSeparator))
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
            if (Metadata.ProcessContext.Schema == null)
            {
                return;
            }
            var names = Metadata.ProcessContext.Schema.ColumnDefinitions.Select(d => Escape(d.ColumnName));
            string joined = String.Join(Metadata.ProcessContext.Options.Separator, names);
            writer.Write(joined);
        }

        public async Task WriteSchemaAsync()
        {
            if (injector != null)
            {
                return;
            }
            if (Metadata.ProcessContext.Schema == null)
            {
                return;
            }
            var names = Metadata.ProcessContext.Schema.ColumnDefinitions.Select(d => Escape(d.ColumnName));
            string joined = String.Join(Metadata.ProcessContext.Options.Separator, names);
            await writer.WriteAsync(joined).ConfigureAwait(false);
        }

        public void WriteRecordSeparator()
        {
            writer.Write(Metadata.ProcessContext.Options.RecordSeparator ?? Environment.NewLine);
        }

        public async Task WriteRecordSeparatorAsync()
        {
            await writer.WriteAsync(Metadata.ProcessContext.Options.RecordSeparator ?? Environment.NewLine).ConfigureAwait(false);
        }
    }
}
