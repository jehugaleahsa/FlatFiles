using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing enumeration values.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
    public sealed class EnumColumn<TEnum> : ColumnDefinition<TEnum>
    {
        private Func<string, TEnum> parser;
        private Func<TEnum, string> formatter;

        /// <summary>
        /// Initializes a new EnumColumn with the given name.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        public EnumColumn(string columnName) 
            : base(columnName)
        {
            parser = DefaultParser;
            formatter = DefaultFormatter;
        }

        private static TEnum DefaultParser(string value)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        private static string DefaultFormatter(TEnum value)
        {
            return Convert.ToInt32(value).ToString();
        }

        /// <summary>
        /// Gets or sets the parser used to convert string values into enumeration values.
        /// </summary>
        public Func<string, TEnum> Parser
        {
            get => parser;
            set => parser = value ?? DefaultParser;
        }

        /// <summary>
        /// Gets or sets the formatter used to convert enumeration values to string values.
        /// </summary>
        public Func<TEnum, string> Formatter
        {
            get => formatter;
            set => formatter = value ?? DefaultFormatter;
        }

        /// <summary>
        /// Parses the given value into its equivalent enum value.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The enum value that was parsed.</returns>
        protected override TEnum OnParse(IColumnContext? context, string value)
        {
            return parser(value);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string OnFormat(IColumnContext? context, TEnum value)
        {
            return formatter(value);
        }
    }
}
