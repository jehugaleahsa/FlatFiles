using System;
using System.IO;

namespace FlatFiles
{
    /// <summary>
    /// Represents a string column that has contains multiple, nested values
    /// </summary>
    public sealed class FixedLengthComplexColumn : ColumnDefinition<object?[]?>
    {
        private readonly FixedLengthSchema schema;

        /// <summary>
        /// Initializes a new FixedLengthComplexColumn with the given schema and options.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="schema">The schema of the data embedded in the column.</param>
        /// <param name="options">The options to use when parsing the embedded data.</param>
        public FixedLengthComplexColumn(string columnName, FixedLengthSchema schema, FixedLengthOptions? options = null)
            : base(columnName)
        {
            this.schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Options = options;
        }

        /// <summary>
        /// Gets or sets the options used to read/write the records.
        /// </summary>
        public FixedLengthOptions? Options { get; set; }

        /// <summary>
        /// Extracts a single record from the embedded data.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value containing the embedded data.</param>
        /// <returns>
        /// An object array containing the values read from the embedded data -or- null if there is no embedded data.
        /// </returns>
        protected override object?[]? OnParse(IColumnContext? context, string value)
        {
            var stringReader = new StringReader(value);
            var reader = new FixedLengthReader(stringReader, schema, Options);
            if (reader.Read())
            {
                return reader.GetValues();
            }
            return null;
        }

        /// <summary>
        /// Formats the given object array into an embedded record.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="values">The object array containing the values of the embedded record.</param>
        /// <returns>A formatted string containing the embedded data.</returns>
        protected override string OnFormat(IColumnContext? context, object?[]? values)
        {
            if (values == null)
            {
                return string.Empty;
            }
            var writer = new StringWriter();
            var recordWriter = new FixedLengthRecordWriter(writer, schema, Options);
            recordWriter.WriteRecord(values);
            return writer.ToString();
        }
    }
}
