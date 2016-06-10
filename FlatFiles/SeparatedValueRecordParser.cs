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
            StringBuilder token = new StringBuilder();
            TokenType tokenType = TokenType.Normal;
            while (tokenType == TokenType.Normal)
            {
                reader.Read();
                token.Append(reader.Current);
                tokenType = getSeparator();
            }
            while (Char.IsWhiteSpace(token[token.Length - 1]))
            {
                token.Length -= 1;
            }
            values.Add(token.ToString());
            return tokenType;
        }

        private TokenType getQuotedToken()
        {
            bool hasMatchingQuote = false;
            TokenType tokenType = TokenType.Normal;
            StringBuilder token = new StringBuilder();
            while (tokenType == TokenType.Normal && reader.Read())
            {
                if (reader.Current != options.Quote)
                {
                    // Keep adding characters until we find a closing quote
                    token.Append(reader.Current);
                }
                else if (reader.IsMatch(options.Quote))
                {
                    token.Append(reader.Current);
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
            values.Add(token.ToString());
            return tokenType;
        }

        private TokenType getSeparator()
        {
            if (reader.EndOfStream)
            {
                return TokenType.EndOfStream;
            }
            else if (reader.IsMatch(options.Separator))
            {
                if (options.RecordSeparator.StartsWith(options.Separator) && reader.IsMatch(options.RecordSeparator.Substring(options.Separator.Length)))
                {
                    return TokenType.EndOfRecord;
                }
                else
                {
                    return TokenType.EndOfToken;
                }
            }
            else if (reader.IsMatch(options.RecordSeparator))
            {
                return TokenType.EndOfRecord;
            }
            else
            {
                return TokenType.Normal;
            }
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
