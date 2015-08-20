using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal static class ExcelHelpers
    {
        public static string GetConnectionString(string fileName)
        {
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();

            string[] newExtensions = new string[]
            {
                ".xlsx", ".xlsb", ".xlsm"
            };
            string[] oldExtensions = new string[]
            {
                ".xls"
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

        public static string GetSelectCommandText(ExcelSchema schema, ExcelOptions options)
        {
            StringBuilder commandBuilder = new StringBuilder();
            commandBuilder.Append("SELECT * FROM [");
            commandBuilder.Append(getWorksheetName(options));
            commandBuilder.Append(getDataRange(schema, options));
            commandBuilder.Append("]");
            return commandBuilder.ToString();
        }

        private static string getWorksheetName(ExcelOptions options)
        {
            return options.WorksheetName + "$";
        }

        private static string getDataRange(ExcelSchema schema, ExcelOptions options)
        {
            StringBuilder builder = new StringBuilder();
            if (options.StartingRow != null || options.EndingRow != null || options.StartingColumn != null || options.EndingColumn != null)
            {
                builder.Append(options.StartingColumn ?? "A");
                builder.Append(options.StartingRow ?? 1);
                builder.Append(":");
                builder.Append(getEndingColumn(schema, options));
                if (options.EndingRow != null)
                {
                    builder.Append(options.EndingRow.Value);
                }
            }
            return builder.ToString();
        }

        private static string getEndingColumn(ExcelSchema schema, ExcelOptions options)
        {
            if (options.EndingColumn != null)
            {
                return options.EndingColumn;
            }
            if (schema == null)
            {
                return GetExcelColumnName(16384);
            }
            int fieldCount = schema.ColumnDefinitions.Count;
            int startIndex = GetExcelColumnIndex(options.StartingColumn ?? "A");
            int endIndex = startIndex + fieldCount - 1;
            string endingColumn = GetExcelColumnName(endIndex);
            return endingColumn;
        }

        public static string GetExcelColumnName(int columnIndex)
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

        public static int GetExcelColumnIndex(string columnName)
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
    }
}
