using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class SeparatedValueRecordParser
    {
        private readonly List<string> tokens = new();
        private readonly StringBuilder token = new();
        private readonly RetryReader reader;
        private readonly ISeparatorMatcher separatorMatcher;
        private readonly ISeparatorMatcher recordSeparatorMatcher;
        private readonly ISeparatorMatcher? postfixMatcher;
        private readonly int separatorLength;

        public SeparatedValueRecordParser(RetryReader reader, SeparatedValueOptions options)
        {
            this.reader = reader;
            Options = options.Clone();
            var separator = options.Separator;
            separatorMatcher = SeparatorMatcher.GetMatcher(reader, separator);
            var recordSeparator = options.RecordSeparator;
            recordSeparatorMatcher = SeparatorMatcher.GetMatcher(reader, recordSeparator);
            if (recordSeparator != null && recordSeparator.StartsWith(separator))
            {
                string postfix = recordSeparator.Substring(separator.Length);
                postfixMatcher = SeparatorMatcher.GetMatcher(reader, postfix);
            }
            separatorLength = Math.Max(recordSeparatorMatcher.Size, separator.Length);
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
            var value = token.ToString();
            tokens.Add(value);
            token.Clear();
        }

        public (string, string[]) ReadRecord()
        {
            var tokenType = GetNextToken();
            while (tokenType == TokenType.EndOfToken)
            {
                tokenType = GetNextToken();
            }
            var record = recordSeparatorMatcher.Trim(reader.GetRecord());
            var results = tokens.ToArray();
            tokens.Clear();
            return (record, results);
        }

        public async Task<(string, string[])> ReadRecordAsync()
        {
            var tokenType = await GetNextTokenAsync().ConfigureAwait(false);
            while (tokenType == TokenType.EndOfToken)
            {
                tokenType = await GetNextTokenAsync().ConfigureAwait(false);
            }
            var record = recordSeparatorMatcher.Trim(reader.GetRecord());
            var results = tokens.ToArray();
            tokens.Clear();
            return (record, results);
        }

        private TokenType GetNextToken()
        {
            if (!Options.PreserveWhiteSpace)
            {
                var tokenType = SkipWhiteSpace();
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
                var tokenType = await SkipWhiteSpaceAsync().ConfigureAwait(false);
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
            var tokenType = GetSeparator();
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
            var tokenType = GetSeparator();
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
            if (char.IsWhiteSpace(token[token.Length - 1]))
            {
                int trailingSize = 1;
                while (char.IsWhiteSpace(token[token.Length - trailingSize - 1]))
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
            var tokenType = TokenType.Normal;
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
                        if (Options.PreserveWhiteSpace)
                        {
                            tokenType = AppendWhiteSpace();
                        }
                        else
                        {
                            tokenType = SkipWhiteSpace();
                        }
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
            var tokenType = TokenType.Normal;
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
                        if (Options.PreserveWhiteSpace)
                        {
                            tokenType = await AppendWhiteSpaceAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            tokenType = await SkipWhiteSpaceAsync().ConfigureAwait(false);
                        }
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
            var tokenType = GetSeparator();
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
            var tokenType = GetSeparator();
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
            var tokenType = GetSeparator();
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
            var tokenType = GetSeparator();
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
