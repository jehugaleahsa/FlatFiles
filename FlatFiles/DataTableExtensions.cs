#if NET45
using System;
using System.Data;

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
        /// <exception cref="System.ArgumentNullException">The table is null.</exception>
        /// <exception cref="System.ArgumentNullException">The reader is null.</exception>
        public static void ReadFlatFile(this DataTable table, IReader reader)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            table.Reset();
            FlatFileReader fileReader = new FlatFileReader(reader);
            table.Load(fileReader, LoadOption.OverwriteChanges);
        }

        /// <summary>
        /// Writes the data table contents to the writer.
        /// </summary>
        /// <param name="table">The table whose contents to write to the writer.</param>
        /// <param name="writer">The writer to write the values to.</param>
        /// <exception cref="System.ArgumentNullException">The table is null.</exception>
        /// <exception cref="System.ArgumentNullException">The writer is null.</exception>
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
            ISchema schema = writer.GetSchema();
            foreach (DataRow row in table.Rows)
            {
                object[] values = new object[schema.ColumnDefinitions.Count];
                for (int index = 0; index != values.Length; ++index)
                {
                    IColumnDefinition column = schema.ColumnDefinitions[index];
                    values[index] = row[column.ColumnName];
                }
                writer.Write(values);
            }
        }
    }
}
#endif