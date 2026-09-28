using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using ShellScript.Core;
using ShellScript.Core.Helpers;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Lexing;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Statements.Operators;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Parsing
{
    public partial class Parser
    {
        /// <summary>
        /// Reads a block (i.e. everything between "{" and "}")
        /// </summary>
        /// <param name="token"></param>
        /// <param name="enumerator"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="IllegalSyntaxException"></exception>
        /// <exception cref="ParserSyntaxException"></exception>
        /// <exception cref="ParserException"></exception>
        public BlockStatement ReadBlockStatement(Token token, IPeekingEnumerator<Token> enumerator,
            ParserContext context)
        {
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, context);

            var statements = new List<IStatement>();

            var continueRead = true;
            while (continueRead)
            {
                if (!enumerator.TryPeek(out var peek))
                {
                    break;
                }

                switch (peek.Type)
                {
                    case TokenType.SequenceTerminator:
                        //case TokenType.SequenceTerminatorNewLine:
                        enumerator.MoveNext();
                        continue;
                    case TokenType.CloseBrace:
                        continueRead = false;
                        break;
                    default:
                    {
                        var statement = ReadStatement(enumerator, context);
                        statements.Add(statement);
                        break;
                    }
                }
            }

            if (!enumerator.MoveNext())
                throw EndOfFile(enumerator.Current, context);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseBrace)
                throw UnexpectedSyntax(token, context);

            return new BlockStatement(statements.ToArray(), CreateStatementInfo(context, token));
        }


        private EvaluationStatement _createEvaluation(LinkedList<IStatement> statements, Token token,
            ParserContext context)
        {
            if (statements.Count == 0)
            {
                return new NopStatement(CreateStatementInfo(context, token));
            }

            while (statements.Count > 1)
            {
                var op = statements.OfType<IOperator>().OrderByDescending(s => s.Order).FirstOrDefault();

                if (op == null)
                {
                    throw UnexpectedSyntax(token, context);
                }

                var node = statements.Find(op);
                if (node == null)
                    throw UnexpectedSyntax(token, context);

                //this block checks if there's a addition or subtraction operator in the first place of statements or
                //- the statement before op is another operator.
                //digit sign (negative/positive operator)
                var prev = node.Previous;
                if (prev == null || prev.Value is IOperator)
                {
                    if (op is SubtractionOperator)
                    {
                        var valNode = node.Next;
                        var val = valNode?.Value as EvaluationStatement;
                        if (val == null) //next may be operator or null !!!
                        {
                            throw new InvalidOperationException();
                        }

                        var negation = ArithmeticEvaluationStatement.CreateNegate(
                            new NegativeNumberOperator(op.Info), val, op.Info);

                        val.ParentStatement = negation;
                        node.Value = negation;
                        statements.Remove(valNode);
                        continue;
                    }

                    if (op is AdditionOperator)
                    {
                        statements.Remove(node);
                        continue;
                    }

                    //throw new InvalidOperationException();
                }

                //digit sign (negative/positive operator)
                var next = node.Next;
                if (next?.Value is IOperator)
                {
                    var nOp = next.Value;
                    var nNode = next.Next;
                    var nOperand = nNode?.Value as EvaluationStatement;

                    if (nOperand == null)
                    {
                        throw new InvalidOperationException();
                    }

                    if (nOp is SubtractionOperator)
                    {
                        var negation = ArithmeticEvaluationStatement.CreateNegate(
                            new NegativeNumberOperator(nOp.Info), nOperand, nOp.Info);

                        nOperand.ParentStatement = negation;
                        next.Value = negation;

                        statements.Remove(nNode);
                    }
                    else if (nOp is AdditionOperator)
                    {
                        statements.Remove(nOp);
                    }
                    else if (nOp is TypeCastOperator && op is TypeCastOperator)
                    {
                        //continue
                    }
                    else //Invalid expression.
                    {
                        throw new InvalidOperationException();
                    }
                }

                switch (op)
                {
                    case IndexerOperator indexerOperator:
                    {
                        var leftNode = prev;
                        var array = leftNode?.Value as EvaluationStatement;
                        if (array == null)
                        {
                            throw UnexpectedSyntax(indexerOperator, context);
                        }

                        var stt = new IndexerAccessStatement(array, indexerOperator.Indexer, indexerOperator.Info);

                        array.ParentStatement = stt;
                        node.Value = stt;

                        statements.Remove(leftNode);

                        break;
                    }
                    case BitwiseOperator bitwiseOperator:
                    {
                        if (op is BitwiseNotOperator bitwiseNotOperator)
                        {
                            var operandNode = next;
                            var operand = operandNode?.Value as EvaluationStatement;
                            if (operand == null)
                            {
                                throw UnexpectedSyntax(bitwiseNotOperator, context);
                            }

                            var stt = BitwiseEvaluationStatement.CreateNot(bitwiseNotOperator, operand,
                                CreateStatementInfo(context, token));

                            operand.ParentStatement = stt;
                            node.Value = stt;

                            statements.Remove(operandNode);
                        }
                        else
                        {
                            var leftNode = prev;
                            var left = leftNode?.Value as EvaluationStatement;
                            var rightNode = next;
                            var right = rightNode?.Value as EvaluationStatement;

                            if (left == null || right == null)
                            {
                                throw UnexpectedSyntax(bitwiseOperator, context);
                            }

                            var stt = new BitwiseEvaluationStatement(left, bitwiseOperator, right,
                                CreateStatementInfo(context, token));

                            left.ParentStatement = stt;
                            right.ParentStatement = stt;
                            node.Value = stt;

                            statements.Remove(leftNode);
                            statements.Remove(rightNode);
                        }

                        break;
                    }
                    case LogicalOperator logicalOperator:
                    {
                        if (op is NotOperator notOperator)
                        {
                            var operandNode = next;
                            var operand = operandNode?.Value as EvaluationStatement;
                            if (operand == null)
                            {
                                throw UnexpectedSyntax(logicalOperator, context);
                            }

                            var stt = LogicalEvaluationStatement.CreateNot(notOperator, operand, logicalOperator.Info);

                            operand.ParentStatement = stt;
                            node.Value = stt;

                            statements.Remove(operandNode);
                        }
                        else
                        {
                            var leftNode = prev;
                            var left = leftNode?.Value as EvaluationStatement;
                            var rightNode = next;
                            var right = rightNode?.Value as EvaluationStatement;

                            if (left == null || right == null)
                            {
                                throw UnexpectedSyntax(logicalOperator, context);
                            }

                            var stt = new LogicalEvaluationStatement(left, logicalOperator, right,
                                logicalOperator.Info);

                            left.ParentStatement = stt;
                            right.ParentStatement = stt;
                            node.Value = stt;

                            statements.Remove(leftNode);
                            statements.Remove(rightNode);
                        }

                        break;
                    }
                    case ArithmeticOperator arithmeticOperator:
                    {
                        if (op is IncrementOperator || op is DecrementOperator)
                        {
                            var operandNode = prev;
                            var operand = operandNode?.Value as VariableAccessStatement;

                            EvaluationStatement statement;
                            if (operand == null)
                            {
                                operandNode = next;
                                operand = operandNode?.Value as VariableAccessStatement;

                                if (operand == null)
                                {
                                    throw UnexpectedSyntax(arithmeticOperator, context);
                                }

                                statement = ArithmeticEvaluationStatement.CreatePrefix(arithmeticOperator, operand,
                                    arithmeticOperator.Info);
                            }
                            else
                            {
                                statement = ArithmeticEvaluationStatement.CreatePostfix(arithmeticOperator, operand,
                                    arithmeticOperator.Info);
                            }

                            operand.ParentStatement = statement;
                            node.Value = statement;

                            statements.Remove(operandNode);
                        }
                        else
                        {
                            var leftNode = prev;
                            var left = leftNode?.Value as EvaluationStatement;
                            var rightNode = next;
                            var right = rightNode?.Value as EvaluationStatement;

                            if (left == null || right == null)
                            {
                                throw UnexpectedSyntax(arithmeticOperator, context);
                            }

                            var stt = new ArithmeticEvaluationStatement(left, arithmeticOperator, right,
                                arithmeticOperator.Info);
                            node.Value = stt;

                            left.ParentStatement = stt;
                            right.ParentStatement = stt;

                            statements.Remove(leftNode);
                            statements.Remove(rightNode);
                        }

                        break;
                    }
                    case TypeCastOperator typeCastOperator:
                    {
                        TypeCastStatement PopTypeCastStatement(TypeCastOperator typeCastOperator1,
                            LinkedListNode<IStatement> currentNode)
                        {
                            var rightNode = currentNode.Next;
                            EvaluationStatement right;
                            var rightNodeValue = rightNode?.Value;

                            if (rightNodeValue is EvaluationStatement evSttm)
                            {
                                right = evSttm;
                            }
                            else if (rightNodeValue is TypeCastOperator tpCastOp)
                            {
                                right = PopTypeCastStatement(tpCastOp, rightNode);
                            }
                            else
                            {
                                throw new InvalidStatementCompilerException(rightNodeValue, rightNodeValue?.Info);
                            }

                            if (right == null)
                            {
                                throw UnexpectedSyntax(typeCastOperator1, context);
                            }

                            var stt = new TypeCastStatement(typeCastOperator1.TypeDescriptor, right,
                                typeCastOperator1.Info);
                            currentNode.Value = stt;

                            right.ParentStatement = stt;

                            statements.Remove(rightNode);

                            return stt;
                        }

                        PopTypeCastStatement(typeCastOperator, node);

                        break;
                    }
                    case AssignmentOperator assignmentOperator:
                    {
                        var leftNode = prev;
                        var left = leftNode?.Value as EvaluationStatement;
                        var rightNode = next;
                        var right = rightNode?.Value as EvaluationStatement;

                        if (left == null || right == null)
                        {
                            throw UnexpectedSyntax(assignmentOperator, context);
                        }

                        var stt = new AssignmentStatement(left, right, assignmentOperator.Info);
                        node.Value = stt;

                        left.ParentStatement = stt;
                        right.ParentStatement = stt;

                        statements.Remove(leftNode);
                        statements.Remove(rightNode);

                        break;
                    }
                    default:
                    {
                        throw UnexpectedSyntax(op, context); //WTF!
                    }
                }
            }

            if (statements.Count == 0)
            {
                return null;
            }

            if (statements.First.Value is EvaluationStatement evaluationStatement)
            {
                return evaluationStatement;
            }

            throw UnexpectedSyntax(token, context);
        }

        /// <summary>
        /// Reads an evaluation statement (i.e. "x = 3 * 4")
        /// </summary>
        /// <param name="token"></param>
        /// <param name="enumerator"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public EvaluationStatement ReadEvaluationStatement(Token token, IPeekingEnumerator<Token> enumerator,
            ParserContext context)
        {
            var statements = new LinkedList<IStatement>();

            for (;;)
            {
                switch (token.Type)
                {
                    case TokenType.OpenParenthesis:
                    {
                        if (!enumerator.MoveNext()) //first token after open parenthesis
                            throw EndOfFile(token, context);

                        token = enumerator.Current;

                        if (token.Type == TokenType.DataType /* || token.Type == TokenType.IdentifierName*/)
                        {
                            if (enumerator.TryPeek(out var closeParenthesisPeek) &&
                                closeParenthesisPeek.Type == TokenType.CloseParenthesis)
                            {
                                var dataType = TokenTypeToDataType(token, context);

                                statements.AddLast(new TypeCastOperator(dataType, CreateStatementInfo(context, token)));
                            }
                            else
                            {
                                statements.AddLast(ReadEvaluationStatement(token, enumerator, context));
                            }
                        }
                        else
                        {
                            statements.AddLast(ReadEvaluationStatement(token, enumerator, context));
                        }

                        if (!enumerator.MoveNext()) //close parenthesis
                            throw EndOfFile(token, context);

                        token = enumerator.Current;
                        if (token.Type != TokenType.CloseParenthesis)
                            throw UnexpectedSyntax(token, context);

                        break;
                    }

                    case TokenType.OpenBracket:
                    {
                        var openBracketToken = token;

                        if (!enumerator.MoveNext()) //first token after open bracket
                            throw EndOfFile(token, context);

                        token = enumerator.Current;

                        var indexer = ReadEvaluationStatement(token, enumerator, context);

                        if (!enumerator.MoveNext()) //close bracket
                            throw EndOfFile(token, context);

                        token = enumerator.Current;
                        if (token.Type != TokenType.CloseBracket)
                            throw UnexpectedSyntax(token, context);

                        statements.AddLast(
                            new IndexerOperator(indexer, CreateStatementInfo(context, openBracketToken)));

                        break;
                    }

                    case TokenType.IdentifierName:
                    {
                        statements.AddLast(ReadVariableOrAssignmentOrFunctionCall(token, enumerator, context));
                        break;
                    }
                    case TokenType.StringValue1:
                    case TokenType.StringValue2:
                    {
                        if (token.IsStringInterpolation)
                        {
                            statements.AddLast(ReadInterpolatedString(token, context));
                        }
                        else
                        {
                            statements.AddLast(new ConstantValueStatement(TypeDescriptor.String, token.Value,
                                CreateStatementInfo(context, token)));
                        }

                        break;
                    }
                    case TokenType.Number:
                    {
                        if (long.TryParse(token.Value, out var integerResult))
                        {
                            statements.AddLast(new ConstantValueStatement(TypeDescriptor.Integer,
                                integerResult.ToString(NumberFormatInfo.InvariantInfo),
                                CreateStatementInfo(context, token)));
                        }
                        else if (double.TryParse(token.Value, out var floatResult))
                        {
                            statements.AddLast(new ConstantValueStatement(TypeDescriptor.Float,
                                floatResult.ToString(NumberFormatInfo.InvariantInfo),
                                CreateStatementInfo(context, token)));
                        }
                        else
                        {
                            throw UnexpectedSyntax(token, context);
                        }

                        break;
                    }

                    case TokenType.New:
                    {
                        statements.AddLast(ReadArrayDefinition(token, enumerator, context));
                        break;
                    }

                    case TokenType.True:
                    {
                        statements.AddLast(new ConstantValueStatement(TypeDescriptor.Boolean,
                            "True",
                            CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.False:
                    {
                        statements.AddLast(new ConstantValueStatement(TypeDescriptor.Boolean,
                            "False",
                            CreateStatementInfo(context, token)));
                        break;
                    }

                    case TokenType.Null:
                    {
                        statements.AddLast(new ConstantValueStatement(TypeDescriptor.String, token.Value,
                            CreateStatementInfo(context, token)));
                        break;
                    }

                    case TokenType.Plus:
                    {
                        statements.AddLast(new AdditionOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.Minus:
                    {
                        statements.AddLast(new SubtractionOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.Asterisk:
                    {
                        statements.AddLast(new MultiplicationOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.Division:
                    {
                        statements.AddLast(new DivisionOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.Increment:
                    {
                        statements.AddLast(new IncrementOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.Decrement:
                    {
                        statements.AddLast(new DecrementOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.BackSlash:
                    {
                        statements.AddLast(new ModulusOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.Reminder:
                    {
                        statements.AddLast(new ModulusOperator(CreateStatementInfo(context, token)));
                        break;
                    }

                    case TokenType.Not:
                    {
                        statements.AddLast(new NotOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.BitwiseNot:
                    {
                        statements.AddLast(new BitwiseNotOperator(CreateStatementInfo(context, token)));
                        break;
                    }

                    case TokenType.Equals:
                    {
                        statements.AddLast(new EqualOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.NotEquals:
                    {
                        statements.AddLast(new NotEqualOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.GreaterEqual:
                    {
                        statements.AddLast(new GreaterEqualOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.Greater:
                    {
                        statements.AddLast(new GreaterOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.LessEqual:
                    {
                        statements.AddLast(new LessEqualOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.Less:
                    {
                        statements.AddLast(new LessOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.And:
                    {
                        statements.AddLast(new BitwiseAndOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.AndLogical:
                    {
                        statements.AddLast(new LogicalAndOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.Or:
                    {
                        statements.AddLast(new BitwiseOrOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.OrLogical:
                    {
                        statements.AddLast(new LogicalOrOperator(CreateStatementInfo(context, token)));
                        break;
                    }
                    case TokenType.Assignment:
                    {
                        statements.AddLast(new AssignmentOperator(CreateStatementInfo(context, token)));
                        break;
                    }

                    case TokenType.CloseParenthesis:
                    case TokenType.CloseBracket:
                    case TokenType.CloseBrace:
                    case TokenType.SequenceTerminator:
                    case TokenType.OpenBrace:
                    case TokenType.SequenceTerminatorNewLine:
                    case TokenType.Comma:
                    case TokenType.Colon: //used in pre-processors
                    {
                        try
                        {
                            var result = _createEvaluation(statements, token, context);

                            return result;
                        }
                        catch (Exception ex)
                        {
                            throw UnexpectedSyntax(token, context, ex);
                        }
                    }
                    default:
                        throw UnexpectedSyntax(token, context);
                }

                if (enumerator.TryPeek(out var peek))
                {
                    switch (peek.Type)
                    {
                        case TokenType.CloseParenthesis:
                        case TokenType.CloseBracket:
                        case TokenType.CloseBrace:
                        case TokenType.SequenceTerminator:
                        case TokenType.OpenBrace:
                        case TokenType.Comma:
                        case TokenType.Colon: //used in pre-processors
                        {
                            try
                            {
                                var result = _createEvaluation(statements, token, context);

                                return result;
                            }
                            catch (Exception ex)
                            {
                                throw UnexpectedSyntax(token, context, ex);
                            }
                        }
                        default:
                        {
                            if (!enumerator.MoveNext())
                                throw EndOfFile(token, context);

                            token = enumerator.Current;

                            break;
                        }
                    }
                }
                else
                {
                    try
                    {
                        var result = _createEvaluation(statements, token, context);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        throw UnexpectedSyntax(token, context, ex);
                    }
                }
            }
        }

        public EvaluationStatement ReadArrayDefinition(Token token, IPeekingEnumerator<Token> enumerator,
            ParserContext context)
        {
            if (token.Type != TokenType.New)
                throw UnexpectedSyntax(token, context);

            var newToken = token;

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;

            if (!TryReadTypeDescriptor(token, enumerator, context, out var type))
            {
                throw UnexpectedSyntax(token, context);
            }

            if (!type.IsArray() && enumerator.TryPeek(out var ctorPeek) &&
                ctorPeek.Type == TokenType.OpenParenthesis)
            {
                return ReadObjectCreation(newToken, type, enumerator, context);
            }

            EvaluationStatement length = null;

            if (type.IsArray())
            {
                if (!enumerator.MoveNext())
                    throw EndOfFile(token, context);

                token = enumerator.Current;
            }
            else
            {
                if (token.Type != TokenType.OpenBracket)
                    throw UnexpectedSyntax(token, context);

                if (!enumerator.MoveNext())
                    throw EndOfFile(token, context);

                token = enumerator.Current;
                if (token.Type != TokenType.CloseBracket)
                {
                    length = ReadEvaluationStatement(token, enumerator, context);

                    if (!enumerator.MoveNext())
                        throw EndOfFile(token, context);

                    token = enumerator.Current;
                    if (token.Type != TokenType.CloseBracket)
                        throw UnexpectedSyntax(token, context);
                }

                if (enumerator.TryPeek(out var peek) && peek.Type == TokenType.OpenBrace)
                {
                    throw UnexpectedSyntax(peek, context);
                }

                return new ArrayStatement(type, length, null, CreateStatementInfo(context, newToken));
            }

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;

            var items = ReadCommaSeparatedEvaluations(token, enumerator, context);
            if (items == null || items.Length == 0)
            {
                throw UnexpectedSyntax(token, context);
            }

            if (!enumerator.MoveNext()) //close brace
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseBrace)
                throw UnexpectedSyntax(token, context);

            return new ArrayStatement(type, null, items, CreateStatementInfo(context, token));
        }

        public bool TryReadTypeDescriptor(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context,
            out TypeDescriptor typeDescriptor)
        {
            if (token.Type == TokenType.DataType || token.Type == TokenType.IdentifierName)
            {
                typeDescriptor = TokenTypeToDataType(token, context);
                return true;
            }

            typeDescriptor = default;
            return false;
        }

        public EvaluationStatement ReadInterpolatedString(Token token, ParserContext context)
        {
            if (!token.IsStringInterpolation || token.Value.Length < 3)
            {
                throw UnexpectedSyntax(token, context);
            }

            var content = token.Value.Substring(2, token.Value.Length - 3);
            var info = CreateStatementInfo(context, token);
            EvaluationStatement result = null;

            var literal = new StringBuilder();
            for (var i = 0; i < content.Length;)
            {
                var ch = content[i];
                if (ch == '\\' && i + 1 < content.Length)
                {
                    literal.Append(content[i + 1]);
                    i += 2;
                    continue;
                }

                if (ch == '{')
                {
                    if (i + 1 < content.Length && content[i + 1] == '{')
                    {
                        literal.Append('{');
                        i += 2;
                        continue;
                    }

                    result = AppendInterpolatedPart(result, CreateLiteralPart(literal.ToString(), info), info);

                    var holeStart = i + 1;
                    var depth = 1;
                    i++;
                    while (i < content.Length && depth > 0)
                    {
                        if (content[i] == '\\' && i + 1 < content.Length)
                        {
                            i += 2;
                            continue;
                        }

                        if (content[i] == '"' || content[i] == '\'')
                        {
                            if (!TrySkipQuotedInterpolationContent(content, ref i))
                            {
                                throw UnexpectedSyntax(token, context);
                            }

                            continue;
                        }

                        if (content[i] == '{')
                        {
                            depth++;
                        }
                        else if (content[i] == '}')
                        {
                            depth--;
                        }

                        i++;
                    }

                    if (depth != 0)
                    {
                        throw UnexpectedSyntax(token, context);
                    }

                    var holeText = content.Substring(holeStart, i - holeStart - 1);
                    result = AppendInterpolatedPart(result, ParseInterpolationHole(holeText, context, info), info);
                    literal.Clear();
                    continue;
                }

                if (ch == '}' && i + 1 < content.Length && content[i + 1] == '}')
                {
                    literal.Append('}');
                    i += 2;
                    continue;
                }

                literal.Append(ch);
                i++;
            }

            if (literal.Length > 0 || result == null)
            {
                result = AppendInterpolatedPart(result, CreateLiteralPart(literal.ToString(), info), info);
            }

            return result ?? new ConstantValueStatement(TypeDescriptor.String, "\"\"", info);
        }

        private static EvaluationStatement CreateLiteralPart(string literal, StatementInfo info)
        {
            if (literal.Length == 0)
            {
                return new ConstantValueStatement(TypeDescriptor.String, "\"\"", info);
            }

            return new ConstantValueStatement(TypeDescriptor.String, StringHelpers.EnQuote(literal), info);
        }

        private static EvaluationStatement AppendInterpolatedPart(EvaluationStatement left, EvaluationStatement right,
            StatementInfo info)
        {
            if (left == null)
            {
                return right;
            }

            if (left is ConstantValueStatement leftConst && leftConst.Value == "\"\"" && leftConst.TypeDescriptor.IsString())
            {
                return right;
            }

            if (right is ConstantValueStatement rightConst && rightConst.Value == "\"\"" && rightConst.TypeDescriptor.IsString())
            {
                return left;
            }

            var addition = new AdditionOperator(info);
            var result = new ArithmeticEvaluationStatement(left, addition, right, info);
            left.ParentStatement = result;
            right.ParentStatement = result;
            return result;
        }

        private EvaluationStatement ParseInterpolationHole(string holeText, ParserContext context, StatementInfo info)
        {
            if (string.IsNullOrWhiteSpace(holeText))
            {
                throw new ParserSyntaxException("Empty interpolation expression.", info, context);
            }

            var lexer = new Lexer();
            var tokens = lexer.Tokenize(new StringReader(holeText)).ToArray();
            if (tokens.Length == 0)
            {
                throw new ParserSyntaxException("Empty interpolation expression.", info, context);
            }

            using (var enumerator = new PeekingEnumerator<Token>(((IEnumerable<Token>)tokens).GetEnumerator()))
            {
                if (!enumerator.MoveNext())
                {
                    throw new ParserSyntaxException("Empty interpolation expression.", info, context);
                }

                return ReadEvaluationStatement(enumerator.Current, enumerator, context);
            }
        }

        private static bool TrySkipQuotedInterpolationContent(string content, ref int index)
        {
            var quote = content[index];
            index++;
            while (index < content.Length)
            {
                if (content[index] == '\\' && index + 1 < content.Length)
                {
                    index += 2;
                    continue;
                }

                if (content[index] == quote)
                {
                    index++;
                    return true;
                }

                index++;
            }

            return false;
        }

        /// <summary>
        /// Reads a constant VALUE (i.e. 3 or "test" or null)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="enumerator"></param>
        /// <param name="context"></param>
        /// <param name="typeDescriptorHint"></param>
        /// <returns></returns>
        /// <exception cref="???"></exception>
        public ConstantValueStatement ReadConstantValue(Token token, IPeekingEnumerator<Token> enumerator,
            ParserContext context, TypeDescriptor typeDescriptorHint)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    typeDescriptorHint = TypeDescriptor.Numeric;
                    break;
                case TokenType.StringValue1:
                case TokenType.StringValue2:
                    typeDescriptorHint = TypeDescriptor.String;
                    break;
                case TokenType.Null:
                    break;
                case TokenType.True:
                case TokenType.False:
                    typeDescriptorHint = TypeDescriptor.Boolean;
                    break;
                default:
                    throw UnexpectedSyntax(token, context);
            }

            return new ConstantValueStatement(typeDescriptorHint, token.Value, CreateStatementInfo(context, token));
        }

        public EvaluationStatement[] ReadCommaSeparatedEvaluations(Token token, IPeekingEnumerator<Token> enumerator,
            ParserContext context)
        {
            var parameters = new List<EvaluationStatement>();

            var continueRead = true;
            for (; continueRead;)
            {
                var statement = ReadEvaluationStatement(token, enumerator, context);
                parameters.Add(statement);

                if (enumerator.TryPeek(out var nextPeek))
                {
                    switch (nextPeek.Type)
                    {
                        case TokenType.Comma:
                        {
                            if (statement is NopStatement)
                                throw UnexpectedSyntax(token, context);

                            enumerator.MoveNext(); //skip the comma

                            if (!enumerator.TryPeek(out var afterComma))
                                throw EndOfFile(token, context);

                            switch (afterComma.Type)
                            {
                                case TokenType.CloseBrace:
                                case TokenType.CloseParenthesis:
                                case TokenType.CloseBracket:
                                    continueRead = false;
                                    break;
                                default:
                                    if (!enumerator.MoveNext())
                                        throw EndOfFile(token, context);
                                    token = enumerator.Current;
                                    break;
                            }

                            break;
                        }
                        case TokenType.CloseBrace:
                        case TokenType.CloseParenthesis:
                        case TokenType.CloseBracket:
                        case TokenType.SequenceTerminator:
                        case TokenType.SequenceTerminatorNewLine:
                        {
                            continueRead = false;
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return parameters.Where(p => !(p is NopStatement)).ToArray();
        }

        public EvaluationStatement ReadVariableOrAssignmentOrFunctionCall(Token token,
            IPeekingEnumerator<Token> enumerator,
            ParserContext context)
        {
            if (token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, context);

            var functionName = token;
            Token className = null;

            if (!enumerator.TryPeek(out var peek))
            {
                return new VariableAccessStatement(null, functionName.Value, CreateStatementInfo(context, token));
            }

            if (peek.Type == TokenType.Dot)
            {
                enumerator.MoveNext(); //read the dot

                if (!enumerator.MoveNext()) //identifier name
                    throw EndOfFile(token, context);

                token = enumerator.Current;
                if (token.Type != TokenType.IdentifierName)
                    throw UnexpectedSyntax(token, context);

                className = functionName;
                functionName = token;

                if (!enumerator.TryPeek(out peek))
                {
                    return new VariableAccessStatement(className.Value, functionName.Value,
                        CreateStatementInfo(context, token));
                }
            }

            if (peek.Type == TokenType.Assignment)
            {
                enumerator.MoveNext(); //read the assignment operator

                if (!enumerator.MoveNext()) //read the next token for ReadEvaluationStatement
                    throw EndOfFile(token, context);

                token = enumerator.Current;
                var statement = ReadEvaluationStatement(token, enumerator, context);

                var sttInfo = CreateStatementInfo(context, token);
                var leftSide = new VariableAccessStatement(className?.Value, functionName.Value, sttInfo);
                var result = new AssignmentStatement(leftSide, statement, sttInfo);

                leftSide.ParentStatement = result;
                statement.ParentStatement = result;

                return result;
            }

            if (peek.Type == TokenType.OpenParenthesis)
            {
                enumerator.MoveNext(); //read the open parenthesis

                EvaluationStatement[] parameters = null;

                if (!enumerator.MoveNext())
                    throw EndOfFile(token, context);

                token = enumerator.Current;

                if (token.Type != TokenType.CloseParenthesis)
                {
                    parameters = ReadCommaSeparatedEvaluations(token, enumerator, context);

                    token = enumerator.Current;

                    if (!enumerator.MoveNext()) //read the close parenthesis
                        throw EndOfFile(token, context);
                }

                var result = new FunctionCallStatement(
                    className?.Value,
                    functionName.Value,
                    TypeDescriptor.Void,
                    parameters,
                    CreateStatementInfo(context, token)
                );

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        param.ParentStatement = result;
                    }
                }

                return result;
            }

            return new VariableAccessStatement(className?.Value, functionName.Value,
                CreateStatementInfo(context, token));
        }

        public FunctionParameterDefinitionStatement ReadParameterDefinition(Token token,
            IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.DataType && token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, context);

            var dataType = TokenTypeToDataType(token, context);

            if (!enumerator.MoveNext()) //variable name.
                throw EndOfFile(token, context);

            var definitionName = enumerator.Current;
            if (definitionName.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(definitionName, context);

            if (enumerator.TryPeek(out var peek) && peek.Type == TokenType.Assignment)
            {
                enumerator.MoveNext();

                if (!enumerator.MoveNext()) //default value.
                    throw EndOfFile(token, context);

                var defaultValue = ReadConstantValue(enumerator.Current, enumerator, context, dataType);

                var result = new FunctionParameterDefinitionStatement(dataType, definitionName.Value, defaultValue,
                    CreateStatementInfo(context, token));

                defaultValue.ParentStatement = result;

                return result;
            }

            return new FunctionParameterDefinitionStatement(dataType, definitionName.Value, null,
                CreateStatementInfo(context, token));
        }

        public FunctionParameterDefinitionStatement[] ReadParameterDefinitions(Token token,
            IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, context);

            var result = new List<FunctionParameterDefinitionStatement>();

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.DataType && token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, context);

            FunctionParameterDefinitionStatement statement;
            while ((statement = ReadParameterDefinition(token, enumerator, context)) != null)
            {
                result.Add(statement);

                if (!enumerator.TryPeek(out var peek))
                {
                    break;
                }

                if (peek.Type != TokenType.Comma)
                {
                    break;
                }

                enumerator.MoveNext(); //read the comma

                if (!enumerator.MoveNext()) //read next definition
                    throw EndOfFile(token, context);
                token = enumerator.Current;
            }

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, context);

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// function XXX[()] {
        ///     []
        /// }
        /// </remarks>
        /// <param name="token"></param>
        /// <param name="enumerator"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="???"></exception>
        public IStatement ReadVariableOrFunctionDefinition(Token token, IPeekingEnumerator<Token> enumerator,
            ParserContext context)
        {
            var constToken = token;

            if (token.Type != TokenType.DataType && token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //variable name or function name or constant type.
                throw EndOfFile(token, context);

            var isConstant = false;
            if (token.Value == ConstantKeyword)
            {
                constToken = token;
                token = enumerator.Current;

                if (token.Type != TokenType.DataType && token.Type != TokenType.IdentifierName)
                    throw UnexpectedSyntax(token, context);

                isConstant = true;

                if (!enumerator.MoveNext()) //constant name.
                    throw EndOfFile(token, context);
            }

            var dataType = TokenTypeToDataType(token, context);
            
            var definitionName = enumerator.Current;
            if (definitionName.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(definitionName, context);
            if (Keywords.Contains(definitionName.Value)) //impossible check.
                throw InvalidIdentifierName(definitionName.Value, token, context);

            Token peek;

            if (enumerator.TryPeek(out peek))
            {
                if (peek.Type == TokenType.Assignment)
                {
                    if (dataType.IsVoid())
                    {
                        throw UnexpectedSyntax(token, context);
                    }

                    enumerator.MoveNext(); //assignment

                    if (!enumerator.MoveNext()) //value
                        throw EndOfFile(token, context);


                    token = enumerator.Current;

                    var value = ReadEvaluationStatement(token, enumerator, context);

                    var result = new VariableDefinitionStatement(dataType, definitionName.Value, isConstant, value,
                        CreateStatementInfo(context, token));

                    value.ParentStatement = result;

                    return result;
                }

                if (peek.Type == TokenType.OpenParenthesis)
                {
                    if (isConstant)
                    {
                        throw UnexpectedSyntax(constToken, context);
                    }

                    enumerator.MoveNext(); //open parenthesis

                    FunctionParameterDefinitionStatement[] parameters = null;
                    BlockStatement block = null;

                    token = enumerator.Current;
                    //if (token.Type == TokenType.OpenParenthesis)
                    {
                        if (enumerator.TryPeek(out peek))
                        {
                            if (peek.Type == TokenType.CloseParenthesis)
                            {
                                enumerator.MoveNext(); //read close parenthesis.
                            }
                            else
                            {
                                parameters = ReadParameterDefinitions(token, enumerator, context);
                            }
                        }
                        else
                        {
                            throw EndOfFile(token, context);
                        }

                        if (!enumerator.MoveNext())
                            throw EndOfFile(token, context);

                        token = enumerator.Current;
                        if (token.Type != TokenType.OpenBrace)
                            throw UnexpectedSyntax(token, context);

                        block = ReadBlockStatement(token, enumerator, context);
                    }
                    //else if (token.Type == TokenType.OpenBrace)
                    //{
                    //    block = ReadBlockStatement(token, enumerator, info);
                    //}
                    //else
                    //{
                    //    throw UnexpectedSyntax(token, info);
                    //}

                    var result = new FunctionStatement(dataType, definitionName.Value, parameters, block,
                        CreateStatementInfo(context, token));

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            if (param.DefaultValue != null)
                            {
                                param.DefaultValue.ParentStatement = result;
                            }
                        }
                    }

                    return result;
                }

                if (peek.Type != TokenType.SequenceTerminator)
                {
                    throw UnexpectedSyntax(peek, context);
                }
            }

            if (dataType.IsVoid())
            {
                throw UnexpectedSyntax(token, context);
            }

            if (isConstant)
            {
                throw UnexpectedSyntax(peek, context);
            }

            return new VariableDefinitionStatement(dataType, definitionName.Value, false, null,
                CreateStatementInfo(context, token));
        }

        public IStatement ReadIdentifierName(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.TryPeek(out var peek))
                throw EndOfFile(token, context);

            if (peek.Type == TokenType.Assignment || peek.Type == TokenType.Dot ||
                peek.Type == TokenType.OpenParenthesis)
            {
                return ReadVariableOrAssignmentOrFunctionCall(token, enumerator, context);
            }

            if (peek.Type == TokenType.IdentifierName)
            {
                return ReadVariableOrFunctionDefinition(token, enumerator, context);
            }

            throw UnexpectedSyntax(token, context);
        }

        public DelegateStatement ReadDelegateDefinition(Token token, IPeekingEnumerator<Token> enumerator,
            ParserContext context)
        {
            //delegate void MyDelegate(int x);
            if (token.Type != TokenType.Delegate)
                throw UnexpectedSyntax(token, context);

            var delegateToken = token;

            if (!enumerator.MoveNext()) //data type
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.DataType && token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, context);

            var dataType = TokenTypeToDataType(token, context);

            if (!enumerator.MoveNext()) //name
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, context);

            var name = token;

            if (!enumerator.MoveNext()) //open parenthesis
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, context);

            if (enumerator.TryPeek(out var peek) &&
                peek.Type == TokenType.DataType || token.Type == TokenType.IdentifierName)
            {
                var parameters = ReadParameterDefinitions(token, enumerator, context);

                return new DelegateStatement(name.Value, dataType, parameters,
                    CreateStatementInfo(context, delegateToken));
            }

            if (!enumerator.MoveNext()) //data type or close parenthesis.
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type == TokenType.CloseParenthesis)
            {
                return new DelegateStatement(name.Value, dataType, null, CreateStatementInfo(context, delegateToken));
            }

            throw UnexpectedSyntax(token, context);
        }

        public ThrowStatement ReadThrow(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.Throw)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, context);

            var exception = new VariableAccessStatement(token.Value, CreateStatementInfo(context, token));
            return new ThrowStatement(exception, CreateStatementInfo(context, token));
        }

        private IStatement ReadEmbeddedStatement(Token token, IPeekingEnumerator<Token> enumerator,
            ParserContext context)
        {
            switch (token.Type)
            {
                case TokenType.Throw:
                    return ReadThrow(token, enumerator, context);
                case TokenType.OpenBrace:
                    return ReadBlockStatement(token, enumerator, context);
                case TokenType.Echo:
                    return ReadEcho(token, enumerator, context);
                case TokenType.Return:
                    return ReadReturn(token, enumerator, context);
                case TokenType.IdentifierName:
                    return ReadIdentifierName(token, enumerator, context);
                case TokenType.DataType:
                    return ReadVariableOrFunctionDefinition(token, enumerator, context);
                case TokenType.If:
                    return ReadIf(token, enumerator, context);
                case TokenType.While:
                    return ReadWhile(token, enumerator, context);
                default:
                    throw UnexpectedSyntax(token, context);
            }
        }

        public IfElseStatement ReadIf(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.If)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //open parenthesis
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //skip parenthesis to read evaluation
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type == TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, context);

            var condition = ReadEvaluationStatement(token, enumerator, context);

            if (!enumerator.MoveNext()) //close parenthesis
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;
            BlockStatement block;
            if (token.Type == TokenType.OpenBrace)
            {
                block = ReadBlockStatement(token, enumerator, context);
            }
            else
            {
                var bodyStatement = ReadEmbeddedStatement(token, enumerator, context);
                block = new BlockStatement(new[] {bodyStatement}, CreateStatementInfo(context, token));
            }

            if (!enumerator.TryPeek(out var peek) || peek.Type != TokenType.Else) //else or else if
            {
                var sttInfo = CreateStatementInfo(context, token);

                var conditionalBlock = new ConditionalBlockStatement(condition, block, sttInfo);
                condition.ParentStatement = conditionalBlock;

                return new IfElseStatement(conditionalBlock, sttInfo);
            }

            var elseIfBlocks = new List<ConditionalBlockStatement>();

            for (;;)
            {
                enumerator.MoveNext(); //get the else.

                token = enumerator.Current;
                if (token.Type != TokenType.Else)
                    throw UnexpectedSyntax(token, context);

                if (!enumerator.MoveNext()) //get the else.
                    throw EndOfFile(token, context);

                token = enumerator.Current;
                if (token.Type == TokenType.If) //else if
                {
                    if (!enumerator.MoveNext()) //open parenthesis
                        throw EndOfFile(token, context);

                    token = enumerator.Current;
                    if (token.Type != TokenType.OpenParenthesis)
                        throw UnexpectedSyntax(token, context);

                    if (!enumerator.MoveNext()) //skip parenthesis to read evaluation
                        throw EndOfFile(token, context);

                    token = enumerator.Current;
                    if (token.Type == TokenType.CloseParenthesis)
                        throw UnexpectedSyntax(token, context);

                    var elifCondition = ReadEvaluationStatement(token, enumerator, context);

                    if (!enumerator.MoveNext()) //close parenthesis
                        throw EndOfFile(token, context);

                    token = enumerator.Current;
                    if (token.Type != TokenType.CloseParenthesis)
                        throw UnexpectedSyntax(token, context);

                    if (!enumerator.MoveNext()) //open brace
                        throw EndOfFile(token, context);

                    token = enumerator.Current;
                    if (token.Type != TokenType.OpenBrace)
                        throw UnexpectedSyntax(token, context);

                    var elifBlock = ReadBlockStatement(token, enumerator, context);

                    var conditionalBlock = new ConditionalBlockStatement(elifCondition, elifBlock,
                        CreateStatementInfo(context, token));
                    elifCondition.ParentStatement = conditionalBlock;

                    elseIfBlocks.Add(conditionalBlock);
                }
                else if (token.Type == TokenType.OpenBrace) //else
                {
                    var elseBlock = ReadBlockStatement(token, enumerator, context);

                    var sttInfo = CreateStatementInfo(context, token);

                    var conditionalBlock = new ConditionalBlockStatement(condition, block, sttInfo);
                    condition.ParentStatement = conditionalBlock;

                    return new IfElseStatement(conditionalBlock, elseIfBlocks.ToArray(), elseBlock, sttInfo);
                }

                if (!enumerator.TryPeek(out peek) || peek.Type != TokenType.Else) //else or else if
                {
                    var sttInfo = CreateStatementInfo(context, token);

                    var conditionalBlock = new ConditionalBlockStatement(condition, block, sttInfo);
                    condition.ParentStatement = conditionalBlock;

                    return new IfElseStatement(conditionalBlock, elseIfBlocks.ToArray(), sttInfo);
                }
            }
        }

        public SwitchCaseStatement ReadSwitchCase(Token token, IPeekingEnumerator<Token> enumerator,
            ParserContext context)
        {
            if (token.Type != TokenType.Switch)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext() || enumerator.Current.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(enumerator.Current, context);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;
            var switchTarget = ReadEvaluationStatement(token, enumerator, context);

            if (!enumerator.MoveNext() || enumerator.Current.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(enumerator.Current, context);

            if (!enumerator.MoveNext() || enumerator.Current.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(enumerator.Current, context);

            token = enumerator.Current;
            var cases = new List<ConditionalBlockStatement>();
            IStatement defaultCase = null;

            while (enumerator.MoveNext())
            {
                token = enumerator.Current;
                if (token.Type == TokenType.CloseBrace)
                {
                    break;
                }

                if (token.Type == TokenType.Case)
                {
                    if (!enumerator.MoveNext())
                        throw EndOfFile(token, context);

                    token = enumerator.Current;
                    var caseValue = ReadEvaluationStatement(token, enumerator, context);

                    if (!enumerator.MoveNext() || enumerator.Current.Type != TokenType.Colon)
                        throw UnexpectedSyntax(enumerator.Current, context);

                    if (!enumerator.MoveNext())
                        throw EndOfFile(token, context);

                    token = enumerator.Current;
                    BlockStatement caseBlock;
                    if (token.Type == TokenType.OpenBrace)
                    {
                        caseBlock = ReadBlockStatement(token, enumerator, context);
                    }
                    else
                    {
                        var body = ReadEmbeddedStatement(token, enumerator, context);
                        caseBlock = new BlockStatement(new[] {body}, CreateStatementInfo(context, token));
                    }

                    var caseCondition = CreateSwitchCaseCondition(switchTarget, caseValue, context);
                    var conditional = new ConditionalBlockStatement(caseCondition, caseBlock,
                        CreateStatementInfo(context, token));
                    caseCondition.ParentStatement = conditional;
                    cases.Add(conditional);
                    continue;
                }

                if (token.Type == TokenType.Default)
                {
                    if (!enumerator.MoveNext() || enumerator.Current.Type != TokenType.Colon)
                        throw UnexpectedSyntax(enumerator.Current, context);

                    if (!enumerator.MoveNext())
                        throw EndOfFile(token, context);

                    token = enumerator.Current;
                    if (token.Type == TokenType.OpenBrace)
                    {
                        defaultCase = ReadBlockStatement(token, enumerator, context);
                    }
                    else
                    {
                        var body = ReadEmbeddedStatement(token, enumerator, context);
                        defaultCase = new BlockStatement(new[] {body}, CreateStatementInfo(context, token));
                    }

                    continue;
                }

                throw UnexpectedSyntax(token, context);
            }

            var info = CreateStatementInfo(context, token);
            switchTarget.ParentStatement = null;

            if (defaultCase != null)
            {
                return new SwitchCaseStatement(switchTarget, cases.ToArray(), defaultCase, info);
            }

            return new SwitchCaseStatement(switchTarget, cases.ToArray(), info);
        }

        private static EvaluationStatement CreateSwitchCaseCondition(EvaluationStatement switchTarget,
            EvaluationStatement caseValue, ParserContext context)
        {
            var targetClone = CloneEvaluationForSwitchCase(switchTarget);
            var valueClone = CloneEvaluationForSwitchCase(caseValue);
            var eq = new EqualOperator(valueClone.Info);
            var condition = new LogicalEvaluationStatement(targetClone, eq, valueClone, eq.Info);
            targetClone.ParentStatement = condition;
            valueClone.ParentStatement = condition;
            return condition;
        }

        private static EvaluationStatement CloneEvaluationForSwitchCase(EvaluationStatement source)
        {
            switch (source)
            {
                case VariableAccessStatement variable:
                    return new VariableAccessStatement(variable.ClassName, variable.VariableName, variable.Info);
                case ConstantValueStatement constant:
                    return new ConstantValueStatement(constant.TypeDescriptor, constant.Value, constant.Info);
                default:
                    return source;
            }
        }

        public WhileStatement ReadWhile(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.While)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //skip parenthesis to read evaluation
                throw EndOfFile(token, context);

            token = enumerator.Current;
            var condition = ReadEvaluationStatement(token, enumerator, context);

            if (!enumerator.MoveNext()) //close parenthesis
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //open brace or semicolon
                throw EndOfFile(token, context);

            WhileStatement result;

            token = enumerator.Current;
            if (token.Type == TokenType.SequenceTerminator)
            {
                result = new WhileStatement(condition, null, CreateStatementInfo(context, token));
            }
            else if (token.Type == TokenType.OpenBrace)
            {
                var statements = ReadBlockStatement(token, enumerator, context);

                result = new WhileStatement(condition, statements, CreateStatementInfo(context, token));
            }
            else
            {
                throw UnexpectedSyntax(token, context);
            }

            condition.ParentStatement = result;
            return result;
        }

        public DoWhileStatement ReadDoWhile(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.Do)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //open brace
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, context);

            var statements = ReadBlockStatement(token, enumerator, context);

            if (!enumerator.MoveNext()) //while
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.While)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //open parenthesis
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //first token of condition
                throw EndOfFile(token, context);

            token = enumerator.Current;
            var condition = ReadEvaluationStatement(token, enumerator, context);

            if (!enumerator.MoveNext()) //close parenthesis
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, context);

            if (context.SemicolonRequired)
            {
                if (!enumerator.MoveNext()) //semicolon
                    throw EndOfFile(token, context);

                token = enumerator.Current;
                if (token.Type != TokenType.SequenceTerminator)
                    throw UnexpectedSyntax(token, context);
            }

            var result = new DoWhileStatement(condition, statements, CreateStatementInfo(context, token));
            condition.ParentStatement = result;

            return result;
        }

        public WhileStatement ReadLoop(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            return ReadWhile(token, enumerator, context);
        }

        public ForStatement ReadFor(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.For)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext() || enumerator.Current.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(enumerator.Current, context);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            IStatement[] initializations = null;
            EvaluationStatement condition = null;
            IStatement[] increments = null;

            token = enumerator.Current;
            if (token.Type != TokenType.SequenceTerminator)
            {
                IStatement initialization = token.Type == TokenType.DataType
                    ? ReadVariableOrFunctionDefinition(token, enumerator, context)
                    : ReadVariableOrAssignmentOrFunctionCall(token, enumerator, context);
                initializations = new[] {initialization};

                if (!enumerator.MoveNext())
                    throw EndOfFile(token, context);
                token = enumerator.Current;
                if (token.Type != TokenType.SequenceTerminator)
                    throw UnexpectedSyntax(token, context);
            }

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);
            token = enumerator.Current;
            if (token.Type != TokenType.SequenceTerminator)
            {
                condition = ReadEvaluationStatement(token, enumerator, context);
                if (!enumerator.MoveNext())
                    throw EndOfFile(token, context);
                token = enumerator.Current;
                if (token.Type != TokenType.SequenceTerminator)
                    throw UnexpectedSyntax(token, context);
            }

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);
            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
            {
                increments = new IStatement[] {ReadEvaluationStatement(token, enumerator, context)};
                if (!enumerator.MoveNext())
                    throw EndOfFile(token, context);
                token = enumerator.Current;
                if (token.Type != TokenType.CloseParenthesis)
                    throw UnexpectedSyntax(token, context);
            }

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);
            token = enumerator.Current;
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, context);

            var block = ReadBlockStatement(token, enumerator, context);
            var result = new ForStatement(initializations, condition, increments, block,
                CreateStatementInfo(context, token));
            if (condition != null)
                condition.ParentStatement = result;
            return result;
        }

        public ForEachStatement ReadForEach(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.ForEach)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //open parenthesis
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //dataType or identifier name
                throw EndOfFile(token, context);

            TypeDescriptor? typeDescriptor = null;

            token = enumerator.Current;
            if (token.Type == TokenType.DataType || token.Type == TokenType.IdentifierName)
            {
                typeDescriptor = TokenTypeToDataType(token, context);

                if (!enumerator.MoveNext()) //identifier
                    throw EndOfFile(token, context);

                token = enumerator.Current;
                if (token.Type != TokenType.IdentifierName)
                    throw UnexpectedSyntax(token, context);
            }
            else if (token.Type != TokenType.IdentifierName)
            {
                throw UnexpectedSyntax(token, context);
            }

            IStatement variable;
            EvaluationStatement variableEval = null;
            if (typeDescriptor == null)
            {
                variableEval = new VariableAccessStatement(token.Value, CreateStatementInfo(context, token));
                variable = variableEval;
            }
            else
            {
                variable = new VariableDefinitionStatement(typeDescriptor.Value, token.Value, false, null,
                    CreateStatementInfo(context, token));
            }

            if (!enumerator.MoveNext()) //in
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.In)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //id
                throw EndOfFile(token, context);

            token = enumerator.Current;

            var iterator = ReadEvaluationStatement(token, enumerator, context);

            //var iterator = new VariableAccessStatement(token.Value, CreateStatementInfo(info, token));

            if (!enumerator.MoveNext()) //close parenthesis
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext()) //open brace
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, context);

            var block = ReadBlockStatement(token, enumerator, context);

            var result = new ForEachStatement(variable, iterator, block, CreateStatementInfo(context, token));

            iterator.ParentStatement = result;
            if (variableEval != null)
            {
                variableEval.ParentStatement = result;
            }

            return result;
        }

        public ObjectCreationStatement ReadObjectCreation(Token newToken, TypeDescriptor classType,
            IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (!enumerator.MoveNext())
                throw EndOfFile(newToken, context);

            var token = enumerator.Current;
            if (token.Type != TokenType.OpenParenthesis)
                throw UnexpectedSyntax(token, context);

            EvaluationStatement[] arguments = null;

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.CloseParenthesis)
            {
                arguments = ReadCommaSeparatedEvaluations(token, enumerator, context);

                if (!enumerator.MoveNext())
                    throw EndOfFile(token, context);

                token = enumerator.Current;
            }

            if (token.Type != TokenType.CloseParenthesis)
                throw UnexpectedSyntax(token, context);

            if (classType.DataType != DataTypes.Lookup && classType.DataType != DataTypes.Class)
            {
                throw UnexpectedSyntax(newToken, context);
            }

            var resolvedType = classType.DataType == DataTypes.Lookup
                ? ObjectModelHelpers.UserClass(classType.Lookup.Value.Name)
                : classType;

            return new ObjectCreationStatement(resolvedType, arguments, CreateStatementInfo(context, newToken));
        }

        public IStatement ReadClass(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.Class)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.IdentifierName)
                throw UnexpectedSyntax(token, context);

            var className = token.Value;
            if (Keywords.Contains(className))
                throw InvalidIdentifierName(className, token, context);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;
            if (token.Type != TokenType.OpenBrace)
                throw UnexpectedSyntax(token, context);

            var fields = new List<VariableDefinitionStatement>();
            var methods = new List<FunctionStatement>();

            while (enumerator.MoveNext())
            {
                token = enumerator.Current;
                if (token.Type == TokenType.CloseBrace)
                {
                    break;
                }

                if (token.Type != TokenType.DataType && token.Type != TokenType.IdentifierName)
                    throw UnexpectedSyntax(token, context);

                var memberType = TokenTypeToDataType(token, context);

                if (!enumerator.MoveNext())
                    throw EndOfFile(token, context);

                token = enumerator.Current;
                if (token.Type != TokenType.IdentifierName)
                    throw UnexpectedSyntax(token, context);

                var memberName = token.Value;

                if (!enumerator.TryPeek(out var peek))
                    throw EndOfFile(token, context);

                if (peek.Type == TokenType.OpenParenthesis)
                {
                    enumerator.MoveNext();
                    token = enumerator.Current;

                    FunctionParameterDefinitionStatement[] parameters = null;
                    if (token.Type == TokenType.OpenParenthesis)
                    {
                        if (enumerator.TryPeek(out peek) && peek.Type != TokenType.CloseParenthesis)
                        {
                            parameters = ReadParameterDefinitions(token, enumerator, context);
                        }
                        else
                        {
                            if (!enumerator.MoveNext())
                                throw EndOfFile(token, context);
                            token = enumerator.Current;
                            if (token.Type != TokenType.CloseParenthesis)
                                throw UnexpectedSyntax(token, context);
                        }
                    }
                    else
                    {
                        throw UnexpectedSyntax(token, context);
                    }

                    if (!enumerator.MoveNext())
                        throw EndOfFile(token, context);

                    token = enumerator.Current;
                    if (token.Type != TokenType.OpenBrace)
                        throw UnexpectedSyntax(token, context);

                    var block = ReadBlockStatement(token, enumerator, context);
                    var isConstructor = string.Equals(memberName, className, StringComparison.Ordinal);

                    methods.Add(new FunctionStatement(
                        isConstructor ? TypeDescriptor.Void : memberType,
                        memberName,
                        parameters,
                        block,
                        CreateStatementInfo(context, token),
                        className,
                        isInstanceMethod: true,
                        isConstructor: isConstructor));
                }
                else
                {
                    if (!memberType.IsVoid())
                    {
                        fields.Add(new VariableDefinitionStatement(memberType, memberName, false, null,
                            CreateStatementInfo(context, token)));
                    }

                    if (!enumerator.MoveNext())
                        throw EndOfFile(token, context);

                    token = enumerator.Current;
                    if (token.Type != TokenType.SequenceTerminator &&
                        token.Type != TokenType.SequenceTerminatorNewLine)
                    {
                        throw UnexpectedSyntax(token, context);
                    }
                }
            }

            return new ClassDeclarationStatement(className, fields.ToArray(), methods.ToArray(),
                CreateStatementInfo(context, token));
        }


        public IStatement ReadEcho(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.Echo)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;

            var statement = ReadEvaluationStatement(token, enumerator, context);

            var result = new EchoStatement(new[] {statement}, CreateStatementInfo(context, token));

            statement.ParentStatement = result;

            return result;
        }

        public ReturnStatement ReadReturn(Token token, IPeekingEnumerator<Token> enumerator, ParserContext context)
        {
            if (token.Type != TokenType.Return)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;

            if (token.Type == TokenType.SequenceTerminator ||
                token.Type == TokenType.SequenceTerminatorNewLine ||
                token.Type == TokenType.CloseBrace)
            {
                return new ReturnStatement(null, CreateStatementInfo(context, token));
            }

            var statement = ReadEvaluationStatement(token, enumerator, context);

            var result = new ReturnStatement(statement, CreateStatementInfo(context, token));

            statement.ParentStatement = result;

            return result;
        }

        public IncludeStatement ReadIncludeStatement(Token token, IPeekingEnumerator<Token> enumerator,
            ParserContext context)
        {
            int x = 0;
            int y = 0;
            x = x + + + - - y;
            
            if (token.Type != TokenType.Include)
                throw UnexpectedSyntax(token, context);

            if (!enumerator.MoveNext())
                throw EndOfFile(token, context);

            token = enumerator.Current;
            
            var statement = ReadEvaluationStatement(token, enumerator, context);
            
            var result = new IncludeStatement(statement, CreateStatementInfo(context, token));

            statement.ParentStatement = result;

            return result;
        }
    }
}