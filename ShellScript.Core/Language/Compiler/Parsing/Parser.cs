using System;
using System.Collections.Generic;
using System.IO;
using ShellScript.Core.Helpers;
using ShellScript.Core.Language.Compiler.Lexing;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.Compiler.Parsing
{
    public partial class Parser
    {
        private Lexer _lexer;

        public Parser()
        {
            _lexer = new Lexer();
        }


        public IEnumerable<IStatement> Parse(TextReader reader, ParserInfo info)
        {
            using (var tokensEnumerator = new PeekingEnumerator<Token>(_lexer.Tokenize(reader).GetEnumerator()))
            {
                IStatement statement;
                while ((statement = ReadStatement(tokensEnumerator, info)) != null)
                {
                    yield return statement;
                }
            }
        }


        public DataTypes TokenTypeToDataType(Token token, DataTypes dataType = DataTypes.Variant)
        {
            switch (token.Type)
            {
                case TokenType.DataType:
                {
                    switch (token.Value)
                    {
                        case "null":
                            return dataType;
                        case "int":
                            return DataTypes.Decimal;
                        case "decimal":
                            return DataTypes.Decimal;
                        case "number":
                            return DataTypes.Numeric;
                        case "float":
                            return DataTypes.Float;
                        case "double":
                            return DataTypes.Float;
                        case "object":
                            return DataTypes.Class;
                        case "variant":
                        case "var":
                            return DataTypes.Variant;

                        case "int[]":
                            return DataTypes.Decimal | DataTypes.Array;
                        case "decimal[]":
                            return DataTypes.Decimal | DataTypes.Array;
                        case "number[]":
                            return DataTypes.Numeric | DataTypes.Array;
                        case "float[]":
                            return DataTypes.Float | DataTypes.Array;
                        case "double[]":
                            return DataTypes.Float | DataTypes.Array;
                        case "object[]":
                            return DataTypes.Class | DataTypes.Array;
                        case "variant[]":
                            return DataTypes.Variant | DataTypes.Array;
                    }

                    break;
                }
                case TokenType.Null:
                    return dataType;
            }

            return dataType;
        }

        protected IStatement ReadStatement(PeekingEnumerator<Token> enumerator, ParserInfo info)
        {
            Token token = null;

            while (enumerator.MoveNext())
            {
                token = enumerator.Current;

                switch (token.Type)
                {
                    case TokenType.Class:
                        //return ReadClass(token, enumerator, info);
                        throw new NotImplementedException();

                    case TokenType.Function:
                        return ReadFunction(token, enumerator, info);

                    case TokenType.If:
                        return ReadIf(token, enumerator, info);

                    case TokenType.Else:
                        throw UnexpectedSyntax(token, info);

                    case TokenType.AndLogical:
                        break;
                    case TokenType.And:
                        break;
                    case TokenType.OrLogical:
                        break;
                    case TokenType.Or:
                        break;
                    case TokenType.OpenParenthesis:
                        break;
                    case TokenType.CloseParenthesis:
                        break;
                    case TokenType.OpenBrace:
                        break;
                    case TokenType.CloseBrace:
                        break;
                    case TokenType.OpenBracket:
                        break;
                    case TokenType.CloseBracket:
                        break;
                    case TokenType.Dot:
                        break;
                    case TokenType.Comma:
                        break;
                    case TokenType.Equals:
                        break;
                    case TokenType.NotEquals:
                        break;
                    case TokenType.Assignment:
                        break;
                    case TokenType.Minus:
                        break;
                    case TokenType.Plus:
                        break;
                    case TokenType.Asterisk:
                        break;
                    case TokenType.Division:
                        break;
                    case TokenType.BackSlash:
                        break;
                    case TokenType.Throw:
                        break;
                    case TokenType.Async:
                        break;
                    case TokenType.Await:
                        break;
                    case TokenType.In:
                        break;
                    case TokenType.NotIn:
                        break;
                    case TokenType.For:
                        break;
                    case TokenType.ForEach:
                        break;
                    case TokenType.While:
                        break;
                    case TokenType.Do:
                        break;
                    case TokenType.Loop:
                        break;
                    case TokenType.Like:
                        break;
                    case TokenType.NotLike:
                        break;
                    case TokenType.Call:
                        break;
                    case TokenType.DataType:
                        break;
                    case TokenType.Echo:
                        break;
                    case TokenType.Number:
                        break;
                    case TokenType.StringValue1:
                        break;
                    case TokenType.StringValue2:
                        break;
                    case TokenType.SequenceTerminator:
                        break;
                    case TokenType.SequenceTerminatorNewLine:
                        break;
                    case TokenType.Comment:
                        break;
                    case TokenType.MultiLineCommentOpen:
                        break;
                    case TokenType.MultiLineCommentClose:
                        break;
                    case TokenType.IdentifierName:
                        break;

                    case TokenType.NotDefined:
                        throw IllegalSyntax(token, info);
                    case TokenType.Invalid:
                        throw IllegalSyntax(token, info);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            throw UnexpectedSyntax(token, info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns type="IllegalSyntaxException">IllegalSyntaxException</returns>
        protected ParserException IllegalSyntax(Token token, ParserInfo info)
        {
            return new IllegalSyntaxException(token?.LineNumber ?? 0, token?.ColumnOffset ?? 0, info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns type="ParserSyntaxException">ParserSyntaxException</returns>
        protected ParserException UnexpectedSyntax(Token token, ParserInfo info)
        {
            return new ParserSyntaxException(token?.LineNumber ?? 0, token?.ColumnOffset ?? 0, info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns type="ParserSyntaxException">ParserSyntaxException</returns>
        protected ParserException EndOfFile(Token token, ParserInfo info)
        {
            return new ParserSyntaxException(token?.LineNumber ?? 0, token?.ColumnOffset ?? 0, info);
        }
    }
}