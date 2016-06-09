using System;
using System.Collections.Generic;
using System.Text;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordParser
    {
        private readonly RetryReader reader;
        private readonly SeparatedValueOptions options;
        private List<string> values;

        public SeparatedValueRecordParser(RetryReader reader, SeparatedValueOptions options)
        {
            this.reader = reader;
            this.options = options.Clone();
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
            if (reader.Current == options.Quote)
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
                if (reader.Current != options.Quote)
                {
                    // Keep adding characters until we find a closing quote
                    tokenChars.Add(reader.Current);
                }
                else if (reader.IsMatch(options.Quote))
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
            if (options.RecordSeparator.Length > options.Separator.Length)
            {
                if (reader.IsMatch(options.RecordSeparator))
                {
                    return TokenType.EndOfRecord;
                }
                else if (reader.IsMatch(options.Separator))
                {
                    return TokenType.EndOfToken;
                }
            }
            else if (options.Separator.Length > options.RecordSeparator.Length)
            {
                if (reader.IsMatch(options.Separator))
                {
                    return TokenType.EndOfToken;
                }
                else if (reader.IsMatch(options.RecordSeparator))
                {
                    return TokenType.EndOfRecord;
                }
            }
            else if (reader.IsMatch(options.RecordSeparator))
            {
                return TokenType.EndOfRecord;
            }
            else if (reader.IsMatch(options.Separator))
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
    }
}
