using System;

namespace FlatFiles
{
    /// <summary>
    /// Defines a column that is part of a record schema.
    /// </summary>
    public interface IColumnDefinition
    {
        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        string? ColumnName { get; }

        /// <summary>
        /// Gets whether the value in this column is returned as a result.
        /// </summary>
        bool IsIgnored { get; }

        /// <summary>
        /// Gets whether nulls are allowed for the column.
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// Gets whether the column contains a nested object structure.
        /// </summary>
        bool IsComplex { get; }

        /// <summary>
        /// Gets or sets the default value to use when a null is encountered on a non-nullable column.
        /// </summary>
        IDefaultValue DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the null formatter instance used to read/write null values.
        /// </summary>
        INullFormatter NullFormatter { get; set; }

        /// <summary>
        /// Gets or sets a function used to preprocess input before trying to parse it.
        /// </summary>
        [Obsolete("This property has been superseded by the OnParsing delegate.")]
        Func<string, string?>? Preprocessor { get; set; }

        /// <summary>
        /// Gets or sets a function used to pre-process input before trying to parse it.
        /// </summary>
        Func<IColumnContext?, string, string?>? OnParsing { get; set; }

        /// <summary>
        /// Gets or sets a function used to post-process input after parsing it.
        /// </summary>
        Func<IColumnContext?, object?, object?>? OnParsed { get; set; }

        /// <summary>
        /// Gets or sets a function used to pre-process output before trying to format it.
        /// </summary>
        Func<IColumnContext?, object?, object?>? OnFormatting { get; set; }

        /// <summary>
        /// Gets or sets a function used to post-process output after formatting it.
        /// </summary>
        Func<IColumnContext?, string, string?>? OnFormatted { get; set; }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        Type ColumnType { get; }

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        object? Parse(IColumnContext? context, string value);

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        string Format(IColumnContext? context, object? value);
    }
}
