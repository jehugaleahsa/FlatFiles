using System;

namespace FlatFiles
{
    /// <summary>
    /// Represents a column containing enumeration values.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
    public class EnumColumn<TEnum> : ColumnDefinition
        where TEnum : Enum
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
            parser = defaultParser;
            formatter = defaultFormatter;
        }

        private static TEnum defaultParser(string value)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        private static string defaultFormatter(TEnum value)
        {
            return Convert.ToInt32(value).ToString();
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType => typeof(TEnum);

        /// <summary>
        /// Gets or sets the parser used to convert string values into enumeration values.
        /// </summary>
        public Func<string, TEnum> Parser
        {
            get => parser;
            set => parser = value ?? defaultParser;
        }

        /// <summary>
        /// Gets or sets the formatter used to convert enumeration values to string values.
        /// </summary>
        public Func<TEnum, string> Formatter
        {
            get => formatter;
            set => formatter = value ?? defaultFormatter;
        }

        /// <summary>
        /// Parses the given value into its equivalent enum value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The enum value that was parsed.</returns>
        public override object Parse(string value)
        {
            if (Preprocessor != null)
            {
                value = Preprocessor(value);
            }
            if (NullHandler.IsNullRepresentation(value))
            {
                return null;
            }
            return parser(value);
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public override string Format(object value)
        {
            if (value == null)
            {
                return NullHandler.GetNullRepresentation();
            }
            TEnum actual = (TEnum)value;
            return formatter(actual);
        }
    }
}
