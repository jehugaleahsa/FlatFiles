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
        private readonly SeparatedValueMetadata metadata;
        private readonly string quoteString;
        private readonly string doubleQuoteString;

        public SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchema schema, SeparatedValueOptions options)
        {
            this.writer = writer;
            this.metadata = new SeparatedValueMetadata()
            {
                Schema = schema,
                Options = options.Clone()
            };
            this.quoteString = String.Empty + options.Quote;
            this.doubleQuoteString = String.Empty + options.Quote + options.Quote;
        }

        public SeparatedValueMetadata Metadata
        {
            get { return metadata; }
        }

        public void WriteRecord(object[] values)
        {
            if (metadata.Schema != null && values.Length != metadata.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(SharedResources.WrongNumberOfValues, "values");
            }
            var formattedValues = formatValues(values);
            var escapedValues = formattedValues.Select(v => escape(v));
            string joined = String.Join(metadata.Options.Separator, escapedValues);
            writer.Write(joined);
        }

        public async Task WriteRecordAsync(object[] values)
        {
            if (metadata.Schema != null && values.Length != metadata.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(SharedResources.WrongNumberOfValues, nameof(values));
            }
            var formattedValues = formatValues(values);
            var escapedValues = formattedValues.Select(v => escape(v));
            string joined = String.Join(metadata.Options.Separator, escapedValues);
            await writer.WriteAsync(joined);
        }

        private IEnumerable<string> formatValues(object[] values)
        {
            if (metadata.Schema == null)
            {
                StringColumn column = new StringColumn("a");
                return values.Select(v => column.Format(v));
            }
            else
            {
                return metadata.Schema.FormatValues(metadata, values);
            }
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
            if (metadata.Options.QuoteBehavior == QuoteBehavior.AlwaysQuote)
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
            if (value.Contains(metadata.Options.Separator))
            {
                return true;
            }
            // Escape strings containing the record separator.
            if (metadata.Options.RecordSeparator != null && value.Contains(metadata.Options.RecordSeparator))
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
            if (metadata.Schema == null)
            {
                return;
            }
            var names = metadata.Schema.ColumnDefinitions.Select(d => escape(d.ColumnName));
            string joined = String.Join(metadata.Options.Separator, names);
            writer.Write(joined);
        }

        public async Task WriteSchemaAsync()
        {
            if (metadata.Schema == null)
            {
                return;
            }
            var names = metadata.Schema.ColumnDefinitions.Select(d => escape(d.ColumnName));
            string joined = String.Join(metadata.Options.Separator, names);
            await writer.WriteAsync(joined);
        }

        public void WriteRecordSeparator()
        {
            writer.Write(metadata.Options.RecordSeparator ?? Environment.NewLine);
        }

        public async Task WriteRecordSeparatorAsync()
        {
            await writer.WriteAsync(metadata.Options.RecordSeparator ?? Environment.NewLine);
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
