using System;
using System.Collections.Concurrent;

namespace FlatFiles
{
    /// <summary>
    /// Defines the expected format of a record in a file.
    /// </summary>
    public sealed class DelimitedSchema : Schema
    {
        private static readonly ConcurrentDictionary<ValueTuple<int, bool>, DelimitedSchema> dynamicSchemas = new();

        /// <summary>
        /// Initializes a new instance of a Schema.
        /// </summary>
        public DelimitedSchema()
        {
        }

        internal static DelimitedSchema BuildDynamicSchema(DelimitedOptions options, int length)
        {
            return dynamicSchemas.GetOrAdd((length, options.PreserveWhiteSpace), (pair) =>
            {
                var schema = new DelimitedSchema();
                for (int columnIndex = 0; columnIndex != pair.Item1; ++columnIndex)
                {
                    var column = new StringColumn($"Column{columnIndex}")
                    {
                        Trim = !pair.Item2
                    };
                    schema.AddColumn(column);
                }
                return schema;
            });
        }

        /// <summary>
        /// Adds a column to the schema, using the given definition to define it.
        /// </summary>
        /// <param name="definition">The definition of the column to add.</param>
        /// <returns>The current schema.</returns>
        public DelimitedSchema AddColumn(IColumnDefinition definition)
        {
            AddColumnBase(definition);
            return this;
        }
    }
}
