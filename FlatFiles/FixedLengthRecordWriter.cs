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
            Metadata = new FixedLengthProcessMetadata()
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

        public FixedLengthProcessMetadata Metadata { get; }

        public void WriteRecord(object[] values)
        {
            Metadata.Schema = GetSchema(values);
            if (values.Length != Metadata.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, nameof(values));
            }
            string[] formattedColumns = FormatValues(values);
            FitWindows(formattedColumns);
            foreach (string column in formattedColumns)
            {
                writer.Write(column);
            }
        }

        public async Task WriteRecordAsync(object[] values)
        {
            Metadata.Schema = GetSchema(values);
            if (values.Length != Metadata.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, nameof(values));
            }
            var formattedColumns = FormatValues(values);
            FitWindows(formattedColumns);
            foreach (string column in formattedColumns)
            {
                await writer.WriteAsync(column).ConfigureAwait(false);
            }
        }

        private FixedLengthSchema GetSchema(object[] values)
        {
            return injector == null ? Metadata.Schema : injector.GetSchema(values);
        }

        private string[] FormatValues(object[] values)
        {
            return Metadata.Schema.FormatValues(Metadata, values);
        }

        private void FitWindows(string[] values)
        {
            for (int index = 0; index != values.Length; ++index)
            {
                var window = Metadata.Schema.Windows[index];
                values[index] = FitWidth(window, values[index]);
            }
        }

        public void WriteSchema()
        {
            if (injector != null)
            {
                return;
            }
            var names = Metadata.Schema.ColumnDefinitions.Select(c => c.ColumnName);
            var fitted = names.Select((v, i) => FitWidth(Metadata.Schema.Windows[i], v));
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
            var fitted = names.Select((v, i) => FitWidth(Metadata.Schema.Windows[i], v));
            foreach (string column in fitted)
            {
                await writer.WriteAsync(column).ConfigureAwait(false);
            }
        }

        private string FitWidth(Window window, string value)
        {
            if (value == null)
            {
                value = String.Empty;
            }
            if (value.Length > window.Width)
            {
                return GetTruncatedValue(value, window);
            }
            if (value.Length < window.Width)
            {
                return GetPaddedValue(value, window);
            }

            return value;
        }

        private string GetTruncatedValue(string value, Window window)
        {
            OverflowTruncationPolicy policy = window.TruncationPolicy ?? Metadata.Options.TruncationPolicy;
            if (policy == OverflowTruncationPolicy.TruncateLeading)
            {
                int start = value.Length - window.Width;  // take characters on the end
                return value.Substring(start, window.Width);
            }

            return value.Substring(0, window.Width);
        }

        private string GetPaddedValue(string value, Window window)
        {
            var alignment = window.Alignment ?? Metadata.Options.Alignment;
            if (alignment == FixedAlignment.LeftAligned)
            {
                return value.PadRight(window.Width, window.FillCharacter ?? Metadata.Options.FillCharacter);
            }

            return value.PadLeft(window.Width, window.FillCharacter ?? Metadata.Options.FillCharacter);
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
    }
}
