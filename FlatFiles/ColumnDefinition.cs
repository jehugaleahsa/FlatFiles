using System;
using System.Globalization;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Defines a column that is part of a record schema.
    /// </summary>
    public abstract class ColumnDefinition : IColumnDefinition
    {
        private string columnName;
        private INullFormatter nullHandler = FlatFiles.NullFormatter.Default;
        private IDefaultValue defaultValue = FlatFiles.DefaultValue.Disabled();

        /// <summary>
        /// Initializes a new instance of a ColumnDefinition.
        /// </summary>
        /// <param name="columnName">The name of the column to define.</param>
        protected ColumnDefinition(string columnName)
            : this(columnName, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a ColumnDefinition.
        /// </summary>
        /// <param name="columnName">The name of the column to define.</param>
        /// <param name="isIgnored">Specifies whether the value in the column appears in the parsed record.</param>
        internal ColumnDefinition(string columnName, bool isIgnored)
        {
            IsIgnored = isIgnored; // Set ignored first or ColumnName setter fails!
            ColumnName = columnName;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string ColumnName
        {
            get => columnName;
            internal set 
            {
                value = value?.Trim();
                if (!IsIgnored && String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(Resources.BlankColumnName);
                }
                columnName = value;
            }
        }

        /// <summary>
        /// Gets whether the value in this column is returned as a result.
        /// </summary>
        public bool IsIgnored { get; }

        /// <summary>
        /// Gets or sets whether nulls are allowed for the column.
        /// </summary>
        public bool IsNullable { get; set; } = true;

        /// <summary>
        /// Gets or sets the default value to use when a null is encountered on a non-nullable column.
        /// </summary>
        public IDefaultValue DefaultValue
        {
            get => defaultValue;
            set => defaultValue = value ?? FlatFiles.DefaultValue.Disabled();
        }

        /// <summary>
        /// Gets or sets the null formatter instance used to read/write null values.
        /// </summary>
        public INullFormatter NullFormatter
        {
            get => nullHandler;
            set => nullHandler = value ?? FlatFiles.NullFormatter.Default;
        }

        /// <summary>
        /// Gets or sets a function used to preprocess input before trying to parse it.
        /// </summary>
        [Obsolete("This property has been superseded by the OnParsing delegate.")]
        public Func<string, string> Preprocessor { get; set; }

        /// <summary>
        /// Gets or sets a function used to pre-process input before trying to parse it.
        /// </summary>
        public Func<IColumnContext, string, string> OnParsing { get; set; }

        /// <summary>
        /// Gets or sets a function used to post-process input after parsing it.
        /// </summary>
        public Func<IColumnContext, object, object> OnParsed { get; set; }

        /// <summary>
        /// Gets or sets a function used to pre-process output before trying to format it.
        /// </summary>
        public Func<IColumnContext, object, object> OnFormatting { get; set; }

        /// <summary>
        /// Gets or sets a function used to post-process output after formatting it.
        /// </summary>
        public Func<IColumnContext, string, string> OnFormatted { get; set; }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public abstract Type ColumnType { get; }

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        public abstract object Parse(IColumnContext context, string value);

        /// <summary>
        /// Removes any leading or trailing whitespace from the value.
        /// </summary>
        /// <param name="value">The value to trim.</param>
        /// <returns>The trimmed value.</returns>
        protected internal static string TrimValue(string value)
        {
            return value?.Trim();
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public abstract string Format(IColumnContext context, object value);

        /// <summary>
        /// Gets the format provider to use. If the given provider is not null, it will be used.
        /// Otherwise, the format provider set on the options object will be used. As a last resort,
        /// the current culture specified by the operating system will be used.
        /// </summary>
        /// <param name="context">The current column context.</param>
        /// <param name="formatProvider">The format provider set on the column.</param>
        /// <returns>The format provider to use.</returns>
        protected static IFormatProvider GetFormatProvider(IColumnContext context, IFormatProvider formatProvider)
        {
            return formatProvider
                ?? context?.RecordContext.ExecutionContext.Options.FormatProvider
                ?? CultureInfo.CurrentCulture;
        }
    }

    /// <summary>
    /// Represents the command base class for defining custom column definitions for a class.
    /// </summary>
    /// <typeparam name="T">The type of the column.</typeparam>
    public abstract class ColumnDefinition<T> : ColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of a ColumnDefinition.
        /// </summary>
        /// <param name="columnName">The name of the column to define.</param>
        protected ColumnDefinition(string columnName) 
            : base(columnName)
        {
        }

        /// <summary>
        /// Gets the type of the values in the column.
        /// </summary>
        public override Type ColumnType => typeof(T);

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        public override object Parse(IColumnContext context, string value)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (Preprocessor != null)
            {
                value = Preprocessor(value);
            }
#pragma warning restore CS0618 // Type or member is obsolete
            if (OnParsing != null)
            {
                value = OnParsing(context, value);
            }
            object result = ParseValue(context, value);
            if (OnParsed != null)
            {
                result = OnParsed(context, result);
            }
            return result;
        }

        private object ParseValue(IColumnContext context, string value)
        {
            if (NullFormatter.IsNullValue(context, value))
            {
                return IsNullable ? null : DefaultValue.GetDefaultValue(context); // Should we check for the expected type?
            }
            else
            {
                string trimmed = IsTrimmed ? TrimValue(value) : value;
                return OnParse(context, trimmed);
            }
        }

        /// <summary>
        /// Gets whether the value should be trimmed prior to parsing.
        /// </summary>
        protected virtual bool IsTrimmed => true;

        /// <summary>
        /// Parses the given value and returns the parsed object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed value.</returns>
        protected abstract T OnParse(IColumnContext context, string value);

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        public override string Format(IColumnContext context, object value)
        {
            if (OnFormatting != null)
            {
                value = OnFormatting(context, value);
            }
            string result = FormatValue(context, value);
            if (OnFormatted != null)
            {
                result = OnFormatted(context, result);
            }
            return result;
        }

        private string FormatValue(IColumnContext context, object value)
        {
            if (value == null)
            {
                return NullFormatter.FormatNull(context);
            }
            else
            {
                return OnFormat(context, (T)value);
            }
        }

        /// <summary>
        /// Formats the given object.
        /// </summary>
        /// <param name="context">Holds information about the column current being processed.</param>
        /// <param name="value">The object to format.</param>
        /// <returns>The formatted value.</returns>
        protected abstract string OnFormat(IColumnContext context, T value);
    }
}
