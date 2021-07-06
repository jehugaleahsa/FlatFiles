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
            var executionContext = new FixedLengthExecutionContext()
            {
                Schema = schema,
                Options = options.Clone()
            };
            Metadata = new FixedLengthRecordContext()
            {
                ExecutionContext = executionContext
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
            var schema = GetSchema(values);
            Metadata.ExecutionContext.Schema = schema;
            if (values.Length != schema.ColumnDefinitions.PhysicalCount)
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
            var schema = GetSchema(values);
            Metadata.ExecutionContext.Schema = schema;
            if (values.Length != schema.ColumnDefinitions.PhysicalCount)
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
                string value = values[index];
                if (index < windows.Count)
                {
                    var window = windows[index];
                    values[index] = FitWidth(window, value);
                }
                else
                {
                    values[index] = value ?? String.Empty;
                }
            }
        }

        public void WriteSchema()
        {
            if (injector != null)
            {
                return;
            }
            var schema = Metadata.ExecutionContext.Schema;
            var definitions = schema.ColumnDefinitions;
            var windows = schema.Windows;
            int columnCount = definitions.Count;
            for (int columnIndex = 0; columnIndex != columnCount; ++columnIndex)
            {
                var window = windows[columnIndex];
                var column = definitions[columnIndex];
                var columnName = column.ColumnName;
                var fittedValue = FitWidth(window, columnName);
                writer.Write(fittedValue);
            }
        }

        public async Task WriteSchemaAsync()
        {
            if (injector != null)
            {
                return;
            }
            var schema = Metadata.ExecutionContext.Schema;
            var definitions = schema.ColumnDefinitions;
            var windows = schema.Windows;
            int columnCount = definitions.Count;
            for (int columnIndex = 0; columnIndex != columnCount; ++columnIndex)
            {
                var window = windows[columnIndex];
                var column = definitions[columnIndex];
                string columnName = column.ColumnName;
                string fittedValue = FitWidth(window, columnName);
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
            var options = Metadata.ExecutionContext.Options;
            var alignment = window.Alignment ?? options.Alignment;
            var fillCharacter = window.FillCharacter ?? options.FillCharacter;
            if (alignment == FixedAlignment.LeftAligned)
            {
                return value.PadRight(window.Width, fillCharacter);
            }
            else
            {
                return value.PadLeft(window.Width, fillCharacter);
            }
        }

        public void WriteRecordSeparator()
        {
            var options = Metadata.ExecutionContext.Options;
            if (options.HasRecordSeparator)
            {
                var separator = options.RecordSeparator ?? Environment.NewLine;
                writer.Write(separator);
            }
        }

        public async Task WriteRecordSeparatorAsync()
        {
            var options = Metadata.ExecutionContext.Options;
            if (options.HasRecordSeparator)
            {
                var separator = options.RecordSeparator ?? Environment.NewLine;
                await writer.WriteAsync(separator).ConfigureAwait(false);
            }
        }

        public void WriteRaw(String data)
        {
            writer.Write(data);
        }

        public Task WriteRawAsync(String data)
        {
            return writer.WriteAsync(data);
        }
    }
}
