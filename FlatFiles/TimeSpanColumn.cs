using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing durations.
    /// </summary>
    public sealed class TimeSpanColumn : ColumnDefinition<TimeSpan>
    {
        /// <summary>
        /// Initializes a new instance of a TimeSpanColumn.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public TimeSpanColumn(string columnName)
            : base(columnName)
        {
        }

        /// <summary>
        /// Creates a column for reading and writing <see cref="TimeSpan"/> values where the days 
        /// are stored as doubles in the flat file.
        /// </summary>
        /// <param name="column">The column for reading/writing doubles from the file.</param>
        /// <returns>A column for reading/writing <see cref="TimeSpan"/> values.</returns>
        public static IColumnDefinition FromDays(DoubleColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            return new ConversionColumn<double, TimeSpan>(column, TimeSpan.FromDays, ts => ts.TotalDays);
        }

        /// <summary>
        /// Creates a column for reading and writing <see cref="TimeSpan"/> values where the hours 
        /// are stored as doubles in the flat file.
        /// </summary>
        /// <param name="column">The column for reading/writing doubles from the file.</param>
        /// <returns>A column for reading/writing <see cref="TimeSpan"/> values.</returns>
        public static IColumnDefinition FromHours(DoubleColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            return new ConversionColumn<double, TimeSpan>(column, TimeSpan.FromHours, ts => ts.TotalHours);
        }

        /// <summary>
        /// Creates a column for reading and writing <see cref="TimeSpan"/> values where the milliseconds 
        /// are stored as doubles in the flat file.
        /// </summary>
        /// <param name="column">The column for reading/writing doubles from the file.</param>
        /// <returns>A column for reading/writing <see cref="TimeSpan"/> values.</returns>
        public static IColumnDefinition FromMillseconds(DoubleColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            return new ConversionColumn<double, TimeSpan>(column, TimeSpan.FromMilliseconds, ts => ts.TotalMilliseconds);
        }

        /// <summary>
        /// Creates a column for reading and writing <see cref="TimeSpan"/> values where the minutes 
        /// are stored as doubles in the flat file.
        /// </summary>
        /// <param name="column">The column for reading/writing doubles from the file.</param>
        /// <returns>A column for reading/writing <see cref="TimeSpan"/> values.</returns>
        public static IColumnDefinition FromMinutes(DoubleColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            return new ConversionColumn<double, TimeSpan>(column, TimeSpan.FromMinutes, ts => ts.TotalMinutes);
        }

        /// <summary>
        /// Creates a column for reading and writing <see cref="TimeSpan"/> values where the seconds 
        /// are stored as doubles in the flat file.
        /// </summary>
        /// <param name="column">The column for reading/writing doubles from the file.</param>
        /// <returns>A column for reading/writing <see cref="TimeSpan"/> values.</returns>
        public static IColumnDefinition FromSeconds(DoubleColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            return new ConversionColumn<double, TimeSpan>(column, TimeSpan.FromSeconds, ts => ts.TotalSeconds);
        }

        /// <summary>
        /// Creates a column for reading and writing <see cref="TimeSpan"/> values where the ticks 
        /// are stored as longs in the flat file.
        /// </summary>
        /// <param name="column">The column for reading/writing doubles from the file.</param>
        /// <returns>A column for reading/writing <see cref="TimeSpan"/> values.</returns>
        public static IColumnDefinition FromTicks(Int64Column column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            return new ConversionColumn<long, TimeSpan>(column, TimeSpan.FromTicks, ts => ts.Ticks);
        }

        /// <summary>
        /// Gets or sets the format provider to use.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Gets or sets the format used to parse the <see cref="TimeSpan"/>.
        /// </summary>
        public string InputFormat { get; set; }

        /// <summary>
        /// Gets or sets the format used to write the <see cref="TimeSpan"/> to the flat file.
        /// </summary>
        public string OutputFormat { get; set; }

        /// <inheritdoc />
        protected override TimeSpan OnParse(IColumnContext context, string value)
        {
            var provider = GetFormatProvider(context, FormatProvider);
            if (InputFormat == null)
            {
                return TimeSpan.Parse(value, provider);
            }
            return TimeSpan.ParseExact(value, InputFormat, provider);
        }

        /// <inheritdoc />
        protected override string OnFormat(IColumnContext context, TimeSpan value)
        {
            if (OutputFormat == null)
            {
                return value.ToString();
            }
            var provider = GetFormatProvider(context, FormatProvider);
            return value.ToString(OutputFormat, provider);
        }
    }
}
