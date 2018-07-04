#if NET45||NETStandard20
using System;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace FlatFiles
{
    /// <summary>
    /// Provides additional helper methods to the IDataRecord interface.
    /// </summary>
    public static class DataRecordExtensions
    {
#region GetBoolean

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column.</returns>
        public static bool GetBoolean(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetBoolean);
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static bool? GetNullableBoolean(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetBoolean, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static bool? GetNullableBoolean(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetBoolean, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static bool? GetNullableBoolean(this IDataRecord record, int i, bool? defaultValue)
        {
            return GetNullable(record, i, record.GetBoolean, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static bool? GetNullableBoolean(this IDataRecord record, string name, bool? defaultValue)
        {
            return GetNullable(record, name, record.GetBoolean, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a Boolean and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a Boolean; otherwise, false.</returns>
        public static bool TryGetBoolean(this IDataRecord record, int i, out bool value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a Boolean and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a Boolean; otherwise, false.</returns>
        public static bool TryGetBoolean(this IDataRecord record, string name, out bool value)
        {
            return TryGet(record, name, out value);
        }

#endregion

#region GetEnum

        /// <summary>
        /// Maps the value of the column to the specified enumeration value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the value to map to.</typeparam>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column mapped to the enumeration value.</returns>
        /// <remarks>This method attempts to generate the enumeration by its name (case-insensitive) or numeric value.</remarks>
        public static TEnum GetEnum<TEnum>(this IDataRecord record, int i)
            where TEnum : struct
        {
            return GetValue<TEnum>(record, i, null);
        }

        /// <summary>
        /// Maps the value of the column to the specified enumeration value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the value to map to.</typeparam>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column mapped to the enumeration value.</returns>
        /// <remarks>This method attempts to generate the enumeration by its name (case-insensitive) or numeric value.</remarks>
        public static TEnum GetEnum<TEnum>(this IDataRecord record, string name)
            where TEnum : struct
        {
            int ordinal = record.GetOrdinal(name);
            return GetValue<TEnum>(record, ordinal, null);
        }

        /// <summary>
        /// Maps the value of the column to an enumeration value.
        /// </summary>
        /// <typeparam name="T">The type of the column.</typeparam>
        /// <typeparam name="TEnum">The type of value to map to.</typeparam>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="mapper">A method that maps from the column's type to the desired type.</param>
        /// <returns>The value of the column mapped to the enumeration value.</returns>
        public static TEnum GetEnum<T, TEnum>(this IDataRecord record, int i, Func<T, TEnum> mapper)
            where TEnum : struct
        {
            return getEnum(record, i, mapper);
        }

        /// <summary>
        /// Maps the value of the column to an enumeration value.
        /// </summary>
        /// <typeparam name="T">The type of the column.</typeparam>
        /// <typeparam name="TEnum">The type of value to map to.</typeparam>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="mapper">A method that maps from the column's type to the desired type.</param>
        /// <returns>The value of the column mapped to the enumeration value.</returns>
        public static TEnum GetEnum<T, TEnum>(this IDataRecord record, string name, Func<T, TEnum> mapper)
            where TEnum : struct
        {
            int ordinal = record.GetOrdinal(name);
            return getEnum(record, ordinal, mapper);
        }

        private static TEnum getEnum<T, TEnum>(IDataRecord record, int i, Func<T, TEnum> mapper)
            where TEnum : struct
        {
            T value = GetValue<T>(record, i, null);
            return mapper(value);
        }

#endregion

#region GetByte

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The 8-bit unsigned integer value of the specified column.</returns>
        public static byte GetByte(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetByte);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static byte? GetNullableByte(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetByte, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static byte? GetNullableByte(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetByte, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static byte? GetNullableByte(this IDataRecord record, int i, byte? defaultValue)
        {
            return GetNullable(record, i, record.GetByte, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static byte? GetNullableByte(this IDataRecord record, string name, byte? defaultValue)
        {
            int ordinal = record.GetOrdinal(name);
            return GetNullable(record, ordinal, record.GetByte, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a byte and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a byte; otherwise, false.</returns>
        public static bool TryGetByte(this IDataRecord record, int i, out byte value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a byte and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a byte; otherwise, false.</returns>
        public static bool TryGetByte(this IDataRecord record, string name, out byte value)
        {
            int ordinal = record.GetOrdinal(name);
            return TryGet(record, ordinal, out value);
        }

#endregion

#region GetBytes

        /// <summary>
        /// Reads a stream of bytes from the specified column offset into the buffer
        /// as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for buffer to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        public static long GetBytes(this IDataRecord record, string name, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            int ordinal = record.GetOrdinal(name);
            return record.GetBytes(ordinal, fieldOffset, buffer, bufferoffset, length);
        }

#endregion

#region GetChar

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The character value of the specified column.</returns>
        public static char GetChar(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetChar);
        }

        /// <summary>
        /// Gets the value of the specified column as a char -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static char? GetNullableChar(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetChar, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a char -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static char? GetNullableChar(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetChar, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a char -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static char? GetNullableChar(this IDataRecord record, int i, char? defaultValue)
        {
            return GetNullable(record, i, record.GetChar, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a char -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static char? GetNullableChar(this IDataRecord record, string name, char? defaultValue)
        {
            return GetNullable(record, name, record.GetChar, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a char and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a char; otherwise, false.</returns>
        public static bool TryGetChar(this IDataRecord record, int i, out char value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a char and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a char; otherwise, false.</returns>
        public static bool TryGetChar(this IDataRecord record, string name, out char value)
        {
            return TryGet(record, name, out value);
        }

#endregion

#region GetChars

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer
        /// as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="fieldoffset">The index within the row from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for buffer to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The actual number of characters read.</returns>
        public static long GetChars(this IDataRecord record, string name, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            int ordinal = record.GetOrdinal(name);
            return record.GetChars(ordinal, fieldoffset, buffer, bufferoffset, length);
        }

#endregion

#region GetData

        /// <summary>
        /// Returns an System.Data.IDataReader for the specified column name.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>An System.Data.IDataReader.</returns>
        public static IDataReader GetData(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetData);
        }

#endregion

#region GetDataTypeName

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The data type information for the specified field.</returns>
        public static string GetDataTypeName(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetDataTypeName);
        }

#endregion

#region GetDateTime

        /// <summary>
        /// Gets the DateTime value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The DateTime value of the specified column.</returns>
        public static DateTime GetDateTime(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetDateTime);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static DateTime? GetNullableDateTime(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetDateTime, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static DateTime? GetNullableDateTime(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetDateTime, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static DateTime? GetNullableDateTime(this IDataRecord record, int i, DateTime? defaultValue)
        {
            return GetNullable(record, i, record.GetDateTime, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static DateTime? GetNullableDateTime(this IDataRecord record, string name, DateTime? defaultValue)
        {
            return GetNullable(record, name, record.GetDateTime, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a DateTime and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a DateTime; otherwise, false.</returns>
        public static bool TryGetDateTime(this IDataRecord record, int i, out DateTime value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a DateTime and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a DateTime; otherwise, false.</returns>
        public static bool TryGetDateTime(this IDataRecord record, string name, out DateTime value)
        {
            return TryGet(record, name, out value);
        }

#endregion

#region GetDecimal

        /// <summary>
        /// Gets the decimal value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The decimal value of the specified column.</returns>
        public static decimal GetDecimal(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetDecimal);
        }

        /// <summary>
        /// Gets the value of the specified column as a decimal -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static decimal? GetNullableDecimal(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetDecimal, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a decimal -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static decimal? GetNullableDecimal(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetDecimal, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a decimal -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static decimal? GetNullableDecimal(this IDataRecord record, int i, decimal? defaultValue)
        {
            return GetNullable(record, i, record.GetDecimal, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a decimal -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static decimal? GetNullableDecimal(this IDataRecord record, string name, decimal? defaultValue)
        {
            return GetNullable(record, name, record.GetDecimal, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a decimal and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a decimal; otherwise, false.</returns>
        public static bool TryGetDecimal(this IDataRecord record, int i, out decimal value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a decimal and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a decimal; otherwise, false.</returns>
        public static bool TryGetDecimal(this IDataRecord record, string name, out decimal value)
        {
            return TryGet(record, name, out value);
        }

#endregion

#region GetDouble

        /// <summary>
        /// Gets the double value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The double value of the specified column.</returns>
        public static double GetDouble(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetDouble);
        }

        /// <summary>
        /// Gets the value of the specified column as a double -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static double? GetNullableDouble(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetDouble, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a double -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static double? GetNullableDouble(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetDouble, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a double -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static double? GetNullableDouble(this IDataRecord record, int i, double? defaultValue)
        {
            return GetNullable(record, i, record.GetDouble, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a double -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static double? GetNullableDouble(this IDataRecord record, string name, double? defaultValue)
        {
            return GetNullable(record, name, record.GetDouble, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a double and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a double; otherwise, false.</returns>
        public static bool TryGetDouble(this IDataRecord record, int i, out double value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a double and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a double; otherwise, false.</returns>
        public static bool TryGetDouble(this IDataRecord record, string name, out double value)
        {
            return TryGet(record, name, out value);
        }

#endregion

#region GetFieldType

        /// <summary>
        /// Gets the System.Type information corresponding to the type of System.Object
        /// that would be returned from System.Data.IDataRecord.GetValue(System.Int32).
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>
        /// The System.Type information corresponding to the type of System.Object that
        /// would be returned from System.Data.IDataRecord.GetValue(System.Int32).
        /// </returns>
        public static Type GetFieldType(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetFieldType);
        }

#endregion

#region GetFloat

        /// <summary>
        /// Gets the float value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The float value of the specified column.</returns>
        public static float GetFloat(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetFloat);
        }

        /// <summary>
        /// Gets the value of the specified column as a float -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static float? GetNullableFloat(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetFloat, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a float -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static float? GetNullableFloat(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetFloat, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a float -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static float? GetNullableFloat(this IDataRecord record, int i, float? defaultValue)
        {
            return GetNullable(record, i, record.GetFloat, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a float -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static float? GetNullableFloat(this IDataRecord record, string name, float? defaultValue)
        {
            return GetNullable(record, name, record.GetFloat, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a float and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a float; otherwise, false.</returns>
        public static bool TryGetFloat(this IDataRecord record, int i, out float value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a float and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a float; otherwise, false.</returns>
        public static bool TryGetFloat(this IDataRecord record, string name, out float value)
        {
            return TryGet(record, name, out value);
        }

#endregion

#region GetGuid

        /// <summary>
        /// Gets the Guid value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The Guid value of the specified column.</returns>
        public static Guid GetGuid(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetGuid);
        }

        /// <summary>
        /// Gets the value of the specified column as a Guid -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static Guid? GetNullableGuid(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetGuid, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a Guid -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static Guid? GetNullableGuid(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetGuid, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a Guid -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static Guid? GetNullableGuid(this IDataRecord record, int i, Guid? defaultValue)
        {
            return GetNullable(record, i, record.GetGuid, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a Guid -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static Guid? GetNullableGuid(this IDataRecord record, string name, Guid? defaultValue)
        {
            return GetNullable(record, name, record.GetGuid, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a Guid and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a Guid; otherwise, false.</returns>
        public static bool TryGetGuid(this IDataRecord record, int i, out Guid value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a Guid and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a Guid; otherwise, false.</returns>
        public static bool TryGetGuid(this IDataRecord record, string name, out Guid value)
        {
            return TryGet(record, name, out value);
        }

#endregion

#region GetInt16

        /// <summary>
        /// Gets the short value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The short value of the specified column.</returns>
        public static short GetInt16(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetInt16);
        }

        /// <summary>
        /// Gets the value of the specified column as a short -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static short? GetNullableInt16(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetInt16, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a short -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static short? GetNullableInt16(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetInt16, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a short -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static short? GetNullableInt16(this IDataRecord record, int i, short? defaultValue)
        {
            return GetNullable(record, i, record.GetInt16, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a short -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static short? GetNullableInt16(this IDataRecord record, string name, short? defaultValue)
        {
            return GetNullable(record, name, record.GetInt16, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a short and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a short; otherwise, false.</returns>
        public static bool TryGetInt16(this IDataRecord record, int i, out short value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a short and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a short; otherwise, false.</returns>
        public static bool TryGetInt16(this IDataRecord record, string name, out short value)
        {
            return TryGet(record, name, out value);
        }

#endregion

#region GetInt32

        /// <summary>
        /// Gets the int value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The int value of the specified column.</returns>
        public static int GetInt32(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetInt32);
        }

        /// <summary>
        /// Gets the value of the specified column as a int -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static int? GetNullableInt32(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetInt32, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a int -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static int? GetNullableInt32(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetInt32, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a int -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static int? GetNullableInt32(this IDataRecord record, int i, int? defaultValue)
        {
            return GetNullable(record, i, record.GetInt32, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a int -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static int? GetNullableInt32(this IDataRecord record, string name, int? defaultValue)
        {
            return GetNullable(record, name, record.GetInt32, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a int and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a int; otherwise, false.</returns>
        public static bool TryGetInt32(this IDataRecord record, int i, out int value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a int and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a int; otherwise, false.</returns>
        public static bool TryGetInt32(this IDataRecord record, string name, out int value)
        {
            return TryGet(record, name, out value);
        }

#endregion

#region GetInt64

        /// <summary>
        /// Gets the long value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The int value of the specified column.</returns>
        public static long GetInt64(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetInt64);
        }

        /// <summary>
        /// Gets the value of the specified column as a long -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static long? GetNullableInt64(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetInt64, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a long -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static long? GetNullableInt64(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetInt64, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a long -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static long? GetNullableInt64(this IDataRecord record, int i, long? defaultValue)
        {
            return GetNullable(record, i, record.GetInt64, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified column as a long -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static long? GetNullableInt64(this IDataRecord record, string name, long? defaultValue)
        {
            return GetNullable(record, name, record.GetInt64, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a long and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a long; otherwise, false.</returns>
        public static bool TryGetInt64(this IDataRecord record, int i, out long value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a long and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a long; otherwise, false.</returns>
        public static bool TryGetInt64(this IDataRecord record, string name, out long value)
        {
            return TryGet(record, name, out value);
        }

#endregion

#region GetString

        /// <summary>
        /// Gets the string value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The int value of the specified column.</returns>
        public static string GetString(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetString);
        }

        /// <summary>
        /// Gets the value of the specified column as a string -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static string GetNullableString(this IDataRecord record, int i)
        {
            return GetNullableString(record, i, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a string -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static string GetNullableString(this IDataRecord record, string name)
        {
            int ordinal = record.GetOrdinal(name);
            return GetNullableString(record, ordinal, null);
        }

        /// <summary>
        /// Gets the value of the specified column as a string -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static string GetNullableString(this IDataRecord record, int i, string defaultValue)
        {
            if (record.IsDBNull(i))
            {
                return defaultValue;
            }
            return record.GetString(i);
        }

        /// <summary>
        /// Gets the value of the specified column as a string -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="defaultValue">The default value to use if the column is null.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static string GetNullableString(this IDataRecord record, string name, string defaultValue)
        {
            int ordinal = record.GetOrdinal(name);
            return GetNullableString(record, ordinal, defaultValue);
        }

        /// <summary>
        /// Tries to get the value of the column as a string and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a string; otherwise, false.</returns>
        public static bool TryGetString(this IDataRecord record, int i, out string value)
        {
            return TryGet(record, i, out value);
        }

        /// <summary>
        /// Tries to get the value of the column as a string and stores it in the out parameter.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <param name="value">The parameter to store the column value in.</param>
        /// <returns>True if the column was a string; otherwise, false.</returns>
        public static bool TryGetString(this IDataRecord record, string name, out string value)
        {
            return TryGet(record, name, out value);
        }

#endregion

#region GetValue

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>The System.Object which will contain the field value upon return.</returns>
        public static object GetValue(this IDataRecord record, string name)
        {
            return Get(record, name, record.GetValue);
        }

        /// <summary>
        /// Returns the value of the specified field.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the field to find.</param>
        /// <param name="provider">A format provider for converting to the desired type.</param>
        /// <returns>The value.</returns>
        public static T GetValue<T>(this IDataRecord record, string name, IFormatProvider provider = null)
        {
            int ordinal = record.GetOrdinal(name);
            return GetValue<T>(record, ordinal, provider);
        }

        /// <summary>
        /// Returns the value of the specified field.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="provider">A format provider for converting to the desired type.</param>
        /// <returns>The value.</returns>
        public static T GetValue<T>(IDataRecord record, int i, IFormatProvider provider = null)
        {
            Type underlyingType = Nullable.GetUnderlyingType(typeof(T));
            Type type = underlyingType ?? typeof(T);
            if (record.IsDBNull(i))
            {
                if (type.IsValueType && underlyingType == null)
                {
                    throw new InvalidCastException();
                }
                return default;
            }
            object value = record.GetValue(i);
            if (type != value.GetType())
            {
                if (type == typeof(Guid))
                {
                    value = GetGuid(value);
                }
                else if (type.IsEnum)
                {
                    value = GetEnum(type, value);
                }
                else if (value is IConvertible)
                {
                    value = Convert.ChangeType(value, type, provider);
                }
            }
            return (T)value;
        }

        private static object GetGuid(object value)
        {
            if (value is string stringValue)
            {
                value = Guid.Parse(stringValue);
            }
            else if (value is byte[] byteArray)
            {
                value = new Guid(byteArray);
            }
            return value;
        }

        private static object GetEnum(Type type, object value)
        {
            try
            {
                if (type.GetCustomAttribute(typeof(FlagsAttribute)) != null)
                {
                    return ToEnum(type, value);
                }
                if (Enum.IsDefined(type, value))
                {
                    return ToEnum(type, value);
                }
                value = Convert.ChangeType(value, Enum.GetUnderlyingType(type));
                if (Enum.IsDefined(type, value))
                {
                    return Enum.ToObject(type, value);
                }
            }
            catch
            {
            }
            return value;
        }

        private static object ToEnum(Type type, object value)
        {
            if (value is string stringValue)
            {
                return Enum.Parse(type, stringValue);
            }
            else
            {
                return Enum.ToObject(type, value);
            }
        }

#endregion

#region GetValues

        /// <summary>
        /// Creates an array of objects with the column values of the current record.
        /// </summary>
        /// <param name="record">The IDataRecord to get the values for.</param>
        /// <returns>An array of objects with the column values of the current record.</returns>
        public static object[] GetValues(this IDataRecord record)
        {
            object[] values = new object[record.FieldCount];
            record.GetValues(values);
            return values;
        }

#endregion

#region IsDBNull

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>true if the specified field is set to null; otherwise, false.</returns>
        public static bool IsDBNull(this IDataRecord record, string name)
        {
            return Get(record, name, record.IsDBNull);
        }

        #endregion

        private static T Get<T>(IDataRecord record, string name, Func<int, T> getter)
        {
            int index = record.GetOrdinal(name);
            return getter(index);
        }

        private static T? GetNullable<T>(IDataRecord record, int index, Func<int, T> getter, T? defaultValue)
            where T : struct
        {
            if (record.IsDBNull(index))
            {
                return defaultValue;
            }
            else
            {
                return getter(index);
            }
        }

        private static T? GetNullable<T>(IDataRecord record, string name, Func<int, T> getter, T? defaultValue)
            where T : struct
        {
            int index = record.GetOrdinal(name);
            return GetNullable<T>(record, index, getter, defaultValue);
        }

        private static bool TryGet<T>(IDataRecord record, int i, out T value)
        {
            if (record.IsDBNull(i))
            {
                value = default;
                return false;
            }
            object result = record.GetValue(i);
            if (result is T)
            {
                value = (T)result;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        private static bool TryGet<T>(IDataRecord record, string name, out T value)
        {
            int index = record.GetOrdinal(name);
            return TryGet<T>(record, index, out value);
        }
    }
}
#endif