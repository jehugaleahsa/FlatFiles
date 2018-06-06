using System;
using System.Globalization;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing the record number metadata.
    /// </summary>
    public class RecordNumberColumn : ColumnDefinition<int>, IMetadataColumn
    {
        /// <summary>
        /// Initializes a new instance of a RecordNumberColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public RecordNumberColumn(string columnName)
            : base(columnName)
        {
            NumberStyles = NumberStyles.Integer;
        }

        /// <summary>
        /// Gets or sets whether the header record, if present, should be included in the count.
        /// </summary>
        public bool IncludeSchema { get; set; }

        /// <summary>
        /// Gets or sets whether filtered records should be included in the count.
        /// </summary>
        public bool IncludeFilteredRecords { get; set; }

        /// <summary>
        /// Gets or sets the format provider to use when parsing.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Gets or sets the number styles to use when parsing.
        /// </summary>
        public NumberStyles NumberStyles { get; set; }

        /// <summary>
        /// Gets or sets the format string to use when converting the value to a string.
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// Gets the number of records that have been parsed.
        /// </summary>
        /// <param name="metadata">The current metadata of the process.</param>
        /// <returns>The number of parsed records.</returns>
        object IMetadataColumn.GetValue(IProcessMetadata metadata)
        {
            if (IncludeFilteredRecords)
            {
                int recordCount = metadata.RecordCount;
                if (metadata.Options.IsFirstRecordSchema && !IncludeSchema)
                {
                    --recordCount;
                }
                return recordCount;
            }
            else
            {
                // We only incrememnt the logical count after we are sure the record is not filtered out.
                // Since the value for the column is generated beforehand, we must increase it by one.
                int offset = (IncludeSchema && metadata.Options.IsFirstRecordSchema) ? 2 : 1;
                return metadata.LogicalRecordCount + offset;
            }
        }

        /// <summary>
        /// Provides a textual representation for the value.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(int value)
        {
            if (OutputFormat == null)
            {
                return value.ToString(FormatProvider ?? CultureInfo.CurrentCulture);
            }
            else
            {
                return value.ToString(OutputFormat, FormatProvider ?? CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Parses a textual representation of the value.
        /// </summary>
        /// <param name="value">The text to parse.</param>
        /// <returns>The parsed value.</returns>
        protected override int OnParse(string value)
        {
            IFormatProvider provider = FormatProvider ?? CultureInfo.CurrentCulture;
            return Int32.Parse(value, NumberStyles, provider);
        }
    }
}
