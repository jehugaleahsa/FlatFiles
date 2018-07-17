#if NET45||NETStandard20
using System;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace FlatFiles
{
    /// <summary>
    /// Provides access to the column values within each row for a <see cref="FlatFileDataReader"/>.
    /// </summary>
    public interface IFlatFileDataRecord : IDataRecord
    {
        /// <summary>
        /// Gets the DateTime value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The DateTime value at the given index.</returns>
        DateTimeOffset GetDateTimeOffset(int i);

        /// <summary>
        /// Gets the sbyte value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The sbyte value at the given index.</returns>
        sbyte GetSByte(int i);

        /// <summary>
        /// Gets the <see cref="TimeSpan"/> from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The string at the given index.</returns>
        TimeSpan GetTimeSpan(int i);

        /// <summary>
        /// Gets the unsigned short value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The unsigned short value at the given index.</returns>
        ushort GetUInt16(int i);

        /// <summary>
        /// Gets the unsigned int value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The unsigned int value at the given index.</returns>
        uint GetUInt32(int i);

        /// <summary>
        /// Gets the unsigned long value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The unsigned long value at the given index.</returns>
        ulong GetUInt64(int i);
    }

    /// <summary>
    /// Provides an ADO.NET adapter (IDataReader) for a flat file reader.
    /// </summary>
    public sealed class FlatFileDataReader : IDataReader, IFlatFileDataRecord
    {
        private ISchema schema;  // cached
        private object[] values; // cached

        /// <summary>
        /// Initializes a new instance of a FlatFileParser.
        /// </summary>
        /// <param name="reader">The reader to use to parse the underlying file.</param>
        /// <param name="options">The options to use to control how the file is read.</param>
        /// <exception cref="System.ArgumentNullException">The parser is null.</exception>
        public FlatFileDataReader(IReader reader, FlatFileDataReaderOptions options = null)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            Options = options ?? new FlatFileDataReaderOptions();
        }

        /// <summary>
        /// Gets the underlying FlatFile reader.
        /// </summary>
        public IReader Reader { get; }

        /// <summary>
        /// Gets the data reader options.
        /// </summary>
        public FlatFileDataReaderOptions Options { get; }

        /// <summary>
        /// Finalizes the FlatFileReader.
        /// </summary>
        ~FlatFileDataReader()
        {
            dispose(false);
        }

        /// <summary>
        /// Releases any resources being held by the reader.
        /// </summary>
        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            IsClosed = true;
        }

        /// <summary>
        /// Closes the underlying record set.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        int IDataReader.Depth => 0;

        /// <summary>
        /// Gets a DataTable containing the schema of the data.
        /// </summary>
        /// <returns>The Schema DataTable.</returns>
        public DataTable GetSchemaTable()
        {
            var schema = GetSchema();
            var schemaTable = GetEmptySchemaDataTable(schema);
            for (int index = 0; index != schema.ColumnDefinitions.Count; ++index)
            {
                var column = schema.ColumnDefinitions[index];
                object[] values = new object[]
                {
                    true,  // AllowDBNull
                    null,  // BaseCatalogName
                    column.ColumnName,  // BaseColumnName
                    null,  // BaseSchemaName
                    null,  // BaseServerName
                    null,  // BaseTableName
                    column.ColumnName,  // ColumnName
                    index,  // ColumnOrdinal
                    Int32.MaxValue,  // ColumnSize
                    column.ColumnType, // DataType
                    column.ColumnType.Name,  // DataTypeName
                    false,  // IsAliased
                    false,  // IsAutoIncrement
                    false,  // IsColumnSet
                    false,  // IsExpression
                    false,  // IsHidden
                    false,  // IsIdentity
                    false,  // IsKey
                    false,  // IsLong
                    false,  // IsReadOnly
                    false,  // IsRowVersion
                    false,  // IsUnique
                    255,  // NumericPrecision
                    255,  // NumericScale
                    column.ColumnType  // ProviderType
                };
                schemaTable.Rows.Add(values);
            }
            schemaTable.AcceptChanges();
            return schemaTable;
        }

        private static DataTable GetEmptySchemaDataTable(ISchema schema)
        {
            DataTable schemaTable = new DataTable();
            schemaTable.Locale = CultureInfo.InvariantCulture;
            schemaTable.MinimumCapacity = schema.ColumnDefinitions.Count;
            schemaTable.Columns.AddRange(new[]
            {
                new DataColumn(SchemaTableColumn.AllowDBNull, typeof(Boolean)),
                new DataColumn(SchemaTableOptionalColumn.BaseCatalogName, typeof(String)),
                new DataColumn(SchemaTableColumn.BaseColumnName, typeof(String)),
                new DataColumn(SchemaTableColumn.BaseSchemaName, typeof(String)),
                new DataColumn(SchemaTableOptionalColumn.BaseServerName, typeof(String)),
                new DataColumn(SchemaTableColumn.BaseTableName, typeof(String)),
                new DataColumn(SchemaTableColumn.ColumnName, typeof(String)),
                new DataColumn(SchemaTableColumn.ColumnOrdinal, typeof(Int32)),
                new DataColumn(SchemaTableColumn.ColumnSize, typeof(Int32)),
                new DataColumn(SchemaTableColumn.DataType, typeof(Type)),
                new DataColumn("DataTypeName", typeof(String)),
                new DataColumn(SchemaTableColumn.IsAliased, typeof(Boolean)),
                new DataColumn(SchemaTableOptionalColumn.IsAutoIncrement, typeof(Boolean)),
                new DataColumn("IsColumnSet", typeof(Boolean)),
                new DataColumn(SchemaTableColumn.IsExpression, typeof(Boolean)),
                new DataColumn(SchemaTableOptionalColumn.IsHidden, typeof(Boolean)),
                new DataColumn("IsIdentity", typeof(Boolean)),
                new DataColumn(SchemaTableColumn.IsKey, typeof(Boolean)),
                new DataColumn(SchemaTableColumn.IsLong, typeof(Boolean)),
                new DataColumn(SchemaTableOptionalColumn.IsReadOnly, typeof(Boolean)),
                new DataColumn(SchemaTableOptionalColumn.IsRowVersion, typeof(Boolean)),
                new DataColumn(SchemaTableColumn.IsUnique, typeof(Boolean)),
                new DataColumn(SchemaTableColumn.NumericPrecision, typeof(Int32)),
                new DataColumn(SchemaTableColumn.NumericScale, typeof(Int32)),
                new DataColumn(SchemaTableColumn.ProviderType, typeof(Type))
            });
            return schemaTable;
        }

        /// <summary>
        /// Gets whether the underlying data source is closed.
        /// </summary>
        public bool IsClosed { get; private set; }

        bool IDataReader.NextResult()
        {
            return false;
        }

        /// <summary>
        /// Advances the reader to the next record.
        /// </summary>
        /// <returns>True if there was another record; otherwise, false.</returns>
        public bool Read()
        {
            if (Reader.Read())
            {
                values = null;  // reset cache
                return true;
            }
            else
            {
                return false;
            }
        }

        int IDataReader.RecordsAffected => 0;

        /// <summary>
        /// Gets the number of fields in the current record.
        /// </summary>
        public int FieldCount => GetSchema().ColumnDefinitions.Count;

        /// <summary>
        /// Gets the boolean value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The boolean value at the given index.</returns>
        public bool GetBoolean(int i)
        {
            var values = GetValues();
            return (bool)values[i];
        }

        /// <summary>
        /// Gets the byte value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The byte value at the given index.</returns>
        public byte GetByte(int i)
        {
            var values = GetValues();
            return (byte)values[i];
        }

        /// <summary>
        /// Copies the bytes from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <param name="fieldOffset">The offset into the byte array to start copying.</param>
        /// <param name="buffer">An array to copy the bytes to.</param>
        /// <param name="bufferoffset">The offset into the given buffer to start copying.</param>
        /// <param name="length">The maximum number of items to copy into the given buffer.</param>
        /// <returns>The number of bytes copied to the buffer.</returns>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            var values = GetValues();
            var bytes = (byte[])values[i];
