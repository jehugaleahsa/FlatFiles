using System;
using System.Collections.Generic;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordParser : IDisposable
    {
        private readonly RetryReader reader;
        private readonly string eor;
        private readonly string eot;
        private readonly char quote;
        private List<string> values;

        public SeparatedValueRecordParser(RetryReader reader, string eor, string eot, char quote)
        {
            this.reader = reader;
            this.eor = eor;
            this.eot = eot;
            this.quote = quote;
        }

        public Encoding Encoding
        {
            get { return reader.Encoding; }
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
            bool isQuoted = isTokenQuoted();
            if (isQuoted)
            {
                return getQuotedToken();
            }
            else
            {
                return getUnquotedToken();
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

        private bool isTokenQuoted()
        {
            reader.Read();
            if (reader.Current == quote)
            {
                return true;
            }
            else
            {
                reader.Undo(reader.Current);
                return false;
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

        private TokenType getQuotedToken()
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

        private enum TokenType
        {
            Normal,
            EndOfStream,
            EndOfRecord,
            EndOfToken
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
