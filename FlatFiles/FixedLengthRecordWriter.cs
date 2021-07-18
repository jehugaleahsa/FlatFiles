using System;
using System.IO;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class FixedLengthRecordWriter
    {
        private readonly TextWriter writer;
        private readonly FixedLengthSchemaInjector? injector;
        private FixedLengthRecordContext? recordContext;

        public FixedLengthRecordWriter(TextWriter writer, FixedLengthSchema? schema, FixedLengthOptions? options)
        {
            this.writer = writer;
            Schema = schema;
            Options = options == null ? new FixedLengthOptions() : options.Clone();
        }

        public FixedLengthRecordWriter(TextWriter writer, FixedLengthSchemaInjector injector, FixedLengthOptions? options)
            : this(writer, (FixedLengthSchema?)null, options)
        {
            this.injector = injector;
        }

        public FixedLengthRecordContext? Metadata => recordContext;

        public FixedLengthSchema? Schema { get; }

        public FixedLengthOptions Options { get; }

        public int PhysicalRecordNumber { get; set;  }

        public int LogicalRecordNumber { get; set; }

        public event EventHandler<ColumnErrorEventArgs>? ColumnError;

        public void WriteRecord(object?[] values)
        {
            this.recordContext = null;
            var formattedColumns = FormatAndFitValues(values);
            foreach (string column in formattedColumns)
            {
                writer.Write(column);
            }
        }

        public async Task WriteRecordAsync(object?[] values)
        {
            this.recordContext = null;
            var formattedColumns = FormatAndFitValues(values);
            foreach (string column in formattedColumns)
            {
                await writer.WriteAsync(column).ConfigureAwait(false);
            }
        }

        private string[] FormatAndFitValues(object?[] values)
        {
            var schema = GetSchema(values);
            var metadata = NewRecordContext(schema, null, null);
            this.recordContext = metadata;
            if (values.Length != schema.ColumnDefinitions.PhysicalCount)
            {
                throw new RecordProcessingException(metadata, Resources.WrongNumberOfValues);
            }
            var formattedColumns = FormatValues(metadata, values);
            FitWindows(schema, formattedColumns);
            return formattedColumns;
        }

        private FixedLengthSchema GetSchema(object?[] values)
        {
            return injector == null ? Schema! : injector.GetSchema(values);
        }

        private FixedLengthRecordContext NewRecordContext(FixedLengthSchema schema, string? record, string[]? values)
        {
            var executionContext = new FixedLengthExecutionContext(schema, Options.Clone());
            var recordContext = new FixedLengthRecordContext(executionContext)
            {
                PhysicalRecordNumber = PhysicalRecordNumber,
                LogicalRecordNumber = LogicalRecordNumber,
                Record = record,
                Values = values
            };
            return recordContext;
        }

        private string[] FormatValues(FixedLengthRecordContext metadata, object?[] values)
        {
            var schema = metadata.ExecutionContext.Schema;
            metadata.ColumnError += ColumnError;
            return schema.FormatValues(metadata, values);
        }

        private void FitWindows(FixedLengthSchema schema, string[] values)
        {
            var windows = schema.Windows;
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
            var definitions = Schema!.ColumnDefinitions;
            var windows = Schema.Windows;
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
            var definitions = Schema!.ColumnDefinitions;
            var windows = Schema.Windows;
            int columnCount = definitions.Count;
            for (int columnIndex = 0; columnIndex != columnCount; ++columnIndex)
            {
                var window = windows[columnIndex];
                var column = definitions[columnIndex];
                var columnName = column.ColumnName;
                var fittedValue = FitWidth(window, columnName);
                await writer.WriteAsync(fittedValue).ConfigureAwait(false);
            }
        }

        private string FitWidth(Window window, string? value)
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
            var policy = window.TruncationPolicy ?? Options.TruncationPolicy;
            switch (policy)
            {
                case OverflowTruncationPolicy.TruncateLeading:
                    int start = value.Length - window.Width;  // take characters on the end
                    return value.Substring(start, window.Width);
                case OverflowTruncationPolicy.TruncateTrailing:
                    return value.Substring(0, window.Width);
                case OverflowTruncationPolicy.ThrowException:
                    throw new FlatFileException(Resources.ValueExceedsWindowWidth);
                default:
                    throw new FlatFileException(Resources.InvalidTruncationPolicy);
            }            
        }

        private string GetPaddedValue(string value, Window window)
        {
            var alignment = window.Alignment ?? Options.Alignment;
            var fillCharacter = window.FillCharacter ?? Options.FillCharacter;
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
            if (Options.HasRecordSeparator)
            {
                var separator = Options.RecordSeparator ?? Environment.NewLine;
                writer.Write(separator);
            }
        }

        public async Task WriteRecordSeparatorAsync()
        {
            if (Options.HasRecordSeparator)
            {
                var separator = Options.RecordSeparator ?? Environment.NewLine;
                await writer.WriteAsync(separator).ConfigureAwait(false);
            }
        }

        public void WriteRaw(string data)
        {
            writer.Write(data);
        }

        public Task WriteRawAsync(string data)
        {
            return writer.WriteAsync(data);
        }
    }
}