#if NET45
            Array.Copy(bytes, fieldOffset, buffer, bufferoffset, length);
#else
            Array.Copy(bytes, (int)fieldOffset, buffer, bufferoffset, length);
#endif
            return Math.Min(bytes.Length - fieldOffset, length);
        }

        /// <summary>
        /// Gets the char value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The char value at the given index.</returns>
        public char GetChar(int i)
        {
            var values = GetValues();
            return (char)values[i];
        }

        /// <summary>
        /// Copies the chars from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <param name="fieldoffset">The offset into the char array to start copying.</param>
        /// <param name="buffer">An array to copy the chars to.</param>
        /// <param name="bufferoffset">The offset into the given buffer to start copying.</param>
        /// <param name="length">The maximum number of items to copy into the given buffer.</param>
        /// <returns>The number of chars copied to the buffer.</returns>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            var values = GetValues();
            var chars = (char[])values[i];
#if NET45
            Array.Copy(buffer, fieldoffset, buffer, bufferoffset, length);
#else
            Array.Copy(buffer, (int)fieldoffset, buffer, bufferoffset, length);
#endif
            return Math.Min(chars.Length - fieldoffset, length);
        }

        IDataReader IDataRecord.GetData(int i)
        {
            throw new InvalidCastException();
        }

        /// <summary>
        /// Gets the type name of the value of the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The type name.</returns>
        public string GetDataTypeName(int i)
        {
            var schema = GetSchema();
            return schema.ColumnDefinitions[i].ColumnType.Name;
        }

        /// <summary>
        /// Gets the DateTime value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The DateTime value at the given index.</returns>
        public DateTime GetDateTime(int i)
        {
            var values = GetValues();
            return (DateTime)values[i];
        }

        /// <summary>
        /// Gets the DateTime value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The DateTime value at the given index.</returns>
        public DateTimeOffset GetDateTimeOffset(int i)
        {
            var values = GetValues();
            return (DateTimeOffset)values[i];
        }

        /// <summary>
        /// Gets the decimal value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The decimal value at the given index.</returns>
        public decimal GetDecimal(int i)
        {
            var values = GetValues();
            return (decimal)values[i];
        }

        /// <summary>
        /// Gets the double value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The double value at the given index.</returns>
        public double GetDouble(int i)
        {
            var values = GetValues();
            return (double)values[i];
        }

        /// <summary>
        /// Gets the type of the value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The type of the value at the given index.</returns>
        public Type GetFieldType(int i)
        {
            var schema = GetSchema();
            return schema.ColumnDefinitions[i].ColumnType;
        }

        /// <summary>
        /// Gets the float value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The float value at the given index.</returns>
        public float GetFloat(int i)
        {
            var values = GetValues();
            return (float)values[i];
        }

        /// <summary>
        /// Gets the GUID from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The GUID at the given index.</returns>
        public Guid GetGuid(int i)
        {
            var values = GetValues();
            return (Guid)values[i];
        }

        /// <summary>
        /// Gets the short value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The short value at the given index.</returns>
        public short GetInt16(int i)
        {
            var values = GetValues();
            return (short)values[i];
        }

        /// <summary>
        /// Gets the int value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The int value at the given index.</returns>
        public int GetInt32(int i)
        {
            var values = GetValues();
            return (int)values[i];
        }

        /// <summary>
        /// Gets the long value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The long value at the given index.</returns>
        public long GetInt64(int i)
        {
            var values = GetValues();
            return (long)values[i];
        }

        /// <summary>
        /// Gets the name of the column at the given index.
        /// </summary>
        /// <param name="i">The index of the column.</param>
        /// <returns>The name of the column at the given index.</returns>
        public string GetName(int i)
        {
            var schema = GetSchema();
            return schema.ColumnDefinitions[i].ColumnName;
        }

        /// <summary>
        /// Gets the index of the column with the given name.
        /// </summary>
        /// <param name="name">The name of the column..</param>
        /// <returns>The index of the column with the given name.</returns>
        public int GetOrdinal(string name)
        {
            var schema = GetSchema();
            return schema.GetOrdinal(name);
        }

        /// <summary>
        /// Gets the sbyte value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The sbyte value at the given index.</returns>
        public sbyte GetSByte(int i)
        {
            var values = GetValues();
            return (sbyte)values[i];
        }

        /// <summary>
        /// Gets the string from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The string at the given index.</returns>
        public string GetString(int i)
        {
            var values = GetValues();
            var value = (string)values[i];
            if (value == null && !Options.IsNullStringAllowed)
            {
                throw new InvalidCastException();
            }
            return value;
        }

        /// <summary>
        /// Gets the <see cref="TimeSpan"/> from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The string at the given index.</returns>
        public TimeSpan GetTimeSpan(int i)
        {
            var values = GetValues();
            return (TimeSpan)values[i];
        }

        /// <summary>
        /// Gets the unsigned short value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The unsigned short value at the given index.</returns>
        public ushort GetUInt16(int i)
        {
            var values = GetValues();
            return (ushort)values[i];
        }

        /// <summary>
        /// Gets the unsigned int value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The unsigned int value at the given index.</returns>
        public uint GetUInt32(int i)
        {
            var values = GetValues();
            return (uint)values[i];
        }

        /// <summary>
        /// Gets the unsigned long value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The unsigned long value at the given index.</returns>
        public ulong GetUInt64(int i)
        {
            var values = GetValues();
            return (ulong)values[i];
        }

        /// <summary>
        /// Gets the value as an object from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The value as an object at the given index.</returns>
        public object GetValue(int i)
        {
            var values = GetValues();
            var value = values[i];
            if (value == null && Options.IsDBNullReturned)
            {
                value = DBNull.Value;
            }
            return value;
        }

        /// <summary>
        /// Copies the values from the current record to the given array.
        /// </summary>
        /// <param name="values">The array to copy the values to.</param>
        /// <returns>The number of values copied to the given array.</returns>
        public int GetValues(object[] values)
        {
            var sources = GetValues();
            var length = Math.Min(sources.Length, values.Length);
            Array.Copy(sources, values, length);
            if (Options.IsDBNullReturned)
            {
                for (int index = 0; index != length; ++index)
                {
                    if (values[index] == null)
                    {
                        values[index] = DBNull.Value;
                    }
                }
            }
            return length;
        }

        /// <summary>
        /// Gets whether the value at given index is null for the current record.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>True if the value is null; otherwise, false.</returns>
        public bool IsDBNull(int i)
        {
            var values = GetValues();
            return values[i] == null;
        }

        /// <summary>
        /// Gets the value from the current record in the column with the given name.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value in the column with the given name.</returns>
        public object this[string name]
        {
            get 
            {  
                var schema = GetSchema();
                var index = schema.GetOrdinal(name);
                return GetValue(index);
            }
        }

        /// <summary>
        /// Gets the value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The value at the given index.</returns>
        public object this[int i]
        {
            get { return GetValue(i); }
        }

        private ISchema GetSchema()
        {
            if (schema == null)
            {
                schema = Reader.GetSchema();
            }
            return schema;
        }

        private object[] GetValues()
        {
            if (values == null)
            {
                values = Reader.GetValues();
            }
            return values;
        }
    }
}
#endif
