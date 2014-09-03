using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Extracts records from an Excel file.
    /// </summary>
    public class ExcelReader : IReader
    {
        private readonly OleDbConnection connection;
        private readonly OleDbCommand command;
        private readonly OleDbDataReader dataReader;
        private readonly ExcelSchema schema;
        private readonly ExcelOptions options;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of an ExcelReader.
        /// </summary>
        /// <param name="fileName">The name of the file to read.</param>
        /// <param name="options">The options for configuring the reader's behavior.</param>
        public ExcelReader(string fileName, ExcelOptions options)
            : this(fileName, null, options, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of an ExcelReader.
        /// </summary>
        /// <param name="fileName">The name of the file to read.</param>
        /// <param name="schema">The predefined schema for the records.</param>
        /// <param name="options">The options for configuring the reader's behavior.</param>
        public ExcelReader(string fileName, ExcelSchema schema, ExcelOptions options)
            : this(fileName, schema, options, true)
        {
        }

        private ExcelReader(string fileName, ExcelSchema schema, ExcelOptions options, bool hasSchema)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(Resources.FileNotFound, fileName);
            }
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            this.options = options;

            string connectionString = ExcelHelpers.GetConnectionString(fileName);
            connection = new OleDbConnection(connectionString);
            connection.Open();
            command = connection.CreateCommand();
            command.CommandText = ExcelHelpers.GetSelectCommandText(schema, options);
            dataReader = command.ExecuteReader();

            if (hasSchema)
            {
                if (options.IsFirstRecordSchema)
                {
                    dataReader.Read();  // skip header record
                }
                this.schema = schema;
            }
            else if (options.IsFirstRecordSchema && dataReader.Read())
            {
                object[] values = new object[dataReader.FieldCount];
                dataReader.GetValues(values);
                int startingIndex = ExcelHelpers.GetExcelColumnIndex(options.StartingColumn ?? "A");
                this.schema = new ExcelSchema();
                for (int valueIndex = 0; valueIndex != values.Length; ++valueIndex)
                {
                    object value = values[valueIndex];
                    string columnName = getColumnName(startingIndex + valueIndex, value);
                    StringColumn column = new StringColumn(columnName);
                    this.schema.AddColumn(column);
                }
            }
        }

        private static string getColumnName(int index, object value)
        {
            if (value == null || value.GetType() != typeof(String))
            {
                return ExcelHelpers.GetExcelColumnName(index);
            }
            return value.ToString();
        }

        /// <summary>
        /// Finalizes the ExcelParser.
        /// </summary>
        ~ExcelReader()
        {
            dispose(false);
        }

        /// <summary>
        /// Releases any resources being held by the parser.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        private void dispose(bool disposing)
        {
            if (disposing)
            {
                dataReader.Dispose();
                command.Dispose();
                connection.Dispose();
            }
            isDisposed = true;
        }

        /// <summary>
        /// Gets the names of the columns found in the file.
        /// </summary>
        /// <returns>The names.</returns>
        public ISchema GetSchema()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("ExcelReader");
            }
            if (schema == null)
            {
                throw new InvalidOperationException(Resources.SchemaNotDefined);
            }
            return schema;
        }

        /// <summary>
        /// Attempts to read the next record from the stream.
        /// </summary>
        /// <returns>True if the next record was read or false if all records have been read.</returns>
        public bool Read()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("ExcelReader");
            }
            return dataReader.Read();
        }

        /// <summary>
        /// Gets the values for the current record.
        /// </summary>
        /// <returns>The values of the current record.</returns>
        public object[] GetValues()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("ExcelReader");
            }
            if (schema == null)
            {
                object[] values = new object[dataReader.FieldCount];
                dataReader.GetValues(values);
                return values;
            }
            return schema.ParseValues(dataReader);
        }
    }
}
