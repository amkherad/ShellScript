using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashEvaluationStatementTranspiler : EvaluationStatementTranspilerBase
    {
        private class ExpressionTypes
        {
            public List<DataTypes> Types { get; set; }
            public bool ContainsFunctionCall { get; set; }
            public List<VariableAccessStatement> NonExistenceVariables { get; set; }

            public HashSet<Type> OperatorTypes { get; set; }
        }

        private static ExpressionTypes TraverseTreeAndGetExpressionTypes(Context context, Scope scope,
            IStatement statement)
        {
            var result = new ExpressionTypes();
            var types = new List<DataTypes>();
            var nonExistenceVariables = new List<VariableAccessStatement>();
            var operators = new HashSet<Type>();

            result.Types = types;
            result.NonExistenceVariables = nonExistenceVariables;
            result.OperatorTypes = operators;

            switch (statement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    types.Add(constantValueStatement.DataType);
                    break;
                }
                case VariableAccessStatement variableAccessStatement:
                {
                    if (scope.TryGetVariableInfo(variableAccessStatement.VariableName, out VariableInfo varInfo))
                    {
                        types.Add(varInfo.DataType);
                    }
                    else
                    {
                        nonExistenceVariables.Add(variableAccessStatement);
                    }

                    break;
                }
                case IOperator op:
                {
                    operators.Add(op.GetType());
                    break;
                }
                default:
                {
                    foreach (var child in statement.TraversableChildren)
                    {
                        var childResult = TraverseTreeAndGetExpressionTypes(context, scope, child);

                        foreach (var t in childResult.Types)
                            types.Add(t);
                        
                        foreach (var nev in childResult.NonExistenceVariables)
                            nonExistenceVariables.Add(nev);
                        
                        foreach (var opType in childResult.OperatorTypes)
                            operators.Add(opType);

                        result.ContainsFunctionCall = result.ContainsFunctionCall || childResult.ContainsFunctionCall;
                    }

                    break;
                }
            }

            return result;
        }


        public override void WriteInline(Context context, Scope scope, TextWriter writer,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            if (!(statement is EvaluationStatement evalStt)) throw new InvalidOperationException();

            var structure = TraverseTreeAndGetExpressionTypes(context, scope, statement);

            if (structure.Types.Contains(DataTypes.String))
            {
                structure.OperatorTypes.Remove(typeof(AdditionOperator));
                if (structure.OperatorTypes.Count > 0)
                {
                    throw new InvalidOperatorForTypeCompilerException(structure.OperatorTypes.First(), DataTypes.String);
                }
                
                
            }
            
            

            writer.Write(expression);
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, IStatement statement)
        {
        }

        public override string PinEvaluationToInline(Context context, Scope scope, TextWriter pinCodeWriter,
            EvaluationStatement statement)
        {
        }
    }
}