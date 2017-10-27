using System;
using System.IO;
using System.Linq;
using FlatFiles.Resources;

namespace FlatFiles
{
    internal sealed class FixedLengthRecordWriter
    {
        private readonly TextWriter writer;
        private readonly FixedLengthSchema schema;
        private readonly FixedLengthOptions options;

        public FixedLengthRecordWriter(TextWriter writer, FixedLengthSchema schema, FixedLengthOptions options)
        {
            this.writer = writer;
            this.schema = schema;
            this.options = options.Clone();
        }

        public FixedLengthSchema Schema
        {
            get { return schema; }
        }

        public FixedLengthOptions Options
        {
            get { return options; }
        }

        public void WriteRecord(object[] values)
        {
            if (values.Length != schema.ColumnDefinitions.HandledCount)
            {
                throw new ArgumentException(SharedResources.WrongNumberOfValues, "values");
            }
            var formattedColumns = schema.FormatValues(values);
            var fittedColumns = formattedColumns.Select((v, i) => fitWidth(schema.Windows[i], v));
            foreach (string column in fittedColumns)
            {
                writer.Write(column);
            }
        }

        public void WriteSchema()
        {
            var names = schema.ColumnDefinitions.Select(c => c.ColumnName);
            var fitted = names.Select((v, i) => fitWidth(schema.Windows[i], v));
            foreach (string column in fitted)
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
            var alignment = window.Alignment ?? options.Alignment;
            if (alignment == FixedAlignment.LeftAligned)
            {
                return value.PadRight(window.Width, window.FillCharacter ?? options.FillCharacter);
            }
            else
            {
                return value.PadLeft(window.Width, window.FillCharacter ?? options.FillCharacter);
            }
        }

        public void WriteRecordSeparator()
        {
            if (!options.HasRecordSeparator)
            {
                return;
            }
            writer.Write(options.RecordSeparator ?? Environment.NewLine);
        }
    }
}
