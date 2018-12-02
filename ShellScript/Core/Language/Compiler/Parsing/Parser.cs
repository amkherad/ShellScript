using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ShellScript.Core.Helpers;
using ShellScript.Core.Language.Compiler.Lexing;
using ShellScript.Core.Language.Compiler.PreProcessors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Parsing
{
    public partial class Parser
    {
        public const string ConstantKeyword = "const";

        public static HashSet<string> Keywords = new HashSet<string>
        {
            "if",
            "else",
            "switch",
            "case",
            "default",
            "while",
            "do",
            "switch",
            "case",
            "function",
            "int",
            "long",
            "double",
            "float",
            "void",
            "integer",
            "null",
            "nil",
            "numeric",
            "string",
            "object",
            "class",
            "delegate",
            "const",
            "array",
            "int[]",
            "long[]",
            "double[]",
            "float[]",
            "var",
            "variant[]",
            "numeric[]",
            "integer[]",
            "new",
            "for",
            "foreach",
            "loop",
            "return",
            "async",
            "await",
            "throw",
            "include",
            "in",
            "notin",
            "like",
            "notlike",
            "call",
            "echo",
        };

        public Context Context { get; }
        private Lexer _lexer;
        private PreProcessorParser _preProcessorParser;

        public Parser(Context context)
        {
            Context = context;
            _lexer = new Lexer();
            _preProcessorParser = new PreProcessorParser(this, context);
        }

        public Parser(Context context, Lexer lexer, PreProcessorParser preProcessorParser)
        {
            Context = context;
            _lexer = lexer;
            _preProcessorParser = preProcessorParser;
        }


        public IEnumerable<IStatement> Parse(TextReader reader, ParserContext context)
        {
            var tokens = _lexer.Tokenize(reader);

            using (var tokensEnumerator = _preProcessorParser.CreateParserProxy(tokens.GetEnumerator(), context))
            {
                IStatement statement;
                while ((statement = ReadStatement(tokensEnumerator, context)) != null)
                {
                    yield return statement;
                }
            }
        }

        public StatementInfo CreateStatementInfo(ParserContext context, Token token)
        {
            return new StatementInfo(context.FilePath, token.LineNumber, token.ColumnStart);
        }


        public TypeDescriptor TokenTypeToDataType(Token token, ParserContext context) =>
            TokenTypeToDataType(token, DataTypes.Void, context);
        public TypeDescriptor TokenTypeToDataType(Token token, DataTypes defaultValue, ParserContext context)
        {
            switch (token.Type)
            {
                case TokenType.DataType:
                {
                    var str = token.Value.ToLower();

                    str = Regex.Replace(str, @"\s", string.Empty);
                    
                    switch (str)
                    {
                        case "int":
                        case "long":
                            return TypeDescriptor.Integer;
                        case "bool":
                        case "boolean":
                            return TypeDescriptor.Boolean;
                        case "float":
                        case "double":
                            return TypeDescriptor.Float;
                        case "number":
                            return TypeDescriptor.Numeric;
                        case "string":
                            return TypeDescriptor.String;
                        case "delegate":
                            return new TypeDescriptor(DataTypes.Delegate);
                        case "object":
                            return new TypeDescriptor(DataTypes.Class);
                        //case "variant":
                        //case "var":
                        //    return DataTypes.Variant;

                        case "void":
                            return TypeDescriptor.Void;

                        case "any":
                            return TypeDescriptor.Any;

                        case "int[]":
                        case "long[]":
                            return new TypeDescriptor(DataTypes.Integer | DataTypes.Array);
                        case "number[]":
                            return new TypeDescriptor(DataTypes.Numeric | DataTypes.Array);
                        case "float[]":
                        case "double[]":
                            return new TypeDescriptor(DataTypes.Float | DataTypes.Array);
                        case "string[]":
                            return new TypeDescriptor(DataTypes.String | DataTypes.Array);
                        case "object[]":
                            return new TypeDescriptor(DataTypes.Class | DataTypes.Array);

                        case "any[]":
                            return new TypeDescriptor(DataTypes.Any | DataTypes.Array);
                        //case "variant[]":
                        //    return DataTypes.Variant | DataTypes.Array;

                        case "null":
                        case "nil":
                            return defaultValue;
                    }

                    throw UnexpectedSyntax(token, context);
                }
                case TokenType.Null:
                    return defaultValue;
                case TokenType.IdentifierName:
                    return new TypeDescriptor(DataTypes.Lookup, new TypeDescriptor.LookupInfo(null, token.Value));

                default:
                    throw new InvalidOperationException();
            }
        }

        protected IStatement ReadStatement(IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            Token token;

            while (enumerator.MoveNext())
            {
                token = enumerator.Current;

                switch (token.Type)
                {
                    case TokenType.Echo:
                        return ReadEcho(token, enumerator, context);
                    case TokenType.IdentifierName:
                        return ReadIdentifierName(token, enumerator,
                            context); //ReadAssignmentOrFunctionCall(token, enumerator, context);
                    case TokenType.DataType:
                        return ReadVariableOrFunctionDefinition(token, enumerator, context);
                    case TokenType.Delegate:
                        return ReadDelegateDefinition(token, enumerator, context);

                    case TokenType.If:
                        return ReadIf(token, enumerator, context);
                    case TokenType.For:
                        return ReadFor(token, enumerator, context);
                    case TokenType.ForEach:
                        return ReadForEach(token, enumerator, context);
                    case TokenType.While:
                        return ReadWhile(token, enumerator, context);
                    case TokenType.Do:
                        return ReadDoWhile(token, enumerator, context);
                    case TokenType.Loop:
                        return ReadLoop(token, enumerator, context);

                    //case TokenType.Class:
                    //    return ReadClass(token, enumerator, info);
                    //case TokenType.Function:
                    //    return ReadFunction(token, enumerator, info);
                    case TokenType.Return:
                        return ReadReturn(token, enumerator, context);

                    case TokenType.OpenBrace:
                        return ReadBlockStatement(token, enumerator, context);

                    case TokenType.Include:
                    {
                        return ReadIncludeStatement(token, enumerator, context);
                    }

                    case TokenType.OpenParenthesis:
                        break;
//                    case TokenType.Throw:
//                        break;
//                    case TokenType.Async:
//                        break;
//                    case TokenType.Await:
//                        break;
//                    case TokenType.Call:
//                        break;

                    case TokenType.SequenceTerminator:
                    case TokenType.SequenceTerminatorNewLine:
                        continue;

                    case TokenType.Comment:
                    case TokenType.MultiLineCommentOpen:
                    case TokenType.MultiLineCommentClose:
                    case TokenType.PreprocessorIf:
                    case TokenType.PreprocessorElse:
                    case TokenType.PreprocessorElseIf:
                    case TokenType.PreprocessorEndIf:
                        throw UnexpectedSyntax(token, context);


                    case TokenType.Minus:
                    case TokenType.Plus:
                    case TokenType.Else:
                    case TokenType.AndLogical:
                    case TokenType.And:
                    case TokenType.OrLogical:
                    case TokenType.Or:
                    case TokenType.Equals:
                    case TokenType.NotEquals:
                    case TokenType.Asterisk:
                    case TokenType.Assignment:
                    case TokenType.CloseParenthesis:
                    case TokenType.CloseBrace:
                    case TokenType.OpenBracket:
                    case TokenType.CloseBracket:
                    case TokenType.Division:
                    case TokenType.BackSlash:
                    case TokenType.Dot:
                    case TokenType.Case:
                    case TokenType.Comma:
                    case TokenType.In:
                    case TokenType.NotIn:
                    case TokenType.Like:
                    case TokenType.NotLike:
                    case TokenType.Number:
                    case TokenType.StringValue1:
                    case TokenType.StringValue2:
                        throw UnexpectedSyntax(token, context);

                    case TokenType.NotDefined:
                        throw IllegalSyntax(token, context);
                    case TokenType.Invalid:
                        throw IllegalSyntax(token, context);

                    default:
                        throw UnexpectedSyntax(token, context);
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="context"></param>
        /// <returns type="IllegalSyntaxException">IllegalSyntaxException</returns>
        protected ParserException IllegalSyntax(Token token, ParserContext context)
        {
            return new IllegalSyntaxException(token?.LineNumber ?? 0, token?.ColumnStart ?? 0, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="context"></param>
        /// <returns type="IllegalSyntaxException">IllegalSyntaxException</returns>
        protected ParserException InvalidIdentifierName(string identifierName, Token token, ParserContext context)
        {
            return new IllegalSyntaxException(
                $"Invalid identifier name '{identifierName}'",
                token?.LineNumber ?? 0, token?.ColumnStart ?? 0, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="context"></param>
        /// <returns type="ParserSyntaxException">ParserSyntaxException</returns>
        protected ParserException UnexpectedSyntax(Token token, ParserContext context)
        {
            return new ParserSyntaxException(
                $"Unexpected token of type {token.Type}({token.Value}) found",
                token?.LineNumber ?? 0, token?.ColumnStart ?? 0, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="context"></param>
        /// <returns type="ParserSyntaxException">ParserSyntaxException</returns>
        protected ParserException UnexpectedSyntax(IStatement statement, ParserContext context)
        {
            return new ParserSyntaxException(
                $"Unexpected syntax of type {statement.GetType().Name}({statement}) found {statement.Info}",
                statement.Info, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="context"></param>
        /// <returns type="ParserSyntaxException">ParserSyntaxException</returns>
        protected ParserException UnexpectedSyntax(Token token, ParserContext context, Exception innerException)
        {
            return new ParserSyntaxException(
                $"Unexpected token '{token.Value}' found",
                token?.LineNumber ?? 0, token?.ColumnStart ?? 0, context,
                innerException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="context"></param>
        /// <returns type="ParserSyntaxException">ParserSyntaxException</returns>
        protected ParserException EndOfFile(Token token, ParserContext context)
        {
            return new ParserSyntaxException(
                "Unexpected end of file reached",
                token?.LineNumber ?? 0, token?.ColumnStart ?? 0, context);
        }
    }
}