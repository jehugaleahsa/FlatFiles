using System;
using System.Collections.Generic;
using System.Linq;
using FlatFileReaders.Properties;

namespace FlatFileReaders
{
    /// <summary>
    /// Defines the expected format of a record in a file.
    /// </summary>
    public sealed class Schema
    {
        private List<ColumnDefinition> definitions;

        /// <summary>
        /// Initializes a new instance of a Schema.
        /// </summary>
        public Schema()
        {
            definitions = new List<ColumnDefinition>();
        }

        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <param name="definition">The definition of the column to add.</param>
        /// <returns>The current schema.</returns>
        public Schema AddColumn(ColumnDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }
            if (definitions.Any(existing => existing.ColumnName == definition.ColumnName))
            {
                throw new ArgumentException(Resources.DuplicateColumnName, "definition");
            }
            definitions.Add(definition);
            return this;
        }

        /// <summary>
        /// Gets the column definitions that make up the schema.
        /// </summary>
        public ColumnCollection ColumnDefinitions
        {
            get { return new ColumnCollection(definitions); }
        }

        /// <summary>
        /// Parses the given values assuming that the are in the same order as the column definitions.
        /// </summary>
        /// <param name="values">The values to parse.</param>
        /// <returns>The parsed objects.</returns>
        internal object[] ParseValues(string[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length != definitions.Count)
            {
                throw new ArgumentException(Resources.WrongNumberOfValues, "values");
            }
            object[] parsed = new object[values.Length];
            for (int index = 0; index != values.Length; ++index)
            {
                ColumnDefinition definition = definitions[index];
                parsed[index] = definition.Parse(values[index]);
            }
            return parsed;
        }
    }
}
