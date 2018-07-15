using System;

namespace FlatFiles.TypeMapping
{
    /// <summary>
    /// Represents the mapping from a type property to a TimeSpan column.
    /// </summary>
    public interface ITimeSpanPropertyMapping
    {
        /// <summary>
        /// Sets the name of the column in the input or output file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ITimeSpanPropertyMapping ColumnName(string name);

        /// <summary>
        /// Sets the date/time format the input is expected to be in.
        /// </summary>
        /// <param name="format">The format to expect.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ITimeSpanPropertyMapping InputFormat(string format);

        /// <summary>
        /// Sets the date/time format to use for output.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ITimeSpanPropertyMapping OutputFormat(string format);

        /// <summary>
        /// Sets the format provider to use when reading and writing date/times.
        /// </summary>
        /// <param name="provider">The provider to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ITimeSpanPropertyMapping FormatProvider(IFormatProvider provider);

        /// <summary>
        /// Sets what value(s) are treated as null.
        /// </summary>
        /// <param name="formatter">The formatter to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Passing null will cause the default formatter to be used.</remarks>
        ITimeSpanPropertyMapping NullFormatter(INullFormatter formatter);

        /// <summary>
        /// Sets the default value to use when a null is encountered on a non-null property.
        /// </summary>
        /// <param name="defaultValue">The default value to use.</param>
        /// <returns>The property mapping for further configuration.</returns>
        /// <remarks>Passing null will cause an exception to be thrown for unexpected nulls.</remarks>
        ITimeSpanPropertyMapping DefaultValue(IDefaultValue defaultValue);

        /// <summary>
        /// Sets a function to preprocess in the input before parsing it.
        /// </summary>
        /// <param name="preprocessor">A preprocessor function.</param>
        /// <returns>The property mapping for further configuration.</returns>
        ITimeSpanPropertyMapping Preprocessor(Func<string, string> preprocessor);
    }

    internal sealed class TimeSpanPropertyMapping : ITimeSpanPropertyMapping, IMemberMapping
    {
        private readonly TimeSpanColumn column;

        public TimeSpanPropertyMapping(TimeSpanColumn column, IMemberAccessor member, int physicalIndex, int logicalIndex)
        {
            this.column = column;
            Member = member;
            PhysicalIndex = physicalIndex;
            LogicalIndex = logicalIndex;
        }

        public ITimeSpanPropertyMapping ColumnName(string name)
        {
            column.ColumnName = name;
            return this;
        }

        public ITimeSpanPropertyMapping InputFormat(string format)
        {
            column.InputFormat = format;
            return this;
        }

        public ITimeSpanPropertyMapping OutputFormat(string format)
        {
            column.OutputFormat = format;
            return this;
        }

        public ITimeSpanPropertyMapping FormatProvider(IFormatProvider provider)
        {
            column.FormatProvider = provider;
            return this;
        }

        public ITimeSpanPropertyMapping NullFormatter(INullFormatter formatter)
        {
            column.NullFormatter = formatter;
            return this;
        }

        public ITimeSpanPropertyMapping DefaultValue(IDefaultValue defaultValue)
        {
            column.DefaultValue = defaultValue;
            return this;
        }

        public ITimeSpanPropertyMapping Preprocessor(Func<string, string> preprocessor)
        {
            column.Preprocessor = preprocessor;
            return this;
        }

        public IMemberAccessor Member { get; }

        public Action<IColumnContext, object, object> Reader => null;

        public Action<IColumnContext, object, object[]> Writer => null;

        public IColumnDefinition ColumnDefinition => column;

        public int PhysicalIndex { get; }

        public int LogicalIndex { get; }
    }
}
