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
            Metadata = new FixedLengthRecordContext()
            {
                ExecutionContext = new FixedLengthExecutionContext()
                {
                    Schema = schema,
                    Options = options.Clone()
                }
            };
        }

        public FixedLengthRecordWriter(TextWriter writer, FixedLengthSchemaInjector injector, FixedLengthOptions options)
            : this(writer, (FixedLengthSchema)null, options)
        {
            this.injector = injector;
        }

        public FixedLengthRecordContext Metadata { get; }

        public void WriteRecord(object[] values)
        {
            Metadata.ExecutionContext.Schema = GetSchema(values);
            if (values.Length != Metadata.ExecutionContext.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new RecordProcessingException(Metadata, Resources.WrongNumberOfValues);
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
            Metadata.ExecutionContext.Schema = GetSchema(values);
            if (values.Length != Metadata.ExecutionContext.Schema.ColumnDefinitions.PhysicalCount)
            {
                throw new RecordProcessingException(Metadata, Resources.WrongNumberOfValues);
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
            return injector == null ? Metadata.ExecutionContext.Schema : injector.GetSchema(values);
        }

        private string[] FormatValues(object[] values)
        {
            return Metadata.ExecutionContext.Schema.FormatValues(Metadata, values);
        }

        private void FitWindows(string[] values)
        {
            var windows = Metadata.ExecutionContext.Schema.Windows;
            for (int index = 0; index != values.Length; ++index)
            {
                var window = windows[index];
                values[index] = FitWidth(window, values[index]);
            }
        }

        public void WriteSchema()
        {
            if (injector != null)
            {
                return;
            }
            var definitions = Metadata.ExecutionContext.Schema.ColumnDefinitions;
            var windows = Metadata.ExecutionContext.Schema.Windows;
            int columnCount = definitions.Count;
            for (int columnIndex = 0; columnIndex != columnCount; ++columnIndex)
            {
                string columnName = definitions[columnIndex].ColumnName;
                string fittedValue = FitWidth(windows[columnIndex], columnName);
                writer.Write(fittedValue);
            }
        }

        public async Task WriteSchemaAsync()
        {
            if (injector != null)
            {
                return;
            }
            var definitions = Metadata.ExecutionContext.Schema.ColumnDefinitions;
            var windows = Metadata.ExecutionContext.Schema.Windows;
            int columnCount = definitions.Count;
            for (int columnIndex = 0; columnIndex != columnCount; ++columnIndex)
            {
                string columnName = definitions[columnIndex].ColumnName;
                string fittedValue = FitWidth(windows[columnIndex], columnName);
                await writer.WriteAsync(fittedValue).ConfigureAwait(false);
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
            var policy = window.TruncationPolicy ?? Metadata.ExecutionContext.Options.TruncationPolicy;
            if (policy == OverflowTruncationPolicy.TruncateLeading)
            {
                int start = value.Length - window.Width;  // take characters on the end
                return value.Substring(start, window.Width);
            }

            return value.Substring(0, window.Width);
        }

        private string GetPaddedValue(string value, Window window)
        {
            var alignment = window.Alignment ?? Metadata.ExecutionContext.Options.Alignment;
            if (alignment == FixedAlignment.LeftAligned)
            {
                return value.PadRight(window.Width, window.FillCharacter ?? Metadata.ExecutionContext.Options.FillCharacter);
            }

            return value.PadLeft(window.Width, window.FillCharacter ?? Metadata.ExecutionContext.Options.FillCharacter);
        }

        public void WriteRecordSeparator()
        {
            if (Metadata.ExecutionContext.Options.HasRecordSeparator)
            {
                writer.Write(Metadata.ExecutionContext.Options.RecordSeparator ?? Environment.NewLine);
            }
        }

        public async Task WriteRecordSeparatorAsync()
        {
            if (Metadata.ExecutionContext.Options.HasRecordSeparator)
            {
                await writer.WriteAsync(Metadata.ExecutionContext.Options.RecordSeparator ?? Environment.NewLine).ConfigureAwait(false);
            }
        }

    }
}
