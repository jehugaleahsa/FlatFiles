using System;
using System.Reflection;

namespace FlatFiles
{
    /// <summary>
    /// Holds the information needed to build a schema.
    /// </summary>
    internal sealed class CustomColumnConfiguration
    {
        /// <summary>
        /// Gets or sets the property being mapped.
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// Gets or sets the name of the column being mapped.
        /// </summary>
        public string ColumnName { get; set; }
    }
}
