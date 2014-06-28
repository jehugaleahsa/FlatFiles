using System;
using System.Collections.Generic;
using System.Data;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Defines the expected format of a record in a file.
    /// </summary>
    public class ExcelSchema : ISchema
    {
        private readonly List<ColumnDefinition> definitions;
        private readonly Dictionary<string, int> ordinals;

        /// <summary>
        /// Initializes a new instance of an ExcelSchema.
        /// </summary>
        public ExcelSchema()
        {
            definitions = new List<ColumnDefinition>();
            ordinals = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <param name="definition">The definition of the column to add.</param>
        /// <returns>The current schema.</returns>
        public ExcelSchema AddColumn(ColumnDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }
            if (ordinals.ContainsKey(definition.ColumnName))
            {
                throw new ArgumentException(Resources.DuplicateColumnName, "definition");
            }
            addColumn(definition);
            return this;
        }

        private void addColumn(ColumnDefinition definition)
        {
            definitions.Add(definition);
            ordinals.Add(definition.ColumnName, definitions.Count - 1);
        }

        /// <summary>
        /// Gets the column definitions that make up the schema.
        /// </summary>
        public ColumnCollection ColumnDefinitions
        {
            get { return new ColumnCollection(definitions); }
        }

        /// <summary>
        /// Gets the index of the column with the given name.
        /// </summary>
        /// <param name="name">The name of the column to get the index for.</param>
        /// <returns>The index of the column with the given name.</returns>
        /// <exception cref="System.IndexOutOfRangeException">There is not a column with the given name.</exception>
        public int GetOrdinal(string name)
        {
            if (!ordinals.ContainsKey(name))
            {
                throw new IndexOutOfRangeException();
            }
            return ordinals[name];
        }

        /// <summary>
        /// Parses the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="record">The record containing the Excel worksheet row data.</param>
        /// <returns>The parsed objects.</returns>
        internal object[] ParseValues(IDataRecord record)
        {
            object[] parsed = new object[definitions.Count];
            for (int index = 0; index != parsed.Length; ++index)
            {
                ColumnDefinition definition = definitions[index];
                object rawValue = record.FieldCount <= index || record.IsDBNull(index) ? null : record.GetValue(index);
                parsed[index] = parseValue(definition, rawValue);
            }
            return parsed;
        }

        private static object parseValue(ColumnDefinition definition, object value)
        {
            // Let the definition interpret nulls.
            if (value == null)
            {
                return definition.Parse(null);
            }
            // If the value is a string, give the parser a chance to interpret it.
            string asString = value as String;
            if (asString != null)
            {
                return definition.Parse(asString);
            }
            // If the value's type differs from the expected type, 
            // convert it to a string and try to interpret it.
            if (value.GetType() != definition.ColumnType)
            {
                return Convert.ChangeType(value, definition.ColumnType);
            }
            // Otherwise, the type of the value matches the expected type.
            return value;
        }
    }
}
