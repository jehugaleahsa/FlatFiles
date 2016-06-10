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
        private readonly string separatorPostfix;
        private readonly Func<string, bool> separatorMatcher;
        private readonly Func<string, bool> recordSeparatorMatcher;
        private readonly Func<string, bool> postfixMatcher;
        private List<string> values;

        public SeparatedValueRecordParser(RetryReader reader, SeparatedValueOptions options)
        {
            this.reader = reader;
            this.options = options.Clone();
            this.separatorMatcher = getMatcher(options.Separator);
            this.recordSeparatorMatcher = getMatcher(options.RecordSeparator);
            if (options.RecordSeparator.StartsWith(options.Separator))
            {
                this.separatorPostfix = options.RecordSeparator.Substring(options.Separator.Length);
                this.postfixMatcher = getMatcher(this.separatorPostfix);
            }
        }

        private Func<string, bool> getMatcher(string separator)
        {
            if (separator.Length == 1)
            {
                return reader.IsMatch1;
            }
            else if (separator.Length == 2)
            {
                return reader.IsMatch2;
            }
            else
            {
                return reader.IsMatch;
            }
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
            if (options.PreserveWhiteSpace)
            {
                TokenType tokenType = getSeparator();
                if (tokenType != TokenType.Normal)
                {
                    values.Add(String.Empty);
                    return tokenType;
                }
            }
            else
            {
                TokenType tokenType = skipLeadingWhiteSpace();
                if (tokenType != TokenType.Normal)
                {
                    values.Add(String.Empty);
                    return tokenType;
                }
            }
            if (reader.IsMatch(options.Quote))
            {
                return getQuotedToken();
            }
            else
            {
                return getUnquotedToken();
            }
        }

        private TokenType skipLeadingWhiteSpace()
        {
            TokenType tokenType = getSeparator();
            while (tokenType == TokenType.Normal && reader.IsMatch(Char.IsWhiteSpace))
            {
                tokenType = getSeparator();
            }
            return tokenType;
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
            if (!options.PreserveWhiteSpace)
            {
                while (Char.IsWhiteSpace(token[token.Length - 1]))
                {
                    token.Length -= 1;
                }
            }
            values.Add(token.ToString());
            return tokenType;
        }

        private TokenType getQuotedToken()
        {
            TokenType tokenType = TokenType.Normal;
            StringBuilder token = new StringBuilder();
            while (tokenType == TokenType.Normal)
            {
                if (!reader.Read())
                {
                    tokenType = TokenType.EndOfStream;
                }
                else if (reader.Current != options.Quote)
                {
                    // Keep adding characters until we find a closing quote
                    token.Append(reader.Current);
                }
                else if (reader.IsMatch(options.Quote))
                {
                    token.Append(reader.Current);
                }
                else if (options.PreserveWhiteSpace)
                {
                    // We've encountered a stand-alone quote.
                    // We go looking for a separator, keeping any leading whitespace.
                    tokenType = getSeparator();
                    while (tokenType == TokenType.Normal && reader.IsMatch(Char.IsWhiteSpace))
                    {
                        token.Append(reader.Current);
                        tokenType = getSeparator();
                    }
                    if (tokenType == TokenType.Normal)
                    {
                        break;
                    }
                    values.Add(token.ToString());
                    return tokenType;
                }
                else
                {
                    // We've encountered a stand-alone quote.
                    // We go looking for a separator, skipping any leading whitespace.
                    tokenType = skipLeadingWhiteSpace();
                    if (tokenType == TokenType.Normal)
                    {
                        break;
                    }
                    // If we find anything other than a separator, it's a syntax error.
                    values.Add(token.ToString());
                    return tokenType;
                }
            }
            throw new SeparatedValueSyntaxException(Resources.UnmatchedQuote);
        }

        private TokenType getSeparator()
        {
            if (reader.EndOfStream)
            {
                return TokenType.EndOfStream;
            }
            else if (separatorMatcher(options.Separator))
            {
                // This code handles the case where the separator is a substring of the record separator.
                // We check to see if the remaining characters make up the record separator.
                if (separatorPostfix != null && postfixMatcher(separatorPostfix))
                {
                    return TokenType.EndOfRecord;
                }
                else
                {
                    return TokenType.EndOfToken;
                }
            }
            else if (separatorPostfix == null && recordSeparatorMatcher(options.RecordSeparator))
            {
                // If the separator is a substring of the record separator and we didn't find it,
                // we won't find the record separator either.
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
