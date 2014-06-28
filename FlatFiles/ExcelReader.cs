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

            string connectionString = getConnectionString(fileName);
            connection = new OleDbConnection(connectionString);
            connection.Open();
            command = connection.CreateCommand();
            command.CommandText = getCommandText(schema, options);
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
                string[] columnNames = new string[dataReader.FieldCount];
                dataReader.GetValues(columnNames);
                this.schema = new ExcelSchema();
                foreach (string columnName in columnNames)
                {
                    StringColumn column = new StringColumn(columnName);
                    this.schema.AddColumn(column);
                }
            }
        }

        private string getConnectionString(string fileName)
        {
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();

            string[] newExtensions = new string[]
            {
                ".xlsx", ".xlsb", ".xlsm"
            };
            string[] oldExtensions = new string[]
            {
                ".xsl"
            };
            string extension = Path.GetExtension(fileName);
            if (newExtensions.Contains(extension))
            {
                builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                builder.Add("Extended Properties", "Excel 12.0 Xml; HDR=No; READONLY=true; IMEX=1");
            }
            else if (oldExtensions.Contains(extension))
            {
                builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                builder.Add("Extended Properties", "Excel 8.0; HDR=No; READONLY=true; IMEX=1");
            }
            else
            {
                throw new ArgumentException(Resources.UnknownExcelExtension, "fileName");
            }

            builder.DataSource = fileName;
            return builder.ConnectionString;
        }

        private static string getCommandText(ExcelSchema schema, ExcelOptions options)
        {
            StringBuilder commandBuilder = new StringBuilder();
            commandBuilder.Append("SELECT * FROM [");
            commandBuilder.Append(options.WorksheetName);
            commandBuilder.Append("$");

            if (options.StartingRow != null || options.EndingRow != null || options.StartingColumn != null || options.EndingColumn != null)
            {
                commandBuilder.Append(options.StartingColumn ?? "A");
                commandBuilder.Append(options.StartingRow ?? 1);
                commandBuilder.Append(":");
                commandBuilder.Append(getEndingColumn(schema, options));
                if (options.EndingRow != null)
                {
                    commandBuilder.Append(options.EndingRow.Value);
                }
            }

            commandBuilder.Append("]");
            return commandBuilder.ToString();
        }

        private static string getEndingColumn(ExcelSchema schema, ExcelOptions options)
        {
            if (options.EndingColumn != null)
            {
                return options.EndingColumn;
            }
            if (schema == null)
            {
                return getExcelColumnName(16384);
            }
            int fieldCount = schema.ColumnDefinitions.Count;
            int startIndex = getExcelColumnIndex(options.StartingColumn ?? "A");
            int endIndex = startIndex + fieldCount - 1;
            string endingColumn = getExcelColumnName(endIndex);
            return endingColumn;
        }

        private static string getExcelColumnName(int columnIndex)
        {
            const int letterCount = 26;

            int dividend = columnIndex;
            List<char> characters = new List<char>();

            while (dividend > 0)
            {
                int modulo = (dividend - 1) % letterCount;
                characters.Add((char)('A' + modulo));
                dividend = (dividend - modulo) / letterCount;
            }

            characters.Reverse();
            return new String(characters.ToArray());
        }

        private static int getExcelColumnIndex(string columnName)
        {
            const int letterCount = 26;

            int index = 0;
            for (int charIndex = 0; charIndex != columnName.Length; ++charIndex)
            {
                index *= letterCount;
                char next = columnName[charIndex];
                index += next - 'A' + 1;
            }
            return index;
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
