using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Adds a column to the schema with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the column.</typeparam>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The current schema.</returns>
        public SeparatedValueSchema AddColumn<T>(string columnName)
        {
            if (String.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentException(Resources.BlankColumnName, "columnName");
            }
            if (ordinals.ContainsKey(columnName))
            {
                throw new ArgumentException(Resources.DuplicateColumnName, "definition");
            }
            ColumnDefinition definition = getColumnDefinition<T>(columnName);
            addColumn(definition);
            return this;
        }

        /// <summary>
        /// Adds a column to the schema with the specified type, using the
        /// given converter to convert parsed string values into their
        /// appropriate type.
        /// </summary>
        /// <typeparam name="T">The type of the column.</typeparam>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="parser">A function that converts the parsed string value to the appropriate type.</param>
        /// <param name="formatter">A function that converts a value into a string.</param>
        /// <returns>The current schema.</returns>
        public SeparatedValueSchema AddColumn<T>(string columnName, Func<string, T> parser, Func<T, string> formatter)
        {
            if (String.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentException(Resources.BlankColumnName, "columnName");
            }
            if (ordinals.ContainsKey(columnName))
            {
                throw new ArgumentException(Resources.DuplicateColumnName, "definition");
            }
            CustomColumn column = new CustomColumn(columnName, typeof(T), s => parser(s), t => formatter((T)t));
            addColumn(column);
            return this;
        }

        private void addColumn(ColumnDefinition definition)
        {
            definitions.Add(definition);
            ordinals.Add(definition.ColumnName, definitions.Count - 1);
        }

        private static readonly Dictionary<Type, Func<string, ColumnDefinition>> converters = new Dictionary<Type, Func<string, ColumnDefinition>>()
        {
            { typeof(Boolean), columnName => new BooleanColumn(columnName) },
            { typeof(byte[]), columnName => new ByteArrayColumn(columnName) },
            { typeof(Byte), columnName => new ByteColumn(columnName) },
            { typeof(char[]), columnName => new CharArrayColumn(columnName) },
            { typeof(Char), columnName => new CharColumn(columnName) },
            { typeof(DateTime), columnName => new DateTimeColumn(columnName) },
            { typeof(Decimal), columnName => new DecimalColumn(columnName) },
            { typeof(Double), columnName => new DoubleColumn(columnName) },
            { typeof(Guid), columnName => new GuidColumn(columnName) },
            { typeof(Int16), columnName => new Int16Column(columnName) },
            { typeof(Int32), columnName => new Int32Column(columnName) },
            { typeof(Int64), columnName => new Int64Column(columnName) },
            { typeof(Single), columnName => new SingleColumn(columnName) },
            { typeof(String), columnName => new StringColumn(columnName) }
        };

        private static ColumnDefinition getColumnDefinition<T>(string columnName)
        {
            if (converters.ContainsKey(typeof(T)))
            {
                Func<string, ColumnDefinition> factory = converters[typeof(T)];
                return factory(columnName);
            }
            else
            {
                return new CustomColumn(columnName, typeof(T), null, null);
            }            
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
