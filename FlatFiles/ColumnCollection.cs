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
        private readonly List<IColumnDefinition> definitions = new List<IColumnDefinition>();
        private readonly Dictionary<string, int> ordinals = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new ColumnCollection.
        /// </summary>
        internal ColumnCollection()
        {
        }

        /// <summary>
        /// Gets the column definition at the given index.
        /// </summary>
        /// <param name="index">The index of the column definition to get.</param>
        /// <returns>The column definition at the given index.</returns>
        public IColumnDefinition this[int index] => definitions[index];

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
        public int Count => definitions.Count;

        /// <summary>
        /// Gets the number of columns that are ignored.
        /// </summary>
        public int IgnoredCount { get; private set; }

        /// <summary>
        /// Gets the number of columns that are metadata.
        /// </summary>
        public int MetadataCount { get; private set; }

        /// <summary>
        /// Gets the number of columns that are not ignored.
        /// </summary>
        internal int PhysicalCount => definitions.Count - IgnoredCount;

        internal void AddColumn(IColumnDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }
            if (!String.IsNullOrEmpty(definition.ColumnName) && ordinals.ContainsKey(definition.ColumnName))
            {
                throw new ArgumentException(Resources.DuplicateColumnName, nameof(definition));
            }
            addColumn(definition);
        }

        private void addColumn(IColumnDefinition definition)
        {
            definitions.Add(definition);
            if (definition is IMetadataColumn)
            {
                ++MetadataCount;
            }
            else if (definition.IsIgnored)
            {
                ++IgnoredCount;
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
            if (ordinals.TryGetValue(columnName, out int ordinal))
            {
                return ordinal;
            }
            else
            {
                return -1;
            }
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

        internal int GetPhysicalIndex(IColumnDefinition definition)
        {
            return definitions.IndexOf(definition);
        }

        internal int GetLogicalIndex(IColumnDefinition definition)
        {
            for (int index = 0, logicalIndex = 0; index != definitions.Count; ++index)
            {
                var current = definitions[index];
                if (current == definition)
                {
                    return logicalIndex;
                }
                if (!current.IsIgnored)
                {
                    ++logicalIndex;
                }
            }
            return -1;
        }
    }
}
