using System;
using System.Collections.Generic;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Defines the expected format of a record in a file.
    /// </summary>
    public sealed class SeparatedValueSchema : ISchema
    {
        private readonly List<ColumnDefinition> definitions;
        private readonly Dictionary<string, int> ordinals;

        /// <summary>
        /// Initializes a new instance of a Schema.
        /// </summary>
        public SeparatedValueSchema()
        {
            definitions = new List<ColumnDefinition>();
            ordinals = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <param name="definition">The definition of the column to add.</param>
        /// <returns>The current schema.</returns>
        public SeparatedValueSchema AddColumn(ColumnDefinition definition)
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
        /// Gets the column definitions that make up the schema.
        /// </summary>
        public ColumnCollection ColumnDefinitions
        {
            get { return new ColumnCollection(definitions); }
        }

        /// <summary>
        /// Parses the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="values">The values to parse.</param>
        /// <returns>The parsed objects.</returns>
        internal object[] ParseValues(string[] values)
        {
            object[] parsed = new object[values.Length];
            for (int index = 0; index != values.Length; ++index)
            {
                ColumnDefinition definition = definitions[index];
                parsed[index] = definition.Parse(values[index]);
            }
            return parsed;
        }

        /// <summary>
        /// Formats the given values assuming that they are in the same order as the column definitions.
        /// </summary>
        /// <param name="values">The values to format.</param>
        /// <returns>The formatted values.</returns>
        internal string[] FormatValues(object[] values)
        {
            string[] formatted = new string[values.Length];
            for (int index = 0; index != values.Length; ++index)
            {
                ColumnDefinition definition = definitions[index];
                formatted[index] = definition.Format(values[index]);
            }
            return formatted;
        }
    }
}
