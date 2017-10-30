using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlatFiles.Resources;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordParser
    {
        private readonly RetryReader reader;
        private readonly SeparatedValueOptions options;
        private readonly ISeparatorMatcher separatorMatcher;
        private readonly ISeparatorMatcher recordSeparatorMatcher;
        private readonly ISeparatorMatcher postfixMatcher;
        private List<string> values;
        private StringBuilder token;

        public SeparatedValueRecordParser(RetryReader reader, SeparatedValueOptions options)
        {
            this.reader = reader;
            this.options = options.Clone();
            this.separatorMatcher = SeparatorMatcher.GetMatcher(reader, options.Separator);
            this.recordSeparatorMatcher = SeparatorMatcher.GetMatcher(reader, options.RecordSeparator);
            if (options.RecordSeparator != null && options.RecordSeparator.StartsWith(options.Separator))
            {
                string postfix = options.RecordSeparator.Substring(options.Separator.Length);
                this.postfixMatcher = SeparatorMatcher.GetMatcher(reader, postfix);
            }
            this.token = new StringBuilder();
        }

        internal SeparatedValueOptions Options
        {
            get { return options; }
        }

        public bool IsEndOfStream()
        {
            return reader.IsEndOfStream();
        }

        public async ValueTask<bool> IsEndOfStreamAsync()
        {
            return await reader.IsEndOfStreamAsync();
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

        public async Task<string[]> ReadRecordAsync()
        {
            values = new List<string>();
            TokenType tokenType = await getNextTokenAsync();
            while (tokenType == TokenType.EndOfToken)
            {
                tokenType = await getNextTokenAsync();
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

        private async ValueTask<TokenType> getNextTokenAsync()
        {
            if (!options.PreserveWhiteSpace)
            {
                TokenType tokenType = await skipWhiteSpaceAsync();
                if (tokenType != TokenType.Normal)
                {
                    addToken();
                    return tokenType;
                }
            }
            if (await reader.IsMatch1Async(options.Quote))
            {
                return await getQuotedTokenAsync();
            }
            else
            {
                return await getUnquotedTokenAsync();
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
            trimTokenEnd();
            addToken();
            return tokenType;
        }

        private async ValueTask<TokenType> getUnquotedTokenAsync()
        {
            TokenType tokenType = await getSeparatorAsync();
            while (tokenType == TokenType.Normal)
            {
                await reader.ReadAsync();
                token.Append(reader.Current);
                tokenType = await getSeparatorAsync();
            }
            trimTokenEnd();
            addToken();
            return tokenType;
        }

        private void trimTokenEnd()
        {
            if (options.PreserveWhiteSpace)
            {
                return;
            }
            if (Char.IsWhiteSpace(token[token.Length - 1]))
            {
                int trailingSize = 1;
                while (Char.IsWhiteSpace(token[token.Length - trailingSize - 1]))
                {
                    ++trailingSize;
                }
                token.Length -= trailingSize;
            }
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
            throw new SeparatedValueSyntaxException(SharedResources.UnmatchedQuote);
        }

        private async ValueTask<TokenType> getQuotedTokenAsync()
        {
            TokenType tokenType = TokenType.Normal;
            while (tokenType == TokenType.Normal && await reader.ReadAsync())
            {
                if (reader.Current != options.Quote)
                {
                    // Keep adding characters until we find a closing quote
                    token.Append(reader.Current);
                }
                else if (await reader.IsMatch1Async(options.Quote))
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
                        tokenType = await appendWhiteSpaceAsync();
                    }
                    else
                    {
                        // We've encountered a stand-alone quote.
                        // We go looking for a separator, skipping any leading whitespace.
                        tokenType = await skipWhiteSpaceAsync();
                    }
                    // If we find anything other than a separator, it's a syntax error.
                    if (tokenType != TokenType.Normal)
                    {
                        addToken();
                        return tokenType;
                    }
                }
            }
            throw new SeparatedValueSyntaxException(SharedResources.UnmatchedQuote);
        }

        private TokenType skipWhiteSpace()
        {
            TokenType tokenType = getSeparator();
            while (tokenType == TokenType.Normal && reader.IsWhitespace())
            {
                tokenType = getSeparator();
            }
            return tokenType;
        }

        private async ValueTask<TokenType> skipWhiteSpaceAsync()
        {
            TokenType tokenType = await getSeparatorAsync();
            while (tokenType == TokenType.Normal && await reader.IsWhitespaceAsync())
            {
                tokenType = await getSeparatorAsync();
            }
            return tokenType;
        }

        private TokenType appendWhiteSpace()
        {
            TokenType tokenType = getSeparator();
            while (tokenType == TokenType.Normal && reader.IsWhitespace())
            {
                token.Append(reader.Current);
                tokenType = getSeparator();
            }
            return tokenType;
        }

        private async ValueTask<TokenType> appendWhiteSpaceAsync()
        {
            TokenType tokenType = await getSeparatorAsync();
            while (tokenType == TokenType.Normal && await reader.IsWhitespaceAsync())
            {
                token.Append(reader.Current);
                tokenType = await getSeparatorAsync();
            }
            return tokenType;
        }

        private TokenType getSeparator()
        {
            if (reader.IsEndOfStream())
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

        private async ValueTask<TokenType> getSeparatorAsync()
        {
            if (await reader.IsEndOfStreamAsync())
            {
                return TokenType.EndOfStream;
            }
            else if (await separatorMatcher.IsMatchAsync())
            {
                // This code handles the case where the separator is a substring of the record separator.
                // We check to see if the remaining characters make up the record separator.
                if (postfixMatcher != null && await postfixMatcher.IsMatchAsync())
                {
                    return TokenType.EndOfRecord;
                }
                else
                {
                    return TokenType.EndOfToken;
                }
            }
            else if (postfixMatcher == null && await recordSeparatorMatcher.IsMatchAsync())
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
