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
        /// Loads the contents returned by the given parser into the DataTable.
        /// </summary>
        /// <param name="table">The table to load the file contents into.</param>
        /// <param name="parser">The parser to use to extract the file schema and data.</param>
        /// <exception cref="System.ArgumentNullException">The table is null.</exception>
        /// <exception cref="System.ArgumentNullException">The parser is null.</exception>
        public static void ReadFlatFile(this DataTable table, IReader parser)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }
            table.Reset();
            FlatFileReader reader = new FlatFileReader(parser);
            table.Load(reader, LoadOption.OverwriteChanges);
        }
    }
}
