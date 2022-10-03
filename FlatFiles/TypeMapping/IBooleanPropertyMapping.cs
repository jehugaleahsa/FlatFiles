using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a Boolean column.
    /// </summary>
    public interface IBooleanPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IBooleanPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the value used to represent true.
        /// </summary>
        /// <param name="value">The value used to represent true.</param>
        /// <returns>The property mapping for futher configuration.</returns>
        IBooleanPropertyMapping TrueString(string? value);

        /// <summary>
        /// Sets the value used to represent false.
        /// </summary>
        /// <param name="value">The value used to represent false.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IBooleanPropertyMapping FalseString(string? value);

        /// <summary>
        /// Sets what value(s) are treated as null.
        /// </summary>
        /// <param name="formatter">The formatter to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Passing null will cause the default formatter to be used.</remarks>
        IBooleanPropertyMapping NullFormatter(INullFormatter formatter);

        /// <summary>
        /// Sets the default value to use when a null is encountered on a non-null property.
        /// </summary>
        /// <param name="defaultValue">The default value to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Passing null will cause an exception to be thrown for unexpected nulls.</remarks>
        IBooleanPropertyMapping DefaultValue(IDefaultValue defaultValue);

        /// <summary>
        /// Sets whether the column is nullable.
        /// </summary>
        /// <param name="isNullable">Whether to set the column nullable or not.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IBooleanPropertyMapping Nullable(bool isNullable);

        /// <summary>
        /// Sets a function to preprocess in the input before parsing it.
        /// </summary>
        /// <param name="preprocessor">A preprocessor function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        [Obsolete("This function has been superseded by the OnParsing function.")]
        IBooleanPropertyMapping Preprocessor(Func<string, string?>? preprocessor);

        /// <summary>
        /// Sets the function to run before the input is parsed.
        /// </summary>
        /// <param name="handler">A function to call before the textual value is parsed.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IBooleanPropertyMapping OnParsing(Func<IColumnContext?, string, string?>? handler);

        /// <summary>
        /// Sets the function to run after the input is parsed.
        /// </summary>
        /// <param name="handler">A function to call after the value is parsed.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IBooleanPropertyMapping OnParsed(Func<IColumnContext?, object?, object?>? handler);

        /// <summary>
        /// Sets the function to run before the output is formatted as a string.
        /// </summary>1
        /// <param name="handler">A function to call before the value is formatted as a string.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IBooleanPropertyMapping OnFormatting(Func<IColumnContext?, object?, object?>? handler);

        /// <summary>
        /// Sets the function to run after the output is formatted as a string.
        /// </summary>
        /// <param name="handler">A function to call after the value is formatted as a string.</param>
        /// <returns>The property mapping for further configuration.</returns>
        IBooleanPropertyMapping OnFormatted(Func<IColumnContext?, string, string?>? handler);
    }
}
