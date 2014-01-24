using System;

namespace FlatFiles
{
    /// <summary>
    /// Allows a property to be mapped to a column.
    /// </summary>
    /// <typeparam name="TType">The type being mapped.</typeparam>
    /// <typeparam name="TProp">The type of the column being mapped.</typeparam>
    public interface IPropertyMapping<TType, TProp>
    {
        /// <summary>
        /// Maps the property to a column with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column to map to the property.</param>
        /// <returns>An object that will allow a custom converter to be provided.</returns>
        Mapper<TType> To(string columnName);
    }
}
