using System;
using System.Globalization;
using System.IO;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashFunctionStatementTranspiler : FunctionStatementTranspilerBase
    {
        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotSupportedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is FunctionStatement funcDefStt)) throw new InvalidOperationException();

            var isClassMethod = !string.IsNullOrEmpty(funcDefStt.ClassName);
            var functionName = isClassMethod
                ? BashObjectModel.GetMethodFunctionName(funcDefStt.ClassName, funcDefStt.Name, funcDefStt.IsConstructor)
                : funcDefStt.Name;

            var skipNameCheck = isClassMethod && funcDefStt.IsConstructor && funcDefStt.Name == funcDefStt.ClassName;
            if (!skipNameCheck && scope.IsIdentifierExists(functionName))
            {
                throw new IdentifierNameExistsCompilerException(functionName, funcDefStt.Info);
            }

            var funcScope = scope.BeginNewScope(ScopeType.MethodRoot);
            IStatement inlinedStatement = null;

            funcScope.SetConfig(c => c.ExplicitEchoStream, context.Flags.DefaultExplicitEchoStream);

            if (context.Flags.UseComments && context.Flags.CommentParameterInfos)
            {
                BashTranspilerHelpers.WriteComment(writer, $"! {funcDefStt.TypeDescriptor} {functionName}");
            }

            if (isClassMethod && funcDefStt.IsInstanceMethod)
            {
                funcScope.SetConfig(c => c.InstanceUsesNameref, "true");
                funcScope.ReserveNewParameter(
                    ObjectModelHelpers.UserClass(funcDefStt.ClassName),
                    ObjectModelHelpers.ThisKeyword,
                    BashObjectModel.SelfNameref);
            }

            if (funcDefStt.Parameters != null && funcDefStt.Parameters.Length > 0)
            {
                for (var i = 0; i < funcDefStt.Parameters.Length; i++)
                {
                    var param = funcDefStt.Parameters[i];
                    var paramIndex = isClassMethod && funcDefStt.IsInstanceMethod ? i + 2 : i + 1;
                    var paramMappedName = paramIndex.ToString(CultureInfo.InvariantCulture);
                    funcScope.ReserveNewParameter(param.TypeDescriptor, param.Name, paramMappedName);

                    if (context.Flags.UseComments && context.Flags.CommentParameterInfos)
                    {
                        BashTranspilerHelpers.WriteComment(writer,
                            $"\\param ${paramMappedName} {param.TypeDescriptor} - {param.Name}");
                    }
                }
            }

            writer.WriteLine($"function {functionName}() {{");

            if (isClassMethod && funcDefStt.IsInstanceMethod)
            {
                writer.WriteLine($"local -n {BashObjectModel.SelfNameref}=$1");
            }

            BashBlockStatementTranspiler.WriteBlockStatement(context, funcScope, writer, metaWriter,
                funcDefStt.Statement, ScopeType.MethodRoot, false, typeof(ReturnStatement));
            TryGetInlinedStatement(context, funcScope, funcDefStt, out inlinedStatement);


            var func = new FunctionInfo(funcDefStt.TypeDescriptor, funcDefStt.Name, functionName, funcDefStt.ClassName,
                funcDefStt.IsParams,
                funcDefStt.Parameters, inlinedStatement);

            scope.ReserveNewFunction(func);
            scope.ReserveNewPrototype(func);

            writer.WriteLine("}");

            scope.IncrementStatements();
        }
    }
}