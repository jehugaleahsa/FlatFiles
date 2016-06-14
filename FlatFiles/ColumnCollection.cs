using System;
using System.Collections;
using System.Collections.Generic;

namespace FlatFiles
{
    /// <summary>
    /// Holds the column definitions that make up a schema.
    /// </summary>
    public sealed class ColumnCollection : IEnumerable<ColumnDefinition>
    {
        private readonly List<ColumnDefinition> definitions;

        /// <summary>
        /// Initializes a new ColumnCollection.
        /// </summary>
        /// <param name="definitions">The column definitions making up the collection.</param>
        internal ColumnCollection(List<ColumnDefinition> definitions)
        {
            this.definitions = definitions;
        }

        /// <summary>
        /// Gets the column definition at the given index.
        /// </summary>
        /// <param name="index">The index of the column definition to get.</param>
        /// <returns>The column definition at the given index.</returns>
        public ColumnDefinition this[int index]
        {
            get { return definitions[index]; }
        }

        /// <summary>
        /// Gets the column definition with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column to get the definition for.</param>
        /// <returns>The column definition with the given name.</returns>
        public ColumnDefinition this[string columnName]
        {
            get 
            {
                Predicate<ColumnDefinition> predicate = (ColumnDefinition c) => StringComparer.CurrentCulture.Compare(c.ColumnName, columnName) == 0;
                int index = definitions.FindIndex(predicate);
                return definitions[index];
            }
        }

        /// <summary>
        /// Gets the number of columns in the collection.
        /// </summary>
        public int Count
        {
            get { return definitions.Count; }
        }

        /// <summary>
        /// Gets an enumerator over the column definitions in the collection.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<ColumnDefinition> GetEnumerator()
        {
            return definitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return definitions.GetEnumerator();
        }
    }
}
