using System;
using System.Text.RegularExpressions;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Holds configuration options for the ExcelParser. 
    /// </summary>
    public class ExcelOptions
    {
        private static readonly Regex columnRegex = new Regex(@"[^A-Z]");
        private int? startingRow;
        private int? endingRow;
        private string startingColumn;
        private string endingColumn;

        /// <summary>
        /// Initializes a new instance of an ExcelOptions.
        /// </summary>
        /// <param name="worksheetName">The name of the worksheet being read from.</param>
        public ExcelOptions(string worksheetName)
        {
            if (String.IsNullOrWhiteSpace(worksheetName))
            {
                throw new ArgumentException(Resources.InvalidWorksheetName, "worksheetName");
            }
            WorksheetName = worksheetName;
        }

        /// <summary>
        /// Gets the name of the worksheet to read the data from.
        /// </summary>
        public string WorksheetName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the first row to read from in the worksheet.
        /// </summary>
        /// <remarks>Excel rows start at 1, not 0.</remarks>
        public int? StartingRow
        {
            get { return startingRow; }
            set
            {
                if (value != null && value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, Resources.InvalidStartingRow);
                }
                startingRow = value;
            }
        }

        /// <summary>
        /// Gets or sets the last row to read from in the worksheet.
        /// </summary>
        /// <remarks>Excel rows start at 1, not 0.</remarks>
        public int? EndingRow
        {
            get { return endingRow; }
            set
            {
                if (value != null && value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, Resources.InvalidEndingRow);
                }
                endingRow = value;
            }
        }

        /// <summary>
        /// Gets or sets the first column to read from the worksheet.
        /// </summary>
        /// <remarks>The name of the column must be a sequence of uppercase letters -or- null.</remarks>
        public string StartingColumn
        {
            get { return startingColumn; }
            set
            {
                if (value != null && (value == String.Empty || columnRegex.IsMatch(value)))
                {
                    throw new ArgumentException(Resources.InvalidStartingColumn, "value");
                }
                startingColumn = value;
            }
        }

        /// <summary>
        /// Gets or sets the last column to read from the worksheet.
        /// </summary>
        /// <remarks>The name of the column must be a sequence of uppercase letters -or- null.</remarks>
        public string EndingColumn
        {
            get { return endingColumn; }
            set
            {
                if (value != null && (value == String.Empty || columnRegex.IsMatch(value)))
                {
                    throw new ArgumentException(Resources.InvalidStartingColumn, "value");
                }
                endingColumn = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the first record is the schema.
        /// </summary>
        public bool IsFirstRecordSchema { get; set; }
    }
}
