#if NET45
using System;
using System.Data;

namespace FlatFiles
{
    /// <summary>
    /// Reads records from a flat file.
    /// </summary>
    public sealed class FlatFileReader : IDataReader
    {
        private readonly IReader parser;

        /// <summary>
        /// Initializes a new instance of a FlatFileParser.
        /// </summary>
        /// <param name="reader">The reader to use to parse the underlying file.</param>
        /// <exception cref="System.ArgumentNullException">The parser is null.</exception>
        public FlatFileReader(IReader reader)
        {
            this.parser = reader ?? throw new ArgumentNullException(nameof(parser));
        }

        /// <summary>
        /// Finalizes the FlatFileReader.
        /// </summary>
        ~FlatFileReader()
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
            ISchema schema = parser.GetSchema();
            DataTable schemaTable = new DataTable();
            schemaTable.Columns.AddRange(new DataColumn[] 
            {
                new DataColumn("AllowDBNull", typeof(Boolean)),
                new DataColumn("BaseCatalogName", typeof(String)),
                new DataColumn("BaseColumnName", typeof(String)),
                new DataColumn("BaseSchemaName", typeof(String)),
                new DataColumn("BaseServerName", typeof(String)),
                new DataColumn("BaseTableName", typeof(String)),
                new DataColumn("ColumnName", typeof(String)),
                new DataColumn("ColumnOrdinal", typeof(Int32)),
                new DataColumn("ColumnSize", typeof(Int32)),
                new DataColumn("DataTypeName", typeof(String)),
                new DataColumn("IsAliased", typeof(Boolean)),
                new DataColumn("IsAutoIncrement", typeof(Boolean)),
                new DataColumn("IsColumnSet", typeof(Boolean)),
                new DataColumn("IsExpression", typeof(Boolean)),
                new DataColumn("IsHidden", typeof(Boolean)),
                new DataColumn("IsIdentity", typeof(Boolean)),
                new DataColumn("IsKey", typeof(Boolean)),
                new DataColumn("IsLong", typeof(Boolean)),
                new DataColumn("IsReadOnly", typeof(Boolean)),
                new DataColumn("IsRowVersion", typeof(Boolean)),
                new DataColumn("IsUnique", typeof(Boolean)),
                new DataColumn("NumericPrecision", typeof(Int32)),
                new DataColumn("NumericScale", typeof(Int32)),
                new DataColumn("ProviderType", typeof(Type)),
                new DataColumn("UdtAssemblyQualifiedName", typeof(String)),
                new DataColumn("XmlSchemaCollectionDatabase", typeof(String)),
                new DataColumn("XmlSchemaCollectionName", typeof(String)),
                new DataColumn("XmlSchemaCollectionOwningSchema", typeof(String)),
            });
            for (int index = 0; index != schema.ColumnDefinitions.Count; ++index)
            {
                IColumnDefinition column = schema.ColumnDefinitions[index];
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
                    column.ColumnType,  // ProviderType
                    null,  // UdtAssemblyQualifiedName
                    null,  // XmlSchemaCollectionDatabase
                    null,  // XmlSchemaCollectionName
                    null,  // XmlSchemaCollectionOwningSchema
                };
                schemaTable.Rows.Add(values);
            }
            schemaTable.AcceptChanges();
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
            return parser.Read();
        }

        int IDataReader.RecordsAffected => 0;

        /// <summary>
        /// Gets the number of fields in the current record.
        /// </summary>
        public int FieldCount
        {
            get 
            {
                ISchema schema = parser.GetSchema();
                return schema.ColumnDefinitions.Count;
            }
        }

        /// <summary>
        /// Gets the boolean value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The boolean value at the given index.</returns>
        public bool GetBoolean(int i)
        {
            object[] values = parser.GetValues();
            return (bool)values[i];
        }

        /// <summary>
        /// Gets the byte value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The byte value at the given index.</returns>
        public byte GetByte(int i)
        {
            object[] values = parser.GetValues();
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
            object[] values = parser.GetValues();
            byte[] bytes = (byte[])values[i];
            Array.Copy(bytes, fieldOffset, buffer, bufferoffset, length);
            return Math.Min(bytes.Length - fieldOffset, length);
        }

        /// <summary>
        /// Gets the char value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The char value at the given index.</returns>
        public char GetChar(int i)
        {
            object[] values = parser.GetValues();
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
            object[] values = parser.GetValues();
            char[] chars = (char[])values[i];
            Array.Copy(chars, fieldoffset, buffer, bufferoffset, length);
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
            ISchema schema = parser.GetSchema();
            return schema.ColumnDefinitions[i].ColumnType.Name;
        }

        /// <summary>
        /// Gets the DateTime value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The DateTime value at the given index.</returns>
        public DateTime GetDateTime(int i)
        {
            object[] values = parser.GetValues();
            return (DateTime)values[i];
        }

        /// <summary>
        /// Gets the decimal value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The decimal value at the given index.</returns>
        public decimal GetDecimal(int i)
        {
            object[] values = parser.GetValues();
            return (Decimal)values[i];
        }

        /// <summary>
        /// Gets the double value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The double value at the given index.</returns>
        public double GetDouble(int i)
        {
            object[] values = parser.GetValues();
            return (Double)values[i];
        }

        /// <summary>
        /// Gets the type of the value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The type of the value at the given index.</returns>
        public Type GetFieldType(int i)
        {
            ISchema schema = parser.GetSchema();
            return schema.ColumnDefinitions[i].ColumnType;
        }

        /// <summary>
        /// Gets the float value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The float value at the given index.</returns>
        public float GetFloat(int i)
        {
            object[] values = parser.GetValues();
            return (Single)values[i];
        }

        /// <summary>
        /// Gets the GUID from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The GUID at the given index.</returns>
        public Guid GetGuid(int i)
        {
            object[] values = parser.GetValues();
            return (Guid)values[i];
        }

        /// <summary>
        /// Gets the short value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The short value at the given index.</returns>
        public short GetInt16(int i)
        {
            object[] values = parser.GetValues();
            return (Int16)values[i];
        }

        /// <summary>
        /// Gets the int value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The int value at the given index.</returns>
        public int GetInt32(int i)
        {
            object[] values = parser.GetValues();
            return (Int32)values[i];
        }

        /// <summary>
        /// Gets the long value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The long value at the given index.</returns>
        public long GetInt64(int i)
        {
            object[] values = parser.GetValues();
            return (Int64)values[i];
        }

        /// <summary>
        /// Gets the name of the column at the given index.
        /// </summary>
        /// <param name="i">The index of the column.</param>
        /// <returns>The name of the column at the given index.</returns>
        public string GetName(int i)
        {
            ISchema schema = parser.GetSchema();
            return schema.ColumnDefinitions[i].ColumnName;
        }

        /// <summary>
        /// Gets the index of the column with the given name.
        /// </summary>
        /// <param name="name">The name of the column..</param>
        /// <returns>The index of the column with the given name.</returns>
        public int GetOrdinal(string name)
        {
            ISchema schema = parser.GetSchema();
            return schema.GetOrdinal(name);
        }

        /// <summary>
        /// Gets the string from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The string at the given index.</returns>
        public string GetString(int i)
        {
            object[] values = parser.GetValues();
            string value = (string)values[i];
            if (values == null)
            {
                throw new InvalidCastException();
            }
            return value;
        }

        /// <summary>
        /// Gets the value as an object from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The value as an object at the given index.</returns>
        public object GetValue(int i)
        {
            object[] values = parser.GetValues();
            object value = values[i];
            if (value == null)
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
            object[] sources = parser.GetValues();
            int length = Math.Min(sources.Length, values.Length);
            Array.Copy(sources, values, length);
            for (int index = 0; index != length; ++index)
            {
                if (values[index] == null)
                {
                    values[index] = DBNull.Value;
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
            object[] values = parser.GetValues();
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
                ISchema schema = parser.GetSchema();
                int index = schema.GetOrdinal(name);
                object[] values = parser.GetValues();
                object value = values[index];
                if (value == null)
                {
                    value = DBNull.Value;
                }
                return value;
            }
        }

        /// <summary>
        /// Gets the value from the current record at the given index.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The value at the given index.</returns>
        public object this[int i]
        {
            get
            {
                object[] values = parser.GetValues();
                object value = values[i];
                if (value == null)
                {
                    value = DBNull.Value;
                }
                return value;
            }
        }
    }
}
#endif