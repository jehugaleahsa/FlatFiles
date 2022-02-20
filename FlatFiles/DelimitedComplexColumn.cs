using System;
using System.IO;

namespace FlatFiles
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a string column that has contains multiple, nested values
    /// </summary>
    public sealed class DelimitedComplexColumn : ColumnDefinition<object?[]?>
    {
        private readonly DelimitedSchema? schema;

        /// <summary>
        /// Initializes a new DelimitedComplexColumn with no schema.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="options">The options to use when parsing the embedded data.</param>
        public DelimitedComplexColumn(string columnName, DelimitedOptions? options = null) 
            : this(columnName, null, options, false)
        {
        }

        /// <summary>
        /// Initializes a new DelimitedComplexColumn with the given schema and options.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="schema">The schema of the data embedded in the column.</param>
        /// <param name="options">The options to use when parsing the embedded data.</param>
        public DelimitedComplexColumn(string columnName, DelimitedSchema schema, DelimitedOptions? options = null)
            : this(columnName, schema, options, true)
        {
        }

        private DelimitedComplexColumn(string columnName, DelimitedSchema? schema, DelimitedOptions? options, bool hasSchema)
            : base(columnName)
        {
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }
            this.schema = schema;
            Options = options;
        }

        /// <summary>
        /// Gets or sets the delimited options.
        /// </summary>
        public DelimitedOptions? Options { get; set; }

        /// <inheritdoc/>
        public override bool IsComplex { get; } = true;

        /// <inheritdoc/>
        protected override bool IsTrimmed => false;

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
            var reader = GetReader(stringReader);
            if (reader.Read())
            {
                return reader.GetValues();
            }
            return null;
        }

        private DelimitedReader GetReader(StringReader stringReader)
        {
            if (schema == null)
            {
                return new DelimitedReader(stringReader, Options);
            }
            return new DelimitedReader(stringReader, schema, Options);
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
                return String.Empty;
            }
            var writer = new StringWriter();
            var recordWriter = GetWriter(writer);
            recordWriter.WriteRecord(values);
            return writer.ToString();
        }

        private DelimitedRecordWriter GetWriter(StringWriter writer)
        {
            return new DelimitedRecordWriter(writer, schema, Options);
        }
    }
}
