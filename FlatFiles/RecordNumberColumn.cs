using System;
using System.Globalization;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing the record number metadata.
    /// </summary>
    public class RecordNumberColumn : MetadataColumn<int>
    {
        /// <summary>
        /// Initializes a new instance of a RecordNumberColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public RecordNumberColumn(string columnName)
            : base(columnName)
        {
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
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Gets or sets the number styles to use when parsing.
        /// </summary>
        public NumberStyles NumberStyles { get; set; } = NumberStyles.Integer;

        /// <summary>
        /// Gets or sets the format string to use when converting the value to a string.
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// Provides a textual representation for the value.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext context)
        {
            var recordNumber = GetRecordNumber(context);
            var provider = GetFormatProvider(context, FormatProvider);
            if (OutputFormat == null)
            {
                return recordNumber.ToString(provider);
            }
            return recordNumber.ToString(OutputFormat, provider);
        }

        /// <summary>
        /// Parses a textual representation of the value.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <returns>The parsed value.</returns>
        protected override int OnParse(IColumnContext context)
        {
            var recordNumber = GetRecordNumber(context);
            return recordNumber;
        }

        private int GetRecordNumber(IColumnContext context)
        {
            if (IncludeSkippedRecords)
            {
                int recordCount = context.RecordContext.PhysicalRecordNumber;
                if (context.RecordContext.ExecutionContext.Options.IsFirstRecordSchema && !IncludeSchema)
                {
                    --recordCount;
                }
                return recordCount;
            }

            // We only incrememnt the logical count after we are sure the record is not filtered out.
            // Since the value for the column is generated beforehand, we must increase it by one.
            int offset = (IncludeSchema && context.RecordContext.ExecutionContext.Options.IsFirstRecordSchema) ? 2 : 1;
            return context.RecordContext.LogicalRecordNumber + offset;
        }
    }
}
