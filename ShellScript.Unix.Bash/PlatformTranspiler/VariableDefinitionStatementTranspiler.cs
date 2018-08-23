using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class VariableDefinitionStatementTranspiler : VariableDefinitionStatementTranspilerBase
    {
        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotImplementedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, IStatement statement)
        {
            if (!(statement is VariableDefinitionStatement varDefStt)) throw new InvalidOperationException();

            if (varDefStt.HasDefaultValue)
            {
                var def = (EvaluationStatement) varDefStt.DefaultValue;
                var transpiler = context.GetEvaluationTranspilerForStatement(def);
                if (transpiler == null)
                {
                    throw new InvalidOperationException();
                }

                //it pins the non-inlineable values to a helper variable:
                //int x = 34 * myFunc();
                //becomes:
                //myFuncResult=myFunc()
                //x=$((34 * myFuncResult))
                var defaultValue = transpiler.PinEvaluationToInline(context, scope, writer, def);
                
                writer.WriteLine($"{varDefStt.Name}={defaultValue}");
            }
            else
            {
                writer.WriteLine($"{varDefStt.Name}=0");
            }
        }
    }
}