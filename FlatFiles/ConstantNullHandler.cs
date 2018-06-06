using System;

namespace FlatFiles
{
    /// <summary>
    /// Uses a constant value to represent nulls in a flat file.
    /// </summary>
    public class ConstantNullHandler : INullHandler
    {
        private readonly string value;

        /// <summary>
        /// Initiates a new instance of a ConstantNullHandler.
        /// </summary>
        /// <param name="value">The constant value to use.</param>
        public ConstantNullHandler(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.value = value;
        }

        /// <summary>
        /// Creates a new NullHandler using the given value.
        /// </summary>
        /// <param name="value">The constant used to represent null in the flat file.</param>
        /// <returns>The created NullHandler.</returns>
        public static INullHandler For(string value)
        {
            return new ConstantNullHandler(value);
        }

        /// <summary>
        /// Gets whether the given string should be interpreted as null.
        /// </summary>
        /// <param name="value">The value to inspect.</param>
        /// <returns>True if the value represents null; otherwise, false.</returns>
        public bool IsNullRepresentation(string value)
        {
            return value == null || value == this.value;
        }

        /// <summary>
        /// Gets the value used to represent null when writing to a flat file.
        /// </summary>
        /// <returns>The string used to represent null in the flat file.</returns>
        public string GetNullRepresentation()
        {
            return value;
        }
    }
}
