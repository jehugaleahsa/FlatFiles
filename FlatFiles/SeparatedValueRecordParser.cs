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
        private readonly IMatcher separatorMatcher;
        private readonly IMatcher recordSeparatorMatcher;
        private readonly IMatcher postfixMatcher;
        private List<string> values;
        private StringBuilder token;

        public SeparatedValueRecordParser(RetryReader reader, SeparatedValueOptions options)
        {
            this.reader = reader;
            this.options = options.Clone();
            this.separatorMatcher = getMatcher(options.Separator);
            this.recordSeparatorMatcher = getMatcher(options.RecordSeparator);
            if (options.RecordSeparator.StartsWith(options.Separator))
            {
                string postfix = options.RecordSeparator.Substring(options.Separator.Length);
                this.postfixMatcher = getMatcher(postfix);
            }
            this.token = new StringBuilder();
        }

        private IMatcher getMatcher(string separator)
        {
            if (separator.Length == 1)
            {
                return new Matcher1(reader, separator[0]);
            }
            else if (separator.Length == 2)
            {
                return new Matcher2(reader, separator[0], separator[1]);
            }
            else
            {
                return new Matcher(reader, separator);
            }
        }

        public bool EndOfStream
        {
            get { return reader.EndOfStream; }
        }

        private void addToken()
        {
            values.Add(token.ToString());
            token.Length = 0;
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
            if (!options.PreserveWhiteSpace)
            {
                TokenType tokenType = skipWhiteSpace();
                if (tokenType != TokenType.Normal)
                {
                    addToken();
                    return tokenType;
                }
            }
            if (reader.IsMatch1(options.Quote))
            {
                return getQuotedToken();
            }
            else
            {
                return getUnquotedToken();
            }
        }

        private TokenType getUnquotedToken()
        {
            TokenType tokenType = getSeparator();
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
            addToken();
            return tokenType;
        }

        private TokenType getQuotedToken()
        {
            TokenType tokenType = TokenType.Normal;
            while (tokenType == TokenType.Normal && reader.Read())
            {
                if (reader.Current != options.Quote)
                {
                    // Keep adding characters until we find a closing quote
                    token.Append(reader.Current);
                }
                else if (reader.IsMatch1(options.Quote))
                {
                    // Escaped quote (two quotes in a row)
                    token.Append(reader.Current);
                }
                else
                {
                    if (options.PreserveWhiteSpace)
                    {
                        // We've encountered a stand-alone quote.
                        // We go looking for a separator, keeping any leading whitespace.
                        tokenType = appendWhiteSpace();
                    }
                    else
                    {
                        // We've encountered a stand-alone quote.
                        // We go looking for a separator, skipping any leading whitespace.
                        tokenType = skipWhiteSpace();
                    }
                    // If we find anything other than a separator, it's a syntax error.
                    if (tokenType != TokenType.Normal)
                    {
                        addToken();
                        return tokenType;
                    }
                }
            }
            throw new SeparatedValueSyntaxException(Resources.UnmatchedQuote);
        }

        private TokenType skipWhiteSpace()
        {
            TokenType tokenType = getSeparator();
            while (tokenType == TokenType.Normal && reader.IsMatch(Char.IsWhiteSpace))
            {
                tokenType = getSeparator();
            }
            return tokenType;
        }

        private TokenType appendWhiteSpace()
        {
            TokenType tokenType = getSeparator();
            while (tokenType == TokenType.Normal && reader.IsMatch(Char.IsWhiteSpace))
            {
                token.Append(reader.Current);
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
            else if (separatorMatcher.IsMatch())
            {
                // This code handles the case where the separator is a substring of the record separator.
                // We check to see if the remaining characters make up the record separator.
                if (postfixMatcher != null && postfixMatcher.IsMatch())
                {
                    return TokenType.EndOfRecord;
                }
                else
                {
                    return TokenType.EndOfToken;
                }
            }
            else if (postfixMatcher == null && recordSeparatorMatcher.IsMatch())
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

        private interface IMatcher
        {
            bool IsMatch();
        }

        private sealed class Matcher1 : IMatcher
        {
            private readonly RetryReader reader;
            private readonly char first;

            public Matcher1(RetryReader reader, char first)
            {
                this.reader = reader;
                this.first = first;
            }

            public bool IsMatch()
            {
                return reader.IsMatch1(first);
            }
        }

        private sealed class Matcher2 : IMatcher
        {
            private readonly RetryReader reader;
            private readonly char first;
            private readonly char second;

            public Matcher2(RetryReader reader, char first, char second)
            {
                this.reader = reader;
                this.first = first;
                this.second = second;
            }

            public bool IsMatch()
            {
                return reader.IsMatch2(first, second);
            }
        }

        private sealed class Matcher : IMatcher
        {
            private readonly RetryReader reader;
            private readonly string separator;

            public Matcher(RetryReader reader, string separator)
            {
                this.reader = reader;
                this.separator = separator;
            }

            public bool IsMatch()
            {
                return reader.IsMatch(separator);
            }
        }
    }
}
