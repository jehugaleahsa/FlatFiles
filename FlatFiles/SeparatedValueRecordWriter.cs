using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordWriter
    {
        private readonly TextWriter writer;
        private readonly SeparatedValueSchema schema;
        private readonly SeparatedValueOptions options;
        private readonly string quoteString;
        private readonly string doubleQuoteString;

        public SeparatedValueRecordWriter(TextWriter writer, SeparatedValueSchema schema, SeparatedValueOptions options)
        {
            this.writer = writer;
            this.schema = schema;
            this.options = options.Clone();
            this.quoteString = String.Empty + options.Quote;
            this.doubleQuoteString = String.Empty + options.Quote + options.Quote;
        }

        public SeparatedValueSchema Schema
        {
            get { return schema; }
        }

        public SeparatedValueOptions Options
        {
            get { return options; }
        }

        public void WriteRecord(object[] values)
        {
            if (schema != null && values.Length != schema.ColumnDefinitions.Count)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, "values");
            }
            var formattedValues = formatValues(values);
            var escapedValues = formattedValues.Select(v => escape(v));
            string joined = String.Join(options.Separator, escapedValues);
            writer.Write(joined);
        }

        private IEnumerable<string> formatValues(object[] values)
        {
            if (schema == null)
            {
                StringColumn column = new StringColumn("a");
                return values.Select(v => column.Format(v));
            }
            else
            {
                return schema.FormatValues(values);
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
            // Don't escape null or empty strings.
            if (String.IsNullOrEmpty(value))
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
            if (value.Contains(options.RecordSeparator))
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
            var names = schema.ColumnDefinitions.Select(d => escape(d.ColumnName));
            string joined = String.Join(options.Separator, names);
            writer.Write(joined);
        }

        public void WriteRecordSeparator()
        {
            writer.Write(options.RecordSeparator);
        }
    }
}
