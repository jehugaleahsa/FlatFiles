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
        private readonly List<IColumnDefinition> _definitions;
        private readonly Dictionary<string, int> _ordinals;

        /// <summary>
        /// Initializes a new ColumnCollection.
        /// </summary>
        internal ColumnCollection()
        {
            _definitions = new List<IColumnDefinition>();
            _ordinals = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Gets the column definition at the given index.
        /// </summary>
        /// <param name="index">The index of the column definition to get.</param>
        /// <returns>The column definition at the given index.</returns>
        public IColumnDefinition this[int index] => _definitions[index];

        /// <summary>
        /// Gets the column definition with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column to get the definition for.</param>
        /// <returns>The column definition with the given name.</returns>
        public IColumnDefinition this[string columnName]
        {
            get 
            {
                int index = _ordinals.ContainsKey(columnName) ? _ordinals[columnName] : -1;
                return _definitions[index];
            }
        }

        /// <summary>
        /// Gets the number of columns in the collection.
        /// </summary>
        public int Count => _definitions.Count;

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
        internal int PhysicalCount => _definitions.Count - IgnoredCount;

        internal void AddColumn(IColumnDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }
            if (!string.IsNullOrEmpty(definition.ColumnName) && _ordinals.ContainsKey(definition.ColumnName))
            {
                throw new ArgumentException(Resources.DuplicateColumnName, nameof(definition));
            }
            addColumn(definition);
        }

        private void addColumn(IColumnDefinition definition)
        {
            _definitions.Add(definition);
            if (definition is IMetadataColumn)
            {
                ++MetadataCount;
            }
            else if (definition.IsIgnored)
            {
                ++IgnoredCount;
            }
            if (!string.IsNullOrEmpty(definition.ColumnName))
            {
                _ordinals.Add(definition.ColumnName, _definitions.Count - 1);
            }
        }

        /// <summary>
        /// Gets the index of the column with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column to get the index for.</param>
        /// <returns>The index of the column with the given name -or- -1 if the column is not found.</returns>
        public int GetOrdinal(string columnName)
        {
            if (!_ordinals.ContainsKey(columnName))
            {
                return -1;
            }
            return _ordinals[columnName];
        }

        /// <summary>
        /// Gets an enumerator over the column definitions in the collection.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<IColumnDefinition> GetEnumerator()
        {
            return _definitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _definitions.GetEnumerator();
        }
    }
}
