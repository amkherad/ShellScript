using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShellScript.Core.Language.CompilerServices.Lexing
{
    public class Lexer
    {
        public const char EscapeCharacter = '\\';
        public const string EscapeCharacterStr = "\\";

        private static readonly Dictionary<TokenType, string> TokenPatterns = new Dictionary<TokenType, string>
        {
            {TokenType.StringValue1, "^(\"|\')(?:\\\\\\1|[^\\1])*?\\1"},
            
            {TokenType.OpenParenthesis, @"^\("},
            {TokenType.CloseParenthesis, @"^\)"},

            {TokenType.OpenBrace, @"^\{"},
            {TokenType.CloseBrace, @"^\}"},

            {TokenType.OpenBracket, @"^\["},
            {TokenType.CloseBracket, @"^\]"},

            {TokenType.AndLogical, @"^&&"},
            {TokenType.And, @"^&(?!&)"},
            {TokenType.OrLogical, @"^\|\|"},
            {TokenType.Or, @"^\|(?!\|)"},

            {TokenType.Dot, @"^\."},
            {TokenType.Comma, "^,"},
            {TokenType.Colon, "^:"},
            
            {TokenType.QuestionMark, @"^\?(?!\?)"},
            {TokenType.NullCoalesce, @"^\?\?"},
            
            {TokenType.Assignment, "^=(?!=)"},
            {TokenType.Equals, "^=="},
            {TokenType.NotEquals, "^!="},
            {TokenType.GreaterEqual, "^>="},
            {TokenType.Greater, "^>(?!=)"},
            {TokenType.LessEqual, "^<="},
            {TokenType.Less, "^<(?!=)"},

            {TokenType.Not, @"^\!(?!=)"},
            {TokenType.BitwiseNot, @"^~"},
            
            {TokenType.Plus, @"^\+(?!\+)"},
            {TokenType.Minus, @"^-(?!-)"},
            {TokenType.Asterisk, @"^\*"},
            {TokenType.Division, @"^/(?!/)"},
            {TokenType.Increment, @"^\+\+"},
            {TokenType.Decrement, @"^--"},

            {TokenType.BackSlash, @"^\\"},

            {TokenType.If, @"^if(?!\w)"},
            {TokenType.Else, @"^else(?!\w)"},
            
            {TokenType.Switch, @"^switch(?!\w)"},
            {TokenType.Case, @"^case(?!\w)"},
            {TokenType.Default, @"^default(?!\w)"},
            
            {TokenType.PreprocessorIf, @"^#if(?!\w)"},
            {TokenType.PreprocessorElseIf, @"^#elseif(?!\w)"},
            {TokenType.PreprocessorElse, @"^#else(?!\w)"},
            {TokenType.PreprocessorEndIf, @"^#endif(?!\w)"},
            
            {TokenType.For, @"^for(?!each)(?!\w)"},
            {TokenType.ForEach, @"^foreach(?!\w)"},
            {TokenType.Do, @"^do(?!uble)(?!\w)"},
            {TokenType.While, @"^while(?!\w)"},
            {TokenType.Loop, @"^loop(?!\w)"},
            
            //{TokenType.Class, @"^class(?!\w)"},
            //{TokenType.Function, @"^function(?!\w)"},
            {TokenType.Return, @"^return(?!\w)"},

            {TokenType.Throw, @"^throw(?!\w)"},
            {TokenType.Delegate, @"^delegate(?!\w)"},
            
            {TokenType.Async, @"^async(?!\w)"},
            {TokenType.Await, @"^await(?!\w)"},

            {TokenType.In, @"^in(?!t)(?!\w)"},
            {TokenType.NotIn, @"^notin(?!\w)"},

            {TokenType.Like, @"^like(?!\w)"},
            {TokenType.NotLike, @"^notlike(?!\w)"},

            {TokenType.Call, @"^call(?!\w)"},

            {TokenType.Null, @"^(null|nil)(?!\w)"},
            
            {TokenType.True, @"^true(?!\w)"},
            {TokenType.False, @"^false(?!\w)"},

            {TokenType.Echo, @"^echo(?!\w)"},

            {TokenType.Comment, "^//.*"},
            {TokenType.SequenceTerminator, "^;"},
        };

        private static readonly Dictionary<TokenType, Regex> TokenRegexes = new Dictionary<TokenType, Regex>(
            TokenPatterns.ToDictionary(
                keySelector => keySelector.Key,
                elementSelector => new Regex(elementSelector.Value, RegexOptions.Compiled)
            )
        );

        public static readonly Regex Number = new Regex(@"^([-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)", RegexOptions.Compiled);
        public static readonly Regex DataType = new Regex(@"^(const|void|int(?!\[\])|bool(?!\[\])|double(?!\[\])|float(?!\[\])|string(?!\[\])|object(?!\[\])|number(?!\[\])|decimal(?!\[\])|int\[\]|bool\[\]|double\[\]|float\[\]|string\[\]|object\[\]|number\[\]|decimal\[\])(?!\w)", RegexOptions.Compiled);
        public static readonly Regex ValidIdentifierName = new Regex(@"^\w+", RegexOptions.Compiled);
        
        public static readonly Regex MultiLineCommentOpen = new Regex(@"^/\*", RegexOptions.Compiled);
        public static readonly Regex MultiLineCommentClose = new Regex(@"^\*/", RegexOptions.Compiled);
        public const string MultiLineCommentCloseText = "*/";

        /// <summary>
        /// This method tokenize the input stream.
        /// </summary>
        /// <remarks>
        /// This method yield returns the tokens.
        /// </remarks>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerable<Token> Tokenize(TextReader reader)
        {
            string line = null;
            var lineNumber = 0;

            var isMultilineCommentOpen = false;

            string cline; //current line
            while ((cline = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(cline))
                {
                    lineNumber++;
                    continue;
                }
                if (cline.Length > 0 && cline[cline.Length - 1] == EscapeCharacter)
                {
                    line += cline.Substring(0, cline.Length - 1); //remove the line escape character.
                    //lineNumber++;
                    continue;
                }
                else if (line == null)
                {
                    line = cline;
                }
                else
                {
                    line += ' ' + cline;
                }
                
                lineNumber++;

                var lineLength = line.Length;

                var tokenizedLine = TokenizeLine(line, lineNumber, isMultilineCommentOpen, out isMultilineCommentOpen);

                line = null;
                
                if (tokenizedLine.Length > 0)
                {
                    foreach (var token in tokenizedLine)
                    {
                        yield return token;
                    }
                }

//                yield return new Token(
//                    Environment.NewLine,
//                    TokenType.SequenceTerminatorNewLine,
//                    lineLength,
//                    lineLength);

                //lineNumber = 0;
            }
        }

        /// <summary>
        /// Tokenizes a single line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineNumber"></param>
        /// <param name="isMultilineCommentOpen"></param>
        /// <param name="isMultilineCommentOpenResult"></param>
        /// <returns></returns>
        public Token[] TokenizeLine(string line, int lineNumber, bool isMultilineCommentOpen, out bool isMultilineCommentOpenResult)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));

            var columnNumber = 0;
            var tokens = new List<Token>(10);

            while (!string.IsNullOrWhiteSpace(line))
            {
                if (char.IsWhiteSpace(line[0]))
                {
                    line = line.Substring(1);
                    columnNumber++;
                }
                else if (TryFindMatch(line, out var matchType, out var matchString))
                {
                    var addToken = true;
                    
                    if (matchType == TokenType.Comment)
                    {
                        if (isMultilineCommentOpen)
                        {
                            var multiLineCommentIndex = line.IndexOf(MultiLineCommentCloseText, StringComparison.Ordinal);

                            if (multiLineCommentIndex > 0)
                            {
                                var offset = multiLineCommentIndex + MultiLineCommentCloseText.Length;
                                line = line.Substring(offset);
                                columnNumber += offset;

                                continue;
                            }
                        }
                        else
                        {
                            break; //break on comment (i.e. "//")
                        }
                    }
                    else if (matchType == TokenType.MultiLineCommentOpen)
                    {
                        isMultilineCommentOpenResult = true;
                        isMultilineCommentOpen = true;
                        addToken = false;
                    }
                    else if (matchType == TokenType.MultiLineCommentClose)
                    {
                        isMultilineCommentOpenResult = false;
                        isMultilineCommentOpen = false;
                        addToken = false;
                    }

                    if (addToken && !isMultilineCommentOpen)
                    {
                        tokens.Add(new Token(matchString, matchType, columnNumber, columnNumber + matchString.Length, lineNumber));
                    }
                    
                    line = line.Substring(matchString.Length);
                    columnNumber += matchString.Length;
                }
                else
                {
                    line = line.Substring(1);
                    columnNumber++;
                }
            }

            isMultilineCommentOpenResult = isMultilineCommentOpen;
            return tokens.ToArray();
        }

        public bool TryFindMatch(string text, out TokenType matchType, out string matchString)
        {
            Match match;

            match = MultiLineCommentOpen.Match(text);
            if (match.Success)
            {
                matchType = TokenType.MultiLineCommentOpen;
                matchString = match.Value;
                return true;
            }

            match = MultiLineCommentClose.Match(text);
            if (match.Success)
            {
                matchType = TokenType.MultiLineCommentClose;
                matchString = match.Value;
                return true;
            }

            match = DataType.Match(text);
            if (match.Success)
            {
                matchType = TokenType.DataType;
                matchString = match.Value;
                return true;
            }
            
            foreach (var token in TokenRegexes)
            {
                match = token.Value.Match(text);
                if (match.Success)
                {
                    matchType = token.Key;
                    matchString = match.Value;
                    return true;
                }
            }

            match = Number.Match(text);
            if (match.Success)
            {
                matchType = TokenType.Number;
                matchString = match.Value;
                return true;
            }

            match = ValidIdentifierName.Match(text);
            if (match.Success)
            {
                matchType = TokenType.IdentifierName;
                matchString = match.Value;
                return true;
            }

            matchType = TokenType.NotDefined;
            matchString = text.Length > 0 ? text[0].ToString() : string.Empty;
            return false;
        }
    }
}