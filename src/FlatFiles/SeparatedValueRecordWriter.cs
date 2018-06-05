using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlatFiles.Resources;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordWriter
    {
        private readonly TextWriter writer;
        private readonly SeparatedValueSchemaInjector injector;
        private readonly string quoteString;
        private readonly string doubleQuoteString;

        public SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchema schema, SeparatedValueOptions options)
            : this(writer, schema, options, false)
        {            
        }

        public SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchemaInjector injector, SeparatedValueOptions options)
            : this(writer, null, options, false)
        {
            this.injector = injector;
        }

        private SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchema schema, SeparatedValueOptions options, bool ignored)
        {
            this.writer = writer;
            this.Metadata = new SeparatedValueMetadata()
            {
                Schema = schema,
                Options = options.Clone()
            };
            this.quoteString = String.Empty + options.Quote;
            this.doubleQuoteString = String.Empty + options.Quote + options.Quote;
        }

        public SeparatedValueMetadata Metadata { get; private set; }

        public void WriteRecord(object[] values)
        {
            var schema = getSchema(values);
            if (schema != null && values.Length != schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(SharedResources.WrongNumberOfValues, nameof(values));
            }
            var formattedValues = formatValues(schema, values);
            var escapedValues = formattedValues.Select(v => escape(v));
            string joined = String.Join(Metadata.Options.Separator, escapedValues);
            writer.Write(joined);
        }

        public async Task WriteRecordAsync(object[] values)
        {
            var schema = getSchema(values);
            if (schema != null && values.Length != schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(SharedResources.WrongNumberOfValues, nameof(values));
            }
            var formattedValues = formatValues(schema, values);
            var escapedValues = formattedValues.Select(v => escape(v));
            string joined = String.Join(Metadata.Options.Separator, escapedValues);
            await writer.WriteAsync(joined);
        }

        private SeparatedValueSchema getSchema(object[] values)
        {
            return injector == null ? Metadata.Schema : injector.GetSchema(values);
        }

        private IEnumerable<string> formatValues(SeparatedValueSchema schema, object[] values)
        {
            if (schema == null)
            {
                return values.Select(v => toString(v));
            }
            else
            {
                var metadata = injector == null ? Metadata : new SeparatedValueMetadata()
                {
                    Schema = schema,
                    Options = Metadata.Options,
                    RecordCount = Metadata.RecordCount,
                    LogicalRecordCount = Metadata.LogicalRecordCount
                };
                return schema.FormatValues(Metadata, values);
            }
        }

        private static string toString(object value)
        {
            return value == null ? String.Empty : value.ToString();
        }

        private string escape(string value)
        {
            if (needsEscaped(value))
            {
                return quoteString + value.Replace(quoteString, doubleQuoteString) + quoteString;
            }
            else
            {
                return value;
            }
        }

        private bool needsEscaped(string value)
        {
            // Never escape null.
            if (value == null)
            {
                return false;
            }
            if (Metadata.Options.QuoteBehavior == QuoteBehavior.AlwaysQuote)
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
            if (value.Contains(Metadata.Options.Separator))
            {
                return true;
            }
            // Escape strings containing the record separator.
            if (Metadata.Options.RecordSeparator != null && value.Contains(Metadata.Options.RecordSeparator))
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
            if (Metadata.Schema == null)
            {
                return;
            }
            var names = Metadata.Schema.ColumnDefinitions.Select(d => escape(d.ColumnName));
            string joined = String.Join(Metadata.Options.Separator, names);
            writer.Write(joined);
        }

        public async Task WriteSchemaAsync()
        {
            if (injector != null)
            {
                return;
            }
            if (Metadata.Schema == null)
            {
                return;
            }
            var names = Metadata.Schema.ColumnDefinitions.Select(d => escape(d.ColumnName));
            string joined = String.Join(Metadata.Options.Separator, names);
            await writer.WriteAsync(joined);
        }

        public void WriteRecordSeparator()
        {
            writer.Write(Metadata.Options.RecordSeparator ?? Environment.NewLine);
        }

        public async Task WriteRecordSeparatorAsync()
        {
            await writer.WriteAsync(Metadata.Options.RecordSeparator ?? Environment.NewLine);
        }

        internal class SeparatedValueMetadata : IProcessMetadata
        {
            public SeparatedValueSchema Schema { get; internal set; }

            ISchema IProcessMetadata.Schema
            {
                get { return Schema; }
            }

            public SeparatedValueOptions Options { get; internal set; }

            IOptions IProcessMetadata.Options
            {
                get { return Options; }
            }

            public int RecordCount { get; internal set; }

            public int LogicalRecordCount { get; internal set; }
        }
    }
}
