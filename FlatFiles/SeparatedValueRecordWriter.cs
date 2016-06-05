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
        private readonly SeparatedValueSchema schema;
        private readonly SeparatedValueOptions options;
        private readonly string quoteString;
        private readonly string doubleQuoteString;

        public SeparatedValueRecordWriter(SeparatedValueSchema schema, SeparatedValueOptions options)
        {
            this.schema = schema;
            this.options = options;
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

        public void WriteRecord(TextWriter writer, object[] values)
        {
            if (schema != null && values.Length != schema.ColumnDefinitions.Count)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, "values");
            }
            var formattedValues = formatValues(writer, values);
            var escapedValues = formattedValues.Select(v => escape(v));
            string joined = String.Join(options.Separator, formattedValues);
            writer.Write(joined);
        }

        private IEnumerable<string> formatValues(TextWriter writer, object[] values)
        {
            if (schema == null)
            {
                StringColumn column = new StringColumn("a");
                return values.Select(v => column.Format(v, writer.Encoding));
            }
            else
            {
                return schema.FormatValues(values, writer.Encoding);
            }
        }

        private string escape(string value)
        {
            if (value == null)
            {
                return null;
            }
            string escaped = value;
            if (value.Contains(options.Separator) || value.Contains(quoteString))
            {
                escaped = quoteString + escaped.Replace(quoteString, doubleQuoteString) + quoteString;
            }
            return escaped;
        }

        public void WriteSchema(TextWriter writer)
        {
            if (schema == null)
            {
                return;
            }
            var names = schema.ColumnDefinitions.Select(d => escape(d.ColumnName));
            string joined = String.Join(options.Separator, names);
            writer.Write(joined);
        }

        public void WriteRecordSeparator(TextWriter writer)
        {
            writer.Write(options.RecordSeparator);
        }
    }
}
