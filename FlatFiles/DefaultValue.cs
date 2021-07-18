using System;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Generates a default value whenever a null is encountered on a non-nullable column.
    /// </summary>
    public sealed class DefaultValue : IDefaultValue
    {
        private readonly Func<IColumnContext?, object?> factory;

        private DefaultValue(Func<IColumnContext?, object?> factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Use the given value as a default.
        /// </summary>
        /// <param name="value">The value to use as a default.</param>
        /// <returns>An instance of a <see cref="IDefaultValue"/> that returns the given value.</returns>
        public static IDefaultValue Use(object? value)
        {
            return new DefaultValue(ctx => value);
        }

        /// <summary>
        /// Use the given delegate to generate the default value.
        /// </summary>
        /// <param name="factory">The value to use as a default.</param>
        /// <returns>An instance of a <see cref="IDefaultValue"/> that returns the result of the delegate.</returns>
        public static IDefaultValue Use(Func<IColumnContext?, object?> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            return new DefaultValue(factory);
        }

        /// <summary>
        /// Throw an exception when an unexpected null is encountered.
        /// </summary>
        /// <returns>An instance of a <see cref="IDefaultValue"/> that throws an exception.</returns>
        public static IDefaultValue Disabled()
        {
            return new DefaultValue(ctx => throw new InvalidCastException(Resources.AssignNullToNonNullable));
        }

        /// <summary>
        /// Gets the default value to use when a null is encountered on a non-nullable column.
        /// </summary>
        /// <param name="context">The current column context.</param>
        /// <returns>The default value.</returns>
        public object? GetDefaultValue(IColumnContext? context)
        {
            return factory(context);
        }
    }
}
