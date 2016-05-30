using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    /// <summary>
    /// Extracts records from a file that has values separated by a separator token.
    /// </summary>
    public sealed class SeparatedValueReader : IReader
    {
        private readonly RecordParser reader;
        private readonly SeparatedValueSchema schema;
        private int recordCount;
        private object[] values;
        private bool endOfFile;
        private bool hasError;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="fileName">The path of the file containing the records to parse.</param>
        public SeparatedValueReader(string fileName)
            : this(File.OpenRead(fileName), null, new SeparatedValueOptions(), false, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="fileName">The path of the file containing the records to extract.</param>
        /// <param name="schema">The predefined schema for the records.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public SeparatedValueReader(string fileName, SeparatedValueSchema schema)
            : this(File.OpenRead(fileName), schema, new SeparatedValueOptions(), true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="fileName">The path of the file containing the records to extract.</param>
        /// <param name="options">The options for configuring the parser's behavior.</param>
        /// <exception cref="System.ArgumentNullException">The options object is null.</exception>
        public SeparatedValueReader(string fileName, SeparatedValueOptions options)
            : this(File.OpenRead(fileName), null, options, false, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="fileName">The path of the file containing the records to extract.</param>
        /// <param name="schema">The predefined schema for the records.</param>
        /// <param name="options">The options for configuring the parser's behavior.</param>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options object is null.</exception>
        public SeparatedValueReader(string fileName, SeparatedValueSchema schema, SeparatedValueOptions options)
            : this(File.OpenRead(fileName), schema, options, true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        public SeparatedValueReader(Stream stream)
            : this(stream, null, new SeparatedValueOptions(), false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <param name="schema">The predefined schema for the records.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        public SeparatedValueReader(Stream stream, SeparatedValueSchema schema)
            : this(stream, schema, new SeparatedValueOptions(), true, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <param name="options">The options for configuring the parser's behavior.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options is null.</exception>
        public SeparatedValueReader(Stream stream, SeparatedValueOptions options)
            : this(stream, null, options, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of a SeparatedValueParser.
        /// </summary>
        /// <param name="stream">A stream containing the records to parse.</param>
        /// <param name="schema">The predefined schema for the records.</param>
        /// <param name="options">The options for configuring the parser's behavior.</param>
        /// <exception cref="System.ArgumentNullException">The stream is null.</exception>
        /// <exception cref="System.ArgumentNullException">The schema is null.</exception>
        /// <exception cref="System.ArgumentNullException">The options is null.</exception>
        public SeparatedValueReader(Stream stream, SeparatedValueSchema schema, SeparatedValueOptions options)
            : this(stream, schema, options, true, false)
        {
        }

        private SeparatedValueReader(Stream stream, SeparatedValueSchema schema, SeparatedValueOptions options, bool hasSchema, bool ownsStream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (hasSchema && schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (options.RecordSeparator == options.Separator)
            {
                throw new ArgumentException(Resources.SameSeparator, "options");
            }
            RetryReader retryReader = new RetryReader(stream, options.Encoding ?? Encoding.Default, ownsStream);
            reader = new RecordParser(retryReader, options.RecordSeparator, options.Separator);
            if (hasSchema)
            {
                if (options.IsFirstRecordSchema)
                {
                    skip();  // skip header record
                }
                this.schema = schema;
            }
            else if (options.IsFirstRecordSchema)
            {
                string[] columnNames = readNextRecord();
                this.schema = new SeparatedValueSchema();
                foreach (string columnName in columnNames)
                {
                    StringColumn column = new StringColumn(columnName);
                    this.schema.AddColumn(column);
                }
            }
        }

        /// <summary>
        /// Finalizes the SeparatedValueParser.
        /// </summary>
        ~SeparatedValueReader()
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
                reader.Dispose();
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
                throw new ObjectDisposedException("SeparatedValueReader");
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
                throw new ObjectDisposedException("SeparatedValueReader");
            }
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            if (reader.EndOfStream)
            {
                endOfFile = true;
                values = null;
                return false;
            }
            string[] rawValues = readNextRecord();
            ++recordCount;
            if (schema == null)
            {
                values = rawValues;
            }
            else if (rawValues.Length != schema.ColumnDefinitions.Count)
            {
                hasError = true;
                throw new FlatFileException(recordCount);
            }
            else
            {
                values = schema.ParseValues(rawValues);
            }
            return true;
        }

        /// <summary>
        /// Attempts to skip the next record from the stream.
        /// </summary>
        /// <returns>True if the next record was skipped or false if all records have been read.</returns>
        /// <remarks>The previously parsed values remain available.</remarks>
        public bool Skip()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("SeparatedValueReader");
            }
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            bool result = skip();
            ++recordCount;
            return result;
        }

        private bool skip()
        {
            if (reader.EndOfStream)
            {
                endOfFile = true;
                values = null;
                return false;
            }
            try
            {
                reader.ReadRecord();
                return true;
            }
            catch (SeparatedValueSyntaxException exception)
            {
                throw new FlatFileException(recordCount, exception);
            }
        }

        /// <summary>
        /// Gets the values for the current record.
        /// </summary>
        /// <returns>The values of the current record.</returns>
        public object[] GetValues()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("SeparatedValueReader");
            }
            if (hasError)
            {
                throw new InvalidOperationException(Resources.ReadingWithErrors);
            }
            if (recordCount == 0)
            {
                throw new InvalidOperationException(Resources.ReadNotCalled);
            }
            if (endOfFile)
            {
                throw new InvalidOperationException(Resources.NoMoreRecords);
            }
            object[] copy = new object[values.Length];
            Array.Copy(values, copy, values.Length);
            return copy;
        }

        private string[] readNextRecord()
        {
            try
            {
                return reader.ReadRecord();
            }
            catch (SeparatedValueSyntaxException exception)
            {
                throw new FlatFileException(recordCount, exception);
            }
        }
        
        private class RecordParser : IDisposable
        {
            private readonly RetryReader reader;
            private readonly string eor;
            private readonly string eot;
            private List<string> values;

            public RecordParser(RetryReader reader, string eor, string eot)
            {
                this.reader = reader;
                this.eor = eor;
                this.eot = eot;
            }

            public bool EndOfStream
            {
                get { return reader.EndOfStream; }
            }

            public string[] ReadRecord()
            {
                values = new List<string>();
                TokenType tokenType = getNextToken();
                while (tokenType == TokenType.EndOfToken)
                {
                    tokenType = getNextToken();
                }
                return values.ToArray();
            }

            private TokenType getNextToken()
            {
                TokenType tokenType = skipLeadingWhitespace();
                if (tokenType != TokenType.Normal)
                {
                    values.Add(String.Empty);
                    return tokenType;
                }
                QuoteType quoteType = getQuoteType();
                if (quoteType == QuoteType.None)
                {
                    return getUnquotedToken();
                }
                else
                {
                    return getQuotedToken(reader.Current);
                }
            }

            private TokenType getUnquotedToken()
            {
                List<char> tokenChars = new List<char>();
                TokenType tokenType = TokenType.Normal;
                while (tokenType == TokenType.Normal)
                {
                    reader.Read();
                    tokenChars.Add(reader.Current);
                    tokenType = getSeparator();
                }
                string token = new String(tokenChars.ToArray());
                token = token.TrimEnd();
                values.Add(token);
                return tokenType;
            }

            private TokenType getQuotedToken(char quote)
            {
                bool hasMatchingQuote = false;
                TokenType tokenType = TokenType.Normal;
                List<char> tokenChars = new List<char>();
                while (tokenType == TokenType.Normal && reader.Read())
                {
                    if (reader.Current != quote)
                    {
                        // Keep adding characters until we find a closing quote
                        tokenChars.Add(reader.Current);
                    }
                    else if (reader.IsMatch(quote))
                    {
                        tokenChars.Add(reader.Current);
                    }
                    else
                    {
                        // We've encountered a stand-alone quote.
                        // We go looking for a separator, skipping any leading whitespace.
                        tokenType = skipLeadingWhitespace();
                        if (tokenType == TokenType.Normal)
                        {
                            // If we find anything other than a separator, it's a syntax error.
                            break;
                        }
                        hasMatchingQuote = true;
                    }
                }
                if (!hasMatchingQuote)
                {
                    throw new SeparatedValueSyntaxException(Resources.UnmatchedQuote);
                }
                string token = new String(tokenChars.ToArray());
                values.Add(token);
                return tokenType;
            }

            private QuoteType getQuoteType()
            {
                reader.Read();
                if (reader.Current == '"')
                {
                    return QuoteType.Double;
                }
                else if (reader.Current == '\'')
                {
                    return QuoteType.Single;
                }
                else
                {
                    reader.Undo(reader.Current);
                    return QuoteType.None;
                }
            }

            private TokenType skipLeadingWhitespace()
            {
                TokenType tokenType = getSeparator();
                while (tokenType == TokenType.Normal)
                {
                    reader.Read();
                    if (!Char.IsWhiteSpace(reader.Current))
                    {
                        reader.Undo(reader.Current);
                        return TokenType.Normal;
                    }
                    tokenType = getSeparator();
                }
                return tokenType;
            }

            private TokenType getSeparator()
            {
                if (reader.EndOfStream)
                {
                    return TokenType.EndOfStream;
                }
                if (eor.Length > eot.Length)
                {
                    if (reader.IsMatch(eor))
                    {
                        return TokenType.EndOfRecord;
                    }
                    else if (reader.IsMatch(eot))
                    {
                        return TokenType.EndOfToken;
                    }
                }
                else if (eot.Length > eor.Length)
                {
                    if (reader.IsMatch(eot))
                    {
                        return TokenType.EndOfToken;
                    }
                    else if (reader.IsMatch(eor))
                    {
                        return TokenType.EndOfRecord;
                    }
                }
                else if (reader.IsMatch(eor))
                {
                    return TokenType.EndOfRecord;
                }
                else if (reader.IsMatch(eot))
                {
                    return TokenType.EndOfToken;
                }
                return TokenType.Normal;
            }

            public enum TokenType
            {
                Normal,
                EndOfStream,
                EndOfRecord,
                EndOfToken
            }

            public enum QuoteType
            {
                None,
                Single,
                Double
            }

            public void Dispose()
            {
                reader.Dispose();
            }
        }
        
        private class RetryReader : IDisposable
        {
            private readonly StreamReader reader;
            private readonly bool ownsStream;
            private readonly Stack<char> retry;
            private char current;
            
            public RetryReader(Stream stream, Encoding encoding, bool ownsStream)
            {
                this.reader = new StreamReader(stream, encoding);
                this.ownsStream = ownsStream;
                this.retry = new Stack<char>();
            }

            ~RetryReader()
            {
                dispose(false);
            }
            
            public bool Read()
            {
                if (retry.Count > 0)
                {
                    current = retry.Pop();
                    return true;
                }
                if (reader.EndOfStream)
                {
                    return false;
                }
                current = (char)reader.Read();
                return true;
            }

            public bool EndOfStream
            {
                get
                {
                    return retry.Count == 0 && reader.EndOfStream;
                }
            }
            
            public char Current
            {
                get { return current; }
            }

            public bool IsMatch(char value)
            {
                if (retry.Count > 0)
                {
                    if (retry.Peek() == value)
                    {
                        current = retry.Pop();
                        return true;
                    }
                    return false;
                }
                if (!reader.EndOfStream && reader.Peek() == value)
                {
                    current = (char)reader.Read();
                    return true;
                }
                return false;
            }

            public bool IsMatch(string value)
            {
                // Optimized for two character separators.
                int position = 0;
                if (!IsMatch(value[position]))
                {
                    return false;
                }
                ++position;
                if (position == value.Length)
                {
                    return true;
                }
                if (!IsMatch(value[position]))
                {
                    Undo(value[0]);
                    return false;
                }
                ++position;
                if (position == value.Length)
                {
                    return true;
                }
                List<char> tail = new List<char>(value.Length);
                tail.Add(value[0]);
                tail.Add(value[1]);
                while (IsMatch(value[position]))
                {
                    tail.Add(current);
                    ++position;
                    if (position == value.Length)
                    {
                        return true;
                    }
                }
                Undo(tail);
                return false;
            }

            public void Undo(char item)
            {
                retry.Push(item);
            }
            
            public void Undo(List<char> items)
            {
                int position = items.Count;
                while (position != 0)
                {
                    --position;
                    retry.Push(items[position]);
                }
            }

            public void Dispose()
            {
                dispose(true);
            }

            private void dispose(bool isDisposing)
            {
                if (isDisposing && ownsStream)
                {
                    reader.Dispose();
                }
            }
        }
    }
}
