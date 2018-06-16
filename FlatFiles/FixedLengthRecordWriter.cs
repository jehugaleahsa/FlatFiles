using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class FixedLengthRecordWriter
    {
        private readonly TextWriter writer;
        private readonly FixedLengthSchemaInjector injector;

        public FixedLengthRecordWriter(TextWriter writer, FixedLengthSchema schema, FixedLengthOptions options)
        {
            this.writer = writer;
            this.Metadata = new FixedLengthWriterMetadata()
            {
                Schema = schema,
                Options = options.Clone()
            };
        }

        public FixedLengthRecordWriter(TextWriter writer, FixedLengthSchemaInjector injector, FixedLengthOptions options)
            : this(writer, (FixedLengthSchema)null, options)
        {
            this.injector = injector;
        }

        public FixedLengthWriterMetadata Metadata { get; private set; }

        public void WriteRecord(object[] values)
        {
            var schema = getSchema(values);
            if (values.Length != schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, nameof(values));
            }
            string[] formattedColumns = formatValues(values, schema);
            fitWindows(schema, formattedColumns);
            foreach (string column in formattedColumns)
            {
                writer.Write(column);
            }
        }

        public async Task WriteRecordAsync(object[] values)
        {
            var schema = getSchema(values);
            if (values.Length != schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, nameof(values));
            }
            var formattedColumns = formatValues(values, schema);
            fitWindows(schema, formattedColumns);
            foreach (string column in formattedColumns)
            {
                await writer.WriteAsync(column).ConfigureAwait(false);
            }
        }

        private FixedLengthSchema getSchema(object[] values)
        {
            return injector == null ? Metadata.Schema : injector.GetSchema(values);
        }

        private string[] formatValues(object[] values, FixedLengthSchema schema)
        {
            var metadata = injector == null ? Metadata : new FixedLengthWriterMetadata()
            {
                Schema = schema,
                Options = Metadata.Options,
                RecordCount = Metadata.RecordCount,
                LogicalRecordCount = Metadata.LogicalRecordCount
            };
            return schema.FormatValues(metadata, values);
        }

        private void fitWindows(FixedLengthSchema schema, string[] values)
        {
            for (int index = 0; index != values.Length; ++index)
            {
                var window = schema.Windows[index];
                values[index] = fitWidth(window, values[index]);
            }
        }

        public void WriteSchema()
        {
            if (injector != null)
            {
                return;
            }
            var names = Metadata.Schema.ColumnDefinitions.Select(c => c.ColumnName);
            var fitted = names.Select((v, i) => fitWidth(Metadata.Schema.Windows[i], v));
            foreach (string column in fitted)
            {
                writer.Write(column);
            }
        }

        public async Task WriteSchemaAsync()
        {
            if (injector != null)
            {
                return;
            }
            var names = Metadata.Schema.ColumnDefinitions.Select(c => c.ColumnName);
            var fitted = names.Select((v, i) => fitWidth(Metadata.Schema.Windows[i], v));
            foreach (string column in fitted)
            {
                await writer.WriteAsync(column).ConfigureAwait(false);
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
            OverflowTruncationPolicy policy = window.TruncationPolicy ?? Metadata.Options.TruncationPolicy;
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
            var alignment = window.Alignment ?? Metadata.Options.Alignment;
            if (alignment == FixedAlignment.LeftAligned)
            {
                return value.PadRight(window.Width, window.FillCharacter ?? Metadata.Options.FillCharacter);
            }
            else
            {
                return value.PadLeft(window.Width, window.FillCharacter ?? Metadata.Options.FillCharacter);
            }
        }

        public void WriteRecordSeparator()
        {
            if (Metadata.Options.HasRecordSeparator)
            {
                writer.Write(Metadata.Options.RecordSeparator ?? Environment.NewLine);
            }
        }

        public async Task WriteRecordSeparatorAsync()
        {
            if (Metadata.Options.HasRecordSeparator)
            {
                await writer.WriteAsync(Metadata.Options.RecordSeparator ?? Environment.NewLine).ConfigureAwait(false);
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
