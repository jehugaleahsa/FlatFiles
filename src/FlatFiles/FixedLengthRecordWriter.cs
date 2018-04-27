using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlatFiles.Resources;

namespace FlatFiles
{
    internal sealed class FixedLengthRecordWriter
    {
        private readonly TextWriter writer;
        private readonly FixedLengthWriterMetadata metadata;

        public FixedLengthRecordWriter(TextWriter writer, FixedLengthSchema schema, FixedLengthOptions options)
        {
            this.writer = writer;
            this.metadata = new FixedLengthWriterMetadata()
            {
                Schema = schema,
                Options = options.Clone()
            };
        }

        public FixedLengthWriterMetadata Metadata
        {
            get { return metadata; }
        }

        public void WriteRecord(object[] values)
        {
            if (values.Length != metadata.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(SharedResources.WrongNumberOfValues, "values");
            }
            var formattedColumns = metadata.Schema.FormatValues(metadata, values);
            var fittedColumns = formattedColumns.Select((v, i) => fitWidth(metadata.Schema.Windows[i], v));
            foreach (string column in fittedColumns)
            {
                writer.Write(column);
            }
        }

        public async Task WriteRecordAsync(object[] values)
        {
            if (values.Length != metadata.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(SharedResources.WrongNumberOfValues, "values");
            }
            var formattedColumns = metadata.Schema.FormatValues(metadata, values);
            var fittedColumns = formattedColumns.Select((v, i) => fitWidth(metadata.Schema.Windows[i], v));
            foreach (string column in fittedColumns)
            {
                await writer.WriteAsync(column);
            }
        }

        public void WriteSchema()
        {
            var names = metadata.Schema.ColumnDefinitions.Select(c => c.ColumnName);
            var fitted = names.Select((v, i) => fitWidth(metadata.Schema.Windows[i], v));
            foreach (string column in fitted)
            {
                writer.Write(column);
            }
        }

        public async Task WriteSchemaAsync()
        {
            var names = metadata.Schema.ColumnDefinitions.Select(c => c.ColumnName);
            var fitted = names.Select((v, i) => fitWidth(metadata.Schema.Windows[i], v));
            foreach (string column in fitted)
            {
                await writer.WriteAsync(column);
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
            OverflowTruncationPolicy policy = window.TruncationPolicy ?? metadata.Options.TruncationPolicy;
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
            var alignment = window.Alignment ?? metadata.Options.Alignment;
            if (alignment == FixedAlignment.LeftAligned)
            {
                return value.PadRight(window.Width, window.FillCharacter ?? metadata.Options.FillCharacter);
            }
            else
            {
                return value.PadLeft(window.Width, window.FillCharacter ?? metadata.Options.FillCharacter);
            }
        }

        public void WriteRecordSeparator()
        {
            if (metadata.Options.HasRecordSeparator)
            {
                writer.Write(metadata.Options.RecordSeparator ?? Environment.NewLine);
            }
        }

        public async Task WriteRecordSeparatorAsync()
        {
            if (metadata.Options.HasRecordSeparator)
            {
                await writer.WriteAsync(metadata.Options.RecordSeparator ?? Environment.NewLine);
            }
        }

        internal class FixedLengthWriterMetadata : IProcessMetadata
        {
            public FixedLengthSchema Schema { get; internal set; }

            ISchema IProcessMetadata.Schema
            {
                get { return Schema; }
            }

            public FixedLengthOptions Options { get; internal set; }

            IOptions IProcessMetadata.Options
            {
                get { return Options; }
            }

            public int RecordCount { get; internal set; }

            public int LogicalRecordCount { get; internal set; }
        }
    }
}
