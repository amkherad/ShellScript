using System;
using System.IO;
using System.Linq;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
{
    public abstract class FunctionStatementTranspilerBase : BlockStatementTranspilerBase, IPlatformStatementTranspiler
    {
        public override Type StatementType => typeof(FunctionStatement);

        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return false;
        }

        public override bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            if (!(statement is FunctionStatement funcDefStt)) throw new InvalidOperationException();

            var functionName = funcDefStt.Name;

            if (scope.IsIdentifierExists(functionName))
            {
                message = IdentifierNameExistsCompilerException.CreateMessage(functionName, funcDefStt.Info);
                return false;
            }

            return base.Validate(context, scope, statement, out message);
        }

        public static bool TryGetInlinedStatement(Context context, Scope scope, FunctionStatement function,
            out IStatement inlinedStatement)
        {
            if (!context.Flags.UseInlining)
            {
                inlinedStatement = null;
                return false;
            }
            
            var statements = function.Block.Statements;

            if (statements.Length != 1)
            {
                inlinedStatement = null;
                return false;
            }

            var stt = statements.First();
            switch (stt)
            {
                case ReturnStatement returnStatement:
                {
                    inlinedStatement = returnStatement.Result;
                    return true;
                }
                case FunctionCallStatement functionCallStatement:
                {
                    if (!context.Flags.InlineCascadingFunctionCalls)
                    {
                        inlinedStatement = null;
                        return false;
                    }
                    
                    inlinedStatement = functionCallStatement;
                    return true;
                }
                default:
                {
                    if (!context.Flags.InlineNonEvaluations)
                    {
                        inlinedStatement = null;
                        return false;
                    }

                    inlinedStatement = null;
                    return false;
                }
            }
        }
    }
}