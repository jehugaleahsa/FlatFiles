using System;
using System.Collections;
using System.Collections.Generic;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Holds the column definitions that make up a schema.
    /// </summary>
    public sealed class ColumnCollection : IEnumerable<IColumnDefinition>
    {
        private readonly List<IColumnDefinition> definitions;
        private readonly Dictionary<string, int> ordinals;
        private int ignoredCount;

        /// <summary>
        /// Initializes a new ColumnCollection.
        /// </summary>
        internal ColumnCollection()
        {
            definitions = new List<IColumnDefinition>();
            ordinals = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Gets the column definition at the given index.
        /// </summary>
        /// <param name="index">The index of the column definition to get.</param>
        /// <returns>The column definition at the given index.</returns>
        public IColumnDefinition this[int index]
        {
            get { return definitions[index]; }
        }

        /// <summary>
        /// Gets the column definition with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column to get the definition for.</param>
        /// <returns>The column definition with the given name.</returns>
        public IColumnDefinition this[string columnName]
        {
            get 
            {
                int index = ordinals.ContainsKey(columnName) ? ordinals[columnName] : -1;
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
        /// Gets the number of columns that are ignored.
        /// </summary>
        public int IgnoredCount
        {
            get { return ignoredCount; }
        }

        /// <summary>
        /// Gets the number of columns that are not ignored.
        /// </summary>
        public int HandledCount
        {
            get { return definitions.Count - ignoredCount; }
        }

        internal void AddColumn(IColumnDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }
            if (!String.IsNullOrEmpty(definition.ColumnName) && ordinals.ContainsKey(definition.ColumnName))
            {
                throw new ArgumentException(Resources.DuplicateColumnName, "definition");
            }
            addColumn(definition);
        }

        private void addColumn(IColumnDefinition definition)
        {
            definitions.Add(definition);
            if (definition.IsIgnored)
            {
                ++ignoredCount;
            }
            if (!String.IsNullOrEmpty(definition.ColumnName))
            {
                ordinals.Add(definition.ColumnName, definitions.Count - 1);
            }
        }

        /// <summary>
        /// Gets the index of the column with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column to get the index for.</param>
        /// <returns>The index of the column with the given name -or- -1 if the column is not found.</returns>
        public int GetOrdinal(string columnName)
        {
            if (!ordinals.ContainsKey(columnName))
            {
                return -1;
            }
            return ordinals[columnName];
        }

        /// <summary>
        /// Gets an enumerator over the column definitions in the collection.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<IColumnDefinition> GetEnumerator()
        {
            return definitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return definitions.GetEnumerator();
        }
    }
}
