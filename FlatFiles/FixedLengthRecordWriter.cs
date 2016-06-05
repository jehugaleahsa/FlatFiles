using System;
using System.IO;
using System.Linq;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class FixedLengthRecordWriter
    {
        private readonly FixedLengthSchema schema;
        private readonly FixedLengthOptions options;

        public FixedLengthRecordWriter(FixedLengthSchema schema, FixedLengthOptions options)
        {
            this.schema = schema;
            this.options = options;
        }

        public FixedLengthSchema Schema
        {
            get { return schema; }
        }

        public void WriteRecord(TextWriter writer, object[] values)
        {
            if (values.Length != schema.ColumnDefinitions.Count)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, "values");
            }
            var formattedColumns = schema.FormatValues(values, writer.Encoding);
            var fittedColumns = formattedColumns.Select((v, i) => fitWidth(schema.Windows[i], v));
            foreach (string column in fittedColumns)
            {
                writer.Write(column);
            }
        }

        private string fitWidth(Window window, string value)
        {
            if (value == null)
            {
                value = String.Empty;
            }
            if (value.Length > window.Width)
            {
                return getTruncatedValue(value, window);
            }
            else if (value.Length < window.Width)
            {
                return getPaddedValue(value, window);
            }
            else
            {
                return value;
            }
        }

        private string getTruncatedValue(string value, Window window)
        {
            OverflowTruncationPolicy policy = window.TruncationPolicy ?? options.TruncationPolicy;
            if (policy == OverflowTruncationPolicy.TruncateLeading)
            {
                int start = value.Length - window.Width;  // take characters on the end
                return value.Substring(start, window.Width);
            }
            else
            {
                return value.Substring(0, window.Width);
            }
        }

        private string getPaddedValue(string value, Window window)
        {
            if (window.Alignment == FixedAlignment.LeftAligned)
            {
                return value.PadRight(window.Width, window.FillCharacter ?? options.FillCharacter);
            }
            else
            {
                return value.PadLeft(window.Width, window.FillCharacter ?? options.FillCharacter);
            }
        }

        public void WriteRecordSeparator(TextWriter writer)
        {
            writer.Write(options.RecordSeparator);
        }
    }
}
