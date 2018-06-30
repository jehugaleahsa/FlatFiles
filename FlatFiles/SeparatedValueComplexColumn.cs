using System;
using System.IO;

namespace FlatFiles
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a string column that has contains multiple, nested values
    /// </summary>
    public class SeparatedValueComplexColumn : ColumnDefinition
    {
        private readonly SeparatedValueSchema schema;

        /// <summary>
        /// Initializes a new SeparatedValueComplexColumn with no schema.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="options">The options to use when parsing the embedded data.</param>
        public SeparatedValueComplexColumn(string columnName, SeparatedValueOptions options = null) 
            : this(columnName, null, options, false)
        {
        }

        /// <summary>
        /// Initializes a new SeparatedValueComplexColumn with the given schema and options.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="schema">The schema of the data embedded in the column.</param>
        /// <param name="options">The options to use when parsing the embedded data.</param>
        public SeparatedValueComplexColumn(string columnName, SeparatedValueSchema schema, SeparatedValueOptions options = null)
            : this(columnName, schema, options, true)
        {
        }

        private SeparatedValueComplexColumn(string columnName, SeparatedValueSchema schema, SeparatedValueOptions options, bool hasSchema)
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
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType => typeof(object[]);

        /// <summary>
        /// Gets or sets the separated value options.
        /// </summary>
        public SeparatedValueOptions Options { get; set; }

        /// <summary>
        /// Extracts a single record from the embedded data.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value containing the embedded data.</param>
        /// <returns>
        /// An object array containing the values read from the embedded data -or- null if there is no embedded data.
        /// </returns>
        public override object Parse(IColumnContext context, string value)
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
            var reader = GetReader(stringReader);
            if (reader.Read())
            {
                return reader.GetValues();
            }
            return null;
        }

        private SeparatedValueReader GetReader(StringReader stringReader)
        {
            if (schema == null)
            {
                return new SeparatedValueReader(stringReader, Options);
            }

            return new SeparatedValueReader(stringReader, schema, Options);
        }

        /// <summary>
        /// Formats the given object array into an embedded record.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object array containing the values of the embedded record.</param>
        /// <returns>A formatted string containing the embedded data.</returns>
        public override string Format(IColumnContext context, object value)
        {
            var values = value as object[];
            if (values == null)
            {
                return NullHandler.GetNullRepresentation();
            }
            var writer = new StringWriter();
            var recordWriter = GetWriter(writer);
            recordWriter.WriteRecord(values);
            return writer.ToString();
        }

        private SeparatedValueRecordWriter GetWriter(StringWriter writer)
        {
            return new SeparatedValueRecordWriter(writer, schema, Options ?? new SeparatedValueOptions());
        }
    }
}
