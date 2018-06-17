using System;
using System.IO;

namespace FlatFiles
{
    /// <summary>
    /// Represents a string column that has contains multiple, nested values
    /// </summary>
    public class FixedLengthComplexColumn : ColumnDefinition
    {
        private readonly FixedLengthSchema schema;

        /// <summary>
        /// Initializes a new FixedLengthComplexColumn with the given schema and options.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="schema">The schema of the data embedded in the column.</param>
        /// <param name="options">The options to use when parsing the embedded data.</param>
        public FixedLengthComplexColumn(string columnName, FixedLengthSchema schema, FixedLengthOptions options = null)
            : base(columnName)
        {
            this.schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Options = options;
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType => typeof(object[]);

        /// <summary>
        /// Gets or sets the options used to read/write the records.
        /// </summary>
        public FixedLengthOptions Options { get; set; }

        /// <summary>
        /// Extracts a single record from the embedded data.
        /// </summary>
        /// <param name="value">The value containing the embedded data.</param>
        /// <returns>
        /// An object array containing the values read from the embedded data -or- null if there is no embedded data.
        /// </returns>
        public override object Parse(string value)
        {
            if (Preprocessor != null)
            {
                value = Preprocessor(value);
            }
            if (NullHandler.IsNullRepresentation(value))
            {
                return null;
            }

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
        /// <param name="value">The object array containing the values of the embedded record.</param>
        /// <returns>A formatted string containing the embedded data.</returns>
        public override string Format(object value)
        {
            var values = value as object[];
            if (values == null)
            {
                return NullHandler.GetNullRepresentation();
            }
            var writer = new StringWriter();
            var recordWriter = new FixedLengthRecordWriter(writer, schema, Options ?? new FixedLengthOptions());
            recordWriter.WriteRecord(values);
            return writer.ToString();
        }
    }
}
