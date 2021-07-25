using System;
using System.Globalization;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing the record number metadata.
    /// </summary>
    public sealed class RecordNumberColumn : MetadataColumn<int>
    {
        private readonly Int32Column column;

        /// <summary>
        /// Initializes a new instance of a RecordNumberColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public RecordNumberColumn(string columnName)
            : base(columnName)
        {
            this.column = new Int32Column(columnName);
        }

        /// <summary>
        /// Gets or sets whether the header record, if present, should be included in the count.
        /// </summary>
        public bool IncludeSchema { get; set; }

        /// <summary>
        /// Gets or sets whether filtered records should be included in the count.
        /// </summary>
        public bool IncludeSkippedRecords { get; set; }

        /// <summary>
        /// Gets or sets the format provider to use when parsing.
        /// </summary>
        public IFormatProvider? FormatProvider 
        {
            get => column.FormatProvider;
            set => column.FormatProvider = value;
        }

        /// <summary>
        /// Gets or sets the number styles to use when parsing.
        /// </summary>
        public NumberStyles NumberStyles 
        {
            get => column.NumberStyles;
            set => column.NumberStyles = value;
        }

        /// <summary>
        /// Gets or sets the format string to use when converting the value to a string.
        /// </summary>
        public string? OutputFormat 
        {
            get => column.OutputFormat;
            set => column.OutputFormat = value;
        }

        /// <summary>
        /// Provides a textual representation for the value.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <returns>The formatted value.</returns>
        /// <exception cref="FlatFileException">This column requires column-level context but it has been disabled.</exception>
        protected override string OnFormat(IColumnContext? context)
        {
            if (context == null)
            {
                throw new FlatFileException(Resources.MetadataExpectingContext);
            }
            var recordNumber = GetRecordNumber(context);
            return column.Format(context, recordNumber);
        }

        /// <summary>
        /// Parses a textual representation of the value.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <returns>The parsed value.</returns>
        /// <exception cref="FlatFileException">This column requires column-level context but it has been disabled.</exception>
        protected override int OnParse(IColumnContext? context)
        {
            if (context == null)
            {
                throw new FlatFileException(Resources.MetadataExpectingContext);
            }
            var recordNumber = GetRecordNumber(context);
            return recordNumber;
        }

        private int GetRecordNumber(IColumnContext context)
        {
            var recordContext = context.RecordContext;
            var options = recordContext.ExecutionContext.Options;
            if (IncludeSkippedRecords)
            {
                int recordCount = recordContext.PhysicalRecordNumber;
                if (options.IsFirstRecordSchema && !IncludeSchema)
                {
                    --recordCount;
                }
                return recordCount;
            }

            // We only incrememnt the logical count after we are sure the record is not filtered out.
            // Since the value for the column is generated beforehand, we must increase it by one.
            int offset = (IncludeSchema && options.IsFirstRecordSchema) ? 2 : 1;
            return recordContext.LogicalRecordNumber + offset;
        }
    }
}
