using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordParser
    {
        private readonly List<string> tokens = new List<string>();
        private readonly StringBuilder token = new StringBuilder();
        private readonly RetryReader reader;
        private readonly ISeparatorMatcher separatorMatcher;
        private readonly ISeparatorMatcher recordSeparatorMatcher;
        private readonly ISeparatorMatcher postfixMatcher;
        private readonly int separatorLength;

        public SeparatedValueRecordParser(RetryReader reader, SeparatedValueOptions options)
        {
            this.reader = reader;
            Options = options.Clone();
            separatorMatcher = SeparatorMatcher.GetMatcher(reader, options.Separator);
            recordSeparatorMatcher = SeparatorMatcher.GetMatcher(reader, options.RecordSeparator);
            if (options.RecordSeparator != null && options.RecordSeparator.StartsWith(options.Separator))
            {
                string postfix = options.RecordSeparator.Substring(options.Separator.Length);
                postfixMatcher = SeparatorMatcher.GetMatcher(reader, postfix);
            }
            separatorLength = Math.Max(Options.RecordSeparator?.Length ?? 2, Options.Separator.Length);
        }

        internal SeparatedValueOptions Options { get; }

        public bool IsEndOfStream()
        {
            if (reader.ShouldLoadBuffer(1))
            {
                reader.LoadBuffer();
            }
            return reader.IsEndOfStream();
        }

        public async ValueTask<bool> IsEndOfStreamAsync()
        {
            if (reader.ShouldLoadBuffer(1))
            {
                await reader.LoadBufferAsync().ConfigureAwait(false);
            }
            return reader.IsEndOfStream();
        }

        private void AddToken()
        {
            string value = token.ToString();
            tokens.Add(value);
            token.Clear();
        }

        public string[] ReadRecord()
        {
            TokenType tokenType = GetNextToken();
            while (tokenType == TokenType.EndOfToken)
            {
                tokenType = GetNextToken();
            }
            string[] results = tokens.ToArray();
            tokens.Clear();
            return results;
        }

        public async Task<string[]> ReadRecordAsync()
        {
            TokenType tokenType = await GetNextTokenAsync().ConfigureAwait(false);
            while (tokenType == TokenType.EndOfToken)
            {
                tokenType = await GetNextTokenAsync().ConfigureAwait(false);
            }
            string[] results = tokens.ToArray();
            tokens.Clear();
            return results;
        }

        private TokenType GetNextToken()
        {
            if (!Options.PreserveWhiteSpace)
            {
                TokenType tokenType = SkipWhiteSpace();
                if (tokenType != TokenType.Normal)
                {
                    AddToken();
                    return tokenType;
                }
            }
            if (reader.ShouldLoadBuffer(1))
            {
                reader.LoadBuffer();
            }
            if (reader.IsMatch1(Options.Quote))
            {
                return GetQuotedToken();
            }

            return GetUnquotedToken();
        }

        private async ValueTask<TokenType> GetNextTokenAsync()
        {
            if (!Options.PreserveWhiteSpace)
            {
                TokenType tokenType = await SkipWhiteSpaceAsync().ConfigureAwait(false);
                if (tokenType != TokenType.Normal)
                {
                    AddToken();
                    return tokenType;
                }
            }
            if (reader.ShouldLoadBuffer(1))
            {
                await reader.LoadBufferAsync().ConfigureAwait(false);
            }
            if (reader.IsMatch1(Options.Quote))
            {
                return await GetQuotedTokenAsync().ConfigureAwait(false);
            }

            return await GetUnquotedTokenAsync().ConfigureAwait(false);
        }

        private TokenType GetUnquotedToken()
        {
            if (reader.ShouldLoadBuffer(separatorLength))
            {
                reader.LoadBuffer();
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                reader.Read();
                token.Append(reader.Current);
                if (reader.ShouldLoadBuffer(separatorLength))
                {
                    reader.LoadBuffer();
                }
                tokenType = GetSeparator();
            }
            TrimTokenEnd();
            AddToken();
            return tokenType;
        }

        private async ValueTask<TokenType> GetUnquotedTokenAsync()
        {
            if (reader.ShouldLoadBuffer(separatorLength))
            {
                await reader.LoadBufferAsync().ConfigureAwait(false);
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                reader.Read();
                token.Append(reader.Current);
                if (reader.ShouldLoadBuffer(separatorLength))
                {
                    await reader.LoadBufferAsync().ConfigureAwait(false);
                }
                tokenType = GetSeparator();
            }
            TrimTokenEnd();
            AddToken();
            return tokenType;
        }

        private void TrimTokenEnd()
        {
            if (Options.PreserveWhiteSpace)
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

        private TokenType GetQuotedToken()
        {
            if (reader.ShouldLoadBuffer(1))
            {
                reader.LoadBuffer();
            }
            TokenType tokenType = TokenType.Normal;
            while (tokenType == TokenType.Normal && reader.Read())
            {
                if (reader.Current != Options.Quote)
                {
                    // Keep adding characters until we find a closing quote
                    token.Append(reader.Current);
                }
                else
                {
                    if (reader.ShouldLoadBuffer(1))
                    {
                        reader.LoadBuffer();
                    }
                    if (reader.IsMatch1(Options.Quote))
                    {
                        // Escaped quote (two quotes in a row)
                        token.Append(reader.Current);
                    }
                    else
                    {
                        tokenType = Options.PreserveWhiteSpace ? AppendWhiteSpace() : SkipWhiteSpace();
                        // If we find anything other than a separator, it's a syntax error.
                        if (tokenType == TokenType.Normal)
                        {
                            break;
                        }

                        AddToken();
                        return tokenType;
                    }
                }
                if (reader.ShouldLoadBuffer(1))
                {
                    reader.LoadBuffer();
                }
            }
            throw new SeparatedValueSyntaxException(Resources.UnmatchedQuote);
        }

        private async ValueTask<TokenType> GetQuotedTokenAsync()
        {
            if (reader.ShouldLoadBuffer(1))
            {
                await reader.LoadBufferAsync().ConfigureAwait(false);
            }
            TokenType tokenType = TokenType.Normal;
            while (tokenType == TokenType.Normal && reader.Read())
            {
                if (reader.Current != Options.Quote)
                {
                    // Keep adding characters until we find a closing quote
                    token.Append(reader.Current);
                }
                else
                {
                    if (reader.ShouldLoadBuffer(1))
                    {
                        await reader.LoadBufferAsync().ConfigureAwait(false);
                    }
                    if (reader.IsMatch1(Options.Quote))
                    {
                        // Escaped quote (two quotes in a row)
                        token.Append(reader.Current);
                    }
                    else
                    {
                        tokenType = await (Options.PreserveWhiteSpace ? AppendWhiteSpaceAsync() : SkipWhiteSpaceAsync()).ConfigureAwait(false);
                        // If we find anything other than a separator, it's a syntax error.
                        if (tokenType == TokenType.Normal)
                        {
                            break;
                        }

                        AddToken();
                        return tokenType;
                    }
                }
                if (reader.ShouldLoadBuffer(1))
                {
                    await reader.LoadBufferAsync().ConfigureAwait(false);
                }
            }
            throw new SeparatedValueSyntaxException(Resources.UnmatchedQuote);
        }

        private TokenType SkipWhiteSpace()
        {
            if (reader.ShouldLoadBuffer(separatorLength))
            {
                reader.LoadBuffer();
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                if (reader.ShouldLoadBuffer(separatorLength + 1))
                {
                    reader.LoadBuffer();
                }
                if (!reader.IsWhitespace())
                {
                    break;
                }
                tokenType = GetSeparator();
            }
            return tokenType;
        }

        private async ValueTask<TokenType> SkipWhiteSpaceAsync()
        {
            if (reader.ShouldLoadBuffer(separatorLength))
            {
                await reader.LoadBufferAsync().ConfigureAwait(false);
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                if (reader.ShouldLoadBuffer(separatorLength + 1))
                {
                    await reader.LoadBufferAsync().ConfigureAwait(false);
                }
                if (!reader.IsWhitespace())
                {
                    break;
                }
                tokenType = GetSeparator();
            }
            return tokenType;
        }

        private TokenType AppendWhiteSpace()
        {
            if (reader.ShouldLoadBuffer(separatorLength))
            {
                reader.LoadBuffer();
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                if (reader.ShouldLoadBuffer(separatorLength + 1))
                {
                    reader.LoadBuffer();
                }
                if (!reader.IsWhitespace())
                {
                    break;
                }
                token.Append(reader.Current);
                tokenType = GetSeparator();
            }
            return tokenType;
        }

        private async ValueTask<TokenType> AppendWhiteSpaceAsync()
        {
            if (reader.ShouldLoadBuffer(separatorLength))
            {
                await reader.LoadBufferAsync().ConfigureAwait(false);
            }
            TokenType tokenType = GetSeparator();
            while (tokenType == TokenType.Normal)
            {
                if (reader.ShouldLoadBuffer(separatorLength + 1))
                {
                    await reader.LoadBufferAsync().ConfigureAwait(false);
                }
                if (!reader.IsWhitespace())
                {
                    break;
                }
                token.Append(reader.Current);
                tokenType = GetSeparator();
            }
            return tokenType;
        }

        private TokenType GetSeparator()
        {
            if (reader.IsEndOfStream())
            {
                return TokenType.EndOfStream;
            }

            if (separatorMatcher.IsMatch())
            {
                // This code handles the case where the separator is a substring of the record separator.
                // We check to see if the remaining characters make up the record separator.
                if (postfixMatcher != null && postfixMatcher.IsMatch())
                {
                    return TokenType.EndOfRecord;
                }

                return TokenType.EndOfToken;
            }

            if (postfixMatcher == null && recordSeparatorMatcher.IsMatch())
            {
                // If the separator is a substring of the record separator and we didn't find it,
                // we won't find the record separator either.
                return TokenType.EndOfRecord;
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
