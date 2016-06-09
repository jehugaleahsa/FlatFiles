using System;
using System.IO;
using System.Text;

namespace FlatFiles
{
    /// <summary>
    /// Represents a string column that has contains multiple, nested values
    /// </summary>
    public class SeparatedValueComplexColumn : ColumnDefinition
    {
        private readonly SeparatedValueSchema schema;
        private SeparatedValueOptions options;

        /// <summary>
        /// Initializes a new SeparatedValueComplexColumn with no schema or options.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public SeparatedValueComplexColumn(string columnName) 
            : this(columnName, null, new SeparatedValueOptions(), false)
        {
        }

        /// <summary>
        /// Initializes a new SeparatedValueComplexColumn with the given schema and default options.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="schema">The schema of the data embedded in the column.</param>
        public SeparatedValueComplexColumn(string columnName, SeparatedValueSchema schema)
            : this(columnName, schema, new SeparatedValueOptions(), true)
        {
        }

        /// <summary>
        /// Initializes a new SeparatedValueComplexColumn with the given options.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="options">The options to use when parsing the embedded data.</param>
        public SeparatedValueComplexColumn(string columnName, SeparatedValueOptions options)
            : this(columnName, null, options, false)
        {
        }

        /// <summary>
        /// Initializes a new SeparatedValueComplexColumn with the given schema and options.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="schema">The schema of the data embedded in the column.</param>
        /// <param name="options">The options to use when parsing the embedded data.</param>
        public SeparatedValueComplexColumn(string columnName, SeparatedValueSchema schema, SeparatedValueOptions options)
            : this(columnName, schema, options, true)
        {
        }

        private SeparatedValueComplexColumn(string columnName, SeparatedValueSchema schema, SeparatedValueOptions options, bool hasSchema)
            : base(columnName)
        {
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            this.schema = schema;
            this.options = options;
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType
        {
            get { return typeof(object[]); }
        }

        /// <summary>
        /// Gets or sets the separated value options.
        /// </summary>
        public SeparatedValueOptions Options
        {
            get { return options; }
            set { options = value ?? new SeparatedValueOptions(); }
        }

        /// <summary>
        /// Extracts a single record from the embedded data.
        /// </summary>
        /// <param name="value">The value containing the embedded data.</param>
        /// <returns>
        /// An object array containing the values read from the embedded data -or- null if there is no embedded data.
        /// </returns>
        public override object Parse(string value)
        {
            if (NullHandler.IsNullRepresentation(value))
            {
                return null;
            }

            StringReader stringReader = new StringReader(value);
            SeparatedValueReader reader = getReader(stringReader);
            if (reader.Read())
            {
                return reader.GetValues();
            }
            return null;
        }

        private SeparatedValueReader getReader(StringReader stringReader)
        {
            if (schema == null)
            {
                return new SeparatedValueReader(stringReader, options);
            }
            else
            {
                return new SeparatedValueReader(stringReader, schema, options);
            }
        }

        /// <summary>
        /// Formats the given object array into an embedded record.
        /// </summary>
        /// <param name="value">The object array containing the values of the embedded record.</param>
        /// <returns>A formatted string containing the embedded data.</returns>
        public override string Format(object value)
        {
            object[] values = value as object[];
            if (values == null)
            {
                return NullHandler.GetNullRepresentation();
            }
            StringWriter writer = new StringWriter();
            SeparatedValueRecordWriter recordWriter = new SeparatedValueRecordWriter(writer, schema, options);
            recordWriter.WriteRecord(values);
            return writer.ToString();
        }
    }
}
