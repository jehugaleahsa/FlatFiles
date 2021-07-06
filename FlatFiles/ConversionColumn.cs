using System;

namespace FlatFiles
{
    /// <summary>
    /// Converts the values of a column to another type.
    /// </summary>
    /// <typeparam name="TSource">The type of the source column.</typeparam>
    /// <typeparam name="TDestination">The type to convert to.</typeparam>
    internal sealed class ConversionColumn<TSource, TDestination> : ColumnDefinition
    {
        private readonly IColumnDefinition columnDefinition;
        private readonly Func<TSource, TDestination> parser;
        private readonly Func<TDestination, TSource> formatter;

        public ConversionColumn(
                IColumnDefinition columnDefinition,
                Func<TSource, TDestination> parser, 
                Func<TDestination, TSource> formatter)
            : base(columnDefinition.ColumnName)
        {
            this.columnDefinition = columnDefinition;
            this.parser = parser;
            this.formatter = formatter;
        }

        /// <inheritdoc />
        public override Type ColumnType => typeof(TDestination);

        /// <inheritdoc />
        public override object Parse(IColumnContext context, string value)
        {
            var sourceValue = columnDefinition.Parse(context, value);
            if (sourceValue == null)
            {
                return null;
            }
            var destinationValue = parser((TSource)sourceValue);
            return destinationValue;
        }

        /// <inheritdoc />
        public override string Format(IColumnContext context, object value)
        {
            var destinationValue = value == null ? (object)null : formatter((TDestination)value);
            var sourceValue = columnDefinition.Format(context, destinationValue);
            return sourceValue;
        }
    }
}
