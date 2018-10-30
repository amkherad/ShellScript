using System.Collections;
using System.Collections.Generic;
using ShellScript.Core.Helpers;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Lexing;
using ShellScript.Core.Language.Compiler.Parsing;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;

namespace ShellScript.Core.Language.Compiler.PreProcessors
{
    public class PreProcessorParser
    {
        public Parser Parser { get; }
        public Context Context { get; }

        public PreProcessorParser(Parser parser, Context context)
        {
            Parser = parser;
            Context = context;
        }

        private bool CalculatePreProcessorCondition(Token token, IPeekingEnumerator<Token> enumerator,
            ParserContext context, bool parseCondition)
        {
            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedToken(token, context, true);

            if (!enumerator.MoveNext()) //read the first eval token
                throw EndOfFile(token, context);

            token = enumerator.Current;

            var evalStatement = Parser.ReadEvaluationStatement(token, enumerator, context);

            if (!enumerator.MoveNext()) //read the close parenthesis
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedToken(token, context);

            if (parseCondition)
            {
                try
                {
                    var stt = EvaluationStatementTranspilerBase.ProcessEvaluation(Context, Context.GeneralScope,
                        evalStatement);

                    if (stt is ConstantValueStatement constantValueStatement)
                    {
                        if (StatementHelpers.TryParseBooleanFromString(constantValueStatement.Value, out var boolVal))
                        {
                            return boolVal;
                        }

                        throw UnexpectedToken(token, context);
                    }
                }
                catch (IdentifierNotFoundCompilerException)
                {
                    return false;
                }

                throw UnexpectedToken(token, context);
            }

            return false;
        }

        public IPeekingEnumerator<Token> CreateParserProxy(IEnumerator<Token> enumerator, ParserContext context)
        {
            return new PreProcessorParserProxyClass(this, enumerator, context);
        }


        private static PreProcessorException EndOfFile(Token token, ParserContext context)
        {
            return new PreProcessorException(
                $"Unexpected end of file reached {context} at {token.LineNumber}:{token.ColumnNumber}");
        }

        private static PreProcessorException UnexpectedToken(Token token, ParserContext context, bool isInCondition = false)
        {
            if (isInCondition)
                return new PreProcessorException(
                    $"Unexpected token {token.Type}('{token.Value}') found (parentheses are required) {context} at {token.LineNumber}:{token.ColumnNumber}");
            return new PreProcessorException(
                $"Unexpected token {token.Type}('{token.Value}') found {context} at {token.LineNumber}:{token.ColumnNumber}");
        }

        private static PreProcessorException PreProcessorNotExited(Token token, ParserContext context)
        {
            return new PreProcessorException(
                $"#endif does not match any corresponding #if {context} at {token.LineNumber}:{token.ColumnNumber}");
        }


        private class PreProcessorParserProxyClass : IPeekingEnumerator<Token>
        {
            private readonly PreProcessorParser _parser;
            private readonly IEnumerator<Token> _enumerator;
            private ParserContext _context;

            private Token _current;
            private Token _next;
            private bool _isNextAvailable;

            private Stack<PreProcessorState> _preProcessors = new Stack<PreProcessorState>();


            public PreProcessorParserProxyClass(PreProcessorParser parser, IEnumerator<Token> enumerator,
                ParserContext context)
            {
                _parser = parser;
                _enumerator = enumerator;
                _context = context;
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            public Token Current => _current;

            object IEnumerator.Current => _current;

            public void Dispose()
            {
                _enumerator.Dispose();
                _current = null;
            }

            public bool MoveNext()
            {
                if (!_isNextAvailable)
                {
                    return _MoveNext(out _current);
                }

                _isNextAvailable = false;
                _current = _next;
                _next = default;
                return true;
            }

            public bool TryPeek(out Token peek)
            {
                if (!_isNextAvailable)
                {
                    var result = _MoveNext(out peek);
                    if (result)
                    {
                        _next = peek;
                        _isNextAvailable = true;
                    }

                    return result;
                }

                peek = _next;
                return true;
            }


            private bool _MoveNext(out Token current)
            {
                bool skipToken = false;
                while (_enumerator.MoveNext())
                {
                    var token = _enumerator.Current;

                    if (token.Type == TokenType.PreprocessorIf)
                    {
                        var state = new PreProcessorState(token);

                        _preProcessors.Push(state);

                        var condition = _parser.CalculatePreProcessorCondition(token, this, _context, true);

                        state.ConditionTaken = condition;
                        skipToken = !condition;
                    }
                    else if (token.Type == TokenType.PreprocessorElseIf)
                    {
                        if (!_preProcessors.TryPeek(out var preProcessor))
                        {
                            throw UnexpectedToken(token, _context);
                        }

                        var condition =
                            _parser.CalculatePreProcessorCondition(token, this, _context,
                                !preProcessor.ConditionTaken);

                        skipToken = !condition;

                        if (preProcessor.ConditionTaken)
                            continue;

                        preProcessor.ConditionTaken = condition;
                    }
                    else if (token.Type == TokenType.PreprocessorElse)
                    {
                        if (!_preProcessors.TryPeek(out var preProcessor))
                        {
                            throw UnexpectedToken(token, _context);
                        }

                        skipToken = false;

                        if (preProcessor.ConditionTaken)
                            continue;
                    }
                    else if (token.Type == TokenType.PreprocessorEndIf)
                    {
                        if (!_preProcessors.TryPop(out var preProcessor))
                        {
                            throw UnexpectedToken(token, _context);
                        }

                        skipToken = false;
                    }
                    else if (!skipToken)
                    {
                        current = token;
                        return true;
                    }
                }

                if (_preProcessors.TryPop(out var preProcessorState))
                {
                    throw PreProcessorNotExited(preProcessorState.FirstToken, _context);
                }

                current = null;
                return false;
            }
        }
    }
}