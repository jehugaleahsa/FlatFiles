using System;

namespace FlatFileReaders
{
    /// <summary>
    /// Specifies the type of a column in a schema.
    /// </summary>
    public enum ColumnType
    {
        /// <summary>
        /// Specifies that the column contains string values.
        /// </summary>
        String,
        /// <summary>
        /// Specifies that the column contains a binary object.
        /// </summary>
        ByteArray,
        /// <summary>
        /// Specifies that the column contains 8-bit integer values.
        /// </summary>
        Byte,
        /// <summary>
        /// Specifies that the column contains 16-bit integer values.
        /// </summary>
        Int16,
        /// <summary>
        /// Specifies that the column contains 32-bit integer values.
        /// </summary>
        Int32,
        /// <summary>
        /// Specifies that the column contains 64-bit integer values.
        /// </summary>
        Int64,
        /// <summary>
        /// Specifies that the column contains 32-bit floating point values.
        /// </summary>
        Single,
        /// <summary>
        /// Specifies that the column contains 64-bit floating point values.
        /// </summary>
        Double,
        /// <summary>
        /// Specifies that the column contains fixed point vales.
        /// </summary>
        Decimal,
        /// <summary>
        /// Specifies that the column contains date and time values.
        /// </summary>
        DateTime,
    }
}
