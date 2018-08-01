#if NET45||NETStandard20
using System;
using System.Data;
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
        /// Gets the value of the specified column as a Boolean -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static bool? GetNullableBoolean(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetBoolean);
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static bool? GetNullableBoolean(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetBoolean);
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
            where TEnum : Enum
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
            where TEnum : Enum
        {
            int ordinal = record.GetOrdinal(name);
            return GetValue<TEnum>(record, ordinal, null);
        }

        /// <summary>
        /// Maps the value of the column to the specified enumeration value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the value to map to.</typeparam>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column mapped to the enumeration value.</returns>
        /// <remarks>This method attempts to generate the enumeration by its name (case-insensitive) or numeric value.</remarks>
        public static TEnum? GetNullableEnum<TEnum>(this IDataRecord record, int i)
            where TEnum : struct, Enum
        {
            return GetValue<TEnum?>(record, i, null);
        }

        /// <summary>
        /// Maps the value of the column to the specified enumeration value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the value to map to.</typeparam>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column mapped to the enumeration value.</returns>
        /// <remarks>This method attempts to generate the enumeration by its name (case-insensitive) or numeric value.</remarks>
        public static TEnum? GetNullableEnum<TEnum>(this IDataRecord record, string name)
            where TEnum : struct, Enum
        {
            int ordinal = record.GetOrdinal(name);
            return GetValue<TEnum?>(record, ordinal, null);
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
            where TEnum : Enum
        {
            T value = GetValue<T>(record, i, null);
            return mapper(value);
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
            where TEnum : Enum
        {
            int ordinal = record.GetOrdinal(name);
            return GetEnum(record, ordinal, mapper);
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
        public static TEnum? GetNullableEnum<T, TEnum>(this IDataRecord record, int i, Func<T, TEnum?> mapper)
            where TEnum : struct, Enum
        {
            T value = GetValue<T>(record, i, null);
            return mapper(value);
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
        public static TEnum? GetNullableEnum<T, TEnum>(this IDataRecord record, string name, Func<T, TEnum?> mapper)
            where TEnum : struct, Enum
        {
            int ordinal = record.GetOrdinal(name);
            return GetNullableEnum(record, ordinal, mapper);
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
        /// Gets the value of the specified column as a byte -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static byte? GetNullableByte(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetByte);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static byte? GetNullableByte(this IDataRecord record, string name)
        {
            int ordinal = record.GetOrdinal(name);
            return GetNullable(record, ordinal, record.GetByte);
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
        /// Gets the value of the specified column as a char -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static char? GetNullableChar(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetChar);
        }

        /// <summary>
        /// Gets the value of the specified column as a char -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static char? GetNullableChar(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetChar);
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
        /// Gets the value of the specified column as a DateTime -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static DateTime? GetNullableDateTime(this IDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetDateTime);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static DateTime? GetNullableDateTime(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetDateTime);
        }

        #endregion

        #region GetDateTimeOffset

        /// <summary>
        /// Gets the DateTime value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The DateTime value of the specified column.</returns>
        public static DateTimeOffset GetDateTimeOffset(this IFlatFileDataRecord record, string name)
        {
            return Get(record, name, record.GetDateTimeOffset);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static DateTimeOffset? GetNullableDateTimeOffset(this IFlatFileDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetDateTimeOffset);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static DateTimeOffset? GetNullableDateTimeOffset(this IFlatFileDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetDateTimeOffset);
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
            return GetNullable(record, i, record.GetDecimal);
        }

        /// <summary>
        /// Gets the value of the specified column as a decimal -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static decimal? GetNullableDecimal(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetDecimal);
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
            return GetNullable(record, i, record.GetDouble);
        }

        /// <summary>
        /// Gets the value of the specified column as a double -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static double? GetNullableDouble(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetDouble);
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
            return GetNullable(record, i, record.GetFloat);
        }

        /// <summary>
        /// Gets the value of the specified column as a float -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static float? GetNullableFloat(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetFloat);
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
            return GetNullable(record, i, record.GetGuid);
        }

        /// <summary>
        /// Gets the value of the specified column as a Guid -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static Guid? GetNullableGuid(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetGuid);
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
            return GetNullable(record, i, record.GetInt16);
        }

        /// <summary>
        /// Gets the value of the specified column as a short -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static short? GetNullableInt16(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetInt16);
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
            return GetNullable(record, i, record.GetInt32);
        }

        /// <summary>
        /// Gets the value of the specified column as a int -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static int? GetNullableInt32(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetInt32);
        }

        #endregion

        #region GetInt64

        /// <summary>
        /// Gets the long value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The long value of the specified column.</returns>
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
            return GetNullable(record, i, record.GetInt64);
        }

        /// <summary>
        /// Gets the value of the specified column as a long -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static long? GetNullableInt64(this IDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetInt64);
        }

        #endregion

        #region GetSByte

        /// <summary>
        /// Gets the sbyte value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The sbyte value of the specified column.</returns>
        public static sbyte GetSByte(this IFlatFileDataRecord record, string name)
        {
            return Get(record, name, record.GetSByte);
        }

        /// <summary>
        /// Gets the value of the specified column as a sbyte -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static sbyte? GetNullableSByte(this IFlatFileDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetSByte);
        }

        /// <summary>
        /// Gets the value of the specified column as a sbyte -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static sbyte? GetNullableSByte(this IFlatFileDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetSByte);
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
            return record.IsDBNull(i) ? null : record.GetString(i);
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
            return GetNullableString(record, ordinal);
        }

        #endregion

        #region GetTimeSpan

        /// <summary>
        /// Gets the DateTime value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The DateTime value of the specified column.</returns>
        public static TimeSpan GetTimeSpan(this IFlatFileDataRecord record, string name)
        {
            return Get(record, name, record.GetTimeSpan);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static TimeSpan? GetNullableTimeSpan(this IFlatFileDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetTimeSpan);
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime -or- the specified default value if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- the default value if the column is null.</returns>
        public static TimeSpan? GetNullableTimeSpan(this IFlatFileDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetTimeSpan);
        }

        #endregion

        #region GetUInt16

        /// <summary>
        /// Gets the ushort value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The ushort value of the specified column.</returns>
        public static ushort GetUInt16(this IFlatFileDataRecord record, string name)
        {
            return Get(record, name, record.GetUInt16);
        }

        /// <summary>
        /// Gets the value of the specified column as a ushort -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static ushort? GetNullableUInt16(this IFlatFileDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetUInt16);
        }

        /// <summary>
        /// Gets the value of the specified column as a ushort -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static ushort? GetNullableUInt16(this IFlatFileDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetUInt16);
        }

        #endregion

        #region GetUInt32

        /// <summary>
        /// Gets the uint value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The uint value of the specified column.</returns>
        public static uint GetUInt32(this IFlatFileDataRecord record, string name)
        {
            return Get(record, name, record.GetUInt32);
        }

        /// <summary>
        /// Gets the value of the specified column as a uint -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static uint? GetNullableUInt32(this IFlatFileDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetUInt32);
        }

        /// <summary>
        /// Gets the value of the specified column as a uint -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static uint? GetNullableUInt32(this IFlatFileDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetUInt32);
        }

        #endregion

        #region GetUInt64

        /// <summary>
        /// Gets the ulong value of the specified column.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The ulong value of the specified column.</returns>
        public static ulong GetUInt64(this IFlatFileDataRecord record, string name)
        {
            return Get(record, name, record.GetUInt64);
        }

        /// <summary>
        /// Gets the value of the specified column as a ulong -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static ulong? GetNullableUInt64(this IFlatFileDataRecord record, int i)
        {
            return GetNullable(record, i, record.GetUInt64);
        }

        /// <summary>
        /// Gets the value of the specified column as a ulong -or- null if the column is null.
        /// </summary>
        /// <param name="record">The IDataRecord to get the value for.</param>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The value of the column -or- null if the column is null.</returns>
        public static ulong? GetNullableUInt64(this IFlatFileDataRecord record, string name)
        {
            return GetNullable(record, name, record.GetUInt64);
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
        public static T GetValue<T>(this IDataRecord record, int i, IFormatProvider provider = null)
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
            var values = new object[record.FieldCount];
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

        private static T? GetNullable<T>(IDataRecord record, int index, Func<int, T> getter)
            where T : struct
        {
            return record.IsDBNull(index) ? (T?)null : getter(index);
        }

        private static T? GetNullable<T>(IDataRecord record, string name, Func<int, T> getter)
            where T : struct
        {
            int index = record.GetOrdinal(name);
            return GetNullable<T>(record, index, getter);
        }
    }
}
#endif