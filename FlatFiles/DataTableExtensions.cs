#if NET451 || NETSTANDARD2_0 || NETCOREAPP
using System;
using System.Data;
using System.Linq;

namespace FlatFiles
{
    /// <summary>
    /// Provides extensions methods for populating a DataTable using flat files.
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Loads the contents returned by the given reader into the DataTable.
        /// </summary>
        /// <param name="table">The table to load the file contents into.</param>
        /// <param name="reader">The reader to use to extract the file schema and data.</param>
        /// <param name="loadOption">Controls how values from the flat file will be applied to existing rows.</param>
        /// <param name="errorHandler">A <see cref="FillErrorEventHandler"/> delegate to call when an error occurs while loading data.</param>
        /// <exception cref="ArgumentNullException">The table is null.</exception>
        /// <exception cref="ArgumentNullException">The reader is null.</exception>
        public static void ReadFlatFile(this DataTable table, IReader reader, LoadOption loadOption = LoadOption.PreserveChanges, FillErrorEventHandler errorHandler = null)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            var fileReader = new FlatFileDataReader(reader);
            table.Load(fileReader, loadOption, errorHandler);
        }

        /// <summary>
        /// Writes the data table contents to the writer.
        /// </summary>
        /// <param name="table">The table whose contents to write to the writer.</param>
        /// <param name="writer">The writer to write the values to.</param>
        /// <exception cref="ArgumentNullException">The table is null.</exception>
        /// <exception cref="ArgumentNullException">The writer is null.</exception>
        public static void WriteFlatFile(this DataTable table, IWriter writer)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            var schema = writer.GetSchema();
            var columnIndexes = schema.ColumnDefinitions
                .Where(c => !c.IsIgnored)
                .Select(c => table.Columns.IndexOf(c.ColumnName))
                .ToArray();
            var values = new object[columnIndexes.Length];
            foreach (DataRow row in table.Rows)
            {
                for (int index = 0; index != values.Length; ++index)
                {
                    int columnIndex = columnIndexes[index];
                    if (columnIndex != -1)
                    {
                        values[index] = row.IsNull(columnIndex) ? null : row[columnIndex];
                    }
                }
                writer.Write(values);
            }
        }
    }
}
#endif